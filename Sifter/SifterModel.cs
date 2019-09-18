using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Sifter.Terms;


namespace Sifter {

    public sealed class SifterModel {

        [FromQuery]
        [CanBeNull]
        public string Sort { get; set; }

        [FromQuery]
        [CanBeNull]
        public string Filter { get; set; }

        [FromQuery]
        [CanBeNull]
        public int? Page { get; set; }

        [FromQuery]
        [CanBeNull]
        public int? PageSize { get; set; }



        [NotNull]
        [ItemNotNull]
        internal IEnumerable<SortTerm> GetSortTerms() {
            if (Sort == null) {
                return new List<SortTerm>();
            }

            return Regexes.SORT_REGEX.Match(Sort)
                .Groups["sortTerm"]
                .Captures
                .Select(c => new SortTerm(c.Value));
        }



        [NotNull]
        [ItemNotNull]
        internal IEnumerable<FilterTerm> GetFilterTerms() {
            if (Filter == null) {
                return new List<FilterTerm>();
            }

            var g = Regexes.FILTER_REGEX.Match(Filter)
                .Groups["filterTerm"]
                .Captures
                .Select(c => new FilterTerm(c.Value));

            return Regexes.FILTER_REGEX.Match(Filter)
                .Groups["filterTerm"]
                .Captures
                .Select(c => new FilterTerm(c.Value));
        }

    }

}