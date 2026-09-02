using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class AxisOptionsRecord : StandardRecord, ICloneable
{
	public static short sid = 4194;

	private static BitField defaultMinimum = BitFieldFactory.GetInstance(1);

	private static BitField defaultMaximum = BitFieldFactory.GetInstance(2);

	private static BitField defaultMajor = BitFieldFactory.GetInstance(4);

	private static BitField defaultMinorUnit = BitFieldFactory.GetInstance(8);

	private static BitField isDate = BitFieldFactory.GetInstance(16);

	private static BitField defaultBase = BitFieldFactory.GetInstance(32);

	private static BitField defaultCross = BitFieldFactory.GetInstance(64);

	private static BitField defaultDateSettings = BitFieldFactory.GetInstance(128);

	private short field_1_minimumCategory;

	private short field_2_maximumCategory;

	private short field_3_majorUnitValue;

	private short field_4_majorUnit;

	private short field_5_minorUnitValue;

	private short field_6_minorUnit;

	private short field_7_baseUnit;

	private short field_8_crossingPoint;

	private short field_9_options;

	protected override int DataSize => 18;

	public override short Sid => sid;

	public short MinimumCategory
	{
		get
		{
			return field_1_minimumCategory;
		}
		set
		{
			field_1_minimumCategory = value;
		}
	}

	public short MaximumCategory
	{
		get
		{
			return field_2_maximumCategory;
		}
		set
		{
			field_2_maximumCategory = value;
		}
	}

	public short MajorUnitValue
	{
		get
		{
			return field_3_majorUnitValue;
		}
		set
		{
			field_3_majorUnitValue = value;
		}
	}

	public short MajorUnit
	{
		get
		{
			return field_4_majorUnit;
		}
		set
		{
			field_4_majorUnit = value;
		}
	}

	public short MinorUnitValue
	{
		get
		{
			return field_5_minorUnitValue;
		}
		set
		{
			field_5_minorUnitValue = value;
		}
	}

	public short MinorUnit
	{
		get
		{
			return field_6_minorUnit;
		}
		set
		{
			field_6_minorUnit = value;
		}
	}

	public short BaseUnit
	{
		get
		{
			return field_7_baseUnit;
		}
		set
		{
			field_7_baseUnit = value;
		}
	}

	public short CrossingPoint
	{
		get
		{
			return field_8_crossingPoint;
		}
		set
		{
			field_8_crossingPoint = value;
		}
	}

	public short Options
	{
		get
		{
			return field_9_options;
		}
		set
		{
			field_9_options = value;
		}
	}

	public bool IsDefaultMinimum
	{
		get
		{
			return defaultMinimum.IsSet(field_9_options);
		}
		set
		{
			field_9_options = defaultMinimum.SetShortBoolean(field_9_options, value);
		}
	}

	public bool IsDefaultMaximum
	{
		get
		{
			return defaultMaximum.IsSet(field_9_options);
		}
		set
		{
			field_9_options = defaultMaximum.SetShortBoolean(field_9_options, value);
		}
	}

	public bool IsDefaultMajor
	{
		get
		{
			return defaultMajor.IsSet(field_9_options);
		}
		set
		{
			field_9_options = defaultMajor.SetShortBoolean(field_9_options, value);
		}
	}

	public bool IsDefaultMinorUnit
	{
		get
		{
			return defaultMinorUnit.IsSet(field_9_options);
		}
		set
		{
			field_9_options = defaultMinorUnit.SetShortBoolean(field_9_options, value);
		}
	}

	public bool IsIsDate
	{
		get
		{
			return isDate.IsSet(field_9_options);
		}
		set
		{
			field_9_options = isDate.SetShortBoolean(field_9_options, value);
		}
	}

	public bool IsDefaultBase
	{
		get
		{
			return defaultBase.IsSet(field_9_options);
		}
		set
		{
			field_9_options = defaultBase.SetShortBoolean(field_9_options, value);
		}
	}

	public bool IsDefaultCross
	{
		get
		{
			return defaultCross.IsSet(field_9_options);
		}
		set
		{
			field_9_options = defaultCross.SetShortBoolean(field_9_options, value);
		}
	}

	public bool IsDefaultDateSettings
	{
		get
		{
			return defaultDateSettings.IsSet(field_9_options);
		}
		set
		{
			field_9_options = defaultDateSettings.SetShortBoolean(field_9_options, value);
		}
	}

	public AxisOptionsRecord()
	{
	}

	public AxisOptionsRecord(RecordInputStream in1)
	{
		field_1_minimumCategory = in1.ReadShort();
		field_2_maximumCategory = in1.ReadShort();
		field_3_majorUnitValue = in1.ReadShort();
		field_4_majorUnit = in1.ReadShort();
		field_5_minorUnitValue = in1.ReadShort();
		field_6_minorUnit = in1.ReadShort();
		field_7_baseUnit = in1.ReadShort();
		field_8_crossingPoint = in1.ReadShort();
		field_9_options = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[AXCEXT]\n");
		stringBuilder.Append("    .minimumCategory      = ").Append("0x").Append(HexDump.ToHex(MinimumCategory))
			.Append(" (")
			.Append(MinimumCategory)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .maximumCategory      = ").Append("0x").Append(HexDump.ToHex(MaximumCategory))
			.Append(" (")
			.Append(MaximumCategory)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .majorUnitValue       = ").Append("0x").Append(HexDump.ToHex(MajorUnitValue))
			.Append(" (")
			.Append(MajorUnitValue)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .majorUnit            = ").Append("0x").Append(HexDump.ToHex(MajorUnit))
			.Append(" (")
			.Append(MajorUnit)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .minorUnitValue       = ").Append("0x").Append(HexDump.ToHex(MinorUnitValue))
			.Append(" (")
			.Append(MinorUnitValue)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .minorUnit            = ").Append("0x").Append(HexDump.ToHex(MinorUnit))
			.Append(" (")
			.Append(MinorUnit)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .baseUnit             = ").Append("0x").Append(HexDump.ToHex(BaseUnit))
			.Append(" (")
			.Append(BaseUnit)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .crossingPoint        = ").Append("0x").Append(HexDump.ToHex(CrossingPoint))
			.Append(" (")
			.Append(CrossingPoint)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .options              = ").Append("0x").Append(HexDump.ToHex(Options))
			.Append(" (")
			.Append(Options)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .defaultMinimum           = ").Append(IsDefaultMinimum).Append('\n');
		stringBuilder.Append("         .defaultMaximum           = ").Append(IsDefaultMaximum).Append('\n');
		stringBuilder.Append("         .defaultMajor             = ").Append(IsDefaultMajor).Append('\n');
		stringBuilder.Append("         .defaultMinorUnit         = ").Append(IsDefaultMinorUnit).Append('\n');
		stringBuilder.Append("         .IsDate                   = ").Append(IsIsDate).Append('\n');
		stringBuilder.Append("         .defaultBase              = ").Append(IsDefaultBase).Append('\n');
		stringBuilder.Append("         .defaultCross             = ").Append(IsDefaultCross).Append('\n');
		stringBuilder.Append("         .defaultDateSettings      = ").Append(IsDefaultDateSettings).Append('\n');
		stringBuilder.Append("[/AXCEXT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_minimumCategory);
		out1.WriteShort(field_2_maximumCategory);
		out1.WriteShort(field_3_majorUnitValue);
		out1.WriteShort(field_4_majorUnit);
		out1.WriteShort(field_5_minorUnitValue);
		out1.WriteShort(field_6_minorUnit);
		out1.WriteShort(field_7_baseUnit);
		out1.WriteShort(field_8_crossingPoint);
		out1.WriteShort(field_9_options);
	}

	public override object Clone()
	{
		return new AxisOptionsRecord
		{
			field_1_minimumCategory = field_1_minimumCategory,
			field_2_maximumCategory = field_2_maximumCategory,
			field_3_majorUnitValue = field_3_majorUnitValue,
			field_4_majorUnit = field_4_majorUnit,
			field_5_minorUnitValue = field_5_minorUnitValue,
			field_6_minorUnit = field_6_minorUnit,
			field_7_baseUnit = field_7_baseUnit,
			field_8_crossingPoint = field_8_crossingPoint,
			field_9_options = field_9_options
		};
	}

	public void SetIsDate(bool value)
	{
	}
}
