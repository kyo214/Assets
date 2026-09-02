using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class WindowOneRecord : StandardRecord
{
	public const short sid = 61;

	private short field_1_h_hold;

	private short field_2_v_hold;

	private short field_3_width;

	private short field_4_height;

	private short field_5_options;

	private static BitField hidden = BitFieldFactory.GetInstance(1);

	private static BitField iconic = BitFieldFactory.GetInstance(2);

	private static BitField reserved = BitFieldFactory.GetInstance(4);

	private static BitField hscroll = BitFieldFactory.GetInstance(8);

	private static BitField vscroll = BitFieldFactory.GetInstance(16);

	private static BitField tabs = BitFieldFactory.GetInstance(32);

	private int field_6_active_sheet;

	private int field_7_first_visible_tab;

	private short field_8_num_selected_tabs;

	private short field_9_tab_width_ratio;

	public short HorizontalHold
	{
		get
		{
			return field_1_h_hold;
		}
		set
		{
			field_1_h_hold = value;
		}
	}

	public short VerticalHold
	{
		get
		{
			return field_2_v_hold;
		}
		set
		{
			field_2_v_hold = value;
		}
	}

	public short Width
	{
		get
		{
			return field_3_width;
		}
		set
		{
			field_3_width = value;
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

	public short Options
	{
		get
		{
			return field_5_options;
		}
		set
		{
			field_5_options = value;
		}
	}

	public bool Hidden
	{
		get
		{
			return hidden.IsSet(field_5_options);
		}
		set
		{
			field_5_options = hidden.SetShortBoolean(field_5_options, value);
		}
	}

	public bool Iconic
	{
		get
		{
			return iconic.IsSet(field_5_options);
		}
		set
		{
			field_5_options = iconic.SetShortBoolean(field_5_options, value);
		}
	}

	public bool DisplayHorizontalScrollbar
	{
		get
		{
			return hscroll.IsSet(field_5_options);
		}
		set
		{
			field_5_options = hscroll.SetShortBoolean(field_5_options, value);
		}
	}

	public bool DisplayVerticalScrollbar
	{
		get
		{
			return vscroll.IsSet(field_5_options);
		}
		set
		{
			field_5_options = vscroll.SetShortBoolean(field_5_options, value);
		}
	}

	public bool DisplayTabs
	{
		get
		{
			return tabs.IsSet(field_5_options);
		}
		set
		{
			field_5_options = tabs.SetShortBoolean(field_5_options, value);
		}
	}

	public int ActiveSheetIndex
	{
		get
		{
			return field_6_active_sheet;
		}
		set
		{
			field_6_active_sheet = value;
		}
	}

	public int FirstVisibleTab
	{
		get
		{
			return field_7_first_visible_tab;
		}
		set
		{
			field_7_first_visible_tab = value;
		}
	}

	public short NumSelectedTabs
	{
		get
		{
			return field_8_num_selected_tabs;
		}
		set
		{
			field_8_num_selected_tabs = value;
		}
	}

	public short TabWidthRatio
	{
		get
		{
			return field_9_tab_width_ratio;
		}
		set
		{
			field_9_tab_width_ratio = value;
		}
	}

	protected override int DataSize => 18;

	public override short Sid => 61;

	public WindowOneRecord()
	{
	}

	public WindowOneRecord(RecordInputStream in1)
	{
		field_1_h_hold = in1.ReadShort();
		field_2_v_hold = in1.ReadShort();
		field_3_width = in1.ReadShort();
		field_4_height = in1.ReadShort();
		field_5_options = in1.ReadShort();
		field_6_active_sheet = in1.ReadShort();
		field_7_first_visible_tab = in1.ReadShort();
		field_8_num_selected_tabs = in1.ReadShort();
		field_9_tab_width_ratio = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[WINDOW1]\n");
		stringBuilder.Append("    .h_hold          = ").Append(StringUtil.ToHexString(HorizontalHold)).Append("\n");
		stringBuilder.Append("    .v_hold          = ").Append(StringUtil.ToHexString(VerticalHold)).Append("\n");
		stringBuilder.Append("    .width           = ").Append(StringUtil.ToHexString(Width)).Append("\n");
		stringBuilder.Append("    .height          = ").Append(StringUtil.ToHexString(Height)).Append("\n");
		stringBuilder.Append("    .options         = ").Append(StringUtil.ToHexString(Options)).Append("\n");
		stringBuilder.Append("        .hidden      = ").Append(Hidden).Append("\n");
		stringBuilder.Append("        .iconic      = ").Append(Iconic).Append("\n");
		stringBuilder.Append("        .hscroll     = ").Append(DisplayHorizontalScrollbar).Append("\n");
		stringBuilder.Append("        .vscroll     = ").Append(DisplayVerticalScrollbar).Append("\n");
		stringBuilder.Append("        .tabs        = ").Append(DisplayTabs).Append("\n");
		stringBuilder.Append("    .activeSheet     = ").Append(StringUtil.ToHexString(ActiveSheetIndex)).Append("\n");
		stringBuilder.Append("    .firstVisibleTab    = ").Append(StringUtil.ToHexString(FirstVisibleTab)).Append("\n");
		stringBuilder.Append("    .numselectedtabs = ").Append(StringUtil.ToHexString(NumSelectedTabs)).Append("\n");
		stringBuilder.Append("    .tabwidthratio   = ").Append(StringUtil.ToHexString(TabWidthRatio)).Append("\n");
		stringBuilder.Append("[/WINDOW1]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(HorizontalHold);
		out1.WriteShort(VerticalHold);
		out1.WriteShort(Width);
		out1.WriteShort(Height);
		out1.WriteShort(Options);
		out1.WriteShort(ActiveSheetIndex);
		out1.WriteShort(FirstVisibleTab);
		out1.WriteShort(NumSelectedTabs);
		out1.WriteShort(TabWidthRatio);
	}
}
