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
```csharp
using Timestamps;

Timestamp ts = new Timestamp();
// do some work
TimeSpan soFar = ts.Elapsed;  // live: grows until End is set
ts.Stop();                    // sets End to DateTime.UtcNow
ts.Stop();                    // no effect, the first Stop() wins
TimeSpan total = ts.Elapsed;  // fixed: End - Start
```

- `Stop()` sets `End` to `DateTime.UtcNow` only if `End` is not already set. Calling it again, or calling it after setting `End` manually, keeps the existing `End`.
- `Elapsed` is a read-only `TimeSpan`. It returns `End - Start` once `End` is set, and `DateTime.UtcNow - Start` before that, so it is live until the timestamp is stopped. Setting `End = null` makes it live again.
- `Elapsed` is not rounded and does not convert values to UTC, so it may differ slightly from `TotalMs`. Use UTC values (`DateTime.UtcNow`) for `Start` and `End`.

## Version History

Please refer to CHANGELOG.md.
