# Serilog.Sinks.Trace [![Build status](https://ci.appveyor.com/api/projects/status/v1oe03lx3wymyy7j/branch/master?svg=true)](https://ci.appveyor.com/project/serilog/serilog-sinks-trace/branch/master) [![NuGet Version](http://img.shields.io/nuget/v/Serilog.Sinks.Trace.svg?style=flat)](https://www.nuget.org/packages/Serilog.Sinks.Trace/)

> **Note:** this sink writes regular log events to `System.Diagnostics.Trace`. If you're looking for 
> distributed/hierarchical tracing with Serilog, see [SerilogTracing](https://github.com/serilog-tracing/serilog-tracing)
> instead.

Writes [Serilog](https://serilog.net) events to `System.Diagnostics.Trace`.

```csharp
var log = new LoggerConfiguration()
    .WriteTo.Trace()
    .CreateLogger();
```

* [Documentation](https://github.com/serilog/serilog/wiki)

_Copyright &copy; Serilog Contributors - Provided under the [Apache License, Version 2.0](http://apache.org/licenses/LICENSE-2.0.html)._
