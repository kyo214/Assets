using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class FormatRecord : StandardRecord, ICloneable
{
	public const short sid = 1054;

	private int field_1_index_code;

	private bool field_3_hasMultibyte;

	private string field_4_formatstring;

	public int IndexCode => field_1_index_code;

	public string FormatString => field_4_formatstring;

	protected override int DataSize => 5 + FormatString.Length * ((!field_3_hasMultibyte) ? 1 : 2);

	public override short Sid => 1054;

	private FormatRecord(FormatRecord other)
	{
		field_1_index_code = other.field_1_index_code;
		field_3_hasMultibyte = other.field_3_hasMultibyte;
		field_4_formatstring = other.field_4_formatstring;
	}

	public FormatRecord(int indexCode, string fs)
	{
		field_1_index_code = indexCode;
		field_4_formatstring = fs;
		field_3_hasMultibyte = StringUtil.HasMultibyte(fs);
	}

	public FormatRecord(RecordInputStream in1)
	{
		field_1_index_code = in1.ReadShort();
		int requestedLength = in1.ReadShort();
		field_3_hasMultibyte = (in1.ReadByte() & 1) != 0;
		if (field_3_hasMultibyte)
		{
			field_4_formatstring = in1.ReadUnicodeLEString(requestedLength);
		}
		else
		{
			field_4_formatstring = in1.ReadCompressedUnicode(requestedLength);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FORMAT]\n");
		stringBuilder.Append("    .indexcode       = ").Append(HexDump.ShortToHex(IndexCode)).Append("\n");
		stringBuilder.Append("    .isUnicode       = ").Append(field_3_hasMultibyte).Append("\n");
		stringBuilder.Append("    .formatstring    = ").Append(FormatString).Append("\n");
		stringBuilder.Append("[/FORMAT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		string formatString = FormatString;
		out1.WriteShort(IndexCode);
		out1.WriteShort(formatString.Length);
		out1.WriteByte(field_3_hasMultibyte ? 1 : 0);
		if (field_3_hasMultibyte)
		{
			StringUtil.PutUnicodeLE(formatString, out1);
		}
		else
		{
			StringUtil.PutCompressedUnicode(formatString, out1);
		}
	}

	public override object Clone()
	{
		return new FormatRecord(this);
	}
}
