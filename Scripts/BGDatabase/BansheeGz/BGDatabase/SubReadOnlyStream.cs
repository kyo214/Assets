using System;
using System.IO;

namespace BansheeGz.BGDatabase;

internal class SubReadOnlyStream : Stream
{
	private readonly long m_offset;

	private readonly bool m_leaveOpen;

	private long? m_length;

	private Stream m_actualStream;

	private long m_position;

	public override long Length
	{
		get
		{
			ThrowIfDisposed();
			if (!m_length.HasValue)
			{
				m_length = m_actualStream.Length - m_offset;
			}
			return m_length.Value;
		}
	}

	public override long Position
	{
		get
		{
			ThrowIfDisposed();
			return m_position - m_offset;
		}
		set
		{
			ThrowIfDisposed();
			throw new NotSupportedException();
		}
	}

	public override bool CanRead => m_actualStream.CanRead;

	public override bool CanSeek => m_actualStream.CanSeek;

	public override bool CanWrite => false;

	public SubReadOnlyStream(Stream actualStream, bool leaveOpen = false)
	{
		m_actualStream = actualStream ?? throw new ArgumentNullException("superStream");
		m_leaveOpen = leaveOpen;
	}

	public SubReadOnlyStream(Stream actualStream, long offset, long length, bool leaveOpen = false)
		: this(actualStream, leaveOpen)
	{
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset");
		}
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		m_offset = offset;
		m_position = offset;
		m_length = length;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		ThrowIfCantRead();
		ThrowIfDisposed();
		if (m_actualStream.Position != m_position)
		{
			m_actualStream.Seek(m_position, SeekOrigin.Begin);
		}
		if (m_length.HasValue)
		{
			long num = m_offset + m_length.Value;
			if (m_position + count > num)
			{
				count = (int)(num - m_position);
			}
		}
		int num2 = m_actualStream.Read(buffer, offset, count);
		m_position += num2;
		return num2;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		ThrowIfDisposed();
		switch (origin)
		{
		case SeekOrigin.Begin:
			m_position = m_actualStream.Seek(m_offset + offset, SeekOrigin.Begin);
			break;
		case SeekOrigin.End:
			m_position = m_actualStream.Seek(m_offset + Length + offset, SeekOrigin.End);
			break;
		default:
			m_position = m_actualStream.Seek(offset, SeekOrigin.Current);
			break;
		}
		return m_position;
	}

	public override void SetLength(long value)
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}

	public override void Flush()
	{
		throw new NotSupportedException();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && m_actualStream != null)
		{
			if (!m_leaveOpen)
			{
				m_actualStream.Dispose();
			}
			m_actualStream = null;
		}
		base.Dispose(disposing);
	}

	private void ThrowIfDisposed()
	{
		if (m_actualStream == null)
		{
			throw new ObjectDisposedException(GetType().ToString(), "");
		}
	}

	private void ThrowIfCantRead()
	{
		if (!CanRead)
		{
			throw new NotSupportedException();
		}
	}
}
