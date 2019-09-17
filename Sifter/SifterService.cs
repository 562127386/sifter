using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using JetBrains.Annotations;


namespace Sifter {

    public abstract class SifterServiceService : ISifterService {

        private readonly SifterMap map;
        private readonly TypeTree typeTree;



        protected SifterServiceService() {
            var builder = new SifterBuilder();
            // ReSharper disable once VirtualMemberCallInConstructor
            onSifterBuild(builder);
            (map, typeTree) = builder.Build();
        }



        protected abstract void onSifterBuild(SifterBuilder builder);



        public virtual IQueryable<T> Sift<T>(IQueryable<T> query, SifterModel model) {
            var sortTerms = model.GetSortTerms();
            var isNestedSort = false;

            foreach (var sortTerm in sortTerms) {
                var identifier = sortTerm.Identifier;
                var sifterInfo = getSifterInfo<T>(identifier);

                if (sifterInfo == null || !sifterInfo.CanSort) {
                    continue;
                }

                query = query.ApplySort(identifier, sifterInfo.PropertyInfo, sortTerm, isNestedSort);
                isNestedSort = true;
            }

            return query;
        }



        [CanBeNull]
        private SifterInfo getSifterInfo<T>(string identifier) {
            var root = map.Get(typeof(T));
            var propertyName = identifier;

            if (!identifier.IsNested()) {
                //TODO search for ToLowerInvariant, and add is as an optional thing
                return root?.Get(propertyName.ToLowerInvariant());
            }

            root = map.Get(getNestedClassType<T>(identifier));
            propertyName = identifier.Split('.').Last();

            return root?.Get(propertyName.ToLowerInvariant());
        }



        private Type getNestedClassType<T>(string identifier) {
            var propertyNames = identifier.Split('.');

            var type = typeof(T);
            var inFirstLoop = true;
            var depth = 1;

            foreach (var propertyName in propertyNames) {
                //This presumes that a sortable or filterable property is not indexed
                if (typeTree.TryGetValue((type, propertyName), out var propertyType)) {
                    type = propertyType;
                    depth++;
                }
                else if (inFirstLoop) {
                    return null;
                }

                inFirstLoop = false;
            }

            return depth == propertyNames.Length ? type : null;
        }

    }

}