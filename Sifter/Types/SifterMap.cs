using System;
using System.Collections.Generic;
using JetBrains.Annotations;


namespace Sifter.Types {

    internal class SifterMap : Dictionary<Type, SifterPropertyInfoMap> {

        [CanBeNull]
        public SifterPropertyInfoMap Get(Type type) {
            if (type == null) {
                return null;
            }

            TryGetValue(type, out var value);
            return value;
        }

    }

}