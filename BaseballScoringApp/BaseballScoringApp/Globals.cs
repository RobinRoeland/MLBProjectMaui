using MetroLog.Maui;

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BaseballScoringApp
{
    internal static class Globals
    {
        //5204 for http, 7204 for https
        //public static readonly string serverURL = "http://localhost:5204"; // on laptop as windows app
        public static readonly string serverURL = "http://192.168.129.10:5204"; //from phone
        //public static readonly string serverURL = "http://10.0.2.2:5204"; // from emulator
        public static ILogger<App> logger;
        public static bool loggedIn = false;
    }
}
