![timestamps](https://raw.githubusercontent.com/jchristn/Timestamps/main/Assets/logo.ico)

# Timestamps

## Simple class with start time, end time, and total time, useful for measuring operational runtime.

[![NuGet Version](https://img.shields.io/nuget/v/Timestamps.svg?style=flat)](https://www.nuget.org/packages/Timestamps/) [![NuGet](https://img.shields.io/nuget/dt/Timestamps.svg)](https://www.nuget.org/packages/Timestamps)    

Timestamps provides a simple class that allows you to record start time, end time, and gather total runtime for a given operation. 

## New in v1.0.x

- Initial release

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

## Version History

Please refer to CHANGELOG.md.
