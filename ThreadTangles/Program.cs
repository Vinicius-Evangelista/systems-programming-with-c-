using System.Runtime.InteropServices;

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
