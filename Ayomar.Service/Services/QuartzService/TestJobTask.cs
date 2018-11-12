using Quartz;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ayomar.Service.Services.QuartzService
{
    public class TestJobTask : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {            
            try
            {
               await Console.Out.WriteLineAsync("Greetings from HelloJob!"+ context.NextFireTimeUtc.Value.DateTime.AddHours(8));
                var httpClient = HttpClientFactory.Create();
                var status = context.NextFireTimeUtc == null ? 0 : 1;
                var response = await httpClient.GetAsync("http://localhost:58809/api/schedule/UpdateStatus?JobName=" + context.JobDetail.Key.Name + "&JobGroup=" + context.JobDetail.Key.Group + "&Status=" + status + "&nextRun=" + context.NextFireTimeUtc.Value.AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"));

            }
            catch(Exception ex)
            {
               await Console.Out.WriteLineAsync(ex.ToString());
            }
        }
    }
}
