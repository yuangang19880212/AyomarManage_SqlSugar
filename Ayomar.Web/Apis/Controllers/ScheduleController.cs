using Ayomar.Service;
using Microsoft.AspNetCore.Mvc;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ayomar.Web.Apis.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ScheduleController : Controller
    {
        private readonly ISysScheduleService sysScheduleService;

        public ScheduleController(ISysScheduleService sysScheduleService)
        {
            this.sysScheduleService = sysScheduleService;
        }

        [HttpGet]
        public void UpdateStatus(string JobName,string JobGroup,int Status,string nextRun)
        {
          sysScheduleService.ExecuteSql("UPDATE SysSchedules SET Status =" + Status + ", PreviousRunTime = '" + DateTime.Now + "',NextRunTime='" + nextRun + "' WHERE JobName ='" + JobName + "' and JobGroup ='" + JobGroup + "'");
        }
    }
}
