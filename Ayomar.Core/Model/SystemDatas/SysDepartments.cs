using SqlSugar;
using System;

namespace Ayomar.Core.Model
{
    public class SysDepartments
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string GUID { get; set; }
        public string DepartmentName { get; set; }        
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
