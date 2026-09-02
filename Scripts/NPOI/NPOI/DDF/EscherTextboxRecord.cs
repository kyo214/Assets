using System;
using System.Text;
using NPOI.Util;

namespace NPOI.DDF;

public class EscherTextboxRecord : EscherRecord, ICloneable
{
	public const short RECORD_ID = -4083;

	public const string RECORD_DESCRIPTION = "msofbtClientTextbox";

	private static readonly byte[] NO_BYTES = new byte[0];

	private byte[] _thedata = NO_BYTES;

	public byte[] Data => _thedata;

	public override int RecordSize => 8 + _thedata.Length;

	public override string RecordName => "ClientTextbox";

	public override int FillFields(byte[] data, int offset, IEscherRecordFactory recordFactory)
	{
		int num = ReadHeader(data, offset);
		_thedata = new byte[num];
		Array.Copy(data, offset + 8, _thedata, 0, num);
		return num + 8;
	}

	public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
	{
		listener.BeforeRecordSerialize(offset, RecordId, this);
		LittleEndian.PutShort(data, offset, Options);
		LittleEndian.PutShort(data, offset + 2, RecordId);
		int value = _thedata.Length;
		LittleEndian.PutInt(data, offset + 4, value);
		Array.Copy(_thedata, 0, data, offset + 8, _thedata.Length);
		int num = offset + 8 + _thedata.Length;
		listener.AfterRecordSerialize(num, RecordId, num - offset, this);
		int num2 = num - offset;
		if (num2 != RecordSize)
		{
			throw new RecordFormatException(num2 + " bytes written but RecordSize reports " + RecordSize);
		}
		return num2;
	}

	public void SetData(byte[] b, int start, int length)
	{
		_thedata = new byte[length];
		Array.Copy(b, start, _thedata, 0, length);
	}

	public void SetData(byte[] b)
	{
		SetData(b, 0, b.Length);
	}

	public override object Clone()
	{
		return new EscherTextboxRecord
		{
			Options = Options,
			RecordId = RecordId,
			_thedata = (byte[])_thedata.Clone()
		};
	}

	public override string ToString()
	{
		string newLine = Environment.NewLine;
		string text = "";
		try
		{
			if (_thedata.Length != 0)
			{
				text = "  Extra Data:" + newLine;
				text += HexDump.Dump(_thedata, 0L, 0);
			}
		}
		catch (Exception)
		{
			text = "Error!!";
		}
		return GetType().Name + ":" + newLine + "  isContainer: " + base.IsContainerRecord + newLine + "  options: 0x" + HexDump.ToHex(Options) + newLine + "  recordId: 0x" + HexDump.ToHex(RecordId) + newLine + "  numchildren: " + ChildRecords.Count + newLine + text;
	}

	public override string ToXml(string tab)
	{
		string text = "";
		try
		{
			if (_thedata.Length != 0)
			{
				text += HexDump.Dump(_thedata, 0L, 0);
			}
		}
		catch (Exception)
		{
			text = "Error!!";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(tab).Append(FormatXmlRecordHeader(GetType().Name, HexDump.ToHex(RecordId), HexDump.ToHex(Version), HexDump.ToHex(Instance))).Append(tab)
			.Append("\t")
			.Append("<ExtraData>")
			.Append(text)
			.Append("</ExtraData>\n");
		stringBuilder.Append(tab).Append("</").Append(GetType().Name)
			.Append(">\n");
		return stringBuilder.ToString();
	}
}
