using ExpressionDemo.MappingExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionDemo
{
    /// <summary>
    /// 1 什么是表达式目录树Expression
    /// 2 动态拼装Expression
    /// 3 基于Expression扩展应用
    /// 
    /// 1 ExpressionVisitor解析表达式目录树
    /// 2 解析Expression生成Sql
    /// 3 Expression扩展拼装链接
    /// 
    /// Sql查询的时候，各种查询条件呢？没有实现  写不出来，
    /// 属性可能是ID  name  account state
    /// 值 可能是int string  datetime
    /// 大于 小于  等于  包含
    /// 一个条件  2个条件 N个条件
    /// 不要说传递sql语句id=3
    /// 土办法  封装了一个对象   column  operation  value     接受一个集合
    /// 表达式目录树，数据结构
    /// 调用方会有各种的条件需要传递下去
    /// 底层需要解析调用方传递的东西
    /// 所以需要一个数据结构(语法/约定)，上端去组装，下端去解析
    /// 
    /// 表达式目录树就是数据结构！！！ 一个能拼装能解析的数据结构
    /// 
    /// 
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                People people = new People()
                {
                    Id = 11,
                    Name = "CIM",
                    Age = 31
                };
                PeopleCopy peopleCopy = ExpressionGenericMapper<People, PeopleCopy>.Trans(people);
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
                   //ExpressionVisitorTest.Show();
                    //ExpressionVisitor访问者类，Visit是个入口，解读表达式的入口
                    //1 lambda会区分参数和方法体，调度到更专业访问(处理)方法
                    //2 根据表达式的类型，将表达式调度到此类中更专用的访问方法之一的表达式
                    //默认的专业处理方法就是按照旧的模式产生一个新的
                    //也可以自行扩展，把一些解读操作进行变化
                    //表达式目录树是个二叉树，visit做的事儿就是一层层的解析下去，一直到最终的叶节点
                }
                //用linq to sql
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
