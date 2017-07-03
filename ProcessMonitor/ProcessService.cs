using System.Threading;
using ProcessMonitor.Utility;

namespace ProcessMonitor
{
    public class ProcessService
    {
        private readonly HttpProcessMonitor _httpProcessMonitor;
        private readonly HttpListenerService _service;

        public ProcessService(string wsprefixes, string httpPrefix)
        {
            _service = new HttpListenerService(wsprefixes, httpPrefix);
            _httpProcessMonitor = new HttpProcessMonitor();
            _httpProcessMonitor.BindToRunningProcesses();
            CheckForHardareInfoChanges();
            _service.SetGetAction(_httpProcessMonitor);
        }

        public void Start()
        {
            _service.StartListen();
        }

        private void CheckForHardareInfoChanges()
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);
                    var hardwareInfoItem = _httpProcessMonitor.CheckForChanges();
                    if (hardwareInfoItem != null)
                    {
                        _service.SendMessageToWebsocket(Serializer.Serialize(hardwareInfoItem));
                    }
                }
            });
            thread.Start();
        }
    }
}