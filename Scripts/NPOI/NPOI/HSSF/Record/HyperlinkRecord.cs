using System;
using System.Text;
using NPOI.HSSF.Util;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class HyperlinkRecord : StandardRecord, ICloneable
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(HyperlinkRecord));

	public const int HLINK_URL = 1;

	public const int HLINK_ABS = 2;

	public const int HLINK_LABEL = 20;

	public const int HLINK_PLACE = 8;

	private const int HLINK_TARGET_FRAME = 128;

	private const int HLINK_UNC_PATH = 256;

	public static readonly GUID STD_MONIKER = GUID.Parse("79EAC9D0-BAF9-11CE-8C82-00AA004BA90B");

	public static readonly GUID URL_MONIKER = GUID.Parse("79EAC9E0-BAF9-11CE-8C82-00AA004BA90B");

	public static readonly GUID FILE_MONIKER = GUID.Parse("00000303-0000-0000-C000-000000000046");

	public static readonly byte[] URL_uninterpretedTail = HexRead.ReadFromString("79 58 81 F4  3B 1D 7F 48   AF 2C 82 5D  C4 85 27 63   00 00 00 00  A5 AB 00 00");

	public static readonly byte[] FILE_uninterpretedTail = HexRead.ReadFromString("FF FF AD DE  00 00 00 00   00 00 00 00  00 00 00 00   00 00 00 00  00 00 00 00");

	private static readonly int TAIL_SIZE = FILE_uninterpretedTail.Length;

	public const short sid = 440;

	private CellRangeAddress _range;

	private GUID _guid;

	private short _fileOpts;

	private int _linkOpts;

	private string _label = string.Empty;

	private string _targetFrame = string.Empty;

	private GUID _moniker;

	private string _shortFilename = string.Empty;

	private string _address = string.Empty;

	private string _textMark = string.Empty;

	private byte[] _uninterpretedTail;

	public int FirstColumn
	{
		get
		{
			return _range.FirstColumn;
		}
		set
		{
			_range.FirstColumn = value;
		}
	}

	public int LastColumn
	{
		get
		{
			return _range.LastColumn;
		}
		set
		{
			_range.LastColumn = value;
		}
	}

	public int FirstRow
	{
		get
		{
			return _range.FirstRow;
		}
		set
		{
			_range.FirstRow = value;
		}
	}

	public int LastRow
	{
		get
		{
			return _range.LastRow;
		}
		set
		{
			_range.LastRow = value;
		}
	}

	public GUID Guid => _guid;

	public GUID Moniker => _moniker;

	public string Label
	{
		get
		{
			return CleanString(_label);
		}
		set
		{
			_label = AppendNullTerm(value);
		}
	}

	public string Address
	{
		get
		{
			if ((_linkOpts & 1) != 0 && _moniker != null && FILE_MONIKER.Equals(_moniker))
			{
				return CleanString((_address != null) ? _address : _shortFilename);
			}
			if ((_linkOpts & 8) != 0)
			{
				return CleanString(_textMark);
			}
			return CleanString(_address);
		}
		set
		{
			if ((_linkOpts & 1) != 0 && _moniker != null && FILE_MONIKER.Equals(_moniker))
			{
				_shortFilename = AppendNullTerm(value);
			}
			else if ((_linkOpts & 8) != 0)
			{
				_textMark = AppendNullTerm(value);
			}
			else
			{
				_address = AppendNullTerm(value);
			}
		}
	}

	public string TextMark
	{
		get
		{
			return CleanString(_textMark);
		}
		set
		{
			_textMark = AppendNullTerm(value);
		}
	}

	public int LinkOptions => _linkOpts;

	public string TargetFrame => CleanString(_targetFrame);

	public string ShortFilename
	{
		get
		{
			return CleanString(_shortFilename);
		}
		set
		{
			_shortFilename = AppendNullTerm(value);
		}
	}

	public int LabelOptions => 2;

	public int FileOptions => _fileOpts;

	public override short Sid => 440;

	protected override int DataSize
	{
		get
		{
			int num = 0;
			num += 8;
			num += 16;
			num += 4;
			num += 4;
			if ((_linkOpts & 0x14) != 0)
			{
				num += 4;
				num += _label.Length * 2;
			}
			if ((_linkOpts & 0x80) != 0)
			{
				num += 4;
				num += _targetFrame.Length * 2;
			}
			if ((_linkOpts & 1) != 0 && (_linkOpts & 0x100) != 0)
			{
				num += 4;
				num += _address.Length * 2;
			}
			if ((_linkOpts & 1) != 0 && (_linkOpts & 0x100) == 0)
			{
				num += 16;
				if (_moniker != null && URL_MONIKER.Equals(_moniker))
				{
					num += 4;
					num += _address.Length * 2;
					if (_uninterpretedTail != null)
					{
						num += TAIL_SIZE;
					}
				}
				else if (_moniker != null && FILE_MONIKER.Equals(_moniker))
				{
					num += 2;
					num += 4;
					num += ((_shortFilename != null) ? _shortFilename.Length : 0);
					num += TAIL_SIZE;
					num += 4;
					if (!string.IsNullOrEmpty(_address))
					{
						num += 6;
						num += _address.Length * 2;
					}
				}
			}
			if ((_linkOpts & 8) != 0)
			{
				num += 4;
				num += _textMark.Length * 2;
			}
			return num;
		}
	}

	public bool IsUrlLink
	{
		get
		{
			if ((_linkOpts & 1) > 0)
			{
				return (_linkOpts & 2) > 0;
			}
			return false;
		}
	}

	public bool IsFileLink
	{
		get
		{
			if ((_linkOpts & 1) > 0)
			{
				return (_linkOpts & 2) == 0;
			}
			return false;
		}
	}

	public bool IsDocumentLink => (_linkOpts & 8) > 0;

	public HyperlinkRecord()
	{
	}

	public HyperlinkRecord(RecordInputStream in1)
	{
		_range = new CellRangeAddress(in1);
		_guid = new GUID(in1);
		int num = in1.ReadInt();
		if (num != 2)
		{
			throw new RecordFormatException("Stream Version must be 0x2 but found " + num);
		}
		_linkOpts = in1.ReadInt();
		if ((_linkOpts & 0x14) != 0)
		{
			int requestedLength = in1.ReadInt();
			_label = in1.ReadUnicodeLEString(requestedLength);
		}
		if ((_linkOpts & 0x80) != 0)
		{
			int requestedLength2 = in1.ReadInt();
			_targetFrame = in1.ReadUnicodeLEString(requestedLength2);
		}
		if ((_linkOpts & 1) != 0 && (_linkOpts & 0x100) != 0)
		{
			_moniker = null;
			int requestedLength3 = in1.ReadInt();
			_address = in1.ReadUnicodeLEString(requestedLength3);
		}
		if ((_linkOpts & 1) != 0 && (_linkOpts & 0x100) == 0)
		{
			_moniker = new GUID(in1);
			if (URL_MONIKER.Equals(_moniker))
			{
				int num2 = in1.ReadInt();
				int remaining = in1.Remaining;
				if (num2 == remaining)
				{
					int requestedLength4 = num2 / 2;
					_address = in1.ReadUnicodeLEString(requestedLength4);
				}
				else
				{
					int requestedLength5 = (num2 - TAIL_SIZE) / 2;
					_address = in1.ReadUnicodeLEString(requestedLength5);
					_uninterpretedTail = ReadTail(URL_uninterpretedTail, in1);
				}
			}
			else if (FILE_MONIKER.Equals(_moniker))
			{
				_fileOpts = in1.ReadShort();
				int nChars = in1.ReadInt();
				_shortFilename = StringUtil.ReadCompressedUnicode(in1, nChars);
				_uninterpretedTail = ReadTail(FILE_uninterpretedTail, in1);
				if (in1.ReadInt() > 0)
				{
					int num3 = in1.ReadInt();
					in1.ReadUShort();
					_address = StringUtil.ReadUnicodeLE(in1, num3 / 2);
				}
				else
				{
					_address = null;
				}
			}
			else if (STD_MONIKER.Equals(_moniker))
			{
				_fileOpts = in1.ReadShort();
				byte[] array = new byte[in1.ReadInt()];
				in1.ReadFully(array);
				_address = Encoding.UTF8.GetString(array);
			}
		}
		if ((_linkOpts & 8) != 0)
		{
			int requestedLength6 = in1.ReadInt();
			_textMark = in1.ReadUnicodeLEString(requestedLength6);
		}
		if (in1.Remaining > 0)
		{
			Console.WriteLine(HexDump.ToHex(in1.ReadRemainder()));
		}
	}

	private static byte[] ReadTail(byte[] expectedTail, ILittleEndianInput in1)
	{
		byte[] array = new byte[TAIL_SIZE];
		in1.ReadFully(array);
		return array;
	}

	private static void WriteTail(byte[] tail, ILittleEndianOutput out1)
	{
		out1.Write(tail);
	}

	private static string CleanString(string s)
	{
		if (s == null)
		{
			return null;
		}
		int num = s.IndexOf('\0');
		if (num < 0)
		{
			return s;
		}
		return s.Substring(0, num);
	}

	private static string AppendNullTerm(string s)
	{
		if (s == null)
		{
			return null;
		}
		return s + "\0";
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		_range.Serialize(out1);
		_guid.Serialize(out1);
		out1.WriteInt(2);
		out1.WriteInt(_linkOpts);
		if ((_linkOpts & 0x14) != 0)
		{
			out1.WriteInt(_label.Length);
			StringUtil.PutUnicodeLE(_label, out1);
		}
		if ((_linkOpts & 0x80) != 0)
		{
			out1.WriteInt(_targetFrame.Length);
			StringUtil.PutUnicodeLE(_targetFrame, out1);
		}
		if ((_linkOpts & 1) != 0 && (_linkOpts & 0x100) != 0)
		{
			out1.WriteInt(_address.Length);
			StringUtil.PutUnicodeLE(_address, out1);
		}
		if ((_linkOpts & 1) != 0 && (_linkOpts & 0x100) == 0)
		{
			_moniker.Serialize(out1);
			if (_moniker != null && URL_MONIKER.Equals(_moniker))
			{
				if (_uninterpretedTail == null)
				{
					out1.WriteInt(_address.Length * 2);
					StringUtil.PutUnicodeLE(_address, out1);
				}
				else
				{
					out1.WriteInt(_address.Length * 2 + TAIL_SIZE);
					StringUtil.PutUnicodeLE(_address, out1);
					WriteTail(_uninterpretedTail, out1);
				}
			}
			else if (_moniker != null && FILE_MONIKER.Equals(_moniker))
			{
				out1.WriteShort(_fileOpts);
				out1.WriteInt(_shortFilename.Length);
				StringUtil.PutCompressedUnicode(_shortFilename, out1);
				WriteTail(_uninterpretedTail, out1);
				if (string.IsNullOrEmpty(_address))
				{
					out1.WriteInt(0);
				}
				else
				{
					int num = _address.Length * 2;
					out1.WriteInt(num + 6);
					out1.WriteInt(num);
					out1.WriteShort(3);
					StringUtil.PutUnicodeLE(_address, out1);
				}
			}
		}
		if ((_linkOpts & 8) != 0)
		{
			out1.WriteInt(_textMark.Length);
			StringUtil.PutUnicodeLE(_textMark, out1);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[HYPERLINK RECORD]\n");
		stringBuilder.Append("    .range            = ").Append(_range.FormatAsString()).Append("\n");
		stringBuilder.Append("    .guid        = ").Append(_guid.FormatAsString()).Append("\n");
		stringBuilder.Append("    .linkOpts          = ").Append(HexDump.IntToHex(_linkOpts)).Append("\n");
		stringBuilder.Append("    .label          = ").Append(Label).Append("\n");
		if ((_linkOpts & 0x80) != 0)
		{
			stringBuilder.Append("    .targetFrame= ").Append(TargetFrame).Append("\n");
		}
		if ((_linkOpts & 1) != 0 && _moniker != null)
		{
			stringBuilder.Append("    .moniker          = ").Append(_moniker.FormatAsString()).Append("\n");
		}
		if ((_linkOpts & 8) != 0)
		{
			stringBuilder.Append("    .targetFrame= ").Append(TextMark).Append("\n");
		}
		stringBuilder.Append("    .address            = ").Append(Address).Append("\n");
		stringBuilder.Append("[/HYPERLINK RECORD]\n");
		return stringBuilder.ToString();
	}

	public void CreateUrlLink()
	{
		_range = new CellRangeAddress(0, 0, 0, 0);
		_guid = STD_MONIKER;
		_linkOpts = 23;
		Label = "";
		_moniker = URL_MONIKER;
		Address = "";
		_uninterpretedTail = URL_uninterpretedTail;
	}

	public void CreateFileLink()
	{
		_range = new CellRangeAddress(0, 0, 0, 0);
		_guid = STD_MONIKER;
		_linkOpts = 21;
		_fileOpts = 0;
		Label = "";
		_moniker = FILE_MONIKER;
		Address = null;
		ShortFilename = "";
		_uninterpretedTail = FILE_uninterpretedTail;
	}

	public void CreateDocumentLink()
	{
		_range = new CellRangeAddress(0, 0, 0, 0);
		_guid = STD_MONIKER;
		_linkOpts = 28;
		Label = "";
		_moniker = FILE_MONIKER;
		Address = "";
		TextMark = "";
	}

	public override object Clone()
	{
		return new HyperlinkRecord
		{
			_range = _range.Copy(),
			_guid = _guid,
			_linkOpts = _linkOpts,
			_fileOpts = _fileOpts,
			_label = _label,
			_address = _address,
			_moniker = _moniker,
			_shortFilename = _shortFilename,
			_targetFrame = _targetFrame,
			_textMark = _textMark,
			_uninterpretedTail = _uninterpretedTail
		};
	}
}
