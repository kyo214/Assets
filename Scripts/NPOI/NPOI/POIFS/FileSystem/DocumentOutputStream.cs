using System.IO;
using NPOI.Util;

namespace NPOI.POIFS.FileSystem;

public class DocumentOutputStream : MemoryStream
{
	private Stream _stream;

	private int _limit;

	private int _written;

	public DocumentOutputStream(Stream stream, int limit)
	{
		_stream = stream;
		_limit = limit;
		_written = 0;
	}

	public void Write(int b)
	{
		LimitCheck(1);
		_stream.WriteByte((byte)b);
	}

	public void Write(byte[] b)
	{
		Write(b, 0, b.Length);
	}

	public override void Write(byte[] b, int off, int len)
	{
		LimitCheck(len);
		_stream.Write(b, off, len);
	}

	public override void Flush()
	{
		_stream.Flush();
	}

	public override void Close()
	{
	}

	public void WriteFiller(int totalLimit, byte Fill)
	{
		if (totalLimit > _written)
		{
			byte[] array = new byte[totalLimit - _written];
			Arrays.Fill(array, Fill);
			_stream.Write(array, 0, array.Length);
		}
	}

	private void LimitCheck(int toBeWritten)
	{
		if (_written + toBeWritten > _limit)
		{
			throw new IOException("tried to write too much data");
		}
		_written += toBeWritten;
	}
}
