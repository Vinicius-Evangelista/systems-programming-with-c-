using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using ExtensionLibrary;


Console.WriteLine("-------------- UNMANAGED EXAMPLE ----------------");

uint threadId;
var threadHandle = CreateThread(IntPtr.Zero, 0, 
MyThreadFunction, IntPtr.Zero,
0, out threadId);

WaitForSingleObject(threadHandle, 1000);

CloseHandle(threadHandle);


static uint MyThreadFunction(IntPtr lpParameter)
{
    for (int i = 0; i < 1000; i++)
        Console.WriteLine("Unmanaged thread");
    return 0;
}

Console.WriteLine("----------------- MANAGED EXAMPLE ------------------");

var myHugeStackSiZe = 8 * 1024 * 1024;
var myManagedThread = new Thread(MyThreadFunction2, myHugeStackSiZe);

myManagedThread.Start();
myManagedThread.Join();


static void MyThreadFunction2()
{
    for (var i = 0; i < 1000; i++)
        Console.WriteLine("Managed thread");
}

Console.WriteLine("----------------- MULTI THREAD EXAMPLE ------------------");

static void MyThreadMultiple(object? myObjectData)
{
    // Verify that we have a ThreadData object
    if (myObjectData is not ThreadData myData)
        throw new ArgumentException("Parameter is not a                     ThreadData object");
    // Get the thread ID
    var currentThreadId = Thread.CurrentThread.ManagedThreadId;
    // Write the data to the Console
    Console.WriteLine(
        $"Managed thread in Thread {currentThreadId} " +
        $"with loop counter {myData.LoopCounter}");
}

for (int i = 0; i < 100; i++)
{
    ThreadData threadData = new(i);
    var newTread = new Thread(MyThreadMultiple);
    newTread.Start(threadData);
    Console.ReadKey();
}

Console.WriteLine("------------- Using the thread pool -------------");
for(int i = 0; i < 100; i++)
{
    ThreadData threadData = new(i);
    ThreadPool.QueueUserWorkItem(MyThreadMultiple, threadData);
    Console.ReadKey();
}

Console.WriteLine("------------- Using the TPL  -------------");

static void DoWork(int id)
{
    Console.WriteLine($"call Id {id}, " +
                      $"running on thread " +
                      $"{Thread.CurrentThread.ManagedThreadId}.");
}


Console.WriteLine($"Our main thread id = {Thread.CurrentThread.ManagedThreadId}.");
Task myTask = new Task(() => DoWork(1));
myTask.ContinueWith((prevTask) => DoWork(2));
myTask.Start();


Console.WriteLine("------------- Parallel Library -------------");
Console.WriteLine($"Our main thread id = {Thread.CurrentThread.ManagedThreadId}.");
int[] myIds = {1, 2, 3, 4, 5, 6, 7, 8, 8, 9, 10};

Parallel.ForEach(myIds, (i) => DoWork(i));


Console.WriteLine("------------- Async/Await -------------");
await DoWork2();
// The program is paused until DoWork is finished.
// This is a waste of CPU!
"Just before calling the long-running DoWork()".Dump(ConsoleColor.DarkBlue);
"Program has finished".Dump(ConsoleColor.DarkBlue);
Console.ReadKey();
static async Task DoWork2()
{
    "We are doing important stuff!".Dump(ConsoleColor.DarkYellow);
    // Do something useful, then wait a bit.
    await Task.Delay(1000);
}

Console.WriteLine("------------- Concurrent Collections -------------");
// var allLines = new List<string>();
// for(int i = 0; i < 1000; i++)
// {
//     allLines.Add($"Line {i:000}");
// }
// ThreadPool.QueueUserWorkItem((_) =>
// {
//     Thread.Sleep(1000);
//     allLines.Clear();
// });
// await DumpArray(allLines);
// "Main app is done.\nPress any key to stop.".Dump(ConsoleColor.White);
// Console.ReadKey();
// return 0;
// async Task DumpArray(List<string> someData)
// {
//     foreach(var data in someData)
//     {
//         data.Dump(ConsoleColor.Yellow);
//         await Task.Delay(100);
//     }
// }
//

Console.WriteLine("------------- Blocking Collection --------------");
// We have a collection that blocks as soon as
// 5 items have been added. Before this thread
// can continue, one has to be taken away first.
var allLines2 = new BlockingCollection<string>(boundedCapacity:5);
ThreadPool.QueueUserWorkItem((_) => {
    for (int i = 0; i < 10; i++)
    {
        allLines2.Add($"Line {i:000}");
        Thread.Sleep(1000);
    }
    allLines2.CompleteAdding();
});
// Give the first thread some time to add items before
// we take them away again.
Thread.Sleep(6000);
// Read all items by taking them away
ThreadPool.QueueUserWorkItem((_) => {
    while (!allLines2.IsCompleted)
    {
        try
        {
            var item = allLines2.Take();
            item.Dump(ConsoleColor.Yellow);
            Thread.Sleep(10);
        }
        catch (InvalidOperationException)
        {
            // This can happen if
            // CompleteAdding has been called
            // but the collection is already empty
            // in our case: this thread finished before the
            // first one
        }
    }
});
"Main app is done.\nPress any key to stop.".Dump(ConsoleColor.White);
Console.ReadKey();




Console.WriteLine("------------- Thread Safe Techiniques -------------");
int iterationCount = 100;
ThreadPool.QueueUserWorkItem(async (state) =>
{
    await Task.Delay(500);
    iterationCount = 0;
    $"We are stopping it...".Dump(ConsoleColor.Red);
});
await WaitAWhile();
$"In the main part of the app.".Dump(ConsoleColor.White);
"Main app is done.\nPress any key to stop.".Dump(ConsoleColor.White);
Console.ReadKey();
return 0;
async Task WaitAWhile()
{
    do
    {
        $"In the loop at iterations {iterationCount}".            Dump(ConsoleColor.Yellow);
        await Task.Delay(1);
    }while (--iterationCount > 0) ;
}

// Value types are thread safe because they are passed by value, so it's a copy
// Reference Types aren't thread safe because they are passed by reference, so any thread can change it
// we have the lock statement, that allows only one thread execute some piece of code

[DllImport("kernel32.dll", SetLastError = true)]
static extern IntPtr CreateThread(
    IntPtr lpThreadAttributes,
    uint dwStackSize,
    ThreadProc lpStartAddres,
    IntPtr lpParameter,
    uint dwCreationFlgs,
    out uint lpThreadId
);

[DllImport("kernel32.dll", SetLastError = true)]
static extern bool CloseHandle(IntPtr hObject);

[DllImport("kernel32.dll", SetLastError = true)]
static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

delegate uint ThreadProc(IntPtr lpParameter);
internal record ThreadData(int LoopCounter);



