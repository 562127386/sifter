namespace Sifter {

    internal static class StringExtensions {

        internal static bool IsNested(this string identifier) {
            return identifier.Contains('.');
        }

    }

}