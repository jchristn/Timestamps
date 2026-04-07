namespace Test.XunitRunner
{
    using System.Threading;
    using System.Threading.Tasks;
    using Test.Shared;
    using Touchstone.Core;
    using global::Xunit;
    using global::Xunit.Abstractions;

    public sealed class TimestampsTheoryTests
    {
        private readonly ITestOutputHelper _Output;

        public TimestampsTheoryTests(ITestOutputHelper output) => _Output = output;

        public static TheoryData<TestCaseDescriptor> TestCases()
        {
            TheoryData<TestCaseDescriptor> data = new();
            foreach (var suite in TimestampsSuites.All)
                foreach (var testCase in suite.Cases)
                    if (!testCase.Skip)
                        data.Add(testCase);
            return data;
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public async Task RunTest(TestCaseDescriptor testCase)
        {
            _Output.WriteLine($"Running: {testCase.DisplayName}");
            await testCase.ExecuteAsync(CancellationToken.None);
        }
    }
}
