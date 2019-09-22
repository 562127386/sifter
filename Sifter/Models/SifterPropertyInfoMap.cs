using System.Collections.Generic;
using JetBrains.Annotations;


namespace Sifter.Models {

    internal class SifterPropertyInfoMap : Dictionary<string, SifterInfo> {

        [CanBeNull]
        public SifterInfo Get(string propName) {
            TryGetValue(propName, out var value);
            return value;
        }



        public void MergeWith(SifterPropertyInfoMap from) {
            foreach (var (propName, sifterInfo) in from) {
                TryAdd(propName, sifterInfo);
            }
        }

    }

}