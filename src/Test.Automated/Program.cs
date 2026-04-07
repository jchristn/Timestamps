using Test.Shared;
using Touchstone.Cli;

string resultsPath = null;

for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--results" && i + 1 < args.Length)
        resultsPath = args[i + 1];
}

return await ConsoleRunner.RunAsync(TimestampsSuites.All, resultsPath: resultsPath);
