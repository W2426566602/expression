using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTree
{
    internal class Program
    {
        private static Func<TSource, TTarget> GetMap<TSource, TTarget>()
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            //构造 p=>
            var parameterExpression = Expression.Parameter(sourceType, "p");

            //构造 p=>new TTarget{ Id=p.Id,Name=p.Name };
            var memberBindingList = new List<MemberBinding>();
            foreach (var sourceItem in sourceType.GetProperties())
            {
                var targetItem = targetType.GetProperty(sourceItem.Name);
                if (targetItem == null || sourceItem.PropertyType != targetItem.PropertyType)
                    continue;

                var property = Expression.Property(parameterExpression, sourceItem);
                var memberBinding = Expression.Bind(targetItem, property);
                memberBindingList.Add(memberBinding);
            }
            var memberInitExpression = Expression.MemberInit(Expression.New(targetType), memberBindingList);

            var lambda = Expression.Lambda<Func<TSource, TTarget>>(memberInitExpression, parameterExpression);

            Console.WriteLine(lambda);
            return lambda.Compile();
        }

        static void Main(string[] args)
        {
          
            IEnumerable<string> a1=new List<string>();
            Func<string> a = ()=>"hello world";
            Console.WriteLine(a.Invoke());
           Expression<Func<string>> exception=()=> "hello world";
             
            ParameterExpression parameterx = Expression.Parameter(typeof(int), "x");
            ParameterExpression parametery = Expression.Parameter(typeof(int), "y");
            ConstantExpression constantExpression = Expression.Constant(2, typeof(int));
            BinaryExpression binaryAdd = Expression.Add(parameterx, parametery);
            BinaryExpression binarySubtract = Expression.Subtract(binaryAdd, constantExpression);
            Expression<Func<int, int, int>> expressionMosaic = Expression.Lambda<Func<int, int, int>>(binarySubtract, new ParameterExpression[]
            {
       parameterx,
       parametery
            });

            int ResultMosaic = expressionMosaic.Compile()(5, 2);
            Console.WriteLine(exception.Compile().Invoke());
        }


    }
    
    public class student
    {
        public int age { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
    public class studentOutputDto
    {
        public int age { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
    public class TestA
      {
          public int Id { get; set; }
        public string Name { get; set; }

        public TestC TestClass { get; set; }

        public IEnumerable<TestC> TestLists { get; set; }
    }

    public class TestB
    {
       public int Id { get; set; }
        public string Name { get; set; }

         public TestD TestClass { get; set; }

         public TestD[] TestLists { get; set; }
     }

     public class TestC
     {
  public int Id { get; set; }
         public string Name { get; set; }

         public TestC SelfClass { get; set; }
     }

     public class TestD
     {
    public int Id { get; set; }
         public string Name { get; set; }

         public TestD SelfClass { get; set; }
     }
}
