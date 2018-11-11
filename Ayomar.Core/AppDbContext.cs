using Ayomar.Core.Model;
using SqlSugar;

namespace Ayomar.Core
{
    public class AppDbContext
    {
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        public AppDbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Server=localhost;Database=ayomar_sugar_db;Uid=root;Pwd=123456;SslMode=None",
                DbType = DbType.MySql,
                IsAutoCloseConnection = true
            });
        }

        #region 系统表

        /// <summary>
        /// 系统管理员
        /// </summary>
        public AppDbSet<SysAdmins> SysAdmins { get { return new AppDbSet<SysAdmins>(Db); } }
        /// <summary>
        /// 
        /// </summary>
        public AppDbSet<SysModules> SysModules { get { return new AppDbSet<SysModules>(Db); } }
        /// <summary>
        /// 
        /// </summary>
        public AppDbSet<SysPermissions> SysPermissions { get { return new AppDbSet<SysPermissions>(Db); } }
        /// <summary>
        /// 
        /// </summary>
        public AppDbSet<SysRoleAssignments> SysRoleAssignments { get { return new AppDbSet<SysRoleAssignments>(Db); } }
        /// <summary>
        /// 
        /// </summary>
        public AppDbSet<SysRolePermissions> SysRolePermissions { get { return new AppDbSet<SysRolePermissions>(Db); } }
        /// <summary>
        /// 
        /// </summary>
        public AppDbSet<SysRoles> SysRoles { get { return new AppDbSet<SysRoles>(Db); } }
        /// <summary>
        /// 
        /// </summary>
        public AppDbSet<SysDepartments> SysDepartments { get { return new AppDbSet<SysDepartments>(Db); } }
        /// <summary>
        /// 
        /// </summary>
        public AppDbSet<SysPendingTrials> SysPendingTrials { get { return new AppDbSet<SysPendingTrials>(Db); } }

        #endregion

    }
}
