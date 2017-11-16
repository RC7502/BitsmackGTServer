using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsmackGTServer
{
    class Logger : BaseService
    {
        public static void LogDebug(string message)
        {
            log.Debug(message);
            Console.WriteLine(message);
        }

        public static void LogError(string message)
        {
            log.Error(message);
            Console.WriteLine(message);
        }
    }
}
