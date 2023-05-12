using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionDemo
{
    /// <summary>
    /// 展示表达式树，协助用的
    /// 编译lambda--反编译C#--得到原始声明方式
    /// </summary>
    public class ExpressionTreeVisualizer
    {
        public static void Show()
        {
            //Expression<Func<int>> expression = () => 123 + 234;
            //Expression<Func<int>> expression1 = Expression.Lambda<Func<int>>(Expression.Constant(357, typeof(int)), new ParameterExpression[0]);
            //Expression<Func<int, int, int>> expression = (m, n) => m * n + m + n + 2;

            //Expression<Func<People, bool>> lambda = x => x.Age > 5;
            //Expression<Func<People, bool>> lambda = x => x.Id.ToString().Equals("5");
            //Expression<Func<People, PeopleCopy>> lambda = p =>
            //            new PeopleCopy()
            //            {
            //                Id = p.Id,
            //                Name = p.Name,
            //                Age = p.Age
            //            };
            Expression<Func<People, bool>> lambda = p => p.Name.Contains("CIM") && p.Age > 5;
        }
    }
}
