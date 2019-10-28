namespace Sifter {

    //TODO refactor this
    public static class StringExtensions {

        public static string ApplyCaseSensitivity(this string str) {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return Constants.CASE_SENSITIVE_PROPERTY_MATCHING ? str : str.ToLowerInvariant();
        }

    }

}