using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Win32.TaskScheduler;

namespace EmpowerRemoteAgent.Controllers
{
 public class ScheduledTaskController : ApiController
 {
 [HttpGet]
 [Route("api/scheduledtask/{taskName}")]
 public IHttpActionResult GetTaskInfo(string taskName)
 {
 using (TaskService ts = new TaskService())
 {
 Task task = ts.FindTask(taskName);
 if (task == null)
 {
 return NotFound();
 }
 var taskInfo = new
 {
 Name = task.Name,
 Path = task.Path,
 State = task.State.ToString(),
 NextRunTime = task.NextRunTime,
 LastRunTime = task.LastRunTime,
 Schedule = GetScheduleInfo(task.Definition.Triggers)
 };
 return Ok(taskInfo);
 }
 }

 private string GetScheduleInfo(TriggerCollection triggers)
 {
 if (triggers.Count ==0)
 {
 return "No triggers defined";
 }
 var trigger = triggers[0];
 if (trigger is TimeTrigger timeTrigger)
 {
 return $"Runs at {timeTrigger.StartBoundary} every {timeTrigger.Repetition.Interval}";
 }
 else if (trigger is DailyTrigger dailyTrigger)
 {
 return $"Runs daily at {dailyTrigger.StartBoundary.TimeOfDay} every {dailyTrigger.DaysInterval} days and repeats every {dailyTrigger.Repetition.Interval}";
 }
 else if (trigger is WeeklyTrigger weeklyTrigger)
 {
 var daysOfWeek = string.Join(", ", Enum.GetValues(typeof(DaysOfTheWeek)).Cast<DaysOfTheWeek>().Where(day => weeklyTrigger.DaysOfWeek.HasFlag(day)).Select(day => day.ToString()));

 return $"Runs weekly on {daysOfWeek} at {weeklyTrigger.StartBoundary.TimeOfDay} every {weeklyTrigger.WeeksInterval} weeks";
 }
 else
 {
 return trigger.GetType().Name;
 }
 }

 [HttpPost]
 [Route("api/scheduledtask/{taskName}/enable")]
 public IHttpActionResult EnableTask(string taskName)
 {
 return UpdateTaskState(taskName, true);
 }

 [HttpPost]
 [Route("api/scheduledtask/{taskName}/disable")]
 public IHttpActionResult DisableTask(string taskName)
 {
 return UpdateTaskState(taskName, false);
 }

 [HttpPost]
 [Route("api/scheduledtask/{taskName}/run")]
 public IHttpActionResult RunTask(string taskName)
 {
 using (TaskService ts = new TaskService())
 {
 Task task = ts.FindTask(taskName);
 if (task == null)
 {
 return NotFound();
 }
 try
 {
 task.Run();
 return Ok();
 }
 catch (Exception ex)
 {
 return InternalServerError(ex);
 }
 }
 }

 [HttpPost]
 [Route("api/scheduledtask/{taskName}/end")]
 public IHttpActionResult EndTask(string taskName)
 {
 using (TaskService ts = new TaskService())
 {
 Task task=ts.FindTask(taskName);
 if(task == null)
 {
 return NotFound();
 }
 try
 {
 task.Stop();
 return Ok();
 }
 catch (Exception ex)
 {
 return InternalServerError(ex);
 }
 }
 }

 public IHttpActionResult UpdateTaskState(string taskName, bool enable)
 {
 using (TaskService ts = new TaskService())
 {
 Task task = ts.FindTask(taskName);
 if (task == null)
 {
 return NotFound();
 }
 try
 {
 task.Enabled = enable;
 return Ok();
 }
 catch (Exception ex)
 {
 return InternalServerError(ex);
 }
 }
 }

 [HttpGet]
 [Route("api/scheduledtasks/status")]
 public IHttpActionResult GetAllNonSystemTaskStatus()
 {
 using (TaskService ts = new TaskService())
 {
 try
 {
 var allTasks = ts.RootFolder.AllTasks;
 var nonSystemTasks = allTasks.Where(t => !t.Path.StartsWith("\\Microsoft"));
 var taskStatusList = nonSystemTasks.Select(task => new 
 {
 Name = task.Name,
 Path = task.Path,
 State = task.State,
 Enabled = task.Enabled,
 NextRunTime = task.NextRunTime,
 LastRunTime = task.LastRunTime,
 Schedule = GetScheduleInfo(task.Definition.Triggers)
 }).ToList();
 return Ok(taskStatusList);
 }
 catch (Exception ex)
 {
 return InternalServerError(ex);
 }
 }
 }
 }
}