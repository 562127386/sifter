// ReSharper disable All

namespace Tests.Helpers {

    public class User {

        public int Id { get; set; }
        public string SteamId { get; set; }
        public int Balance { get; set; }
        public Inner Inner { get; set; }
        public int InnerId { get; set; }
        public int NotSortable { get; set; }

    }


    public class SubUser : User {

        public int SubUserProp { get; set; }

    }


    public class Inner {

        public int Id { get; set; }
        public int Number { get; set; }

    }

}