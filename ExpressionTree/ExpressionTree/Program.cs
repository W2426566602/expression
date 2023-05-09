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
        static void Main(string[] args)
        {
            Func<string> a = ()=>"hello world";
            Console.WriteLine(a.Invoke());
           Expression<Func<string>> exception=()=> "hello world";
            
            Console.WriteLine(exception.Compile().Invoke());
        }
    }
}
