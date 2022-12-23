using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BSN.Resa.Core.Commons
{
    public static class ExpressionExtention
    {
        //this method is extention for left.OR(right) make left exprition or with right exprition 
        public static Expression<Func<T, bool>> OR<T>(this Expression<Func<T, Boolean>> left, Expression<Func<T, Boolean>> right)
        {
            Expression<Func<T, Boolean>> combined = Expression.Lambda<Func<T, Boolean>>(
                Expression.Or(
                    left.Body,
                    new ExpressionParameterReplacer(right.Parameters, left.Parameters).Visit(right.Body)
                    ), left.Parameters);

            return combined;
        }

        //this method is for or operation between all perdicate in input parameter 
        public static Expression<Func<T, bool>> OrAllPerdicatesThatSelectFromInputPramasOfProperty<T>(IEnumerable<Expression<Func<T, bool>>> predicates)
        {
            var parameter = Expression.Parameter(typeof(T));
            var newBody = predicates.Select(predicate => predicate.Body.ReplaceParameter(predicate.Parameters[0], parameter))
                .DefaultIfEmpty(Expression.Constant(false))
                .Aggregate((a, b) => Expression.OrElse(a, b));

            return Expression.Lambda<Func<T, bool>>(newBody, parameter);
        }

        //this method compose extention for exprition for make  func of TSourceT and TIntermediate prarameter and 
        //func of TIntermediate and TResult parameter to func of TSource and TResult parameter
        //for example compose make Func<Doctor,string > and Func<string,bool> to Func<doctor,bool>
        public static Expression<Func<TSource, TResult>> Compose<TSource, TIntermediate, TResult>(
         this Expression<Func<TSource, TIntermediate>> first,
         Expression<Func<TIntermediate, TResult>> second)
        {
            var param = Expression.Parameter(typeof(TSource));
            var intermediateValue = first.Body.ReplaceParameter(first.Parameters[0], param);
            var body = second.Body.ReplaceParameter(second.Parameters[0], intermediateValue);

            return Expression.Lambda<Func<TSource, TResult>>(body, param);
        }

        public static Expression ReplaceParameter(this Expression expression,
           ParameterExpression toReplace,
           Expression newExpression)
        {
            return new ParameterReplaceVisitor(toReplace, newExpression)
                .Visit(expression);
        }
    }
}
