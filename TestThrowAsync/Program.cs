using BenchmarkDotNet.Running;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestThrowAsync
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var summary = BenchmarkRunner.Run<ThrowAsync>();
                //var summaryAwait = BenchmarkRunner.Run<TestAwait>();
                //var summaryReturn = BenchmarkRunner.Run<TestReturn>();

                Console.WriteLine("Starting call of async method with exception");

                var testAsync = new ThrowAsync();

                await testAsync.GenerateTaskFromException();
                await testAsync.GenerateChildTasksFromException();
                await testAsync.GenerateMultipleSingleTaskAsync();
                await testAsync.GenerateMultipleTasksAsync();
                await testAsync.GenerateMultipleChildTasksAsync();

                Console.WriteLine("Finished async method with exception");

                Console.ReadKey();
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}