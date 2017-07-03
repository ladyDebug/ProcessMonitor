using ProcessMonitor;
using System.Configuration;

namespace PerfServiceStarter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            var service = new ProcessService(settings["WsString"].Value, settings["HttpString"].Value);
            service.Start();
        }
    }
}