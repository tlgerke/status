using System;
using EmpowerStatusSite.Helpers;

#if SERILOG
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Trace;
using Serilog.Sinks.File;
#endif

namespace EmpowerStatusSite.App_Start
{
 public static class SerilogConfig
 {
 public static void Configure()
 {
#if SERILOG
 try
 {
 // compile-time Serilog configuration (requires Serilog packages and the SERILOG symbol)
 Serilog.Log.Logger = new Serilog.LoggerConfiguration()
 .MinimumLevel.Debug()
 .WriteTo.Trace(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
 .WriteTo.File("logs\\empower-status-.log", rollingInterval: Serilog.RollingInterval.Day, retainedFileCountLimit:14, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
 .CreateLogger();
 }
 catch (Exception)
 {
 // fallback to simple trace logging if Serilog fails to initialize
 Logging.Configure();
 }
#else
 // Serilog not enabled at compile-time; use fallback trace-based logger
 Logging.Configure();
#endif
 }
 }
}
