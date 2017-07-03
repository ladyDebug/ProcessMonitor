using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ProcessMonitor.Entity;
using ProcessMonitor.Utility;

namespace ProcessMonitor
{
    public class HttpProcessMonitor : IHttpProcess
    {
        private readonly PerformanceCounter _cpuCounter;

        private readonly ConcurrentQueue<PerformanceData> _hardWareInfo =
            new ConcurrentQueue<PerformanceData>();

        private readonly PerformanceCounter _ramCounter;
        private PerformanceData _hardwareParam = new PerformanceData();

        public HttpProcessMonitor()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);
        }

        public string GetProcessInfo()
        {
            var process = GetProcessList();
            var processListDatas = new ProcessListData {Process = new List<ProcessData>()};
            foreach (var p in process)
            {
                processListDatas.Process.Add(new ProcessData
                {
                    Id = p.Id,
                    ProcessName = p.ProcessName,
                    WorkingSet = p.WorkingSet64
                });
            }
            return Serializer.Serialize(processListDatas);
        }

        private void PrintProcInfo()
        {
            var updated = false;
            var cpuNext = _cpuCounter.NextValue();
            var ramNext = _ramCounter.NextValue();
            if (cpuNext >= 90 || ramNext <= 1024)
            {
                updated = true;
            }
            if (updated == false)
            {
                return;
            }
            _hardwareParam = new PerformanceData {Cpu = cpuNext, Ram = ramNext};
            _hardWareInfo.Enqueue(_hardwareParam);
            Console.WriteLine("fresh cpu: " + _hardwareParam.Cpu);
            Console.WriteLine("fresh ram: " + _hardwareParam.Ram);
        }

        public void BindToRunningProcesses()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                while (true)
                {
                    Thread.Sleep(2000);
                    PrintProcInfo();
                }
            });
        }

        public PerformanceData CheckForChanges()
        {
            PerformanceData hardwareItem = null;
            if (_hardWareInfo.Count > 0)
            {
                _hardWareInfo.TryDequeue(out hardwareItem);
            }
            return hardwareItem;
        }

        private Process[] GetProcessList()
        {
            return Process.GetProcesses();
        }
    }
}