namespace Sifter.Extensions {

    internal static class StringExtensions {

        internal static bool IsNested(this string identifier) {
            return identifier.Contains('.');
        }
        
        internal static Operator? ToOperator(this string str) {
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