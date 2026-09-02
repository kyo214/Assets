using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class RowRecord : StandardRecord, IComparable
{
	public const short sid = 520;

	public const int ENCODED_SIZE = 20;

	private const int OPTION_BITS_ALWAYS_SET = 256;

	[Obsolete]
	public const int MAX_ROW_NUMBER = 65535;

	private int field_1_row_number;

	private int field_2_first_col;

	private int field_3_last_col;

	private short field_4_height;

	private short field_5_optimize;

	private short field_6_reserved;

	private int field_7_option_flags;

	private static BitField outlineLevel = BitFieldFactory.GetInstance(7);

	private static BitField colapsed = BitFieldFactory.GetInstance(16);

	private static BitField zeroHeight = BitFieldFactory.GetInstance(32);

	private static BitField badFontHeight = BitFieldFactory.GetInstance(64);

	private static BitField formatted = BitFieldFactory.GetInstance(128);

	private int field_8_option_flags;

	private static BitField xfIndex = BitFieldFactory.GetInstance(4095);

	private static BitField topBorder = BitFieldFactory.GetInstance(4096);

	private static BitField bottomBorder = BitFieldFactory.GetInstance(8192);

	private static BitField phoeneticGuide = BitFieldFactory.GetInstance(16384);

	public bool IsEmpty => (field_2_first_col | field_3_last_col) == 0;

	public int RowNumber
	{
		get
		{
			return field_1_row_number;
		}
		set
		{
			field_1_row_number = value;
		}
	}

	public int FirstCol
	{
		get
		{
			return field_2_first_col;
		}
		set
		{
			field_2_first_col = value;
		}
	}

	public int LastCol
	{
		get
		{
			return field_3_last_col;
		}
		set
		{
			field_3_last_col = value;
		}
	}

	public short Height
	{
		get
		{
			return field_4_height;
		}
		set
		{
			field_4_height = value;
		}
	}

	public short Optimize
	{
		get
		{
			return field_5_optimize;
		}
		set
		{
			field_5_optimize = value;
		}
	}

	public short OptionFlags
	{
		get
		{
			return (short)field_7_option_flags;
		}
		set
		{
			field_7_option_flags = value | 0x100;
		}
	}

	public short OutlineLevel
	{
		get
		{
			return (short)outlineLevel.GetValue(field_7_option_flags);
		}
		set
		{
			field_7_option_flags = outlineLevel.SetValue(field_7_option_flags, value);
		}
	}

	public bool Colapsed
	{
		get
		{
			return colapsed.IsSet(field_7_option_flags);
		}
		set
		{
			field_7_option_flags = colapsed.SetBoolean(field_7_option_flags, value);
		}
	}

	public bool ZeroHeight
	{
		get
		{
			return zeroHeight.IsSet(field_7_option_flags);
		}
		set
		{
			field_7_option_flags = zeroHeight.SetBoolean(field_7_option_flags, value);
		}
	}

	public bool BadFontHeight
	{
		get
		{
			return badFontHeight.IsSet(field_7_option_flags);
		}
		set
		{
			field_7_option_flags = badFontHeight.SetBoolean(field_7_option_flags, value);
		}
	}

	public bool Formatted
	{
		get
		{
			return formatted.IsSet(field_7_option_flags);
		}
		set
		{
			field_7_option_flags = formatted.SetBoolean(field_7_option_flags, value);
		}
	}

	public short OptionFlags2 => (short)field_8_option_flags;

	public short XFIndex
	{
		get
		{
			return xfIndex.GetShortValue((short)field_8_option_flags);
		}
		set
		{
			field_8_option_flags = xfIndex.SetValue(field_8_option_flags, value);
		}
	}

	public bool TopBorder
	{
		get
		{
			return topBorder.IsSet(field_8_option_flags);
		}
		set
		{
			field_8_option_flags = topBorder.SetBoolean(field_8_option_flags, value);
		}
	}

	public bool BottomBorder
	{
		get
		{
			return bottomBorder.IsSet(field_8_option_flags);
		}
		set
		{
			field_8_option_flags = bottomBorder.SetBoolean(field_8_option_flags, value);
		}
	}

	public bool PhoeneticGuide
	{
		get
		{
			return phoeneticGuide.IsSet(field_8_option_flags);
		}
		set
		{
			field_8_option_flags = phoeneticGuide.SetBoolean(field_8_option_flags, value);
		}
	}

	protected override int DataSize => 16;

	public override int RecordSize => 20;

	public override short Sid => 520;

	public RowRecord(int rowNumber)
	{
		if (rowNumber < 0)
		{
			throw new ArgumentException("Invalid row number (" + rowNumber + ")");
		}
		field_1_row_number = rowNumber;
		field_4_height = 255;
		field_5_optimize = 0;
		field_6_reserved = 0;
		field_7_option_flags = 256;
		field_8_option_flags = 15;
		SetEmpty();
	}

	public RowRecord(RecordInputStream in1)
	{
		field_1_row_number = in1.ReadUShort();
		if (field_1_row_number < 0)
		{
			throw new ArgumentException("Invalid row number " + field_1_row_number + " found in InputStream");
		}
		field_2_first_col = in1.ReadShort();
		field_3_last_col = in1.ReadShort();
		field_4_height = in1.ReadShort();
		field_5_optimize = in1.ReadShort();
		field_6_reserved = in1.ReadShort();
		field_7_option_flags = in1.ReadShort();
		field_8_option_flags = in1.ReadShort();
	}

	public void SetEmpty()
	{
		field_2_first_col = 0;
		field_3_last_col = 0;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[ROW]\n");
		stringBuilder.Append("    .rownumber      = ").Append(StringUtil.ToHexString(RowNumber)).Append("\n");
		stringBuilder.Append("    .firstcol       = ").Append(StringUtil.ToHexString(FirstCol)).Append("\n");
		stringBuilder.Append("    .lastcol        = ").Append(StringUtil.ToHexString(LastCol)).Append("\n");
		stringBuilder.Append("    .height         = ").Append(StringUtil.ToHexString(Height)).Append("\n");
		stringBuilder.Append("    .optimize       = ").Append(StringUtil.ToHexString(Optimize)).Append("\n");
		stringBuilder.Append("    .reserved       = ").Append(StringUtil.ToHexString(field_6_reserved)).Append("\n");
		stringBuilder.Append("    .optionflags    = ").Append(StringUtil.ToHexString(OptionFlags)).Append("\n");
		stringBuilder.Append("        .outlinelvl = ").Append(StringUtil.ToHexString(OutlineLevel)).Append("\n");
		stringBuilder.Append("        .colapsed   = ").Append(Colapsed).Append("\n");
		stringBuilder.Append("        .zeroheight = ").Append(ZeroHeight).Append("\n");
		stringBuilder.Append("        .badfontheig= ").Append(BadFontHeight).Append("\n");
		stringBuilder.Append("        .formatted  = ").Append(Formatted).Append("\n");
		stringBuilder.Append("    .optionsflags2  = ").Append(StringUtil.ToHexString(OptionFlags2)).Append("\n");
		stringBuilder.Append("        .xFindex       = ").Append(StringUtil.ToHexString(XFIndex)).Append("\n");
		stringBuilder.Append("        .topBorder     = ").Append(TopBorder).Append("\n");
		stringBuilder.Append("        .bottomBorder  = ").Append(BottomBorder).Append("\n");
		stringBuilder.Append("        .phoeneticGuide= ").Append(PhoeneticGuide).Append("\n");
		stringBuilder.Append("[/ROW]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(RowNumber);
		out1.WriteShort((FirstCol != -1) ? FirstCol : 0);
		out1.WriteShort((LastCol != -1) ? LastCol : 0);
		out1.WriteShort(Height);
		out1.WriteShort(Optimize);
		out1.WriteShort(field_6_reserved);
		out1.WriteShort(OptionFlags);
		out1.WriteShort(OptionFlags2);
	}

	public int CompareTo(object obj)
	{
		RowRecord rowRecord = (RowRecord)obj;
		if (RowNumber == rowRecord.RowNumber)
		{
			return 0;
		}
		if (RowNumber < rowRecord.RowNumber)
		{
			return -1;
		}
		if (RowNumber > rowRecord.RowNumber)
		{
			return 1;
		}
		return -1;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is RowRecord))
		{
			return false;
		}
		RowRecord rowRecord = (RowRecord)obj;
		if (RowNumber == rowRecord.RowNumber)
		{
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return RowNumber;
	}

	public override object Clone()
	{
		return new RowRecord(field_1_row_number)
		{
			field_2_first_col = field_2_first_col,
			field_3_last_col = field_3_last_col,
			field_4_height = field_4_height,
			field_5_optimize = field_5_optimize,
			field_6_reserved = field_6_reserved,
			field_7_option_flags = field_7_option_flags,
			field_8_option_flags = field_8_option_flags
		};
	}
}
