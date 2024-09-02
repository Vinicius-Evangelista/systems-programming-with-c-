using static System.Threading.Thread;
namespace ExtensionLibrary;
public static class StringExtensions
{
    public static string Dump(this string message,         ConsoleColor printColor = ConsoleColor.Cyan)
    {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = printColor;
      Console.WriteLine($"({CurrentThread.ManagedThreadId})\t :         {message}");
        Console.ForegroundColor = oldColor;
        return message;
    }
}