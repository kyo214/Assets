using System;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherChildAnchorRecord : EscherRecord
{
	public const short RECORD_ID = -4081;

	public const string RECORD_DESCRIPTION = "MsofbtChildAnchor";

	private int field_1_dx1;

	private int field_2_dy1;

	private int field_3_dx2;

	private int field_4_dy2;

	public override int RecordSize => 24;

	public override short RecordId => -4081;

	public override string RecordName => "ChildAnchor";

	public int Dx1
	{
		get
		{
			return field_1_dx1;
		}
		set
		{
			field_1_dx1 = value;
		}
	}

	public int Dy1
	{
		get
		{
			return field_2_dy1;
		}
		set
		{
			field_2_dy1 = value;
		}
	}

	public int Dx2
	{
		get
		{
			return field_3_dx2;
		}
		set
		{
			field_3_dx2 = value;
		}
	}

	public int Dy2
	{
		get
		{
			return field_4_dy2;
		}
		set
		{
			field_4_dy2 = value;
		}
	}

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		ReadHeader(data, offset);
		int num = offset + 8;
		int num2 = 0;
		field_1_dx1 = LittleEndian.GetInt(data, num + num2);
		num2 += 4;
		field_2_dy1 = LittleEndian.GetInt(data, num + num2);
		num2 += 4;
		field_3_dx2 = LittleEndian.GetInt(data, num + num2);
		num2 += 4;
		field_4_dy2 = LittleEndian.GetInt(data, num + num2);
		num2 += 4;
		return 8 + num2;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		int num = offset;
		LittleEndian.PutShort(data, num, Options);
		num += 2;
		LittleEndian.PutShort(data, num, RecordId);
		num += 2;
		LittleEndian.PutInt(data, num, RecordSize - 8);
		num += 4;
		LittleEndian.PutInt(data, num, field_1_dx1);
		num += 4;
		LittleEndian.PutInt(data, num, field_2_dy1);
		num += 4;
		LittleEndian.PutInt(data, num, field_3_dx2);
		num += 4;
		LittleEndian.PutInt(data, num, field_4_dy2);
		num += 4;
		listener.AfterRecordSerialize(num, RecordId, num - offset, this);
		return num - offset;
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		return GetType().Name + ":" + newLine + "  RecordId: 0x" + HexDump.ToHex((short)(-4081)) + newLine + "  Version: 0x" + HexDump.ToHex(Version) + newLine + "  Instance: 0x" + HexDump.ToHex(Instance) + newLine + "  X1: " + field_1_dx1 + newLine + "  Y1: " + field_2_dy1 + newLine + "  X2: " + field_3_dx2 + newLine + "  Y2: " + field_4_dy2 + newLine;
	}

	public override string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(GetType().Name, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance))).Append(tab)
			.Append("\t")
			.Append("<X1>")
			.Append(field_1_dx1)
			.Append("</X1>\n")
			.Append(tab)
			.Append("\t")
			.Append("<Y1>")
			.Append(field_2_dy1)
			.Append("</Y1>\n")
			.Append(tab)
			.Append("\t")
			.Append("<X2>")
			.Append(field_3_dx2)
			.Append("</X2>\n")
			.Append(tab)
			.Append("\t")
			.Append("<Y2>")
			.Append(field_4_dy2)
			.Append("</Y2>\n");
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}
}
