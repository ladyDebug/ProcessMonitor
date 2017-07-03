using System.Runtime.Serialization;

namespace ProcessMonitor.Entity
{
    [DataContract]
    public class ProcessData
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string ProcessName { get; set; }

        [DataMember]
        public long WorkingSet { get; set; }
    }
}