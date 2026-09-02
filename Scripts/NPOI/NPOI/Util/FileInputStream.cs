using System;
using System.IO;

namespace NPOI.Util;

public class FileInputStream : InputStream
{
	private Stream inner;

	public override bool CanRead => inner.CanRead;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

	public override long Length => inner.Length;

	public override long Position
	{
		get
		{
			return inner.Position;
		}
		set
		{
			inner.Position = value;
		}
	}

	public FileInputStream(Stream fs)
	{
		inner = fs;
	}

	public override void Flush()
	{
		throw new NotImplementedException();
	}

	public override int Read()
	{
		return inner.ReadByte();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}

	public override void Close()
	{
		if (inner != null)
		{
			inner.Close();
		}
	}
}
