using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ayomar.Service
{
    /// <summary>
    /// Describe：仓储接口
    /// Author：YuanGang
    /// Date：2016/07/16
    /// Blogs:http://www.cnblogs.com/yuangang
    /// </summary>
    /// <typeparam name="T">实体模型</typeparam>
    public interface IRepository<T> where T : class
    {
        SqlSugar.SqlSugarClient GetDb();

        #region Add
        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        Task<bool> SaveAsync(T entity);
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task<bool> SaveAsync(List<T> entitys);
        #endregion

        #region Delete
        /// <summary>
        /// 根据主键 删除一条记录
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteAsync(string Guid);
        /// <summary>
        /// 根据主键 批量删除一条记录
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteAsync(string[] Guids);
        /// <summary>
        /// 根据表达式 删除一条记录
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate);
        #endregion

        #region Update
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity);
        /// <summary>
        /// 更新多条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(List<T> entitys);
        #endregion

        #region Select
        /// <summary>
        /// 查询是否存在
        /// </summary>
        /// <returns></returns>
        Task<bool> IsAnyAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <returns></returns>
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <param name="Guids">主键集合</param>
        /// <returns></returns>
        Task<List<T>> GetAllAsync(string[] Guids);
        /// <summary>
        /// 查询多条记录返回ISugarQueryable
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="take">返回条数</param>
        /// <returns></returns>
        Task<SqlSugar.ISugarQueryable<T>> GetAllQueryableAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="take">返回条数</param>
        /// <returns></returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderExperssion">排序（"DateTime desc"）</param>
        /// <param name="take">返回条数</param>
        /// <returns></returns>
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate, string orderExperssion, int take = 0);
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderExperssion">排序（"DateTime desc"）</param>
        /// <param name="take">返回条数</param>
        /// <returns></returns>
        Task<Core.Model.Page<T>> GetPageAsync(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, string orderExperssion = "");
        /// <summary>
        /// sql语句查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        List<T> Sql(string sql);
        #endregion

        /// <summary>
        /// 执行sql语句返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int ExecuteSql(string sql);
        /// <summary>
        /// 执行sql语句返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int ExecuteSql(string sql, List<SqlSugar.SugarParameter> parameters);
        /// <summary>
        /// 执行sql语句返回受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int ExecuteSql(string sql, SqlSugar.SugarParameter[] parameters);
        /// <summary>
        /// 增加或更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <param name="IsSave">是否增加</param>
        /// <returns></returns>
        Task<bool> SaveOrUpdateAsync(T entity, bool IsSave);
    }
}
