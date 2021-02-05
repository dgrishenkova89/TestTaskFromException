using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Jobs;
using System.Threading.Tasks;

namespace TestThrowAsync
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [ThreadingDiagnoser]
    [TailCallDiagnoser]
    [MemoryDiagnoser]
    [MinColumn, MaxColumn]
    public class TestReturn
    {
        [Params(500, 1000, 2000)]
        public static int counter;

        [Benchmark(Description = "ReturnSingleTask")]
        public Task ReturnTask()
        {
            return GetSecondReturn();
        }

        [Benchmark(Description = "ReturnForMultipleAwaitTask")]
        public async Task ReturnForMultipleAwaitTask()
        {
            for (int i = 0; i <= counter; i++)
            {
                await GetSecondReturn();
            }
        }

        private Task GetSecondReturn()
        {
            return GetThirdReturn();
        }

        private Task GetThirdReturn()
        {
            return GetFourthReturn();
        }

        private Task GetFourthReturn()
        {
            return GetFifthReturn();
        }

        private Task GetFifthReturn()
        {
            return GetSixthReturn();
        }

        private Task GetSixthReturn()
        {
            return GenerateChildTasksAsync();
        }

        private Task ReturnRunTask()
        {
            return Task.Run(() => Task.Delay(10));
        }

        public Task GenerateChildTasksAsync()
        {
            return Task.Run(() => Task.Delay(10))
                .ContinueWith(exec => { Task.Delay(10); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(10); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(10); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(10); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(10); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(10); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(10); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(10); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(10); }, TaskContinuationOptions.AttachedToParent);
        }
    }
}
