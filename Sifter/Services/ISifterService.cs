using System.Linq;
using JetBrains.Annotations;
using Sifter.Models;


namespace Sifter.Services {

    [PublicAPI]
    public interface ISifterService {

        [NotNull]
        IQueryable<T> Sift<T>([NotNull] IQueryable<T> query, [NotNull] SifterModel model);

    }

}