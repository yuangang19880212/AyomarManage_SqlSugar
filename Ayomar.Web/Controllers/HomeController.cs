using Ayomar.Core.Model;
using Ayomar.Web.Model;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Diagnostics;

namespace Ayomar.Web.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            //Core.AppDbContext db = new Core.AppDbContext();
            //foreach (var t in db.Db.DbMaintenance.GetTableInfoList())
            //{
            //    if (t.Name != "__efmigrationshistory")
            //    {
            //        var newName = t.Name.Split('_');
            //        db.Db.MappingTables.Add(System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(newName[0]) + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(newName[1]), t.Name);
            //        db.Db.DbFirst.IsCreateAttribute().CreateClassFile("d:\\Demo\\3");
            //    }
            //}

            //SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            //{
            //    ConnectionString = "Server=localhost;Database=ayomar_sugar_db;Uid=root;Pwd=123456;SslMode=None",
            //    DbType = DbType.MySql,
            //    IsAutoCloseConnection = true,
            //    InitKeyType = InitKeyType.Attribute
            //});

            //Db.CodeFirst.InitTables(typeof(SysLogs), typeof(SysLogs));
            //Db.CodeFirst.InitTables(typeof(SysModules), typeof(SysModules));
            //Db.CodeFirst.InitTables(typeof(SysPermissions), typeof(SysPermissions));
            //Db.CodeFirst.InitTables(typeof(SysRoleAssignments), typeof(SysRoleAssignments));
            //Db.CodeFirst.InitTables(typeof(SysRoles), typeof(SysRoles));
            //Db.CodeFirst.InitTables(typeof(SysSystem), typeof(SysSystem));
            //Db.CodeFirst.InitTables(typeof(SysRolePermissions), typeof(SysRolePermissions));

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
