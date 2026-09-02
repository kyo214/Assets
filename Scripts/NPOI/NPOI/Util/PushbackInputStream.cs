using System;
using System.IO;

namespace NPOI.Util;

public class PushbackInputStream : FilterInputStream
{
	protected byte[] buf;

	private int bufint = -1;

	protected int pos;

	public override bool CanRead => input.CanRead;

	public override bool CanSeek => input.CanSeek;

	public override bool CanWrite => input.CanWrite;

	public override long Length => input.Length;

	public override long Position
	{
		get
		{
			return input.Position;
		}
		set
		{
			input.Position = value;
		}
	}

	public PushbackInputStream(InputStream input)
		: this(input, 1)
	{
	}

	public PushbackInputStream(InputStream input, int size)
		: base(input)
	{
		if (size <= 0)
		{
			throw new ArgumentException("size <= 0");
		}
		buf = new byte[size];
		pos = size;
	}

	protected override void Dispose(bool disposing)
	{
		input = null;
		base.Dispose(disposing);
	}

	public override int ReadByte()
	{
		if (bufint != -1)
		{
			int result = bufint;
			bufint = -1;
			return result;
		}
		return input.ReadByte();
	}

	public override int Read()
	{
		ensureOpen();
		if (pos < buf.Length)
		{
			return buf[pos++] & 0xFF;
		}
		return base.Read();
	}

	public override int Read(byte[] b, int off, int len)
	{
		ensureOpen();
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
		int num = buf.Length - pos;
		if (num > 0)
		{
			if (len < num)
			{
				num = len;
			}
			Array.Copy(buf, pos, b, off, num);
			pos += num;
			off += num;
			len -= num;
		}
		if (len > 0)
		{
			len = base.Read(b, off, len);
			if (len == -1)
			{
				if (num != 0)
				{
					return num;
				}
				return -1;
			}
			return num + len;
		}
		return num;
	}

	public virtual void Unread(int b)
	{
		ensureOpen();
		if (pos == 0)
		{
			throw new IOException("Push back buffer is full");
		}
		buf[--pos] = (byte)b;
	}

	public void Unread(byte[] b)
	{
		Unread(b, 0, b.Length);
	}

	public override int Available()
	{
		ensureOpen();
		int num = buf.Length - pos;
		int num2 = base.Available();
		if (num <= int.MaxValue - num2)
		{
			return num + num2;
		}
		return int.MaxValue;
	}

	private void ensureOpen()
	{
		if (input == null)
		{
			throw new IOException("Stream closed");
		}
	}

	public void Unread(byte[] b, int off, int len)
	{
		ensureOpen();
		if (len > pos)
		{
			throw new IOException("Push back buffer is full");
		}
		pos -= len;
		Array.Copy(b, off, buf, pos, len);
	}

	public override long Skip(long n)
	{
		ensureOpen();
		if (n <= 0)
		{
			return 0L;
		}
		long num = buf.Length - pos;
		if (num > 0)
		{
			if (n < num)
			{
				num = n;
			}
			pos += (int)num;
			n -= num;
		}
		if (n > 0)
		{
			num += base.Skip(n);
		}
		return num;
	}

	public override bool MarkSupported()
	{
		return false;
	}

	public override void Close()
	{
		if (input != null)
		{
			input.Close();
			input = null;
			buf = null;
		}
	}

	public override void Flush()
	{
		input.Flush();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return input.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		input.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		input.Write(buffer, offset, count);
	}

	public override void WriteByte(byte value)
	{
		input.WriteByte(value);
	}
}
