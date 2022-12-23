using System;
using System.Linq;
using System.Linq.Expressions;

namespace BSN.Resa.Core.Commons.Extentions
{
    public static class PersianToArabicStringExtention
    {
        public static Expression<Func<T, bool>> ModifyArabicToPersianString<T>(string term, params Expression<Func<T, string>>[] property)
        {
            var predicates = property.Select( p => p.Compose(value => (value
                .Replace("ي", "ی")
                .Replace("ك", "ک")
                .Replace("ى", "ی")
                .Contains(term
                .Replace("ي", "ی")
                .Replace("ك", "ک")
                .Replace("ى", "ی"))))); 

            return ExpressionExtention.OrAllPerdicatesThatSelectFromInputPramasOfProperty(predicates);
        }
    }
}
