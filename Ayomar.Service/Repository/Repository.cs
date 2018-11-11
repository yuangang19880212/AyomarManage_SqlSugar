using Ayomar.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ayomar.Service
{
    /// <summary>
    /// Describe：仓储实现类
    /// Author：YuanGang
    /// Date：2016/07/16
    /// Blogs:http://www.cnblogs.com/yuangang
    /// </summary>
    /// <typeparam name="T">实体模型</typeparam>
    public class Repository<T> : AppDbContext, IRepository<T> where T:class,new()
    {
        public SqlSugar.SqlSugarClient GetDb()
        {
            return Db;
        }

        #region Add
        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        public virtual async Task<bool> SaveAsync(T entity)
        {
            return await Db.Insertable(entity).With(SqlSugar.SqlWith.UpdLock).ExecuteCommandAsync() > 0;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public virtual async Task<bool> SaveAsync(List<T> entitys)
        {
            return await Db.Insertable(entitys.ToArray()).With(SqlSugar.SqlWith.UpdLock).ExecuteCommandAsync() > 0;
        }
        #endregion

        #region Delete
        /// <summary>
        /// 根据主键 删除一条记录
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(string Guid)
        {
            return await Db.Deleteable<T>(Guid).ExecuteCommandAsync() > 0;
        }
        /// <summary>
        /// 根据主键 批量删除一条记录
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(string[] Guids)
        {
            return await Db.Deleteable<T>(Guids).ExecuteCommandAsync() > 0;
        }
        /// <summary>
        /// 根据表达式 删除记录
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            return await Db.Deleteable<T>(predicate).ExecuteCommandAsync() > 0;
        }
        #endregion

        #region Update
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            return await Db.Updateable(entity).With(SqlSugar.SqlWith.UpdLock).ExecuteCommandAsync() > 0;
        }
        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(List<T> entitys)
        {
            return await Db.Updateable(entitys).With(SqlSugar.SqlWith.UpdLock).ExecuteCommandAsync() > 0;
        }
        #endregion

        #region Select
        /// <summary>
        /// 查询是否存在
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> IsAnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await Db.Queryable<T>().Where(predicate).AnyAsync();
        }
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <returns></returns>
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await Db.Queryable<T>().FirstAsync(predicate);
        }
        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <param name="Guids">主键集合</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAsync(string[] Guids)
        {
            return await Db.Queryable<T>().In(Guids).ToListAsync();
        }
        /// <summary>
        /// 查询多条记录返回ISugarQueryable
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="take">返回条数</param>
        /// <returns></returns>
        public virtual async Task<SqlSugar.ISugarQueryable<T>> GetAllQueryableAsync(Expression<Func<T, bool>> predicate)
        {
            return predicate==null? await Task.Run(()=> Db.Queryable<T>()): await Task.Run(() => Db.Queryable<T>().Where(predicate));
        }
        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="take">返回条数</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return predicate == null ? await Db.Queryable<T>().ToListAsync() : await Db.Queryable<T>().Where(predicate).ToListAsync();
        }
        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderExperssion">排序（"DateTime desc"）</param>
        /// <param name="take">返回条数</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate,string orderExperssion, int take = 0)
        {
            return predicate == null ? take == 0 ? await Db.Queryable<T>().OrderBy(orderExperssion).ToListAsync() : await Db.Queryable<T>().OrderBy(orderExperssion).Take(take).ToListAsync() : take == 0 ? await Db.Queryable<T>().Where(predicate).OrderBy(orderExperssion).ToListAsync() : await Db.Queryable<T>().Where(predicate).OrderBy(orderExperssion).Take(take).ToListAsync();
        }       
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderExperssion">排序（"DateTime desc"）</param>
        /// <param name="take">返回条数</param>
        /// <returns></returns>
        public virtual async Task<Core.Model.Page<T>> GetPageAsync(int pageIndex,int pageSize, Expression<Func<T, bool>> predicate, string orderExperssion="")
        {
            var result = new Core.Model.Page<T>() { CurrentPage = pageIndex, ItemsPerPage = pageSize };

            var Tempdb = predicate == null ? Db.Queryable<T>() : Db.Queryable<T>().Where(predicate);

            Tempdb = string.IsNullOrEmpty(orderExperssion) ? Tempdb: Tempdb.OrderBy(orderExperssion);

            result.TotalItems = Tempdb.Count();

            result.TotalPages = (int)Math.Ceiling((Decimal)result.TotalItems / pageSize);

            var Skip = (pageIndex -1)* pageSize;

            var Take = pageSize;            

            if (pageIndex*pageSize > result.TotalItems / 2)
            {
                var orderType = "";
                if (!string.IsNullOrEmpty(orderExperssion))
                {
                    if (orderExperssion.ToLower().Contains("desc"))
                        orderType = orderExperssion.ToLower().Replace("desc", "asc");
                    else
                        orderType = orderExperssion.ToLower().Replace("asc", "desc");
                }
                else
                    orderType = "GUID DESC";

                Tempdb.OrderBy(orderType);

                var Mod = result.TotalItems % pageSize;
                
                if (pageIndex * pageSize >= result.TotalItems)
                {
                    Skip = 0; Take = Mod == 0 ? pageSize : Mod;
                }
                else
                {
                    Skip = (result.TotalPages - pageIndex - 1) * pageSize + Mod;
                }
            }

            Tempdb.Skip(Skip);
            Tempdb.Take(Take);

            result.Items = Tempdb.ToList();

            return await Task.Run(() => result);

        }
        /// <summary>
        /// sql语句查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual List<T> Sql(string sql)
        {
            return Db.Ado.SqlQuery<T>(sql);
        }

        #endregion

        /// <summary>
        /// 执行sql语句返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual int ExecuteSql(string sql)
        {
            return Db.Ado.ExecuteCommand(sql);
        }
        /// <summary>
        /// 执行sql语句返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual int ExecuteSql(string sql,List<SqlSugar.SugarParameter> parameters )
        {
            return Db.Ado.ExecuteCommand(sql, parameters);
        }
        /// <summary>
        /// 执行sql语句返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual int ExecuteSql(string sql,SqlSugar.SugarParameter[] parameters)
        {
            return Db.Ado.ExecuteCommand(sql, parameters);
        }

        /// <summary>
        /// 增加或更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsSave">是否增加</param>
        /// <returns></returns>
        public virtual async Task<bool> SaveOrUpdateAsync(T entity, bool IsSave)
        {
            return IsSave ? await SaveAsync(entity) : await UpdateAsync(entity);
        }

    }
}
