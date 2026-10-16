![timestamps](https://raw.githubusercontent.com/jchristn/Timestamps/main/Assets/logo.ico)

# Timestamps

## Simple class with start time, end time, and total time, useful for measuring operational runtime.

[![NuGet Version](https://img.shields.io/nuget/v/Timestamps.svg?style=flat)](https://www.nuget.org/packages/Timestamps/) [![NuGet](https://img.shields.io/nuget/dt/Timestamps.svg)](https://www.nuget.org/packages/Timestamps)    

Timestamps provides a simple class that allows you to record start time, end time, and gather total runtime for a given operation. 

## New in v1.0.13

- Updated test dependencies (Touchstone 0.2.0, Microsoft.NET.Test.Sdk 18.10.1)

## Help or Feedback

Need help or have feedback? Please file an issue here!

## Simple Examples
```csharp
using Timestamps;

void Main(string[] args)
{
  Timestamp ts = new Timestamp();
  ts.Start = DateTime.UtcNow;
  ts.End = DateTime.UtcNow.AddSeconds(10);
  Console.WriteLine("Total milliseconds: " + ts.TotalMs + "ms");

  // add log messages
  ts.AddMessage("Hello, world!");
  ts.AddMessage("Hello, but a different world!");
  Dictionary<DateTime, string> messages = ts.Messages;

  // add metadata
  ts.Metadata = myObject; // anything you like
}
```

### Stop and Elapsed

Call `Stop()` to set `End` to the current UTC time. If `End` is already set, `Stop()` does nothing, so it is safe to call more than once (for example in a `finally` block) and the first end time is kept. `Elapsed` returns the duration as a `TimeSpan`: while `End` is not set it is measured against `DateTime.UtcNow` and keeps growing; once `End` is set it is fixed at `End - Start`. Use UTC values for `Start` and `End`.

```csharp
Timestamp ts = new Timestamp();
// ... do work ...
Console.WriteLine("Running for: " + ts.Elapsed);
ts.Stop();
ts.Stop(); // no effect, the first end time is kept
TimeSpan elapsed = ts.Elapsed; // fixed after Stop()
Console.WriteLine("Elapsed: " + elapsed.TotalMilliseconds + "ms");
```

## Version History

Please refer to CHANGELOG.md.
