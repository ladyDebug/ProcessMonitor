using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ProcessMonitor.Entity
{
    [DataContract]
    public class ProcessListData
    {
        [DataMember]
        public List<ProcessData> Process { get; set; }
    }
}