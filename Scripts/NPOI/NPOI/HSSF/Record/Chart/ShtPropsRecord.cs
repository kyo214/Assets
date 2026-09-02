using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class ShtPropsRecord : StandardRecord
{
	public const short sid = 4164;

	private short field_1_flags;

	private BitField manSerAlloc = BitFieldFactory.GetInstance(1);

	private BitField plotVisibleOnly = BitFieldFactory.GetInstance(2);

	private BitField doNotSizeWithWindow = BitFieldFactory.GetInstance(4);

	private BitField manPlotArea = BitFieldFactory.GetInstance(8);

	private BitField alwaysAutoPlotArea = BitFieldFactory.GetInstance(16);

	private byte field_2_mdBlank;

	private byte field_3_reserved;

	public const byte EMPTY_NOT_PLOTTED = 0;

	public const byte EMPTY_ZERO = 1;

	public const byte EMPTY_INTERPOLATED = 2;

	protected override int DataSize => 4;

	public override short Sid => 4164;

	public short Flags
	{
		get
		{
			return field_1_flags;
		}
		set
		{
			field_1_flags = value;
		}
	}

	public byte Blank
	{
		get
		{
			return field_2_mdBlank;
		}
		set
		{
			field_2_mdBlank = value;
		}
	}

	public bool IsManSerAlloc
	{
		get
		{
			return manSerAlloc.IsSet(field_1_flags);
		}
		set
		{
			field_1_flags = manSerAlloc.SetShortBoolean(field_1_flags, value);
		}
	}

	public bool IsPlotVisibleOnly
	{
		get
		{
			return plotVisibleOnly.IsSet(field_1_flags);
		}
		set
		{
			field_1_flags = plotVisibleOnly.SetShortBoolean(field_1_flags, value);
		}
	}

	public bool IsNotSizeWithWindow
	{
		get
		{
			return doNotSizeWithWindow.IsSet(field_1_flags);
		}
		set
		{
			field_1_flags = doNotSizeWithWindow.SetShortBoolean(field_1_flags, value);
		}
	}

	public bool IsManPlotArea
	{
		get
		{
			return manPlotArea.IsSet(field_1_flags);
		}
		set
		{
			field_1_flags = manPlotArea.SetShortBoolean(field_1_flags, value);
		}
	}

	public bool IsAlwaysAutoPlotArea
	{
		get
		{
			return alwaysAutoPlotArea.IsSet(field_1_flags);
		}
		set
		{
			field_1_flags = alwaysAutoPlotArea.SetShortBoolean(field_1_flags, value);
			if (value)
			{
				IsManPlotArea = value;
			}
		}
	}

	public ShtPropsRecord()
	{
	}

	public ShtPropsRecord(RecordInputStream in1)
	{
		field_1_flags = in1.ReadShort();
		field_2_mdBlank = (byte)in1.ReadByte();
		field_3_reserved = (byte)in1.ReadByte();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SHTPROPS]\n");
		stringBuilder.Append("    .flags                = ").Append("0x").Append(HexDump.ToHex(Flags))
			.Append(" (")
			.Append(Flags)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .chartTypeManuallyFormatted     = ").Append(IsManSerAlloc).Append('\n');
		stringBuilder.Append("         .plotVisibleOnly          = ").Append(IsPlotVisibleOnly).Append('\n');
		stringBuilder.Append("         .doNotSizeWithWindow      = ").Append(IsNotSizeWithWindow).Append('\n');
		stringBuilder.Append("         .defaultPlotDimensions     = ").Append(IsManPlotArea).Append('\n');
		stringBuilder.Append("         .autoPlotArea             = ").Append(IsAlwaysAutoPlotArea).Append('\n');
		stringBuilder.Append("    .empty                = ").Append("0x").Append(HexDump.ToHex(Blank))
			.Append(" (")
			.Append(Blank)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/SHTPROPS]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_flags);
		out1.WriteByte(field_2_mdBlank);
		out1.WriteByte(0);
	}

	public override object Clone()
	{
		return new ShtPropsRecord
		{
			field_1_flags = field_1_flags,
			field_2_mdBlank = field_2_mdBlank,
			field_3_reserved = field_3_reserved
		};
	}
}
