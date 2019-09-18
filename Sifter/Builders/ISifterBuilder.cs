namespace Sifter.Builders {

    public interface ISifterBuilder {

        void IndexDbSets<T>() where T : class;

        ISifterPropertyBuilder<T> Properties<T>();

    }

}