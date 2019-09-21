using Microsoft.EntityFrameworkCore;


namespace Sifter.Builders {

    public interface ISifterBuilder {

        void IndexDbSets<T>() where T : DbContext;

        ISifterPropertyBuilder<T> Properties<T>();

    }

}