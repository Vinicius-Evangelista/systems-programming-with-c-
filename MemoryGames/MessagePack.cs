using System.Runtime.InteropServices;
using MessagePack;

[MessagePackObject]
public class SimpleClass
{
    [Key(0)]
    public int X {get; set;}

    [Key(1)]
    public string Y {get; set;}


    public static byte[] SerializeToByteArray(SimpleClass simpleClass)
    {
        byte[] data = MessagePackSerializer.Serialize(simpleClass);

        return data;

    }

    public static SimpleClass DeserializeFromByteArray(IntPtr ptr, int length)
    {
        byte[] data = new byte[length];

        Marshal.Copy(ptr, data, 0, length);

        var simpleClass = MessagePackSerializer.Deserialize<SimpleClass>(data);

        return simpleClass;
    }
}