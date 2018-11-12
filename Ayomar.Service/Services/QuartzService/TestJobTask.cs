using Quartz;
using System;
using System.Threading.Tasks;

namespace Ayomar.Service.Services.QuartzService
{
    public class TestJobTask : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
               await Console.Out.WriteLineAsync("Greetings from HelloJob!");
            }catch(Exception ex)
            {
               await Console.Out.WriteLineAsync(ex.ToString());
            }
        }
    }
}
