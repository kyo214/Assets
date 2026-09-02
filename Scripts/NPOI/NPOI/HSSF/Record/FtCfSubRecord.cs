using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class FtCfSubRecord : SubRecord, ICloneable
{
	public const short sid = 7;

	public const short length = 2;

	public static short METAFILE_BIT = 2;

	public static short BITMAP_BIT = 9;

	public static short UNSPECIFIED_BIT = -1;

	private short flags;

	public override int DataSize => 2;

	public override short Sid => 7;

	public short Flags
	{
		get
		{
			return flags;
		}
		set
		{
			flags = value;
		}
	}

	public FtCfSubRecord()
	{
	}

	public FtCfSubRecord(ILittleEndianInput in1, int size)
	{
		if (size != 2)
		{
			throw new RecordFormatException("Unexpected size (" + size + ")");
		}
		flags = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FtCf ]\n");
		stringBuilder.Append("  size     = ").Append((short)2).Append("\n");
		stringBuilder.Append("  flags    = ").Append(HexDump.ToHex(flags)).Append("\n");
		stringBuilder.Append("[/FtCf ]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(7);
		out1.WriteShort(2);
		out1.WriteShort(flags);
	}

	public override object Clone()
	{
		return new FtCfSubRecord
		{
			flags = flags
		};
	}
}
