using System;
using System.IO;

namespace NPOI.Util;

public abstract class OutputStream : Stream
{
	public abstract void Write(int b);

	public virtual void Write(byte[] b)
	{
		Write(b, 0, b.Length);
	}

	public override void Write(byte[] b, int off, int len)
	{
		if (b == null)
		{
			throw new NullReferenceException();
		}
		if (off < 0 || off > b.Length || len < 0 || off + len > b.Length || off + len < 0)
		{
			throw new IndexOutOfRangeException();
		}
		if (len != 0)
		{
			for (int i = 0; i < len; i++)
			{
				Write(b[off + i]);
			}
		}
	}
}
