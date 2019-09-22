using System;
using System.Linq.Expressions;
using JetBrains.Annotations;


namespace Sifter.Builders {

    [PublicAPI]
    public interface ISifterPropertyBuilder<TClass> {

        ISifterPropertyBuilder<TClass> CanFilter<TProp>(Expression<Func<TClass, TProp>> expression);

        ISifterPropertyBuilder<TClass> CanSort<TProp>(Expression<Func<TClass, TProp>> expression);

        ISifterPropertyBuilder<TClass> CanFilterAndSort<TProp>(Expression<Func<TClass, TProp>> expression);

    }

}