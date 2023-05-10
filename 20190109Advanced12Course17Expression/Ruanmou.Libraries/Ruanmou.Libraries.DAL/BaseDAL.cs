using Ruanmou.Framework;
using Ruanmou.Framework.AttributeExtend;
using Ruanmou.Framework.Model;
using Ruanmou.Libraries.IDAL;
using Ruanmou.Libraries.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ruanmou.Libraries.DAL
{
    public class BaseDAL : IBaseDAL
    {
        /// <summary>
        /// 约束是为了正确的调用，才能int id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public T Find<T>(int id) where T : BaseModel
        {
            Type type = typeof(T);
            //string columnString = string.Join(",", type.GetProperties().Select(p => $"[{p.GetColumnName()}]"));
            //string sql = $"SELECT {columnString} FROM [{type.Name}] WHERE Id={id}";
            string sql = $"{TSqlHelper<T>.FindSql}{id};";
            T t = null;// (T)Activator.CreateInstance(type);
            Func<SqlCommand, T> func = new Func<SqlCommand, T>(command =>
            {
                SqlDataReader reader = command.ExecuteReader();
                List<T> list = this.ReaderToList<T>(reader);
                T tResult = list.FirstOrDefault();
                return tResult;
            });
            t = this.ExcuteSql<T>(sql, func);
            //using (SqlConnection conn = new SqlConnection(StaticConstant.SqlServerConnString))
            //{
            //    SqlCommand command = new SqlCommand(sql, conn);
            //    conn.Open();
            //    SqlDataReader reader = command.ExecuteReader();
            //    List<T> list = this.ReaderToList<T>(reader);
            //    t = list.FirstOrDefault();
            //    //if (reader.Read())//表示有数据  开始读
            //    //{
            //    //    foreach (var prop in type.GetProperties())
            //    //    {
            //    //        prop.SetValue(t, reader[prop.Name] is DBNull ? null : reader[prop.Name]);
            //    //    }
            //    //}
            //}
            return t;
        }
        //还有查询条件呢？ 没有实现  写不出来，
        //属性可能是ID  name  account state
        //值 可能是int string  datetime
        //大于 小于  等于  包含
        //一个条件  2个条件 N个条件

        //不要说传递sql语句id=3
        //土办法  封装了一个对象   column  operation  value     接受一个集合
        //表达式目录树，数据结构
        //调用方会有各种的条件需要传递下去
        //底层需要解析调用方传递的东西
        //所以需要一个数据结构(语法/约定)，上端去组装，下端去解析
        public List<T> FindAll<T>() where T : BaseModel
        {
            Type type = typeof(T);
            string sql = TSqlHelper<T>.FindAllSql;
            List<T> list = new List<T>();
            Func<SqlCommand, List<T>> func = command =>
              {
                  SqlDataReader reader = command.ExecuteReader();
                  List<T> listResult = this.ReaderToList<T>(reader);
                  return listResult;
              };
            //using (SqlConnection conn = new SqlConnection(StaticConstant.SqlServerConnString))
            //{
            //    SqlCommand command = new SqlCommand(sql, conn);
            //    conn.Open();
            //    SqlDataReader reader = command.ExecuteReader();
            //    list = this.ReaderToList<T>(reader);
            //}
            list = this.ExcuteSql<List<T>>(sql, func);
            return list;
        }

        ////Id>5   Age>5
        //public List<T> FindWhere<T>(Expression<Func<T, bool>> expression)
        //{
        //    //expression=x=>x.Id>5&&x.Age<10
        //}


        public void Update<T>(T t) where T : BaseModel
        {
            if (!t.Validate<T>())
            {
                throw new Exception("数据不正确");
            }

            Type type = typeof(T);
            var propArray = type.GetProperties().Where(p => !p.Name.Equals("Id"));
            string columnString = string.Join(",", propArray.Select(p => $"[{p.GetColumnName()}]=@{p.GetColumnName()}"));
            var parameters = propArray.Select(p => new SqlParameter($"@{p.GetColumnName()}", p.GetValue(t) ?? DBNull.Value)).ToArray();
            //必须参数化  否则引号？  或者值里面还有引号
            string sql = $"UPDATE [{type.Name}] SET {columnString} WHERE Id={t.Id}";

            Func<SqlCommand, int> func = command =>
              {
                  command.Parameters.AddRange(parameters);
                  int iResult = command.ExecuteNonQuery();
                  return iResult;
              };

            //using (SqlConnection conn = new SqlConnection(StaticConstant.SqlServerConnString))
            //{
            //    using (SqlCommand command = new SqlCommand(sql, conn))
            //    {
            //        command.Parameters.AddRange(parameters);
            //        conn.Open();
            //        int iResult = command.ExecuteNonQuery();
            //        if (iResult == 0)
            //            throw new Exception("Update数据不存在");
            //    }
            //}

            int i = this.ExcuteSql<int>(sql, func);
            if (i == 0)
                throw new Exception("Update数据不存在");
        }

        //多个方法里面重复对数据库的访问  想通过委托解耦，去掉重复代码
        private T ExcuteSql<T>(string sql, Func<SqlCommand, T> func)
        {
            using (SqlConnection conn = new SqlConnection(StaticConstant.SqlServerConnString))
            {
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    conn.Open();
                    SqlTransaction sqlTransaction = conn.BeginTransaction();
                    try
                    {
                        command.Transaction = sqlTransaction;
                        //command.Parameters.AddRange(parameters);
                        T tResult = func.Invoke(command);
                        sqlTransaction.Commit();
                        return tResult;
                        //int iResult = command.ExecuteNonQuery();
                        //if (iResult == 0)
                        //    throw new Exception("Update数据不存在");

                    }
                    catch (Exception ex)
                    {
                        sqlTransaction.Rollback();
                        throw;
                    }
                }
            }
        }



        /// <summary>
        /// 自己完成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Insert<T>(T t) where T : BaseModel
        {
        }
        /// <summary>
        /// 自己完成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        public void Delete<T>(int id) where T : BaseModel
        {
        }
        #region PrivateMethod
        private List<T> ReaderToList<T>(SqlDataReader reader) where T : BaseModel
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            while (reader.Read())//表示有数据  开始读
            {
                T t = (T)Activator.CreateInstance(type);
                foreach (var prop in type.GetProperties())
                {
                    object oValue = reader[prop.GetColumnName()];
                    if (oValue is DBNull)
                        oValue = null;
                    prop.SetValue(t, oValue);//除了guid和枚举
                }
                list.Add(t);
            }
            return list;
        }
        #endregion

    }
}
