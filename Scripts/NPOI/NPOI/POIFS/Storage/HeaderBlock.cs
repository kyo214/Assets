using System;
using System.IO;
using NPOI.HSSF;
using NPOI.POIFS.Common;
using NPOI.POIFS.FileSystem;
using NPOI.Util;

namespace NPOI.POIFS.Storage;

public class HeaderBlock : HeaderBlockConstants
{
	private static byte[] MAGIC_BIFF2 = new byte[8] { 9, 0, 4, 0, 0, 0, 112, 0 };

	private static byte[] MAGIC_BIFF3 = new byte[8] { 9, 2, 6, 0, 0, 0, 112, 0 };

	private static byte[] MAGIC_BIFF4a = new byte[8] { 9, 4, 6, 0, 0, 0, 112, 0 };

	private static byte[] MAGIC_BIFF4b = new byte[8] { 9, 4, 6, 0, 0, 0, 0, 1 };

	private static byte _default_value = byte.MaxValue;

	private POIFSBigBlockSize bigBlockSize;

	private int _bat_count;

	private int _property_start;

	private int _sbat_start;

	private int _sbat_count;

	private int _xbat_start;

	private int _xbat_count;

	private byte[] _data;

	public int PropertyStart
	{
		get
		{
			return _property_start;
		}
		set
		{
			_property_start = value;
		}
	}

	public int SBATStart
	{
		get
		{
			return _sbat_start;
		}
		set
		{
			_sbat_start = value;
		}
	}

	public int SBATCount
	{
		get
		{
			return _sbat_count;
		}
		set
		{
			_sbat_count = value;
		}
	}

	public int SBATBlockCount
	{
		get
		{
			return _sbat_count;
		}
		set
		{
			_sbat_count = value;
		}
	}

	public int BATCount
	{
		get
		{
			return _bat_count;
		}
		set
		{
			_bat_count = value;
		}
	}

	public int[] BATArray
	{
		get
		{
			int[] array = new int[Math.Min(_bat_count, 109)];
			int num = 76;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = LittleEndian.GetInt(_data, num);
				num += 4;
			}
			return array;
		}
		set
		{
			int num = Math.Min(value.Length, 109);
			int num2 = 109 - num;
			int num3 = 76;
			for (int i = 0; i < num; i++)
			{
				LittleEndian.PutInt(_data, num3, value[i]);
				num3 += 4;
			}
			for (int j = 0; j < num2; j++)
			{
				LittleEndian.PutInt(_data, num3, -1);
				num3 += 4;
			}
		}
	}

	public int XBATCount
	{
		get
		{
			return _xbat_count;
		}
		set
		{
			_xbat_count = value;
		}
	}

	public int XBATIndex
	{
		get
		{
			return _xbat_start;
		}
		set
		{
			_xbat_count = value;
		}
	}

	public int XBATStart
	{
		set
		{
			_xbat_start = value;
		}
	}

	public POIFSBigBlockSize BigBlockSize => bigBlockSize;

	public HeaderBlock(Stream stream)
	{
		try
		{
			stream.Position = 0L;
			PrivateHeaderBlock(ReadFirst512(stream));
			if (bigBlockSize.GetBigBlockSize() != 512)
			{
				byte[] b = new byte[bigBlockSize.GetBigBlockSize() - 512];
				IOUtils.ReadFully(stream, b);
			}
		}
		catch (IOException ex)
		{
			throw ex;
		}
	}

	public HeaderBlock(ByteBuffer buffer)
		: this(IOUtils.ToByteArray(buffer, 512))
	{
	}

	public HeaderBlock(byte[] buffer)
	{
		try
		{
			PrivateHeaderBlock(buffer);
		}
		catch (IOException ex)
		{
			throw ex;
		}
	}

	public void PrivateHeaderBlock(byte[] data)
	{
		_data = data;
		long num = LittleEndian.GetLong(_data, 0);
		if (num != -2226271756974174256L)
		{
			if (cmp(POIFSConstants.OOXML_FILE_HEADER, data))
			{
				throw new OfficeXmlFileException("The supplied data appears to be in the Office 2007+ XML. You are calling the part of POI that deals with OLE2 Office Documents. You need to call a different part of POI to process this data (eg XSSF instead of HSSF)");
			}
			if (cmp(POIFSConstants.RAW_XML_FILE_HEADER, data))
			{
				throw new NotOLE2FileException("The supplied data appears to be a raw XML file. Formats such as Office 2003 XML are not supported");
			}
			if (cmp(MAGIC_BIFF2, data))
			{
				throw new OldExcelFormatException("The supplied data appears to be in BIFF2 format. HSSF only supports the BIFF8 format, try OldExcelExtractor");
			}
			if (cmp(MAGIC_BIFF3, data))
			{
				throw new OldExcelFormatException("The supplied data appears to be in BIFF3 format. HSSF only supports the BIFF8 format, try OldExcelExtractor");
			}
			if (cmp(MAGIC_BIFF4a, data) || cmp(MAGIC_BIFF4b, data))
			{
				throw new OldExcelFormatException("The supplied data appears to be in BIFF4 format. HSSF only supports the BIFF8 format, try OldExcelExtractor");
			}
			throw new NotOLE2FileException("Invalid header signature; read " + new string(HexDump.LongToHex(num)) + ", expected " + new string(HexDump.LongToHex(-2226271756974174256L)) + " - Your file appears not to be a valid OLE2 document");
		}
		if (_data[30] == 12)
		{
			bigBlockSize = POIFSConstants.LARGER_BIG_BLOCK_SIZE_DETAILS;
		}
		else
		{
			if (_data[30] != 9)
			{
				throw new IOException("Unsupported blocksize  (2^" + _data[30] + "). Expected 2^9 or 2^12.");
			}
			bigBlockSize = POIFSConstants.SMALLER_BIG_BLOCK_SIZE_DETAILS;
		}
		_bat_count = new IntegerField(44, _data).Value;
		_property_start = new IntegerField(48, _data).Value;
		_sbat_start = new IntegerField(60, _data).Value;
		_sbat_count = new IntegerField(64, _data).Value;
		_xbat_start = new IntegerField(68, _data).Value;
		_xbat_count = new IntegerField(72, _data).Value;
	}

	public HeaderBlock(POIFSBigBlockSize bigBlockSize)
	{
		this.bigBlockSize = bigBlockSize;
		_data = new byte[512];
		for (int i = 0; i < _data.Length; i++)
		{
			_data[i] = _default_value;
		}
		new LongField(0, -2226271756974174256L, _data);
		new IntegerField(8, 0, _data);
		new IntegerField(12, 0, _data);
		new IntegerField(16, 0, _data);
		new IntegerField(20, 0, _data);
		new ShortField(24, 59, ref _data);
		new ShortField(26, 3, ref _data);
		new ShortField(28, -2, ref _data);
		new ShortField(30, bigBlockSize.GetHeaderValue(), ref _data);
		new IntegerField(32, 6, _data);
		new IntegerField(36, 0, _data);
		new IntegerField(40, 0, _data);
		new IntegerField(52, 0, _data);
		new IntegerField(56, 4096, _data);
		_bat_count = 0;
		_sbat_count = 0;
		_xbat_count = 0;
		_property_start = -2;
		_sbat_start = -2;
		_xbat_start = -2;
	}

	private static byte[] ReadFirst512(Stream stream)
	{
		byte[] array = new byte[512];
		int num = IOUtils.ReadFully(stream, array);
		if (num != 512)
		{
			AlertShortRead(num, 512);
		}
		return array;
	}

	private static IOException AlertShortRead(int read, int expectedReadSize)
	{
		if (read < 0)
		{
			read = 0;
		}
		string text = " byte" + ((read == 1) ? "" : "s");
		return new IOException("Unable to Read entire header; " + read + text + " Read; expected " + expectedReadSize + " bytes");
	}

	public void WriteData(Stream stream)
	{
		try
		{
			new IntegerField(44, _bat_count, _data);
			new IntegerField(48, _property_start, _data);
			new IntegerField(60, _sbat_start, _data);
			new IntegerField(64, _sbat_count, _data);
			new IntegerField(68, _xbat_start, _data);
			new IntegerField(72, _xbat_count, _data);
			stream.Write(_data, 0, 512);
			for (int i = 512; i < bigBlockSize.GetBigBlockSize(); i++)
			{
				stream.WriteByte(0);
			}
		}
		catch (IOException ex)
		{
			throw ex;
		}
	}

	private static bool cmp(byte[] magic, byte[] data)
	{
		int num = 0;
		foreach (byte b in magic)
		{
			byte b2 = data[num++];
			if (b2 != b && (b != 112 || (b2 != 16 && b2 != 32 && b2 != 64)))
			{
				return false;
			}
		}
		return true;
	}
}
