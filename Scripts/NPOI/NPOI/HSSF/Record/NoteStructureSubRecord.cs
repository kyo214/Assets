using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class NoteStructureSubRecord : SubRecord, ICloneable
{
	public const short sid = 13;

	private const int ENCODED_SIZE = 22;

	private byte[] reserved;

	public override int DataSize => reserved.Length;

	public override short Sid => 13;

	public NoteStructureSubRecord()
	{
		reserved = new byte[22];
	}

	public NoteStructureSubRecord(ILittleEndianInput in1, int size)
	{
		if (size != 22)
		{
			throw new RecordFormatException("Unexpected size (" + size + ")");
		}
		byte[] buf = new byte[size];
		in1.ReadFully(buf);
		reserved = buf;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		string newLine = Environment.NewLine;
		stringBuilder.Append("[ftNts ]" + newLine);
		stringBuilder.Append("  size     = ").Append(DataSize).Append(newLine);
		stringBuilder.Append("  reserved = ").Append(HexDump.ToHex(reserved)).Append(newLine);
		stringBuilder.Append("[/ftNts ]" + newLine);
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(13);
		out1.WriteShort(reserved.Length);
		out1.Write(reserved);
	}

	public override object Clone()
	{
		NoteStructureSubRecord noteStructureSubRecord = new NoteStructureSubRecord();
		byte[] array = new byte[reserved.Length];
		Array.Copy(reserved, 0, array, 0, array.Length);
		noteStructureSubRecord.reserved = array;
		return noteStructureSubRecord;
	}
}
