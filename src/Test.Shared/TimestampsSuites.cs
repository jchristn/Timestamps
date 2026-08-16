namespace Test.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using Timestamps;
    using Touchstone.Core;

    /// <summary>
    /// Touchstone test suites for the Timestamps library.
    /// </summary>
    public static class TimestampsSuites
    {
        /// <summary>
        /// All test suites.
        /// </summary>
        public static IReadOnlyList<TestSuiteDescriptor> All
        {
            get
            {
                return new List<TestSuiteDescriptor>
                {
                    ConstructorSuite(),
                    TotalMsSuite(),
                    MessagesSuite(),
                    MetadataSuite(),
                    DisposeSuite()
                };
            }
        }

        /// <summary>
        /// Tests for constructor and default state.
        /// </summary>
        public static TestSuiteDescriptor ConstructorSuite()
        {
            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>
            {
                new TestCaseDescriptor(
                    suiteId: "Constructor",
                    caseId: "StartIsSet",
                    displayName: "Start is set to approximately UtcNow on construction",
                    executeAsync: async ct =>
                    {
                        DateTime before = DateTime.UtcNow;
                        Timestamp ts = new Timestamp();
                        DateTime after = DateTime.UtcNow;

                        if (ts.Start < before || ts.Start > after)
                            throw new Exception(
                                $"Start {ts.Start:O} is not between {before:O} and {after:O}");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Constructor",
                    caseId: "EndIsNull",
                    displayName: "End is null by default",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        if (ts.End != null)
                            throw new Exception("End should be null by default");
                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Constructor",
                    caseId: "MessagesEmpty",
                    displayName: "Messages dictionary is empty by default",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        if (ts.Messages == null || ts.Messages.Count != 0)
                            throw new Exception("Messages should be empty by default");
                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Constructor",
                    caseId: "MetadataIsNull",
                    displayName: "Metadata is null by default",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        if (ts.Metadata != null)
                            throw new Exception("Metadata should be null by default");
                        await Task.CompletedTask;
                    })
            };

            return new TestSuiteDescriptor(
                suiteId: "Constructor",
                displayName: "Constructor and Default State",
                cases: cases);
        }

        /// <summary>
        /// Tests for TotalMs calculation.
        /// </summary>
        public static TestSuiteDescriptor TotalMsSuite()
        {
            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>
            {
                new TestCaseDescriptor(
                    suiteId: "TotalMs",
                    caseId: "WithoutEnd",
                    displayName: "TotalMs returns elapsed time when End is null",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        await Task.Delay(50, ct);
                        double? ms = ts.TotalMs;
                        if (ms == null || ms < 40)
                            throw new Exception($"Expected TotalMs >= 40 but got {ms}");
                    }),

                new TestCaseDescriptor(
                    suiteId: "TotalMs",
                    caseId: "WithEnd",
                    displayName: "TotalMs returns fixed duration when End is set",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        await Task.Delay(50, ct);
                        ts.End = DateTime.UtcNow;
                        double? snapshot1 = ts.TotalMs;
                        await Task.Delay(50, ct);
                        double? snapshot2 = ts.TotalMs;

                        if (snapshot1 == null || snapshot2 == null)
                            throw new Exception("TotalMs should not be null");
                        if (snapshot1 != snapshot2)
                            throw new Exception(
                                $"TotalMs should be stable once End is set: {snapshot1} vs {snapshot2}");
                    }),

                new TestCaseDescriptor(
                    suiteId: "TotalMs",
                    caseId: "ClearEnd",
                    displayName: "Clearing End resumes live elapsed calculation",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        await Task.Delay(50, ct);
                        ts.End = DateTime.UtcNow;
                        double? frozen = ts.TotalMs;

                        ts.End = null;
                        await Task.Delay(50, ct);
                        double? live = ts.TotalMs;

                        if (live == null || frozen == null || live <= frozen)
                            throw new Exception(
                                $"After clearing End, TotalMs should grow: frozen={frozen}, live={live}");
                    }),

                new TestCaseDescriptor(
                    suiteId: "TotalMs",
                    caseId: "NonNegative",
                    displayName: "TotalMs is non-negative",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        ts.End = DateTime.UtcNow;
                        double? ms = ts.TotalMs;
                        if (ms == null || ms < 0)
                            throw new Exception($"TotalMs should be >= 0, got {ms}");
                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "TotalMs",
                    caseId: "Deterministic",
                    displayName: "TotalMs equals the exact span between fixed Start and End",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp
                        {
                            Start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                            End = new DateTime(2024, 1, 1, 0, 0, 10, DateTimeKind.Utc)
                        };

                        double? ms = ts.TotalMs;
                        if (ms != 10000.0)
                            throw new Exception($"Expected exactly 10000ms, got {ms}");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "TotalMs",
                    caseId: "RoundsToTwoDecimals",
                    displayName: "TotalMs rounds sub-millisecond durations to two decimals",
                    executeAsync: async ct =>
                    {
                        // 12345 ticks = 1.2345 ms, which should round to 1.23.
                        DateTime start = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        Timestamp ts = new Timestamp
                        {
                            Start = start,
                            End = start.AddTicks(12345)
                        };

                        double? ms = ts.TotalMs;
                        if (ms != 1.23)
                            throw new Exception($"Expected 1.23ms after rounding, got {ms}");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "TotalMs",
                    caseId: "NegativeWhenStartAfterEnd",
                    displayName: "TotalMs is negative when Start is after End",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp
                        {
                            Start = new DateTime(2024, 1, 1, 0, 0, 10, DateTimeKind.Utc),
                            End = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        };

                        double? ms = ts.TotalMs;
                        if (ms != -10000.0)
                            throw new Exception($"Expected -10000ms when Start is after End, got {ms}");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "TotalMs",
                    caseId: "NormalizesDateTimeKind",
                    displayName: "TotalMs normalizes mixed DateTimeKind values to UTC",
                    executeAsync: async ct =>
                    {
                        // Both endpoints are the same wall-clock instant expressed as Local,
                        // five seconds apart. After ToUniversalTime() the offset cancels, so
                        // the result must be exactly 5000ms regardless of the machine's zone.
                        DateTime localStart = new DateTime(2024, 6, 1, 12, 0, 0, DateTimeKind.Local);
                        Timestamp ts = new Timestamp
                        {
                            Start = localStart,
                            End = localStart.AddSeconds(5)
                        };

                        double? ms = ts.TotalMs;
                        if (ms != 5000.0)
                            throw new Exception($"Expected 5000ms across Local timestamps, got {ms}");

                        await Task.CompletedTask;
                    })
            };

            return new TestSuiteDescriptor(
                suiteId: "TotalMs",
                displayName: "TotalMs Calculations",
                cases: cases);
        }

        /// <summary>
        /// Tests for Messages functionality.
        /// </summary>
        public static TestSuiteDescriptor MessagesSuite()
        {
            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>
            {
                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "AddSingle",
                    displayName: "AddMessage stores a single message",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        ts.AddMessage("hello");
                        if (ts.Messages.Count != 1)
                            throw new Exception($"Expected 1 message, got {ts.Messages.Count}");
                        if (!ts.Messages.Values.First().Equals("hello"))
                            throw new Exception("Message content mismatch");
                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "AddMultiple",
                    displayName: "AddMessage stores multiple messages in order",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        ts.AddMessage("first");
                        ts.AddMessage("second");
                        ts.AddMessage("third");

                        Dictionary<DateTime, string> msgs = ts.Messages;
                        if (msgs.Count != 3)
                            throw new Exception($"Expected 3 messages, got {msgs.Count}");

                        List<string> values = msgs.Values.ToList();
                        if (values[0] != "first" || values[1] != "second" || values[2] != "third")
                            throw new Exception("Messages not in expected order");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "DuplicateKeyAvoidance",
                    displayName: "Rapid AddMessage calls do not throw duplicate key exception",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        for (int i = 0; i < 100; i++)
                            ts.AddMessage($"msg-{i}");

                        if (ts.Messages.Count != 100)
                            throw new Exception(
                                $"Expected 100 messages, got {ts.Messages.Count}");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "ConcurrentDuplicateKeyAvoidance",
                    displayName: "Concurrent AddMessage calls from multiple threads do not throw",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        int threadCount = 8;
                        int messagesPerThread = 50;
                        int totalExpected = threadCount * messagesPerThread;
                        ConcurrentBag<Exception> errors = new ConcurrentBag<Exception>();

                        List<Thread> threads = new List<Thread>();
                        for (int t = 0; t < threadCount; t++)
                        {
                            int threadId = t;
                            Thread thread = new Thread(() =>
                            {
                                try
                                {
                                    for (int i = 0; i < messagesPerThread; i++)
                                        ts.AddMessage($"thread-{threadId}-msg-{i}");
                                }
                                catch (Exception ex)
                                {
                                    errors.Add(ex);
                                }
                            });
                            threads.Add(thread);
                        }

                        foreach (Thread thread in threads)
                            thread.Start();
                        foreach (Thread thread in threads)
                            thread.Join();

                        if (errors.Count > 0)
                            throw new AggregateException(
                                $"{errors.Count} thread(s) threw exceptions", errors);

                        int actual = ts.Messages.Count;
                        if (actual != totalExpected)
                            throw new Exception(
                                $"Expected {totalExpected} messages, got {actual}");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "ChronologicalOrder",
                    displayName: "Messages are returned in chronological key order",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        ts.AddMessage("a");
                        ts.AddMessage("b");
                        ts.AddMessage("c");

                        List<DateTime> keys = ts.Messages.Keys.ToList();
                        for (int i = 1; i < keys.Count; i++)
                        {
                            if (keys[i] < keys[i - 1])
                                throw new Exception("Keys are not in chronological order");
                        }

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "NullThrows",
                    displayName: "AddMessage throws on null input",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        bool threw = false;
                        try
                        {
                            ts.AddMessage(null);
                        }
                        catch (ArgumentNullException)
                        {
                            threw = true;
                        }

                        if (!threw)
                            throw new Exception("Expected ArgumentNullException for null message");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "EmptyThrows",
                    displayName: "AddMessage throws on empty string",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        bool threw = false;
                        try
                        {
                            ts.AddMessage("");
                        }
                        catch (ArgumentNullException)
                        {
                            threw = true;
                        }

                        if (!threw)
                            throw new Exception("Expected ArgumentNullException for empty message");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "SetMessages",
                    displayName: "Messages setter replaces the dictionary",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        ts.AddMessage("original");

                        Dictionary<DateTime, string> replacement = new Dictionary<DateTime, string>
                        {
                            { DateTime.UtcNow, "replaced" }
                        };
                        ts.Messages = replacement;

                        if (ts.Messages.Count != 1 || ts.Messages.Values.First() != "replaced")
                            throw new Exception("Setter did not replace messages");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "SetNull",
                    displayName: "Setting Messages to null resets to empty dictionary",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        ts.AddMessage("something");
                        ts.Messages = null;

                        if (ts.Messages == null || ts.Messages.Count != 0)
                            throw new Exception("Setting null should reset to empty dictionary");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "SortedOnGetWhenSetUnordered",
                    displayName: "Getter returns messages sorted by key even when set out of order",
                    executeAsync: async ct =>
                    {
                        DateTime t0 = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                        // Insert keys out of chronological order.
                        Dictionary<DateTime, string> unordered = new Dictionary<DateTime, string>
                        {
                            { t0.AddSeconds(3), "third" },
                            { t0.AddSeconds(1), "first" },
                            { t0.AddSeconds(2), "second" }
                        };

                        Timestamp ts = new Timestamp();
                        ts.Messages = unordered;

                        List<string> values = ts.Messages.Values.ToList();
                        if (values.Count != 3
                            || values[0] != "first"
                            || values[1] != "second"
                            || values[2] != "third")
                            throw new Exception(
                                "Getter should return values ordered by ascending key: "
                                + string.Join(",", values));

                        List<DateTime> keys = ts.Messages.Keys.ToList();
                        for (int i = 1; i < keys.Count; i++)
                            if (keys[i] < keys[i - 1])
                                throw new Exception("Keys are not in ascending order");

                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Messages",
                    caseId: "WhitespaceAccepted",
                    displayName: "AddMessage accepts whitespace-only messages",
                    executeAsync: async ct =>
                    {
                        // AddMessage only rejects null/empty; whitespace is a valid message.
                        Timestamp ts = new Timestamp();
                        ts.AddMessage("   ");

                        if (ts.Messages.Count != 1 || ts.Messages.Values.First() != "   ")
                            throw new Exception("Whitespace message should be stored verbatim");

                        await Task.CompletedTask;
                    })
            };

            return new TestSuiteDescriptor(
                suiteId: "Messages",
                displayName: "Messages Functionality",
                cases: cases);
        }

        /// <summary>
        /// Tests for Metadata property.
        /// </summary>
        public static TestSuiteDescriptor MetadataSuite()
        {
            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>
            {
                new TestCaseDescriptor(
                    suiteId: "Metadata",
                    caseId: "SetAndGet",
                    displayName: "Metadata can be set and retrieved",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        ts.Metadata = "test-metadata";
                        if (!ts.Metadata.Equals("test-metadata"))
                            throw new Exception("Metadata mismatch");
                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Metadata",
                    caseId: "ComplexObject",
                    displayName: "Metadata supports complex objects",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        var obj = new { Name = "test", Value = 42 };
                        ts.Metadata = obj;
                        if (ts.Metadata != obj)
                            throw new Exception("Metadata should hold reference to complex object");
                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Metadata",
                    caseId: "SetNull",
                    displayName: "Metadata can be set to null",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        ts.Metadata = "something";
                        ts.Metadata = null;
                        if (ts.Metadata != null)
                            throw new Exception("Metadata should be null");
                        await Task.CompletedTask;
                    })
            };

            return new TestSuiteDescriptor(
                suiteId: "Metadata",
                displayName: "Metadata Property",
                cases: cases);
        }

        /// <summary>
        /// Tests for Dispose behavior.
        /// </summary>
        public static TestSuiteDescriptor DisposeSuite()
        {
            List<TestCaseDescriptor> cases = new List<TestCaseDescriptor>
            {
                new TestCaseDescriptor(
                    suiteId: "Dispose",
                    caseId: "DoesNotThrow",
                    displayName: "Dispose does not throw",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        ts.AddMessage("before dispose");
                        ts.Dispose();
                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Dispose",
                    caseId: "DoubleDispose",
                    displayName: "Double dispose does not throw",
                    executeAsync: async ct =>
                    {
                        Timestamp ts = new Timestamp();
                        ts.Dispose();
                        ts.Dispose();
                        await Task.CompletedTask;
                    }),

                new TestCaseDescriptor(
                    suiteId: "Dispose",
                    caseId: "UsingBlock",
                    displayName: "Works correctly in a using block",
                    executeAsync: async ct =>
                    {
                        double? ms;
                        using (Timestamp ts = new Timestamp())
                        {
                            await Task.Delay(20, ct);
                            ts.End = DateTime.UtcNow;
                            ms = ts.TotalMs;
                        }

                        if (ms == null || ms < 10)
                            throw new Exception($"Expected TotalMs >= 10 in using block, got {ms}");
                    })
            };

            return new TestSuiteDescriptor(
                suiteId: "Dispose",
                displayName: "Dispose Behavior",
                cases: cases);
        }
    }
}
