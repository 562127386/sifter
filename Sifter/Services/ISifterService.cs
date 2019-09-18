using System.Linq;
using JetBrains.Annotations;


namespace Sifter.Services {

    public interface ISifterService {

        [NotNull]
        IQueryable<T> Sift<T>([NotNull] IQueryable<T> query, [NotNull] SifterModel model);

    }

}