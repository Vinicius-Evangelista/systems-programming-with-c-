//Optimizing a copying of an array
using System.Buffers;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;

int [] mybufferf = new int[100];

var firstPart = new Span<int>(mybufferf, 0, 50);
var secondPart = new Span<int>(mybufferf, 50, 50);

foreach (var part in secondPart)
    Console.WriteLine($"{part}, ");

var simpleClass = new SimpleClass()
{
    X = 42,
    Y = "Systems Programming Rules!"
};

var memory = IntPtr.Zero;

try
{
    byte[] serializedData = SimpleClass.SerializeToByteArray(simpleClass);
    memory = Marshal.AllocHGlobal(serializedData.Length); 
    Marshal.Copy(serializedData, 0, memory, serializedData.Length); 
    var deserializedSimpleClass = SimpleClass.DeserializeFromByteArray(memory, serializedData.Length);
}
finally
{

        Marshal.FreeHGlobal(memory);
}


//{
//    var pointer = IntPtr.Zero;
//try
//{
//    byte[] serializedData = SimpleClass.SerializeToByteArray(simpleClass);
//    pointer = Marshal.AllocHGlobal(serializedData.Length);
//    unsafe
//    {
//        // copy the data using pointer arithmetic
//        byte* pByte = (byte*)pointer;
//        for (int i = 0; i < serializedData.Length; i++)
//        {
//            *pByte = serializedData[i];
//            pByte++;
//        }
//        //deserialization is done here
//        byte[] deserializeData = new byte[serializedData.Length];
//        pByte = (byte*)pointer;
//        for (int i = 0; i < serializedData.Length; i++)
//        {
//            deserializeData[i] = *pByte;
//            pByte++;
//        }
//        var deserializedObject = MessagePackSerializer.Deserialize<SimpleClass>(deserializeData);
//    }
//}
//finally
//{
//    Marshal.FreeHGlobal(pointer);
//}
//}