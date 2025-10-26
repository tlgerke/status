using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;

namespace EmpowerStatusSite.Controllers
{
    public class EventLogController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetEvents(string logname = "Application", int maxEvents = 100 )
        {
            string excludeSources = "VstsAgentService,VSS,Microsoft-Windows-Perflib,PerfNet,MSMQ,Group Policy Local Users and Groups,Group Policy Files,Microsoft-Windows-User Profiles Service,SceCli,AutoEnrollment,Desktop Window Manager,ESENT,Software Protection Platform Service,Microsoft-Windows-LoadPerf,Wlclntfy,MsiInstaller,PASAgent,MSDTC 2,VMUpgradeHelper,COM+,WAS-LA,VMTools,Microsoft-Windows-WMI,R7ScanAssistant,MSDTC,EventSystem,Microsoft-Windows-CAPI2,Microsoft-Windows-RestartManager,Microsoft-Windows-Defrag,CertEnroll,MSMQTriggers,.NET Runtime Optimization Service,Configuration Manager Agent,MSDTC Client 2,SentinelHelperService";
            try
            {
                //Accss the specified event log
                EventLog eventLog = new EventLog(logname);
                string[] excludedSourcesArray = excludeSources.Split(',').Select(s => s.Trim()).ToArray();
                //Get the most recent entries
                var entries = eventLog.Entries
                    .Cast<EventLogEntry>()
                    .Where(e => !excludedSourcesArray.Contains(e.Source))
                    .OrderByDescending(e => e.TimeGenerated)
                    .Take(maxEvents)
                    .Select(e => new
                    {
                        e.TimeGenerated,
                        e.Source,
                        e.InstanceId,
                        EntryType = e.EntryType.ToString(),
                        e.Message
                    });
                return Ok(entries);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
