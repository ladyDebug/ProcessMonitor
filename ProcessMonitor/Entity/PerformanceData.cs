using System.Runtime.Serialization;

namespace ProcessMonitor.Entity
{
    [DataContract]
    public class PerformanceData
    {
        [DataMember]
        public float Ram { get; set; }
        [DataMember]
        public float Cpu { get; set; }

    }
}