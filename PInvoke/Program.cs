using System.Runtime.InteropServices;

[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
static extern bool WriteConsole
(IntPtr hConsoleOutput, 
 string lpBuffer,
 uint nNumberOfCharsToWrite,
 out uint lpNumbersOfCharsWritten,
 IntPtr lpReserved
);

[DllImport("Kernel32.dll", SetLastError = true)]
static extern IntPtr GetStdHandle(int nStdHandle);

const int STD_OUTPUT_HANDLE = -11;

IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);

if (stdHandle == IntPtr.Zero)
{
    Console.WriteLine("Could not retrieve standard output");
    return;
}

string output = "Hello, System Programmers!";

uint charsWritten;

if (!WriteConsole(stdHandle, output, (uint)output.Length, out charsWritten, IntPtr.Zero))
{
    Console.WriteLine("Failed to write using Win32 API.");
}


