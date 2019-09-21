using Sifter.Builders;
using Sifter.Services;


namespace Tests.Helpers {

    public class MySifterService : SifterService {

        protected override void onSifterBuild(ISifterBuilder builder) {
            builder.IndexDbSets<DataContext>();

            builder.Properties<User>()
                .CanFilterAndSort(u => u.Id)
                .CanFilterAndSort(u => u.Balance);

            builder.Properties<SubUser>()
                .CanFilterAndSort(s => s.SubUserProp);

            builder.Properties<Inner>()
                .CanFilterAndSort(c => c.Id)
                .CanFilterAndSort(c => c.Number);
        }

    }

}