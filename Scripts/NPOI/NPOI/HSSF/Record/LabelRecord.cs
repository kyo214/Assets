using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class LabelRecord : Record, CellValueRecordInterface, ICloneable
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(LabelRecord));

	public const short sid = 516;

	private int field_1_row;

	private int field_2_column;

	private short field_3_xf_index;

	private short field_4_string_len;

	private byte field_5_unicode_flag;

	private string field_6_value;

	public int Row
	{
		get
		{
			return field_1_row;
		}
		set
		{
			throw new NotSupportedException("Use LabelSST instead");
		}
	}

	public int Column
	{
		get
		{
			return field_2_column;
		}
		set
		{
			throw new NotSupportedException("Use LabelSST instead");
		}
	}

	public short XFIndex
	{
		get
		{
			return field_3_xf_index;
		}
		set
		{
			throw new NotSupportedException("Use LabelSST instead");
		}
	}

	public short StringLength => field_4_string_len;

	public bool IsUncompressedUnicode => (field_5_unicode_flag & 1) != 0;

	public string Value => field_6_value;

	public override int RecordSize
	{
		get
		{
			throw new RecordFormatException("Label Records are supported READ ONLY...convert to LabelSST");
		}
	}

	public override short Sid => 516;

	public LabelRecord()
	{
	}

	public LabelRecord(RecordInputStream in1)
	{
		field_1_row = in1.ReadUShort();
		field_2_column = in1.ReadUShort();
		field_3_xf_index = in1.ReadShort();
		field_4_string_len = in1.ReadShort();
		field_5_unicode_flag = (byte)in1.ReadByte();
		if (field_4_string_len > 0)
		{
			if (IsUncompressedUnicode)
			{
				field_6_value = in1.ReadUnicodeLEString(field_4_string_len);
			}
			else
			{
				field_6_value = in1.ReadCompressedUnicode(field_4_string_len);
			}
		}
		else
		{
			field_6_value = "";
		}
		if (in1.Remaining > 0)
		{
			logger.Log(3, "LabelRecord data remains: " + in1.Remaining + " : " + HexDump.ToHex(in1.ReadRemainder()));
		}
	}

	public override int Serialize(int offset, byte[] data)
	{
		throw new RecordFormatException("Label Records are supported Read ONLY...Convert to LabelSST");
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[LABEL]\n");
		stringBuilder.Append("    .row            = ").Append(StringUtil.ToHexString(Row)).Append("\n");
		stringBuilder.Append("    .column         = ").Append(StringUtil.ToHexString(Column)).Append("\n");
		stringBuilder.Append("    .xfindex        = ").Append(StringUtil.ToHexString(XFIndex)).Append("\n");
		stringBuilder.Append("    .string_len       = ").Append(StringUtil.ToHexString(field_4_string_len)).Append("\n");
		stringBuilder.Append("    .unicode_flag       = ").Append(StringUtil.ToHexString(field_5_unicode_flag)).Append("\n");
		stringBuilder.Append("    .value       = ").Append(Value).Append("\n");
		stringBuilder.Append("[/LABEL]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return new LabelRecord
		{
			field_1_row = field_1_row,
			field_2_column = field_2_column,
			field_3_xf_index = field_3_xf_index,
			field_4_string_len = field_4_string_len,
			field_5_unicode_flag = field_5_unicode_flag,
			field_6_value = field_6_value
		};
	}
}
