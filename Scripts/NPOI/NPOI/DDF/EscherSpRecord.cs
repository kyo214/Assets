using System;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherSpRecord : EscherRecord
{
	public const short RECORD_ID = -4086;

	public const string RECORD_DESCRIPTION = "MsofbtSp";

	public const int FLAG_GROUP = 1;

	public const int FLAG_CHILD = 2;

	public const int FLAG_PATRIARCH = 4;

	public const int FLAG_DELETED = 8;

	public const int FLAG_OLESHAPE = 16;

	public const int FLAG_HAVEMASTER = 32;

	public const int FLAG_FLIPHORIZ = 64;

	public const int FLAG_FLIPVERT = 128;

	public const int FLAG_CONNECTOR = 256;

	public const int FLAG_HAVEANCHOR = 512;

	public const int FLAG_BACKGROUND = 1024;

	public const int FLAG_HASSHAPETYPE = 2048;

	private int field_1_shapeId;

	private int field_2_flags;

	public override int RecordSize => 16;

	public override short RecordId => -4086;

	public override string RecordName => "Sp";

	public int ShapeId
	{
		get
		{
			return field_1_shapeId;
		}
		set
		{
			field_1_shapeId = value;
		}
	}

	public int Flags
	{
		get
		{
			return field_2_flags;
		}
		set
		{
			field_2_flags = value;
		}
	}

	public short ShapeType
	{
		get
		{
			return Instance;
		}
		set
		{
			Instance = value;
		}
	}

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		ReadHeader(data, offset);
		int num = offset + 8;
		int num2 = 0;
		field_1_shapeId = LittleEndian.GetInt(data, num + num2);
		num2 += 4;
		field_2_flags = LittleEndian.GetInt(data, num + num2);
		num2 += 4;
		return RecordSize;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		LittleEndian.PutShort(data, offset, Options);
		LittleEndian.PutShort(data, offset + 2, RecordId);
		int value = 8;
		LittleEndian.PutInt(data, offset + 4, value);
		LittleEndian.PutInt(data, offset + 8, field_1_shapeId);
		LittleEndian.PutInt(data, offset + 12, field_2_flags);
		listener.AfterRecordSerialize(offset + RecordSize, RecordId, RecordSize, this);
		return 16;
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		return GetType().Name + ":" + newLine + "  RecordId: 0x" + HexDump.ToHex((short)(-4086)) + newLine + "  Version: 0x" + HexDump.ToHex(Version) + newLine + "  ShapeType: 0x" + HexDump.ToHex(ShapeType) + newLine + "  ShapeId: " + field_1_shapeId + newLine + "  Flags: " + DecodeFlags(field_2_flags) + " (0x" + HexDump.ToHex(field_2_flags) + ")" + newLine;
	}

	public override string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(GetType().Name, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance))).Append(tab)
			.Append("\t")
			.Append("<ShapeType>0x")
			.Append(HexDump.ToHex(ShapeType))
			.Append("</ShapeType>\n")
			.Append(tab)
			.Append("\t")
			.Append("<ShapeId>")
			.Append(field_1_shapeId)
			.Append("</ShapeId>\n")
			.Append(tab)
			.Append("\t")
			.Append("<Flags>")
			.Append(DecodeFlags(field_2_flags) + " (0x" + HexDump.ToHex(field_2_flags) + ")")
			.Append("</Flags>\n");
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}

	private string DecodeFlags(int flags)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(((flags & 1) != 0) ? "|GROUP" : "");
		stringBuilder.Append(((flags & 2) != 0) ? "|CHILD" : "");
		stringBuilder.Append(((flags & 4) != 0) ? "|PATRIARCH" : "");
		stringBuilder.Append(((flags & 8) != 0) ? "|DELETED" : "");
		stringBuilder.Append(((flags & 0x10) != 0) ? "|OLESHAPE" : "");
		stringBuilder.Append(((flags & 0x20) != 0) ? "|HAVEMASTER" : "");
		stringBuilder.Append(((flags & 0x40) != 0) ? "|FLIPHORIZ" : "");
		stringBuilder.Append(((flags & 0x80) != 0) ? "|FLIPVERT" : "");
		stringBuilder.Append(((flags & 0x100) != 0) ? "|CONNECTOR" : "");
		stringBuilder.Append(((flags & 0x200) != 0) ? "|HAVEANCHOR" : "");
		stringBuilder.Append(((flags & 0x400) != 0) ? "|BACKGROUND" : "");
		stringBuilder.Append(((flags & 0x800) != 0) ? "|HASSHAPETYPE" : "");
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(0, 1);
		}
		return stringBuilder.ToString();
	}
}
