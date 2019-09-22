using System;
using System.Linq.Expressions;
using System.Reflection;
using Sifter.Models;


namespace Sifter.Builders {

    internal class SifterPropertyBuilder<TClass> : ISifterPropertyBuilder<TClass> {

        public readonly SifterPropertyInfoMap map = new SifterPropertyInfoMap();



        public ISifterPropertyBuilder<TClass> CanFilter<TProp>(Expression<Func<TClass, TProp>> expression) {
            addToMap(expression, true, false);
            return this;
        }



        public ISifterPropertyBuilder<TClass> CanSort<TProp>(Expression<Func<TClass, TProp>> expression) {
            addToMap(expression, false, true);
            return this;
        }



        public ISifterPropertyBuilder<TClass> CanFilterAndSort<TProp>(Expression<Func<TClass, TProp>> expression) {
            addToMap(expression, true, true);
            return this;
        }



        private void addToMap<TProp>(Expression<Func<TClass, TProp>> expression, bool canFilter, bool canSort) {
            var propertyInfo = getPropertyInfo(expression);
            map.Add(
                propertyInfo.Name.ToLowerInvariant(),
                new SifterInfo {
                    PropertyInfo = propertyInfo,
                    CanFilter = canFilter,
                    CanSort = canSort
                }
            );
        }



        private static PropertyInfo getPropertyInfo<TProp>(Expression<Func<TClass, TProp>> expression) {
            var classType = typeof(TClass);
            var memberExpression = expression.Body as MemberExpression;
            var propInfo = memberExpression?.Member as PropertyInfo;

            if (propInfo == null || propInfo.DeclaringType == null) {
                throw new Exception(); //TODO throw more specific error
            }

            if (classType != propInfo.DeclaringType && !propInfo.DeclaringType.IsAssignableFrom(classType)) {
                throw new Exception();
            }

            return propInfo;
        }

    }

}