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

## Stop and Elapsed

Call `Stop()` to set `End` to the current UTC time. Only the first call takes effect; if `End` is already set, `Stop()` leaves it unchanged. `Elapsed` returns a `TimeSpan`: while `End` is not set it is measured live against `DateTime.UtcNow`, and once `End` is set it is fixed at `End - Start`. Unlike `TotalMs`, `Elapsed` is not rounded. Setting `End` back to `null` makes `Elapsed` live again.

```csharp
Timestamp ts = new Timestamp();
Console.WriteLine("Running: " + ts.Elapsed);   // live, keeps growing

ts.Stop();                                      // sets End
ts.Stop();                                      // no effect, first End is kept
Console.WriteLine("Elapsed: " + ts.Elapsed.TotalMilliseconds + "ms");  // fixed value
```

## Version History

Please refer to CHANGELOG.md.
