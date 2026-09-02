using System;
using System.IO;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherBlipRecord : EscherRecord
{
	public const short RECORD_ID_START = -4072;

	public const short RECORD_ID_END = -3817;

	public const string RECORD_DESCRIPTION = "msofbtBlip";

	private const int HEADER_SIZE = 8;

	protected byte[] field_pictureData;

	public override int RecordSize => field_pictureData.Length + 8;

	public override string RecordName => "Blip";

	public byte[] PictureData
	{
		get
		{
			return field_pictureData;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("picture data can't be null");
			}
			field_pictureData = new byte[value.Length];
			if (value.Length != 0)
			{
				Array.Copy(value, field_pictureData, value.Length);
			}
		}
	}

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		int num = ReadHeader(data, offset);
		int sourceIndex = offset + 8;
		field_pictureData = new byte[num];
		Array.Copy(data, sourceIndex, field_pictureData, 0, num);
		return num + 8;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		LittleEndian.PutShort(data, offset, Options);
		LittleEndian.PutShort(data, offset + 2, RecordId);
		Array.Copy(field_pictureData, 0, data, offset + 4, field_pictureData.Length);
		listener.AfterRecordSerialize(offset + 4 + field_pictureData.Length, RecordId, field_pictureData.Length + 4, this);
		return field_pictureData.Length + 4;
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		string empty = string.Empty;
		using MemoryStream memoryStream = new MemoryStream();
		try
		{
			HexDump.Dump(field_pictureData, 0L, memoryStream, 0);
			empty = HexDump.ToHex(memoryStream.ToArray());
		}
		catch (Exception ex)
		{
			empty = ex.ToString();
		}
		return GetType().Name + ":" + newLine + "  RecordId: 0x" + HexDump.ToHex(RecordId) + newLine + "  Options: 0x" + HexDump.ToHex(Options) + newLine + "  Version: 0x" + HexDump.ToHex(Version) + newLine + "  Instance: 0x" + HexDump.ToHex(Instance) + newLine + "  Extra Data:" + newLine + empty;
	}

	public override string ToXml(string tab)
	{
		string value = HexDump.ToHex(field_pictureData, 32);
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
