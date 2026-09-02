using System;
using System.IO;

namespace NPOI.Util;

public class BlockingInputStream : Stream
{
	protected Stream stream;

	public override bool CanRead => true;

	public override bool CanSeek => true;

	public override bool CanWrite => false;

	public override long Length => stream.Length;

	public override long Position
	{
		get
		{
			return stream.Position;
		}
		set
		{
			stream.Position = value;
		}
	}

	public BlockingInputStream(Stream stream)
	{
		this.stream = stream;
	}

	public int Available()
	{
		return (int)(stream.Length - stream.Position);
	}

	public void close()
	{
		stream.Close();
	}

	public void Mark(int readLimit)
	{
		throw new NotImplementedException();
	}

	public bool MarkSupported()
	{
		return false;
	}

	public int Read()
	{
		return stream.ReadByte();
	}

	public int Read(byte[] bf)
	{
		int num = 0;
		int num2 = 4611;
		while (num < bf.Length)
		{
			num2 = stream.ReadByte();
			if (num2 == -1)
			{
				break;
			}
			bf[num++] = (byte)num2;
		}
		if (num == 0 && num2 == -1)
		{
			return -1;
		}
		return num;
	}

	public override int Read(byte[] bf, int s, int l)
	{
		return stream.Read(bf, s, l);
	}

	public void Reset()
	{
		stream.Seek(0L, SeekOrigin.Begin);
	}

	public long Skip(long n)
	{
		return stream.Seek(n, SeekOrigin.Begin);
	}

	public override void Flush()
	{
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return stream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		stream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}
}
