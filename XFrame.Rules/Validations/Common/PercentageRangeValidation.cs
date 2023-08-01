using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFrame.Rules.Validations.Common
{
    public class PercentageRangeValidation<T> 
        : DecimalRangeValidation<T>, IRangeValidation where T : class
    {
        #region Virtual Members

        protected override decimal? OnGetMinimum()
        {
            return 0.0m;
        }

        protected override decimal? OnGetMaximum()
        {
            return 100.0m;
        }

        #endregion
    }

    public class PercentageRangeValidation<T, C> 
        : PercentageRangeValidation<T>
        where T : class
        where C : class
    {
        #region Methods

        public C GetContext()
        {
            return (C)Context;
        }

        #endregion
    }
}
