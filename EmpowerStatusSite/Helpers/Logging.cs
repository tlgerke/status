using System;
using System.Diagnostics;

namespace EmpowerStatusSite.Helpers
{
 public static class Logging
 {
 public static void Configure()
 {
#if SERILOG
 // Serilog will be configured in SerilogConfig.Configure(); nothing to do here
#else
 // no-op fallback: configure simple trace listener
 try
 {
 Trace.Listeners.Add(new TextWriterTraceListener("logs\\trace.log"));
 Trace.AutoFlush = true;
 }
 catch { }
#endif
 }

 public static void LogInformation(string message)
 {
#if SERILOG
 Serilog.Log.Information(message);
#else
 Trace.TraceInformation(message);
#endif
 }

 public static void LogWarning(string message)
 {
#if SERILOG
 Serilog.Log.Warning(message);
#else
 Trace.TraceWarning(message);
#endif
 }

 public static void LogError(string message)
 {
#if SERILOG
 Serilog.Log.Error(message);
#else
 Trace.TraceError(message);
#endif
 }

 public static void LogError(Exception ex, string message)
 {
#if SERILOG
 Serilog.Log.Error(ex, message);
#else
 Trace.TraceError(message + " - " + ex);
#endif
 }
 }
}
