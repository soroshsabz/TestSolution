using System.Linq.Expressions;

namespace BSN.Resa.Core.Commons
{
    public class ParameterReplaceVisitor : ExpressionVisitor
    {
        private ParameterExpression from;
        private Expression to;

        public ParameterReplaceVisitor(ParameterExpression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        //The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == from ? to : base.Visit(node);
        }
    }
}
