using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;


namespace Sifter {

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