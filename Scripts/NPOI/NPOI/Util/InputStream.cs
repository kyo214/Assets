using System;
using System.IO;

namespace NPOI.Util;

public abstract class InputStream : Stream
{
	private static int MAX_SKIP_BUFFER_SIZE = 2048;

	public abstract int Read();

	public virtual int Read(byte[] b)
	{
		return Read(b, 0, b.Length);
	}

	public override int Read(byte[] b, int off, int len)
	{
		if (b == null)
		{
			throw new ArgumentNullException();
		}
		if (off < 0 || len < 0 || len > b.Length - off)
		{
			throw new IndexOutOfRangeException();
		}
		if (len == 0)
		{
			return 0;
		}
		int num = Read();
		if (num == -1)
		{
			return -1;
		}
		b[off] = (byte)num;
		int i = 1;
		try
		{
			for (; i < len; i++)
			{
				num = Read();
				if (num != -1)
				{
					b[off + i] = (byte)num;
					continue;
				}
				break;
			}
		}
		catch (IOException)
		{
		}
		return i;
	}

	public virtual long Skip(long n)
	{
		long num = n;
		if (n <= 0)
		{
			return 0L;
		}
		int num2 = (int)Math.Min(MAX_SKIP_BUFFER_SIZE, num);
		byte[] buffer = new byte[num2];
		while (num > 0)
		{
			int num3 = Read(buffer, 0, (int)Math.Min(num2, num));
			if (num3 < 0)
			{
				break;
			}
			num -= num3;
		}
		return n - num;
	}

	public virtual int Available()
	{
		return 0;
	}

	public override void Close()
	{
	}

	public virtual void Mark(int readlimit)
	{
	}

	public virtual void Reset()
	{
		throw new IOException("mark/reset not supported");
	}

	public virtual bool MarkSupported()
	{
		return false;
	}
}
