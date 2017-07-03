using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ProcessMonitor.Utility
{
    public static class Serializer
    {
        public static string Serialize(object data)
        {
            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(data.GetType());
            ser.WriteObject(ms, data);
            var json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }
    }
}