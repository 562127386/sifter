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

        private const string identifierRegex = @"(?:[A-z_]\w*(?:\.[A-z_]\w*)*)";
        private const string operatorRegex = @"(!?[@^$]\*?|[=!]=\*|[=!<>]=|[<>])";
        private const string stringVariableRegex = @"(""([^\\""]|\\.)*""|[\w ]*)";
        private const string numberVariableRegex = @"(-?[0-9]+(\.[0-9]*)?)";
        private static readonly string sortTermRegex = $"(?'sortTerm'-?{identifierRegex})";

        private static readonly Regex sortRegex = new Regex($@"^{sortTermRegex}( *, *{sortTermRegex})*$");

        private static readonly Regex filterTermRegex =
            new Regex($"^{identifierRegex} ?{operatorRegex} ? ({stringVariableRegex}|{numberVariableRegex})$");



        [NotNull]
        [ItemNotNull]
        internal IEnumerable<SortTerm> GetSortTerms() {
            if (Sort == null) {
                return new List<SortTerm>();
            }

            return sortRegex.Match(Sort)
                .Groups["sortTerm"]
                .Captures
                .Select(c => new SortTerm(c.Value));
        }



        [NotNull]
        [ItemNotNull]
        internal IEnumerable<FilterTerm> GetFilterTerms() {
            if (Sort == null) {
                return new List<FilterTerm>();
            }

            return Sort.Split(',')
                .Select(s => s.Trim())
                .Where(s => sortRegex.IsMatch(s))
                .Select(s => new FilterTerm(s));
        }

    }


//TODO this currently has no support for OR/combining statements, maybe add later
    internal class FilterTerm {

        public string LeftOperand { get; }
        public Operator Operator { get; }
        public string RightOperand { get; }

        public FilterTerm(string str) { }

    }


    public enum Operator {

        EQUAL,
        NOT_EQUAL,
        GREATER_THAN,
        LESS_THAN,
        GREATER_OR_EQUAL_TO,
        LESS_OR_EQUAL_TO,
        EQUAL_CASE_INSENSITIVE,
        NOT_EQUAL_CASE_INSENSITIVE,
        CONTAINS,
        DOES_NOT_CONTAIN,
        STARTS_WITH,
        DOES_NOT_START_WITH,
        ENDS_WITH,
        DOES_NOT_END_WITH,
        CONTAINS_INSENSITIVE,
        DOES_NOT_CONTAIN_INSENSITIVE,
        STARTS_WITH_INSENSITIVE,
        DOES_NOT_START_WITH_INSENSITIVE,
        ENDS_WITH_INSENSITIVE,
        DOES_NOT_END_WITH_INSENSITIVE

    }


    public static class OperatorParser {

        public static Operator? ToOperator(string str) {
            return str switch {
                "==" => Operator.EQUAL,
                "!=" => Operator.NOT_EQUAL,
                ">" => Operator.GREATER_THAN,
                "<" => Operator.LESS_THAN,
                ">=" => Operator.GREATER_OR_EQUAL_TO,
                "<=" => Operator.LESS_OR_EQUAL_TO,
                "==*" => Operator.EQUAL_CASE_INSENSITIVE,
                "!=*" => Operator.NOT_EQUAL_CASE_INSENSITIVE,
                "@" => Operator.CONTAINS,
                "!@" => Operator.DOES_NOT_CONTAIN,
                "^" => Operator.STARTS_WITH,
                "!^" => Operator.DOES_NOT_START_WITH,
                "$" => Operator.ENDS_WITH,
                "!$" => Operator.DOES_NOT_END_WITH,
                "@*" => Operator.CONTAINS_INSENSITIVE,
                "!@*" => Operator.DOES_NOT_CONTAIN_INSENSITIVE,
                "^*" => Operator.STARTS_WITH_INSENSITIVE,
                "!^*" => Operator.DOES_NOT_START_WITH_INSENSITIVE,
                "$*" => Operator.ENDS_WITH_INSENSITIVE,
                "!$*" => Operator.DOES_NOT_END_WITH_INSENSITIVE,
                _ => (Operator?) null
            };
        }

    }

}