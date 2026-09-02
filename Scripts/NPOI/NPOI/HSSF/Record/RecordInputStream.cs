using System;
using System.IO;
using NPOI.HSSF.Record.Crypto;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class RecordInputStream : Stream, ILittleEndianInput
{
	public const short MAX_RECORD_DATA_SIZE = 8224;

	private const int INVALID_SID_VALUE = -1;

	private const int DATA_LEN_NEEDS_TO_BE_READ = -1;

	protected int _currentSid;

	protected int _currentDataLength = -1;

	protected int _nextSid = -1;

	private int _currentDataOffset;

	private long pos;

	private BiffHeaderInput _bhi;

	private ILittleEndianInput _dataInput;

	public short Sid => (short)_currentSid;

	public override long Position
	{
		get
		{
			return pos;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public long CurrentLength => _currentDataLength;

	public int RecordOffset => _currentDataOffset;

	public bool HasNextRecord
	{
		get
		{
			if (_currentDataLength != -1 && _currentDataLength != _currentDataOffset)
			{
				throw new LeftoverDataException(_currentSid, Remaining);
			}
			if (_currentDataLength != -1)
			{
				_nextSid = ReadNextSid();
			}
			return _nextSid != -1;
		}
	}

	public int Remaining
	{
		get
		{
			if (_currentDataLength == -1)
			{
				return 0;
			}
			return _currentDataLength - _currentDataOffset;
		}
	}

	public bool IsContinueNext
	{
		get
		{
			if (_currentDataLength != -1 && _currentDataOffset != _currentDataLength)
			{
				throw new InvalidOperationException("Should never be called before end of current record");
			}
			if (!HasNextRecord)
			{
				return false;
			}
			return _nextSid == 60;
		}
	}

	public override long Length => _currentDataLength;

	public override bool CanRead => true;

	public override bool CanSeek => false;

	public override bool CanWrite => false;

	public RecordInputStream(Stream in1)
		: this(in1, null, 0)
	{
	}

	public RecordInputStream(Stream in1, Biff8EncryptionKey key, int initialOffset)
	{
		if (key == null)
		{
			_dataInput = SimpleHeaderInput.GetLEI(in1);
			_bhi = new SimpleHeaderInput(in1);
		}
		else
		{
			_dataInput = (ILittleEndianInput)(_bhi = new Biff8DecryptingStream(in1, initialOffset, key));
		}
		_nextSid = ReadNextSid();
	}

	public int Available()
	{
		return Remaining;
	}

	public int Read()
	{
		CheckRecordPosition(1);
		_currentDataOffset++;
		pos++;
		return _dataInput.ReadByte();
	}

	private int ReadNextSid()
	{
		int num = _bhi.Available();
		if (num < 4)
		{
			_ = 0;
			return -1;
		}
		int num2 = _bhi.ReadRecordSID();
		if (num2 == -1)
		{
			throw new RecordFormatException("Found invalid sid (" + num2 + ")");
		}
		_currentDataLength = -1;
		return num2;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotSupportedException();
	}

	public void NextRecord()
	{
		if (_nextSid == -1)
		{
			throw new InvalidDataException("EOF - next record not available");
		}
		if (_currentDataLength != -1)
		{
			throw new InvalidDataException("Cannot call nextRecord() without checking hasNextRecord() first");
		}
		_currentSid = _nextSid;
		_currentDataOffset = 0;
		_currentDataLength = _bhi.ReadDataSize();
		pos += 2L;
		if (_currentDataLength > 8224)
		{
			throw new RecordFormatException("The content of an excel record cannot exceed " + (short)8224 + " bytes");
		}
	}

	protected void CheckRecordPosition(int requiredByteCount)
	{
		int remaining = Remaining;
		if (remaining < requiredByteCount)
		{
			if (remaining != 0 || !IsContinueNext)
			{
				throw new RecordFormatException("Not enough data (" + remaining + ") to read requested (" + requiredByteCount + ") bytes");
			}
			NextRecord();
		}
	}

	public override int ReadByte()
	{
		CheckRecordPosition(1);
		_currentDataOffset++;
		pos++;
		return _dataInput.ReadByte();
	}

	public short ReadShort()
	{
		CheckRecordPosition(2);
		_currentDataOffset += 2;
		pos += 2L;
		return _dataInput.ReadShort();
	}

	public int ReadInt()
	{
		CheckRecordPosition(4);
		_currentDataOffset += 4;
		pos += 4L;
		return _dataInput.ReadInt();
	}

	public long ReadLong()
	{
		CheckRecordPosition(8);
		_currentDataOffset += 8;
		pos += 8L;
		return _dataInput.ReadLong();
	}

	public int ReadUByte()
	{
		int num = ReadByte();
		if (num < 0)
		{
			num += 256;
		}
		return num;
	}

	public int ReadUShort()
	{
		CheckRecordPosition(2);
		_currentDataOffset += 2;
		pos += 2L;
		return _dataInput.ReadUShort();
	}

	public double ReadDouble()
	{
		CheckRecordPosition(8);
		_currentDataOffset += 8;
		double result = BitConverter.Int64BitsToDouble(_dataInput.ReadLong());
		pos += 8L;
		return result;
	}

	public void ReadFully(byte[] buf)
	{
		ReadFully(buf, 0, buf.Length);
	}

	public void ReadFully(byte[] buf, int off, int len)
	{
		int num = len;
		if (buf == null)
		{
			throw new ArgumentNullException();
		}
		if (off < 0 || len < 0 || len > buf.Length - off)
		{
			throw new IndexOutOfRangeException();
		}
		while (len > 0)
		{
			int num2 = Math.Min(Available(), len);
			if (num2 == 0)
			{
				if (!HasNextRecord)
				{
					throw new RecordFormatException("Can't read the remaining " + len + " bytes of the requested " + num + " bytes. No further record exists.");
				}
				NextRecord();
				num2 = Math.Min(Available(), len);
			}
			CheckRecordPosition(num2);
			_dataInput.ReadFully(buf, off, num2);
			_currentDataOffset += num2;
			off += num2;
			len -= num2;
			pos += num2;
		}
	}

	public string ReadUnicodeLEString(int requestedLength)
	{
		return ReadStringCommon(requestedLength, pIsCompressedEncoding: false);
	}

	public string ReadCompressedUnicode(int requestedLength)
	{
		return ReadStringCommon(requestedLength, pIsCompressedEncoding: true);
	}

	private string ReadStringCommon(int requestedLength, bool pIsCompressedEncoding)
	{
		if (requestedLength < 0 || requestedLength > 1048576)
		{
			throw new ArgumentException("Bad requested string length (" + requestedLength + ")");
		}
		char[] array = new char[requestedLength];
		bool flag = pIsCompressedEncoding;
		int i = 0;
		while (true)
		{
			int num = (flag ? Remaining : (Remaining / 2));
			if (requestedLength - i <= num)
			{
				for (; i < requestedLength; i++)
				{
					char c = ((!flag) ? ((char)ReadShort()) : ((char)ReadUByte()));
					array[i] = c;
				}
				return new string(array);
			}
			while (num > 0)
			{
				char c2 = ((!flag) ? ((char)ReadShort()) : ((char)ReadUByte()));
				array[i] = c2;
				i++;
				num--;
			}
			if (!IsContinueNext)
			{
				throw new RecordFormatException("Expected to find a ContinueRecord in order to read remaining " + (requestedLength - i) + " of " + requestedLength + " chars");
			}
			if (Remaining != 0)
			{
				break;
			}
			NextRecord();
			flag = (byte)ReadByte() == 0;
		}
		throw new RecordFormatException("Odd number of bytes(" + Remaining + ") left behind");
	}

	public string ReadString()
	{
		int requestedLength = ReadUShort();
		byte b = (byte)ReadByte();
		return ReadStringCommon(requestedLength, b == 0);
	}

	public byte[] ReadRemainder()
	{
		int remaining = Remaining;
		if (remaining == 0)
		{
			return new byte[0];
		}
		byte[] array = new byte[remaining];
		ReadFully(array);
		return array;
	}

	public byte[] ReadAllContinuedRemainder()
	{
		using MemoryStream memoryStream = new MemoryStream(16448);
		while (true)
		{
			byte[] array = ReadRemainder();
			memoryStream.Write(array, 0, array.Length);
			if (!IsContinueNext)
			{
				break;
			}
			NextRecord();
		}
		return memoryStream.ToArray();
	}

	public override void SetLength(long value)
	{
		_currentDataLength = (int)value;
	}

	public override void Flush()
	{
		throw new NotSupportedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotSupportedException();
	}

	public override int Read(byte[] b, int off, int len)
	{
		int num = Math.Min(len, Remaining);
		if (num == 0)
		{
			return 0;
		}
		ReadFully(b, off, num);
		return num;
	}

	public int GetNextSid()
	{
		return _nextSid;
	}
}
