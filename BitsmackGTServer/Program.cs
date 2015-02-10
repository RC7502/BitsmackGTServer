using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitsmackGTServer.Models;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace BitsmackGTServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int refreshInterval = 15;
            var taskList = new List<Task<bool>>();
            var pedometerService = new PedometerService();
            var run = true;
            while (run)
            {

                var task1 = pedometerService.Update();
                taskList.Add(task1);

                Task.WaitAll(taskList.ToArray());
                if (taskList.Any(x => x.Result == false))
                    refreshInterval = 60;

                Console.Write("Next update at {0} {1}", DateTime.Now.AddMinutes(refreshInterval), Environment.NewLine);
                Thread.Sleep(60000 * refreshInterval);
                //run = false;
            }
        }

    }
}
