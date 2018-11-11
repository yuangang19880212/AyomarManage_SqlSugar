using Ayomar.Core.Model;
using System.Collections.Generic;

namespace Ayomar.Service
{
    public interface ISysAdminService:IRepository<SysAdmins>
    {
        Admins ValidateLogin(string account, string password);
        List<SysPermissions> VerifyPermission(string moduleAlias, List<SysPermissions> permissions, List<SysModules> adminmodules);
    }
}
