using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmpowerStatusSite
{
    public class EventProps
    {
        public DateTime TimeGenerated { get; set; }
        public string Source { get; set; }
        public long InstanceId {  get; set; }
        public string EntryType { get; set; }
        public string Message { get; set; }
    }
}