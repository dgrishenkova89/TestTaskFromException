using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Jobs;

namespace TestThrowAsync
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [ThreadingDiagnoser]
    [TailCallDiagnoser]
    [MemoryDiagnoser]
    [MinColumn, MaxColumn]
    public class ThrowAsync
    {
        [Params(500, 1000, 2000)]
        public static int counter;

        [Benchmark(Description = "GenerateMultipleSingleTask")]
        public async Task GenerateMultipleSingleTaskAsync()
        {
            var token = new CancellationToken();
            for (int i = 0; i <= counter; i++)
            {
                try
                {
                    await GenerateTaskAsync(token);
                }
                catch (Exception exception)
                {
                    // Console.WriteLine(exception.Message);
                }
            }
        }

        [Benchmark(Baseline = true, Description = "GenerateMultipleTasksFromException")]
        public async Task GenerateMultipleTasksAsync()
        {
            var token = new CancellationToken();
            for (int i = 0; i <= counter; i++)
            {
                try
                {
                    await GenerateMultipleTasksAsync(token);
                }
                catch (Exception exception)
                {
                    // Console.WriteLine(exception.Message);
                }
            }
        }

        [Benchmark(Description = "GenerateMultipleChildTasks")]
        public async Task GenerateMultipleChildTasksAsync()
        {
            var token = new CancellationToken();
            for (int i = 0; i <= counter; i++)
            {
                try
                {
                    await GenerateChildTasksAsync(token);
                }
                catch (Exception exception)
                {
                    // Console.WriteLine(exception.Message);
                }
            }
        }

        [Benchmark(Description = "GenerateTaskFromException")]
        public async Task GenerateTaskFromException()
        {
            for (int i = 0; i <= counter; i++)
            {
                try
                {
                    await GenerateTaskFromExceptionAsync();
                }
                catch (Exception exception)
                {
                    // Console.WriteLine(exception.Message);
                }
            }
        }

        [Benchmark(Description = "GenerateChildTasksFromExceptionAsync")]
        public async Task GenerateChildTasksFromException()
        {
            var token = new CancellationToken();
            for (int i = 0; i <= counter; i++)
            {
                try
                {
                    await GenerateChildTasksFromExceptionAsync(token);
                }
                catch (Exception exception)
                {
                    // Console.WriteLine(exception.Message);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Task GenerateTaskAsync(CancellationToken token)
        {
            return Task.Run(() => throw new Exception("Test exception"), token);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task GenerateChildTasksAsync(CancellationToken token)
        {
            await Task.Run(() =>
                {
                    Task.Delay(100);
                    throw new Exception("Test exception");
                }, token)
                .ContinueWith(exec => { Task.Delay(100); new Exception("Test exception 2"); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { throw new Exception("Test exception 3"); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { throw new InvalidOperationException("Test exception 4"); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { throw new ApplicationException("Test exception 5"); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { throw new Exception("Test exception 6"); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(1000); /*new Exception("Test exception 7"); */}, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { throw new NullReferenceException("Test exception 8"); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(1000); /*new Exception("Test exception 9");*/ }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(1000); new Exception("Test exception 10"); }, TaskContinuationOptions.AttachedToParent);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task GenerateMultipleTasksAsync(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Task.Delay(100);
                // Console.WriteLine("Test 1");
            }, token);
            await Task.Run(() =>
            {
                Task.Delay(100);
                // Console.WriteLine("Test 2");
            }, token);
            await Task.FromException(new Exception("Test exception 1"));
            await Task.Run(() => Console.WriteLine("Test 3"), token);
            await Task.Run(() =>
            {
                Task.Delay(100);
                Console.WriteLine("Test 4");
            }, token);
            await Task.Run(() =>
            {
                Task.Delay(100);
                Console.WriteLine("Test 5");
            }, token);
            await Task.Run(() =>
            {
                Task.Delay(100);
                Console.WriteLine("Test 6");
            }, token);
            await Task.Run(() =>
            {
                Task.Delay(10000);
                throw new Exception("Test exception 2");
            }, token);
        }

        private async Task GenerateTaskFromExceptionAsync()
        {
            await Task.FromException(new Exception("Test exception"));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task GenerateChildTasksFromExceptionAsync(CancellationToken token)
        {
            await Task.Run(() =>
            {
                Task.Delay(100);
                return Task.FromException(new Exception("Test exception"));
            }, token)
                .ContinueWith(exec => { Task.Delay(100); return Task.FromException(new Exception("Test exception 2")); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { return Task.FromException(new Exception("Test exception 3")); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { return Task.FromException(new InvalidOperationException("Test exception 4")); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { return Task.FromException(new ApplicationException("Test exception 5")); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { return Task.FromException(new Exception("Test exception 6")); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(1000); /*new Exception("Test exception 7"); */}, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { return Task.FromException(new NullReferenceException("Test exception 8")); }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(1000); /*new Exception("Test exception 9");*/ }, TaskContinuationOptions.AttachedToParent)
                .ContinueWith(exec => { Task.Delay(1000); return Task.FromException(new Exception("Test exception 10")); }, TaskContinuationOptions.AttachedToParent);
        }
    }
}
