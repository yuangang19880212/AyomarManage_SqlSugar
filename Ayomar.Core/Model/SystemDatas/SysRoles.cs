using SqlSugar;
using System;

namespace Ayomar.Core.Model
{
    public partial class SysRoles
    {
        [SugarColumn(IsPrimaryKey=true)]
        public string GUID {get;set;}

        public string RoleName { get; set; }
        public string RoleExplain { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}
