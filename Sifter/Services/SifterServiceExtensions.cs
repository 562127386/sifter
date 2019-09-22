using System.Linq;
using JetBrains.Annotations;
using Sifter.Models;


namespace Sifter.Services {

    [PublicAPI]
    public static class SifterServiceExtensions {

        [NotNull]
        public static IQueryable<T> Sift<T>(
            [NotNull] this IQueryable<T> query,
            [NotNull] ISifterService sifter,
            [NotNull] SifterModel sifterModel
        ) {
            return sifter.Sift(query, sifterModel);
        }

    }

}