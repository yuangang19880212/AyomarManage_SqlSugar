using Ayomar.Core.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ayomar.Service
{
    public class SysAdminService : Repository<SysAdmins>, ISysAdminService
    {
        public Admins ValidateLogin(string account,string password)
        {
            var User = SysAdmins.GetSingle(p => p.Account.Equals(account) || p.Mobile.Equals(account) || p.Email.Equals(account));

            var Model = new Admins();

            if (User == null)
                return null;
            else
            {
                Model.User = User;
                if (!password.Equals(new Common.CryptHelper.AESCrypt().Decrypt(User.Password)))
                    return null;
                else
                {
                    if (Model.User.IsSuper)
                    {
                        var list = Db.Queryable<SysPermissions, SysModules>((sp, sm) => new object[] {
                            JoinType.Inner,sp.Module_GUID==sm.GUID
                        })
                        .Select((sp, sm) => new ViewAdmins { Modules = sm, Permissions = sp }).ToList();

                        Model.Modules = list.Select(p => p.Modules).GroupBy(p => p.GUID).Select(p => p.First()).ToList();
                        Model.Permissions = list.Select(p => p.Permissions).ToList();                       
                    }
                    else
                    {
                        var list = Db.Queryable<SysRoleAssignments, SysRolePermissions, SysPermissions, SysModules>((sra, srp, sp, sm) => new object[] {
                            JoinType.Inner,sra.Role_GUID==srp.Role_GUID,
                            JoinType.Inner,srp.Permission_GUID==sp.GUID,
                            JoinType.Inner,sp.Module_GUID==sm.GUID
                        })
                        .Where((sra) => sra.Admin_GUID == Model.User.GUID)
                        .Select((sra, srp, sp, sm) => new ViewAdmins { Modules = sm, Permissions = sp }).ToList();

                        Model.Modules = list.Select(p => p.Modules).GroupBy(p=>p.GUID).Select(p=>p.First()).ToList();
                        Model.Permissions = list.Select(p => p.Permissions).ToList();
                      
                    }

                    //获取Modules所有级别菜单
                    var ModulesAll = SysModules.GetList(p => p.Levels < 2);
                    var ModulesParents = new List<SysModules>();
                    Model.Modules.ForEach(it => {
                        var secodModule = ModulesAll.Find(p => p.GUID == it.Parent_GUID);
                        var firstModule = ModulesAll.Find(p => p.GUID == secodModule.Parent_GUID);
                        if (!ModulesParents.Any(p => p == secodModule))
                            ModulesParents.Add(secodModule);
                        if (!ModulesParents.Any(p => p == firstModule))
                            ModulesParents.Add(firstModule);
                    });

                    Model.Modules = Model.Modules.Union(ModulesParents).ToList();

                    return Model;
                }
            }
            
            
        }

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="moduleAlias">模块别名</param>
        /// <param name="permissions">用户权限</param>
        /// <returns></returns>
        public List<SysPermissions> VerifyPermission(string moduleAlias, List<SysPermissions> permissions,List<SysModules> adminmodules)
        {
            if (moduleAlias.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Length > 0)
            {
                var modules = adminmodules.FindAll(p => moduleAlias.ToLower().Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(p.Alias.ToLower())).Select(p => p.GUID);
                return permissions.FindAll(p => modules.Contains(p.Module_GUID));
            }
            else
                return null;
        }
    }

    public class ViewAdmins
    {
        public SysPermissions Permissions { get; set; }
        public SysModules Modules { get; set; }
    }
}
