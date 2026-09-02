using System;
using System.IO;

namespace NPOI.POIFS.FileSystem;

[Obsolete]
public class POIFSDocumentWriter : Stream
{
	private int limit;

	private Stream stream;

	private int written;

	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

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

	public POIFSDocumentWriter(Stream stream, int limit)
	{
		this.stream = stream;
		this.limit = limit;
		written = 0;
	}

	public override void Close()
	{
		stream.Close();
	}

	public override void Flush()
	{
		stream.Flush();
	}

	private void LimitCheck(int toBeWritten)
	{
		if (written + toBeWritten > limit)
		{
			throw new IOException("tried to write too much data");
		}
		written += toBeWritten;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return 0L;
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public void Write(int b)
	{
		LimitCheck(1);
		stream.WriteByte((byte)b);
	}

	public void Write(byte[] b)
	{
		Write(b, 0, b.Length);
	}

	public override void Write(byte[] b, int off, int len)
	{
		LimitCheck(len);
		stream.Write(b, off, len);
	}

	public override void WriteByte(byte b)
	{
		LimitCheck(1);
		stream.WriteByte(b);
	}

	public virtual void WriteFiller(int totalLimit, byte fill)
	{
		if (totalLimit > written)
		{
			byte[] array = new byte[totalLimit - written];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = fill;
			}
			stream.Write(array, 0, array.Length);
		}
	}
}
