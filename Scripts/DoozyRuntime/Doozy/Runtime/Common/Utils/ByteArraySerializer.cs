using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Doozy.Runtime.Common.Utils;

public static class ByteArraySerializer
{
	public static byte[] Serialize<T>(this T data)
	{
		using MemoryStream memoryStream = new MemoryStream();
		new BinaryFormatter().Serialize(memoryStream, data);
		return memoryStream.ToArray();
	}

	public static T Deserialize<T>(this byte[] byteArray)
	{
		using MemoryStream serializationStream = new MemoryStream(byteArray);
		return (T)new BinaryFormatter().Deserialize(serializationStream);
	}
}
