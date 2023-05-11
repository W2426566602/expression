using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ExpressionDemo.Extend;
using System.Diagnostics;
using ExpressionDemo.MappingExtend;
using System.Reflection;

namespace ExpressionDemo
{
    /// <summary>
    /// 认识/拼装 表达式目录树
    /// 拼装表达式
    /// 应用
    /// </summary>
    public class ExpressionTest
    {
        public static void Show()
        {
            {
                #region -----------------------------------------------表达式目录树定义的两种方式---------------------------------------------------------------------
                Func<int, int, int> func = (m, n) => m * n + 2;
                //lambda实例化委托  是个方法 是实例化委托的参数
                Expression<Func<int, int, int>> exp = (m, n) => m * n + 2;
                //lambda表达式声明表达式目录树(快捷方式)，是一个数据结构
                //lambda就像声明了多个变量以及变量之间的操作啊关系的，需要的时候还能解开

                //Expression<Func<int, int, int>> exp1 = (m, n) =>
                //    {
                //        return m * n + 2;
                //    };//不能有语句体   只能是一行，不能有大括号

                //表达式目录树：语法树，或者说是一种数据结构
                int iResult1 = func.Invoke(12, 23);
                int iResult2 = exp.Compile().Invoke(12, 23);
                //exp.Compile()之后就是一个委托
                #endregion

            }

            {
                //手动拼装表达式目录树，不是用的lambda的快捷方式
                {
                    //Expression<Func<int>> expression = () => 123 + 234;
                    //int iResult = expression.Compile().Invoke();
                    ConstantExpression right = Expression.Constant(234);
                    ConstantExpression left = Expression.Constant(123);
                    BinaryExpression plus = Expression.Add(left, right);
                    Expression<Func<int>> expression = Expression.Lambda<Func<int>>(plus, new ParameterExpression[] { });
                    int iResult = expression.Compile().Invoke();
                }
                {
                    //Expression<Func<int, int, int>> expression = (m, n) => m * n + m + n + 2;
                    //int iResult = expression.Compile().Invoke(23, 34);

                    ParameterExpression m = Expression.Parameter(typeof(int), "m");
                    ParameterExpression n = Expression.Parameter(typeof(int), "n");
                    var constant = Expression.Constant(2);

                    var mutiply = Expression.Multiply(m, n);
                    var plus1 = Expression.Add(mutiply, m);
                    var plus2 = Expression.Add(plus1, n);
                    var plus3 = Expression.Add(plus2, constant);
                    Expression<Func<int, int, int>> expression = Expression.Lambda<Func<int, int, int>>(plus3, new ParameterExpression[] { m, n });
                    int iResult = expression.Compile().Invoke(23, 34);
                }
                {
                    //Expression<Func<People, bool>> lambda = x => x.Id.ToString().Equals("5");
                    ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");
                    var constantExp = Expression.Constant("5");
                    FieldInfo field = typeof(People).GetField("Id");
                    var fieldExp = Expression.Field(parameterExpression, field);
                    var toString = typeof(int).GetMethod("ToString", new Type[] { });
                    var toStringExp = Expression.Call(fieldExp, toString, new Expression[0]);
                    var equals = typeof(string).GetMethod("Equals", new Type[] { typeof(string) });
                    var equalsExp = Expression.Call(toStringExp, equals, new Expression[] { constantExp });
                    Expression<Func<People, bool>> expression = Expression.Lambda<Func<People, bool>>(equalsExp, new ParameterExpression[]
                    {
                    parameterExpression
                    });
                    bool bResult = expression.Compile()(new People()
                    {
                        Id = 5,
                        Name = "CIM",
                        Age = 28
                    });
                }

                {
                    ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");
                    var constantExp = Expression.Constant("5");
                    FieldInfo field = typeof(People).GetField("Id");
                    var fieldExp = Expression.Field(parameterExpression, field);
                    var toString = typeof(int).GetMethod("ToString", new Type[] { });
                    var toStringExp = Expression.Call(fieldExp, toString, new Expression[0]);
                    var equals = typeof(string).GetMethod("Equals", new Type[] { typeof(string) });
                    var equalsExp = Expression.Call(toStringExp, equals, new Expression[] { constantExp });
                    Expression<Action<People>> expression = Expression.Lambda<Action<People>>(equalsExp, new ParameterExpression[]
                    {
                    parameterExpression
                    });
                    expression.Compile()(new People()
                    {
                        Id = 5,
                        Name = "CIM",
                        Age = 28
                    });
                }
                {
                    //常量
                    ConstantExpression conLeft = Expression.Constant(345);
                    ConstantExpression conRight = Expression.Constant(456);
                    BinaryExpression binary = Expression.Add(conLeft, conRight);
                    Expression<Action> actExpression = Expression.Lambda<Action>(binary, null);
                    //只能执行表示Lambda表达式的表达式目录树，即LambdaExpression或者Expression<TDelegate>类型。如果表达式目录树不是表示Lambda表达式，需要调用Lambda方法创建一个新的表达式
                    actExpression.Compile()();//()=>345+456
                }
                {
                    ParameterExpression paraLeft = Expression.Parameter(typeof(int), "a");//左边
                    ParameterExpression paraRight = Expression.Parameter(typeof(int), "b");//右边
                    BinaryExpression binaryLeft = Expression.Multiply(paraLeft, paraRight);//a*b
                    ConstantExpression conRight = Expression.Constant(2, typeof(int));//右边常量
                    BinaryExpression binaryBody = Expression.Add(binaryLeft, conRight);//a*b+2

                    Expression<Func<int, int, int>> lambda =
                        Expression.Lambda<Func<int, int, int>>(binaryBody, paraLeft, paraRight);
                    Func<int, int, int> func = lambda.Compile();//Expression Compile成委托
                    int result = func(3, 4);
                }
                {
                    //Expression<Func<People, bool>> lambda = x => x.Id.ToString().Equals("5");
                    ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");
                    Expression field = Expression.Field(parameterExpression, typeof(People).GetField("Id"));
                    MethodCallExpression toString = Expression.Call(field, typeof(People).GetMethod("ToString"), new Expression[0]);
                    ConstantExpression constantExpression = Expression.Constant("5", typeof(string));

                    MethodCallExpression equals = Expression.Call(toString, typeof(People).GetMethod("Equals"), new Expression[] { constantExpression });
                    Expression<Func<People, bool>> lambda = Expression.Lambda<Func<People, bool>>(equals, new ParameterExpression[]
                    {
                    parameterExpression
                    });
                    bool bResult = lambda.Compile()(new People()
                    {
                        Id = 11,
                        Name = "CIM",
                        Age = 31
                    });
                }
            }

            //动态
            {
                {
                    ////以前根据用户输入拼装条件
                    //string sql = "SELECT * FROM USER WHERE 1=1";
                    //Console.WriteLine("用户输入个名称，为空就跳过");
                    //string name = Console.ReadLine();
                    //if (!string.IsNullOrWhiteSpace(name))
                    //{
                    //    sql += $" and name like '%{name}%'";
                    //}

                    //Console.WriteLine("用户输入个账号，为空就跳过");
                    //string account = Console.ReadLine();
                    //if (!string.IsNullOrWhiteSpace(account))
                    //{
                    //    sql += $" and account like '%{account}%'";
                    //}
                }
                {
                    {
                        //现在是Linq to SQL
                        var dbSet = new List<People>().AsQueryable();//EF DbSet

                        dbSet.Where(p => p.Id > 10 & p.Name.Contains("Eleven"));

                        Expression<Func<People, bool>> exp = null;
                        Console.WriteLine("用户输入个名称，为空就跳过");
                        string name = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            exp = p => p.Name.Contains(name);
                            //dbSet=dbSet.Where(p => p.Name.Contains(name));
                        }
                        Console.WriteLine("用户输入个最小年纪，为空就跳过");
                        string age = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(age) && int.TryParse(age, out int iAge))
                        {
                            exp = p => p.Age > iAge;
                            //dbSet = dbSet.Where(p => p => p.Age > iAge);
                        }
                        //也许都可以呢？
                        {
                            //exp= p => p.Name.Contains("Eleven") && p.Age > 5;
                        }
                    }
                    //多来几个条件，根本没法写！
                    //直接基于dbSet来计算？ 不对！ 暴露dbSet，这是一整张表，很容易出事儿
                    //1 以前有个表达式树的扩展，扩展了and和or，明天解决这个 基于visitor
                    //2 根据字符串+条件自动拼装起来！
                    {
                        //可以封装一个表达式目录树的自动生成！根据用户界面的输入
                        ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "p");
                        //if(name 不为空)
                        var name = typeof(People).GetProperty("Name");
                        var eleven = Expression.Constant("CIM", typeof(string));
                        var nameExp = Expression.Property(parameterExpression, name);
                        var contains = typeof(string).GetMethod("Contains");
                        var containsExp = Expression.Call(nameExp, contains, new Expression[] { eleven });

                        //if(Age 是输入了)
                        var age = typeof(People).GetProperty("Age");
                        var age5 = Expression.Constant(5);
                        var ageExp = Expression.Property(parameterExpression, age);
                        var greatorThan = Expression.GreaterThan(ageExp, age5);

                        //if()
                        var body = Expression.AndAlso(containsExp, greatorThan);
                        Expression<Func<People, bool>> expression = Expression.Lambda<Func<People, bool>>(body, new ParameterExpression[] { parameterExpression });
                        expression.Compile()(new People()
                        {
                            Id = 10,
                            Name = "CIM"
                        });
                    }
                }

                ////现在entity framework查询的时候，需要一个表达式目录树
                //IQueryable<int> list = null;

                //if (true)//只过滤A
                //{
                //    Expression<Func<int, bool>> exp1 = x => x > 1;
                //}
                //if (true)//只过滤B
                //{
                //    Expression<Func<int, bool>> exp2 = x => x > 2;
                //}
                ////都过滤
                //Expression<Func<int, bool>> exp3 = x => x > 1 && x > 2;
                ////list.Where()

                ////拼装表达式目录树，交给下端用
                ////Expression<Func<People, bool>> lambda = x => x.Age > 5;
                //ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");
                //Expression propertyExpression = Expression.Property(parameterExpression, typeof(People).GetProperty("Age"));
                ////Expression property = Expression.Field(parameterExpression, typeof(People).GetField("Id"));
                //ConstantExpression constantExpression = Expression.Constant(5, typeof(int));
                //BinaryExpression binary = Expression.GreaterThan(propertyExpression, constantExpression);//添加方法的
                //Expression<Func<People, bool>> lambda = Expression.Lambda<Func<People, bool>>(binary, new ParameterExpression[]
                //{
                //    parameterExpression
                //});
                //bool bResult = lambda.Compile()(new People()
                //{
                //    Id = 11,
                //    Name = "Eleven",
                //    Age = 31
                //});
            }


        }

        public static void MapperTest()
        {
            {
                //People people = new People()
                //{
                //    Id = 11,
                //    Name = "Eleven",
                //    Age = 31
                //};
                ////PeopleCopy copy = (PeopleCopy)people;

                //PeopleCopy peopleCopy = new PeopleCopy()
                //{
                //    Id = people.Id,
                //    Name = people.Name,
                //    Age = people.Age
                //};
                //////假如程序中有很多这样的转换，为每个类型都这样硬编码？！
                ////PeopleCopy peopleCopy1=ReflectionMapper.Trans<People, PeopleCopy>(people);//1 反射
                ////PeopleCopy peopleCopy2=SerializeMapper.Trans<People, PeopleCopy>(people); //2 序列化反序列化
                //////既能通用，又能性能好
                ////Func<People, PeopleCopy> func = p => new PeopleCopy()
                ////{
                ////    Id = p.Id,
                ////    Name = p.Name,
                ////    Age = p.Age
                ////};
                ////PeopleCopy peopleCopy3 = func.Invoke(people);
                ////想办法去动态拼装这个委托，然后缓存下委托，后面再次转换时就没有性能损耗了
                ////PeopleCopy peopleCopy4 = ExpressionMapper.Trans<People, PeopleCopy>(people);
                ////PeopleCopy peopleCopy5 = ExpressionMapper.Trans<People, PeopleCopy>(people);

                ////PeopleCopy peopleCopy6 = ExpressionGenericMapper<People, PeopleCopy>.Trans(people);
                ////PeopleCopy peopleCopy7 = ExpressionGenericMapper<People, PeopleCopy>.Trans(people);

                ////Expression<Func<People, PeopleCopy>> lambda = p =>
                ////        new PeopleCopy()
                ////        {
                ////            Id = p.Id,
                ////            Name = p.Name,
                ////            Age = p.Age
                ////        };
                ////lambda.Compile()(people);

                //ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "p");
                //List<MemberBinding> memberBindingList = new List<MemberBinding>();
                //foreach (var item in typeof(PeopleCopy).GetProperties())
                //{
                //    MemberExpression property = Expression.Property(parameterExpression, typeof(People).GetProperty(item.Name));
                //    MemberBinding memberBinding = Expression.Bind(item, property);
                //    memberBindingList.Add(memberBinding);
                //}
                //foreach (var item in typeof(PeopleCopy).GetFields())
                //{
                //    MemberExpression property = Expression.Field(parameterExpression, typeof(People).GetField(item.Name));
                //    MemberBinding memberBinding = Expression.Bind(item, property);
                //    memberBindingList.Add(memberBinding);
                //}
                //MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(PeopleCopy)), memberBindingList.ToArray());
                //Expression<Func<People, PeopleCopy>> lambda = Expression.Lambda<Func<People, PeopleCopy>>(memberInitExpression, new ParameterExpression[]
                //{
                //    parameterExpression
                //});
                //Func<People, PeopleCopy> func = lambda.Compile();
                //PeopleCopy copy = func(people);
            }

            {
                People people = new People()
                {
                    Id = 11,
                    Name = "CIM",
                    Age = 31
                };
                long common = 0;
                long generic = 0;
                long cache = 0;
                long reflection = 0;
                long serialize = 0;
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    for (int i = 0; i < 1_000_000; i++)
                    {
                        PeopleCopy peopleCopy = new PeopleCopy()
                        {
                            Id = people.Id,
                            Name = people.Name,
                            Age = people.Age
                        };
                    }
                    watch.Stop();
                    common = watch.ElapsedMilliseconds;
                }
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    for (int i = 0; i < 1_000_000; i++)
                    {
                        PeopleCopy peopleCopy = ReflectionMapper.Trans<People, PeopleCopy>(people);
                    }
                    watch.Stop();
                    reflection = watch.ElapsedMilliseconds;
                }
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    for (int i = 0; i < 1_000_000; i++)
                    {
                        PeopleCopy peopleCopy = SerializeMapper.Trans<People, PeopleCopy>(people);
                    }
                    watch.Stop();
                    serialize = watch.ElapsedMilliseconds;
                }
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    for (int i = 0; i < 1_000_000; i++)
                    {
                        PeopleCopy peopleCopy = ExpressionMapper.Trans<People, PeopleCopy>(people);
                    }
                    watch.Stop();
                    cache = watch.ElapsedMilliseconds;
                }
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    for (int i = 0; i < 1_000_000; i++)
                    {
                        PeopleCopy peopleCopy = ExpressionGenericMapper<People, PeopleCopy>.Trans(people);
                    }
                    watch.Stop();
                    generic = watch.ElapsedMilliseconds;
                }

                Console.WriteLine($"common = { common} ms");
                Console.WriteLine($"reflection = { reflection} ms");
                Console.WriteLine($"serialize = { serialize} ms");
                Console.WriteLine($"cache = { cache} ms");
                Console.WriteLine($"generic = { generic} ms");
                //性能比automapper

            }
            //既需要动态(通用)，又要保证性能(硬编码)---动态生成硬编码---表达式目录树拼装！！
        }


    }
}
