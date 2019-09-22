using System.Text.RegularExpressions;


namespace Sifter.Helpers {

    internal static class Regexes {

        private const string identifierRegex = @"(?<identifier>[A-z_]\w*(?:\.[A-z_]\w*)*)";

        private const string operatorRegex = @"(?<operator>!?[@^$]\*?|[=!]=\*|[=!<>]=|[<>])";
        private const string stringVariableRegex = @"(?<string>""(?:[^\\""]|\\.)*"")";
        private const string numberVariableRegex = @"(?<number>-?[0-9]+(:?\.[0-9]*)?)";
        private const string boolVariableRegex = @"(?<bool>true|false)";
        private static readonly string sortTermRegex = $"(?<sortTerm>-?{identifierRegex})";

        private static readonly string variableRegex =
            $"(?<variable>{stringVariableRegex}|{numberVariableRegex}|{boolVariableRegex})";

        private static readonly string filterTermRegex =
            $"(?<filterTerm>{identifierRegex} *{operatorRegex} *{variableRegex})";

        public static readonly Regex SORT_REGEX = new Regex($"^{sortTermRegex}(?: *, *{sortTermRegex})*$");
        public static readonly Regex FILTER_TERM_REGEX = new Regex($"^{filterTermRegex}$");
        public static readonly Regex FILTER_REGEX = new Regex($"^{filterTermRegex}(?: *, *{filterTermRegex})*$");

    }

}