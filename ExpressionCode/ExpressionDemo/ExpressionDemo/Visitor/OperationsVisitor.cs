using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionDemo.Visitor
{
    /// <summary>
    /// 表达式目录树访问者扩展，遇到Add换成Subtract
    /// </summary>
    public class OperationsVisitor : ExpressionVisitor
    {
        public Expression Modify(Expression expression)
        {
            return this.Visit(expression);
        }

        /// <summary>
        /// Visit是个入口，解读node表示式
        /// 根据表达式的类型，将表达式调度到此类中更专用的访问方法之一的表达式
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override Expression Visit(Expression node)
        {
            Console.WriteLine($"Visit {node.ToString()}");
            return base.Visit(node);
        }
        protected override Expression VisitBinary(BinaryExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            if (b.NodeType == ExpressionType.Add)
            {
                return Expression.Subtract(left, right);
            }
            else if (b.NodeType == ExpressionType.Multiply)
            {
                //Expression left = this.Visit(b.Left);//可能还是需要进一步解析的
                //Expression right = this.Visit(b.Right);
                return Expression.Divide(left, right);

                //return Expression.Divide(b.Left, b.Right);
            }
            Expression result = base.VisitBinary(b);//默认的二元访问，其实什么都不干
            return result;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return base.VisitConstant(node);
        }
    }
}
