using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Iterations;

public class TasksAndThreadsComparison
{

    public static Semaphore semaphore = new(20,20);
    public static SemaphoreSlim semaphoreSlim = new(20);

    public static void Main()
    {
        int loopCounter = 100;

        //Thread work
        var threadList = new List<Thread>();
        
        Stopwatch threadStopWatch = new();
        threadStopWatch.Start();
        for (int i = 0; i < loopCounter; i++)
        {
            var t = new Thread(DoSomethingThread);
            t.Start(i);
            threadList.Add(t);
        }


        for (int i = 0; i < loopCounter; i++)
        {
            threadList[i].Join();
        }
        threadStopWatch.Stop();
        Console.WriteLine("Thread elapsed : " + threadStopWatch.ElapsedMilliseconds);
        
        // Task work
        var taskList = new List<Task>();

        Stopwatch taskStopWatch = new();
        taskStopWatch.Start();
        for (int i = 0; i < loopCounter; i++)
        {
            taskList.Add(DoSomethingTask(i));
        }

        Task.WaitAll(taskList.ToArray());
        taskStopWatch.Stop();
        Console.WriteLine("Task elapsed : " + taskStopWatch.ElapsedMilliseconds);

        Console.ReadLine();
    }

    public static async Task DoSomethingTask(int i)
    {
        Console.WriteLine($"Thread {Environment.CurrentManagedThreadId} - DoSomethingTask - Instance {i} waiting");
        // semaphore.WaitOne();
        await semaphoreSlim.WaitAsync();
        Console.WriteLine($"Thread {Environment.CurrentManagedThreadId} - DoSomethingTask - Instance {i} started");
        await Task.Delay(1000);
        // Thread.Sleep(1000); <-- in tasks, state machine up to here is on thread 1, so this affectively serializes
        Console.WriteLine($"Thread {Environment.CurrentManagedThreadId} - DoSomethingTask - Instance {i} finished");
        semaphoreSlim.Release();
    }
    public static void DoSomethingThread(object i)
    {
        Console.WriteLine($"Thread {Environment.CurrentManagedThreadId} - DoSomethingThread - Instance {i} waiting");
        // semaphore.WaitOne();
        semaphoreSlim.Wait();
        Console.WriteLine($"Thread {Environment.CurrentManagedThreadId} - DoSomethingThread - Instance {i} started");
        Thread.Sleep(1000);
        Console.WriteLine($"Thread {Environment.CurrentManagedThreadId} - DoSomethingThread - Instance {i} finished");
        semaphoreSlim.Release();
    }
}

// // Create the task object by using an Action(Of Object) to pass in the loop
// // counter. This produces an unexpected result.
