using System;
using System.IO;

namespace NPOI.OpenXml4Net.OPC.Internal;

public class MemoryPackagePartOutputStream : Stream
{
	private MemoryPackagePart _part;

	private MemoryStream _buff;

	public override bool CanRead => false;

	public override bool CanWrite => true;

	public override bool CanSeek => false;

	public override long Length => _buff.Length;

	public override long Position
	{
		get
		{
			return _buff.Position;
		}
		set
		{
			_buff.Position = value;
		}
	}

	public MemoryPackagePartOutputStream(MemoryPackagePart part)
	{
		_part = part;
		_part.data = new MemoryStream();
		_buff = _part.data;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}

	public void Write(int b)
	{
		_buff.WriteByte((byte)b);
	}

	public override void SetLength(long value)
	{
		_buff.SetLength(value);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return _buff.Seek(offset, origin);
	}

	public override void Close()
	{
		Flush();
	}

	public override void Flush()
	{
		_buff.Flush();
		_buff.Position = 0L;
	}

	public override void Write(byte[] b, int off, int len)
	{
		_buff.Write(b, off, len);
	}

	public void Write(byte[] b)
	{
		_buff.Write(b, (int)_buff.Position, b.Length);
	}
}
