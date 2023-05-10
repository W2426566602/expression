using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionDemo
{
    /// <summary>
    /// 能听见我说话的，能看到我桌面的，打个1
    /// 高级班的传统，准备好学习的小伙伴儿，给Eleven老师刷个字母E，然后课程就正式开始了！！！
    /// 
    /// 1 什么是表达式目录树Expression
    /// 2 动态拼装Expression
    /// 3 基于Expression扩展应用
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("欢迎来到.net高级班vip课程，今晚学习表达式树Expression");
                //{
                //    new List<int>().Where(i => i > 10);
                //    new List<int>().AsQueryable().Where(i => i > 10);
                //}
                {
                    Console.WriteLine("****************认识表达式目录树*************");
                    //ExpressionTest.Show();
                }
                {
                    Console.WriteLine("********************MapperTest********************");
                    ExpressionTest.MapperTest();
                }
                {
                    Console.WriteLine("********************解析表达式目录树********************");
                    ExpressionVisitorTest.Show();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
