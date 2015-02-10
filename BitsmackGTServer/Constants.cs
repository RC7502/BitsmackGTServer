using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitsmackGTServer
{
    class Constants
    {
        public static string APIURL()
        {
            return ConfigurationManager.AppSettings["APIURL"];
        }

        public const string Token = "df5a867c-b7a0-4d70-b15d-5fb92627ce56";
    }
}
