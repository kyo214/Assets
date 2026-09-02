using System;
using System.IO;

namespace NPOI.POIFS.FileSystem;

[Obsolete]
public class POIFSDocumentReader : Stream
{
	private bool _closed;

	private int _current_offset;

	private OPOIFSDocument _document;

	private int _document_size;

	private byte[] _tiny_buffer;

	private const int _EOD = 0;

	private bool EOD => _current_offset == _document_size;

	public int Available
	{
		get
		{
			if (_closed)
			{
				throw new IOException("This stream is closed");
			}
			return (int)(Length - Position);
		}
	}

	public override bool CanRead => true;

	public override bool CanSeek => true;

	public override bool CanWrite => false;

	public override long Length => _document_size;

	public override long Position
	{
		get
		{
			return _current_offset;
		}
		set
		{
			_current_offset = Convert.ToInt32(value);
		}
	}

	public POIFSDocumentReader(DocumentEntry document)
	{
		_current_offset = 0;
		_document_size = document.Size;
		_closed = false;
		_tiny_buffer = null;
		if (!(document is DocumentNode))
		{
			throw new IOException("Cannot open internal document storage");
		}
		_document = ((DocumentNode)document).Document;
	}

	public POIFSDocumentReader(OPOIFSDocument document)
	{
		_current_offset = 0;
		_document_size = document.Size;
		_closed = false;
		_tiny_buffer = null;
		_document = document;
	}

	public override void Close()
	{
		_closed = true;
	}

	private void DieIfClosed()
	{
		if (_closed)
		{
			throw new IOException("cannot perform requested operation on a closed stream");
		}
	}

	public override void Flush()
	{
		throw new NotImplementedException();
	}

	public int Read(byte[] b)
	{
		return Read(b, 0, b.Length);
	}

	public override int Read(byte[] b, int off, int len)
	{
		DieIfClosed();
		if (b == null)
		{
			throw new NullReferenceException("buffer is null");
		}
		if (off < 0 || len < 0 || b.Length < off + len)
		{
			throw new IndexOutOfRangeException("can't read past buffer boundaries");
		}
		if (len == 0)
		{
			return 0;
		}
		if (EOD)
		{
			return -1;
		}
		int num = Math.Min(Available, len);
		if (off == 0 && num == b.Length)
		{
			_document.Read(b, _current_offset);
		}
		else
		{
			byte[] array = new byte[num];
			_document.Read(array, _current_offset);
			Array.Copy(array, 0, b, off, num);
		}
		_current_offset += num;
		return num;
	}

	public override int ReadByte()
	{
		DieIfClosed();
		if (EOD)
		{
			return -1;
		}
		if (_tiny_buffer == null)
		{
			_tiny_buffer = new byte[1];
		}
		_document.Read(_tiny_buffer, _current_offset++);
		return _tiny_buffer[0] & 0xFF;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		if (!CanSeek)
		{
			throw new NotSupportedException();
		}
		switch (origin)
		{
		case SeekOrigin.Begin:
			if (0 > offset)
			{
				throw new ArgumentOutOfRangeException("offset", "offset must be positive");
			}
			Position = ((offset < Length) ? offset : Length);
			break;
		case SeekOrigin.Current:
			Position = ((Position + offset < Length) ? (Position + offset) : Length);
			break;
		case SeekOrigin.End:
			Position = Length;
			break;
		default:
			throw new ArgumentException("incorrect SeekOrigin", "origin");
		}
		return Position;
	}

	public override void SetLength(long value)
	{
	}

	public long Skip(long n)
	{
		DieIfClosed();
		if (n < 0)
		{
			return 0L;
		}
		int num = _current_offset + (int)n;
		if (num < _current_offset)
		{
			num = _document_size;
		}
		else if (num > _document_size)
		{
			num = _document_size;
		}
		long result = num - _current_offset;
		_current_offset = num;
		return result;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}
}
