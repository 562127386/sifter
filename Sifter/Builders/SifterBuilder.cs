﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Sifter.Models;


namespace Sifter.Builders {

    internal class SifterBuilder : ISifterBuilder {

        private readonly HashSet<Type> indexedTypes = new HashSet<Type>();

        private readonly SifterMap sifterMap = new SifterMap();



        public void IndexDbSets<T>() where T : DbContext {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            var properties = typeof(T).GetProperties(bindingFlags);
            var types = properties.Select(p => p.PropertyType.GenericTypeArguments.First());

            foreach (var type in types) {
                indexedTypes.Add(type);
            }
        }



        public ISifterPropertyBuilder<T> Properties<T>() {
            var propertyBuilder = new SifterPropertyBuilder<T>();
            sifterMap.Add(typeof(T), propertyBuilder.map);

            return propertyBuilder;
        }



        public (SifterMap, TypeTree) Build() {
            var map = resolveIndexedTypes();
            var typeTree = SifterBuilder.typeTree(map);
            return (map, typeTree);
        }



        private static TypeTree typeTree(SifterMap map) {
            var typeTree = new TypeTree();

            foreach (var (type, _) in map) {
                const BindingFlags bindingFlags =
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
                var properties = type.GetProperties(bindingFlags);

                foreach (var property in properties) {
                    var propertyType = property.PropertyType;

                    if (map.Keys.Contains(propertyType)) {
                        typeTree.Add((type, property.Name), propertyType);
                    }
                }
            }

            return typeTree;
        }



        private SifterMap resolveIndexedTypes() {
            var map = new SifterMap();

            foreach (var (type, sifterPropertyInfoMap) in sifterMap) {
                var children = indexedTypes.Where(i => i.IsSubclassOf(type));

                foreach (var childType in children) {
                    if (map.TryAdd(childType, sifterPropertyInfoMap)) {
                        continue;
                    }

                    map[childType].MergeWith(sifterPropertyInfoMap);
                }

                if (!map.TryAdd(type, sifterPropertyInfoMap)) {
                    map[type].MergeWith(sifterPropertyInfoMap);
                }
            }

            return map;
        }

    }

}