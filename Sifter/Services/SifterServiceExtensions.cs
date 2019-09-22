using System.Linq;
using JetBrains.Annotations;
using Sifter.Models;


namespace Sifter.Services {

    
    //TODO add extension method for registering the service to the container
    
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