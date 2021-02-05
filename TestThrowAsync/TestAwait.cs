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
    public class TestAwait
    {
        [Params(500, 1000, 2000)]
        public static int counter;

        [Benchmark(Description = "AwaitSingleTask")]
        public async Task AwaitTask()
        {
            await GetSecondAwait();
        }

        [Benchmark(Description = "AwaitForMultipleAwaitTask")]
        public async Task AwaitForMultipleAwaitTask()
        {
            for (int i = 0; i <= counter; i++)
            {
                await GetSecondAwait();
            }
        }

        private async Task GetSecondAwait()
        {
            await GetThirdAwait();
        }

        private async Task GetThirdAwait()
        {
            await GetFourthAwait();
        }

        private async Task GetFourthAwait()
        {
            await GetFifthAwait();
        }

        private async Task GetFifthAwait()
        {
            await GetSixthAwait();
        }

        private async Task GetSixthAwait()
        {
            await GenerateChildTasksAsync();
        }

        private async Task AwaitRunTask()
        {
            await Task.Run(() => Task.Delay(10));
        }

        public async Task GenerateChildTasksAsync()
        {
            await Task.Run(() => Task.Delay(10))
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
