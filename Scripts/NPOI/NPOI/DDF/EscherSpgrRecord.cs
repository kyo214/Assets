using System;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherSpgrRecord : EscherRecord
{
	public const short RECORD_ID = -4087;

	public const string RECORD_DESCRIPTION = "MsofbtSpgr";

	private int field_1_rectX1;

	private int field_2_rectY1;

	private int field_3_rectX2;

	private int field_4_rectY2;

	public override int RecordSize => 24;

	public override short RecordId => -4087;

	public override string RecordName => "Spgr";

	public int RectX1
	{
		get
		{
			return field_1_rectX1;
		}
		set
		{
			field_1_rectX1 = value;
		}
	}

	public int RectX2
	{
		get
		{
			return field_3_rectX2;
		}
		set
		{
			field_3_rectX2 = value;
		}
	}

	public int RectY1
	{
		get
		{
			return field_2_rectY1;
		}
		set
		{
			field_2_rectY1 = value;
		}
	}

	public int RectY2
	{
		get
		{
			return field_4_rectY2;
		}
		set
		{
			field_4_rectY2 = value;
		}
	}

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		int num = ReadHeader(data, offset);
		int num2 = offset + 8;
		int num3 = 0;
		field_1_rectX1 = LittleEndian.GetInt(data, num2 + num3);
		num3 += 4;
		field_2_rectY1 = LittleEndian.GetInt(data, num2 + num3);
		num3 += 4;
		field_3_rectX2 = LittleEndian.GetInt(data, num2 + num3);
		num3 += 4;
		field_4_rectY2 = LittleEndian.GetInt(data, num2 + num3);
		num3 += 4;
		num -= num3;
		if (num != 0)
		{
			throw new RecordFormatException("Expected no remaining bytes but got " + num);
		}
		return 8 + num3 + num;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		LittleEndian.PutShort(data, offset, Options);
		LittleEndian.PutShort(data, offset + 2, RecordId);
		int value = 16;
		LittleEndian.PutInt(data, offset + 4, value);
		LittleEndian.PutInt(data, offset + 8, field_1_rectX1);
		LittleEndian.PutInt(data, offset + 12, field_2_rectY1);
		LittleEndian.PutInt(data, offset + 16, field_3_rectX2);
		LittleEndian.PutInt(data, offset + 20, field_4_rectY2);
		listener.AfterRecordSerialize(offset + RecordSize, RecordId, offset + RecordSize, this);
		return 24;
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		return GetType().Name + ":" + newLine + "  RecordId: 0x" + HexDump.ToHex((short)(-4087)) + newLine + "  Version: 0x" + HexDump.ToHex(Version) + newLine + "  Instance: 0x" + HexDump.ToHex(Instance) + newLine + "  RectX: " + field_1_rectX1 + newLine + "  RectY: " + field_2_rectY1 + newLine + "  RectWidth: " + field_3_rectX2 + newLine + "  RectHeight: " + field_4_rectY2 + newLine;
	}

	public override string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(GetType().Name, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance))).Append(tab)
			.Append("\t")
			.Append("<RectX>")
			.Append(field_1_rectX1)
			.Append("</RectX>\n")
			.Append(tab)
			.Append("\t")
			.Append("<RectY>")
			.Append(field_2_rectY1)
			.Append("</RectY>\n")
			.Append(tab)
			.Append("\t")
			.Append("<RectWidth>")
			.Append(field_3_rectX2)
			.Append("</RectWidth>\n")
			.Append(tab)
			.Append("\t")
			.Append("<RectHeight>")
			.Append(field_4_rectY2)
			.Append("</RectHeight>\n");
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}
}
