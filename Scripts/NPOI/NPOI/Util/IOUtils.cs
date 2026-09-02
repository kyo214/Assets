using System;
using System.IO;

namespace NPOI.Util;

public class IOUtils
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(IOUtils));

	public static byte[] PeekFirst8Bytes(InputStream stream)
	{
		stream.Mark(8);
		byte[] array = new byte[8];
		int num = ReadFully(stream, array);
		if (num < 1)
		{
			throw new EmptyFileException();
		}
		if (stream is PushbackInputStream)
		{
			stream.Position -= num;
		}
		else
		{
			stream.Reset();
		}
		return array;
	}

	public static byte[] PeekFirst8Bytes(Stream stream)
	{
		long position = stream.Position;
		byte[] array = new byte[8];
		int num = ReadFully(stream, array);
		if (num < 1)
		{
			throw new EmptyFileException();
		}
		if (stream is PushbackInputStream)
		{
			stream.Position -= num;
		}
		else
		{
			stream.Position = position;
		}
		return array;
	}

	public static byte[] ToByteArray(Stream stream)
	{
		return ToByteArray(stream, int.MaxValue);
	}

	public static byte[] ToByteArray(Stream stream, int length)
	{
		ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream((length == int.MaxValue) ? 4096 : length);
		byte[] array = new byte[4096];
		int num = 0;
		int num2;
		do
		{
			num2 = stream.Read(array, 0, Math.Min(array.Length, length - num));
			num += Math.Max(num2, 0);
			if (num2 > 0)
			{
				byteArrayOutputStream.Write(array, 0, num2);
			}
		}
		while (num < length && num2 > 0);
		if (length != int.MaxValue && num < length)
		{
			throw new IOException("unexpected EOF");
		}
		return byteArrayOutputStream.ToByteArray();
	}

	public static byte[] ToByteArray(ByteBuffer buffer, int length)
	{
		if (buffer.HasBuffer && buffer.Offset == 0)
		{
			return buffer.Buffer;
		}
		byte[] array = new byte[length];
		buffer.Read(array);
		return array;
	}

	public static int ReadFully(Stream stream, byte[] b)
	{
		return ReadFully(stream, b, 0, b.Length);
	}

	public static int ReadFully(Stream stream, byte[] b, int off, int len)
	{
		int num = 0;
		do
		{
			int num2 = stream.Read(b, off + num, len - num - off);
			if (num2 <= 0)
			{
				if (num != 0)
				{
					return num;
				}
				return -1;
			}
			num += num2;
		}
		while (num != len);
		return num;
	}

	public static void Copy(Stream inp, Stream out1)
	{
		byte[] array = new byte[4096];
		int count;
		while ((count = inp.Read(array, 0, array.Length)) > 0)
		{
			out1.Write(array, 0, count);
		}
	}

	public static long CalculateChecksum(byte[] data)
	{
		return (long)new CRC32().ByteCRC(ref data);
	}

	public static void CloseQuietly(Stream closeable)
	{
		if (closeable == null)
		{
			return;
		}
		try
		{
			closeable.Close();
		}
		catch (Exception ex)
		{
			logger.Log(7, "Unable to close resource: " + ex, ex);
		}
	}

	public static void CloseQuietly(ICloseable closeable)
	{
		if (closeable == null)
		{
			return;
		}
		try
		{
			closeable.Close();
		}
		catch (Exception ex)
		{
			logger.Log(7, "Unable to close resource: " + ex, ex);
		}
	}
}
