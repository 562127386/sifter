using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;


namespace Sifter.Builders {

    [PublicAPI]
    public interface ISifterBuilder {

        void IndexDbSets<T>() where T : DbContext;

        ISifterPropertyBuilder<T> Properties<T>();

    }

}