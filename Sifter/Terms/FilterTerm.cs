using JetBrains.Annotations;
using Sifter.Extensions;


namespace Sifter.Terms {

//TODO this currently has no support for OR/combining statements, maybe add later
    internal class FilterTerm {

        [CanBeNull]
        public string Identifier { get; }

        public Operator? Operator { get; }

        [CanBeNull]
        public string Variable { get; }



        public FilterTerm(string str) {
            var match = Regexes.FILTER_TERM_REGEX.Match(str);
            Identifier = match.Groups["identifier"].Value;
            Operator = match.Groups["operator"].Value.ToOperator();
            Variable = match.Groups["variable"].Value;
        }

    }

}