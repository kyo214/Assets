using System;

namespace NPOI.Util;

public class BoundedInputStream : ByteArrayInputStream
{
	private ByteArrayInputStream in1;

	private long max;

	public bool IsPropagateClose { get; set; }

	public BoundedInputStream(ByteArrayInputStream in1, long size)
	{
		IsPropagateClose = true;
		max = size;
		this.in1 = in1;
	}

	public BoundedInputStream(ByteArrayInputStream in1)
		: this(in1, -1L)
	{
	}

	public override int Read()
	{
		if (max >= 0 && pos == max)
		{
			return -1;
		}
		int result = in1.Read();
		pos++;
		return result;
	}

	public override int Read(byte[] b)
	{
		return Read(b, 0, b.Length);
	}

	public override int Read(byte[] b, int off, int len)
	{
		if (max >= 0 && pos >= max)
		{
			return -1;
		}
		long num = ((max >= 0) ? Math.Min(len, max - pos) : len);
		int num2 = in1.Read(b, off, (int)num);
		if (num2 == -1)
		{
			return -1;
		}
		pos += num2;
		return num2;
	}

	public override void Close()
	{
		if (IsPropagateClose)
		{
			in1.Close();
		}
	}

	public override void Reset()
	{
		in1.Reset();
		pos = mark;
	}

	public override void Mark(int readlimit)
	{
		in1.Mark(readlimit);
		mark = pos;
	}

	public override bool MarkSupported()
	{
		return in1.MarkSupported();
	}
}
