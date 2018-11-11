using Ayomar.Common.ResultHelper;
using Ayomar.Core.Model;
using Ayomar.Service;
using Ayomar.Web.Areas.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ayomar.Web.Areas.SchedulingManage.Controllers
{
    [Area("SchedulingManage")]
    public class SchedulingController : BaseController
    {
        private readonly ISysScheduleService sysScheduleService;

        [ManageAuthorize(ModuleAlias = "Scheduling", Operatinos = "View")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
