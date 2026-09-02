using System.IO;

namespace NPOI.Util;

public class ByteArrayOutputStream : MemoryStream
{
	public ByteArrayOutputStream()
		: this(32)
	{
	}

	public ByteArrayOutputStream(int size)
		: base(size)
	{
	}

	public virtual void Write(int b)
	{
		WriteByte((byte)b);
	}

	public virtual void Write(byte[] b)
	{
		Write(b, 0, b.Length);
	}

	public void Reset()
	{
		Position = 0L;
	}

	public byte[] ToByteArray()
	{
		return ToArray();
	}
}
