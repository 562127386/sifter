using System.Reflection;


namespace Sifter {

    internal class SifterInfo {

        public PropertyInfo PropertyInfo { get; set; }

        public bool CanFilter { get; set; }
        public bool CanSort { get; set; }

    }

}