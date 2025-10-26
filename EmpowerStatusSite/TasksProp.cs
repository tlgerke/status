using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32.TaskScheduler;

namespace EmpowerStatusSite
{
    [DataContract]
    public class TasksProp
    {
        public string Server {  get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public TaskState State { get; set; }
        public bool Enabled { get; set; }
        public DateTime NextRunTime { get; set; }
        public DateTime LastRunTime { get; set; }
        public string Schedule {  get; set; }
        
    }
}