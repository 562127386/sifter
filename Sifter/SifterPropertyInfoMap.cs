﻿using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;


namespace Sifter {

    internal class SifterPropertyInfoMap : Dictionary<string, SifterInfo> {

        [CanBeNull]
        internal SifterInfo Get(string propName) {
            TryGetValue(propName, out var value);
            return value;
        }
        
        internal void MergeWith(SifterPropertyInfoMap from) {
            foreach (var (propName, sifterInfo) in from) {
                TryAdd(propName, sifterInfo);
            }
        }
    }

}