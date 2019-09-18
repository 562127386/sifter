namespace Sifter {

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

}