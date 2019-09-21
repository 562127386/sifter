using Sifter.Builders;
using Sifter.Services;


namespace Tests.Helpers {

    public class MySifterService : SifterService {

        protected override void onSifterBuild(ISifterBuilder builder) {
            builder.IndexDbSets<DataContext>();
//TODO add option to register all properties of a class
            builder.Properties<User>()
                .CanFilterAndSort(u => u.Id)
                .CanFilterAndSort(u => u.Balance)
                .CanFilterAndSort(u => u.SteamId);

            builder.Properties<SubUser>()
                .CanFilterAndSort(s => s.SubUserProp);

            builder.Properties<Inner>()
                .CanFilterAndSort(c => c.Id)
                .CanFilterAndSort(c => c.Number);
        }

    }

}