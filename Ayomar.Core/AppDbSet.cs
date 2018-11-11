using SqlSugar;
using System.Collections.Generic;

namespace Ayomar.Core
{
    public class AppDbSet<T> : SimpleClient<T> where T : class, new()
    {
        public AppDbSet(SqlSugarClient context) : base(context)
        {

        }
        //SimpleClient中的方法满足不了你，你可以扩展自已的方法
        public List<T> GetByIds(dynamic[] ids)
        {
            return Context.Queryable<T>().In(ids).ToList(); ;
        }
    }
}
