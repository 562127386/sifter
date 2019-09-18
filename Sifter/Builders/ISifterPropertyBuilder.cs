using System;
using System.Linq.Expressions;


namespace Sifter.Builders {

    public interface ISifterPropertyBuilder<TClass> {

        ISifterPropertyBuilder<TClass> CanFilter<TProp>(Expression<Func<TClass, TProp>> expression);

        ISifterPropertyBuilder<TClass> CanSort<TProp>(Expression<Func<TClass, TProp>> expression);

        ISifterPropertyBuilder<TClass> CanFilterAndSort<TProp>(Expression<Func<TClass, TProp>> expression);

    }

}