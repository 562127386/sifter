using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;


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

        private const string identifierRegex = @"[A-z_][\w]*([.][A-z_][\w]*)*";
        private static readonly Regex sortTermRegex = new Regex($@"^[-]?{identifierRegex}$");



        [NotNull]
        [ItemNotNull]
        internal IEnumerable<SortTerm> GetSortTerms() {
            if (Sort == null) {
                return new List<SortTerm>();
            }

            return Sort.Split(',')
                .Select(s => s.Trim())
                .Where(s => sortTermRegex.IsMatch(s))
                .Select(s => new SortTerm(s));
        }

    }


    internal class SortTerm {

        public string Identifier { get; }
        public bool IsDescending { get; }
        
        public SortTerm(string str) {
            if (str.StartsWith('-')) {
                Identifier = str.Substring(1);
                IsDescending = true;
            }
            else {
                Identifier = str;
                IsDescending = false;
            }
        }

    }


    internal class FilterTerm { }

}