using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sifter.Builders;
using Sifter.Extensions;
using Sifter.Terms;
using Sifter.Types;


namespace Sifter.Services {

//TODO make internal and only expose interface if possible
    public abstract class SifterService : ISifterService {

        private readonly SifterMap map;
        private readonly TypeTree typeTree;



        protected SifterService() {
            var builder = new SifterBuilder();
            // ReSharper disable once VirtualMemberCallInConstructor
            onSifterBuild(builder);
            (map, typeTree) = builder.Build();
        }



        protected abstract void onSifterBuild(ISifterBuilder builder);



        public virtual IQueryable<T> Sift<T>(IQueryable<T> query, SifterModel model) {
            query = applySortTerms(query, model.GetSortTerms());
//            query = applyFilterTerms(query,)

            return query;
        }

//TODO change all internal modifiers to public, and then make classes internal

//TODO add option for max sort depth
//TODO check if circular dependencies work
        private IQueryable<T> applySortTerms<T>(IQueryable<T> query, IEnumerable<SortTerm> sortTerms) {
            var isNestedSort = false;

            foreach (var sortTerm in sortTerms) {
                var identifier = sortTerm.Identifier;
                var sifterInfo = getSifterInfo<T>(identifier);

                if (sifterInfo == null || !sifterInfo.CanSort) {
                    continue;
                }

                query = sortTerm.ApplySort(query, identifier, sifterInfo.PropertyInfo, isNestedSort);
                isNestedSort = true;
            }

            return query;
        }



        [CanBeNull]
        private SifterInfo getSifterInfo<T>(string identifier) {
            //TODO identifier isn't checked for null here
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