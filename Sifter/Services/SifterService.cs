using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sifter.Builders;
using Sifter.Models;
using Sifter.Terms;


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



        public IQueryable<T> Sift<T>(IQueryable<T> query, SifterModel model) {
            query = applySortTerms(query, model.GetSortTerms());
            query = applyFilterTerms(query, model.GetFilterTerms());
            query = model.GetPaginationTerm().ApplyPagination(query);

            return query;
        }



        protected abstract void onSifterBuild(ISifterBuilder builder);

//TODO change all internal modifiers to public, and then make classes internal



//TODO add option for max sort depth
//TODO check if circular dependencies work
        private IQueryable<T> applySortTerms<T>(IQueryable<T> query, IEnumerable<SortTerm> sortTerms) {
            var isSecondarySort = false;

            foreach (var sortTerm in sortTerms) {
                var sifterInfo = getSifterInfo<T>(sortTerm.Identifier);

                if (sifterInfo == null || !sifterInfo.CanSort) {
                    continue;
                }

                query = sortTerm.ApplySort(query, sifterInfo.PropertyInfo, isSecondarySort);
                isSecondarySort = true;
            }

            return query;
        }



        private IQueryable<T> applyFilterTerms<T>(IQueryable<T> query, IEnumerable<FilterTerm> filterTerms) {
            foreach (var filterTerm in filterTerms) {
                var sifterInfo = getSifterInfo<T>(filterTerm.Identifier);

                if (sifterInfo == null || !sifterInfo.CanFilter) {
                    continue;
                }

                query = filterTerm.ApplyFilter(query, sifterInfo.PropertyInfo);
            }

            return query;
        }



        [CanBeNull]
        private SifterInfo getSifterInfo<T>(string identifier) {
            if (!identifier.Contains('.')) {
                //TODO search for ToLowerInvariant in the code, and add is as an optional thing
                var _root = map.Get(typeof(T));
                return _root?.Get(identifier.ToLowerInvariant());
            }

            var root = map.Get(getNestedClassType<T>(identifier));
            var propertyName = identifier.Split('.').Last();

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