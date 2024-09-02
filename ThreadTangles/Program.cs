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



