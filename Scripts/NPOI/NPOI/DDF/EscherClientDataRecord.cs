using System;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherClientDataRecord : EscherRecord
{
	public const short RECORD_ID = -4079;

	public const string RECORD_DESCRIPTION = "MsofbtClientData";

	private byte[] remainingData;

	public override int RecordSize => 8 + ((remainingData != null) ? remainingData.Length : 0);

	public override short RecordId => -4079;

	public override string RecordName => "ClientData";

	public byte[] RemainingData
	{
		get
		{
			return remainingData;
		}
		set
		{
			remainingData = ((value == null) ? new byte[0] : ((byte[])value.Clone()));
		}
	}

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		int num = ReadHeader(data, offset);
		int sourceIndex = offset + 8;
		remainingData = new byte[num];
		Array.Copy(data, sourceIndex, remainingData, 0, num);
		return 8 + num;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		if (remainingData == null)
		{
			remainingData = new byte[0];
		}
		LittleEndian.PutShort(data, offset, Options);
		LittleEndian.PutShort(data, offset + 2, RecordId);
		LittleEndian.PutInt(data, offset + 4, remainingData.Length);
		Array.Copy(remainingData, 0, data, offset + 8, remainingData.Length);
		int num = offset + 8 + remainingData.Length;
		listener.AfterRecordSerialize(num, RecordId, num - offset, this);
		return num - offset;
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		string text = HexDump.Dump(remainingData, 0L, 0);
		return GetType().Name + ":" + newLine + "  RecordId: 0x" + HexDump.ToHex((short)(-4079)) + newLine + "  Version: 0x" + HexDump.ToHex(Version) + newLine + "  Instance: 0x" + HexDump.ToHex(Instance) + newLine + "  Extra Data:" + newLine + text;
	}

	public override string ToXml(string tab)
	{
		string value = HexDump.Dump(remainingData, 0L, 0).Trim();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(GetType().Name, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance))).Append(tab)
			.Append("\t")
			.Append("<ExtraData>")
			.Append(value)
			.Append("</ExtraData>\n");
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}
}
