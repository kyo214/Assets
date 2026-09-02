using System;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherDgRecord : EscherRecord
{
	public const short RECORD_ID = -4088;

	public const string RECORD_DESCRIPTION = "MsofbtDg";

	private int field_1_numShapes;

	private int field_2_lastMSOSPID;

	public override int RecordSize => 16;

	public override short RecordId => -4088;

	public override string RecordName => "Dg";

	public int NumShapes
	{
		get
		{
			return field_1_numShapes;
		}
		set
		{
			field_1_numShapes = value;
		}
	}

	public int LastMSOSPID
	{
		get
		{
			return field_2_lastMSOSPID;
		}
		set
		{
			field_2_lastMSOSPID = value;
		}
	}

	public short DrawingGroupId => (short)(Options >> 4);

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		ReadHeader(data, offset);
		int num = offset + 8;
		int num2 = 0;
		field_1_numShapes = LittleEndian.GetInt(data, num + num2);
		num2 += 4;
		field_2_lastMSOSPID = LittleEndian.GetInt(data, num + num2);
		num2 += 4;
		return RecordSize;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		LittleEndian.PutShort(data, offset, Options);
		LittleEndian.PutShort(data, offset + 2, RecordId);
		LittleEndian.PutInt(data, offset + 4, 8);
		LittleEndian.PutInt(data, offset + 8, field_1_numShapes);
		LittleEndian.PutInt(data, offset + 12, field_2_lastMSOSPID);
		listener.AfterRecordSerialize(offset + 16, RecordId, RecordSize, this);
		return RecordSize;
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		return GetType().Name + ":" + newLine + "  RecordId: 0x" + HexDump.ToHex((short)(-4088)) + newLine + "  Version: 0x" + HexDump.ToHex(Version) + newLine + "  Instance: 0x" + HexDump.ToHex(Instance) + newLine + "  NumShapes: " + field_1_numShapes + newLine + "  LastMSOSPID: " + field_2_lastMSOSPID + newLine;
	}

	public override string ToXml(string tab)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(GetType().Name, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance))).Append(tab)
			.Append("\t")
			.Append("<NumShapes>")
			.Append(field_1_numShapes)
			.Append("</NumShapes>\n")
			.Append(tab)
			.Append("\t")
			.Append("<LastMSOSPID>")
			.Append(field_2_lastMSOSPID)
			.Append("</LastMSOSPID>\n");
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}

	public void IncrementShapeCount()
	{
		field_1_numShapes++;
	}
}
