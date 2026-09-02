using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class FtPioGrbitSubRecord : SubRecord, ICloneable
{
	public const short sid = 8;

	public const short length = 2;

	public static int AUTO_PICT_BIT = 1;

	public static int DDE_BIT = 2;

	public static int PRINT_CALC_BIT = 4;

	public static int ICON_BIT = 8;

	public static int CTL_BIT = 16;

	public static int PRSTM_BIT = 32;

	public static int CAMERA_BIT = 128;

	public static int DEFAULT_SIZE_BIT = 256;

	public static int AUTO_LOAD_BIT = 512;

	private short flags;

	public override int DataSize => 2;

	public override short Sid => 8;

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

	public FtPioGrbitSubRecord()
	{
	}

	public FtPioGrbitSubRecord(ILittleEndianInput in1, int size)
	{
		if (size != 2)
		{
			throw new RecordFormatException("Unexpected size (" + size + ")");
		}
		flags = in1.ReadShort();
	}

	public void SetFlagByBit(int bitmask, bool enabled)
	{
		if (enabled)
		{
			flags |= (short)bitmask;
		}
		else
		{
			flags &= (short)(0xFFFF ^ bitmask);
		}
	}

	public bool GetFlagByBit(int bitmask)
	{
		return (flags & bitmask) != 0;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FtPioGrbit ]\n");
		stringBuilder.Append("  size     = ").Append((short)2).Append("\n");
		stringBuilder.Append("  flags    = ").Append(HexDump.ToHex(flags)).Append("\n");
		stringBuilder.Append("[/FtPioGrbit ]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(8);
		out1.WriteShort(2);
		out1.WriteShort(flags);
	}

	public override object Clone()
	{
		return new FtPioGrbitSubRecord
		{
			flags = flags
		};
	}
}
