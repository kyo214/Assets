using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class WindowTwoRecord : StandardRecord
{
	public const short sid = 574;

	private BitField displayFormulas = BitFieldFactory.GetInstance(1);

	private BitField displayGridlines = BitFieldFactory.GetInstance(2);

	private BitField displayRowColHeadings = BitFieldFactory.GetInstance(4);

	private BitField freezePanes = BitFieldFactory.GetInstance(8);

	private BitField displayZeros = BitFieldFactory.GetInstance(16);

	private BitField defaultHeader = BitFieldFactory.GetInstance(32);

	private BitField arabic = BitFieldFactory.GetInstance(64);

	private BitField displayGuts = BitFieldFactory.GetInstance(128);

	private BitField freezePanesNoSplit = BitFieldFactory.GetInstance(256);

	private BitField selected = BitFieldFactory.GetInstance(512);

	private BitField active = BitFieldFactory.GetInstance(1024);

	private BitField savedInPageBreakPreview = BitFieldFactory.GetInstance(2048);

	private short field_1_options;

	private short field_2_top_row;

	private short field_3_left_col;

	private int field_4_header_color;

	private short field_5_page_break_zoom;

	private short field_6_normal_zoom;

	private int field_7_reserved;

	public short Options
	{
		get
		{
			return field_1_options;
		}
		set
		{
			field_1_options = value;
		}
	}

	public bool DisplayFormulas
	{
		get
		{
			return displayFormulas.IsSet(field_1_options);
		}
		set
		{
			field_1_options = displayFormulas.SetShortBoolean(field_1_options, value);
		}
	}

	public bool DisplayGridlines
	{
		get
		{
			return displayGridlines.IsSet(field_1_options);
		}
		set
		{
			field_1_options = displayGridlines.SetShortBoolean(field_1_options, value);
		}
	}

	public bool DisplayRowColHeadings
	{
		get
		{
			return displayRowColHeadings.IsSet(field_1_options);
		}
		set
		{
			field_1_options = displayRowColHeadings.SetShortBoolean(field_1_options, value);
		}
	}

	public bool FreezePanes
	{
		get
		{
			return freezePanes.IsSet(field_1_options);
		}
		set
		{
			field_1_options = freezePanes.SetShortBoolean(field_1_options, value);
		}
	}

	public bool DisplayZeros
	{
		get
		{
			return displayZeros.IsSet(field_1_options);
		}
		set
		{
			field_1_options = displayZeros.SetShortBoolean(field_1_options, value);
		}
	}

	public bool DefaultHeader
	{
		get
		{
			return defaultHeader.IsSet(field_1_options);
		}
		set
		{
			field_1_options = defaultHeader.SetShortBoolean(field_1_options, value);
		}
	}

	public bool Arabic
	{
		get
		{
			return arabic.IsSet(field_1_options);
		}
		set
		{
			field_1_options = arabic.SetShortBoolean(field_1_options, value);
		}
	}

	public bool DisplayGuts
	{
		get
		{
			return displayGuts.IsSet(field_1_options);
		}
		set
		{
			field_1_options = displayGuts.SetShortBoolean(field_1_options, value);
		}
	}

	public bool FreezePanesNoSplit
	{
		get
		{
			return freezePanesNoSplit.IsSet(field_1_options);
		}
		set
		{
			field_1_options = freezePanesNoSplit.SetShortBoolean(field_1_options, value);
		}
	}

	public bool IsSelected
	{
		get
		{
			return selected.IsSet(field_1_options);
		}
		set
		{
			field_1_options = selected.SetShortBoolean(field_1_options, value);
		}
	}

	public bool IsActive
	{
		get
		{
			return active.IsSet(field_1_options);
		}
		set
		{
			field_1_options = active.SetShortBoolean(field_1_options, value);
		}
	}

	public bool SavedInPageBreakPreview
	{
		get
		{
			return savedInPageBreakPreview.IsSet(field_1_options);
		}
		set
		{
			field_1_options = savedInPageBreakPreview.SetShortBoolean(field_1_options, value);
		}
	}

	public short TopRow
	{
		get
		{
			return field_2_top_row;
		}
		set
		{
			field_2_top_row = value;
		}
	}

	public short LeftCol
	{
		get
		{
			return field_3_left_col;
		}
		set
		{
			field_3_left_col = value;
		}
	}

	public int HeaderColor
	{
		get
		{
			return field_4_header_color;
		}
		set
		{
			field_4_header_color = value;
		}
	}

	public short PageBreakZoom
	{
		get
		{
			return field_5_page_break_zoom;
		}
		set
		{
			field_5_page_break_zoom = value;
		}
	}

	public short NormalZoom
	{
		get
		{
			return field_6_normal_zoom;
		}
		set
		{
			field_6_normal_zoom = value;
		}
	}

	public int Reserved
	{
		get
		{
			return field_7_reserved;
		}
		set
		{
			field_7_reserved = value;
		}
	}

	protected override int DataSize => 18;

	public override short Sid => 574;

	public WindowTwoRecord()
	{
	}

	public WindowTwoRecord(RecordInputStream in1)
	{
		int remaining = in1.Remaining;
		field_1_options = in1.ReadShort();
		field_2_top_row = in1.ReadShort();
		field_3_left_col = in1.ReadShort();
		field_4_header_color = in1.ReadInt();
		if (remaining > 10)
		{
			field_5_page_break_zoom = in1.ReadShort();
			field_6_normal_zoom = in1.ReadShort();
		}
		if (remaining > 14)
		{
			field_7_reserved = in1.ReadInt();
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[WINDOW2]\n");
		stringBuilder.Append("    .options        = ").Append(StringUtil.ToHexString(Options)).Append("\n");
		stringBuilder.Append("       .dispformulas= ").Append(DisplayFormulas).Append("\n");
		stringBuilder.Append("       .dispgridlins= ").Append(DisplayGridlines).Append("\n");
		stringBuilder.Append("       .disprcheadin= ").Append(DisplayRowColHeadings).Append("\n");
		stringBuilder.Append("       .freezepanes = ").Append(FreezePanes).Append("\n");
		stringBuilder.Append("       .Displayzeros= ").Append(DisplayZeros).Append("\n");
		stringBuilder.Append("       .defaultheadr= ").Append(DefaultHeader).Append("\n");
		stringBuilder.Append("       .arabic      = ").Append(Arabic).Append("\n");
		stringBuilder.Append("       .Displayguts = ").Append(DisplayGuts).Append("\n");
		stringBuilder.Append("       .frzpnsnosplt= ").Append(FreezePanesNoSplit).Append("\n");
		stringBuilder.Append("       .selected    = ").Append(IsSelected).Append("\n");
		stringBuilder.Append("       .active       = ").Append(IsActive).Append("\n");
		stringBuilder.Append("       .svdinpgbrkpv= ").Append(SavedInPageBreakPreview).Append("\n");
		stringBuilder.Append("    .toprow         = ").Append(StringUtil.ToHexString(TopRow)).Append("\n");
		stringBuilder.Append("    .leftcol        = ").Append(StringUtil.ToHexString(LeftCol)).Append("\n");
		stringBuilder.Append("    .headercolor    = ").Append(StringUtil.ToHexString(HeaderColor)).Append("\n");
		stringBuilder.Append("    .pagebreakzoom  = ").Append(StringUtil.ToHexString(PageBreakZoom)).Append("\n");
		stringBuilder.Append("    .normalzoom     = ").Append(StringUtil.ToHexString(NormalZoom)).Append("\n");
		stringBuilder.Append("    .reserved       = ").Append(StringUtil.ToHexString(Reserved)).Append("\n");
		stringBuilder.Append("[/WINDOW2]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(Options);
		out1.WriteShort(TopRow);
		out1.WriteShort(LeftCol);
		out1.WriteInt(HeaderColor);
		out1.WriteShort(PageBreakZoom);
		out1.WriteShort(NormalZoom);
		out1.WriteInt(Reserved);
	}

	public override object Clone()
	{
		return new WindowTwoRecord
		{
			field_1_options = field_1_options,
			field_2_top_row = field_2_top_row,
			field_3_left_col = field_3_left_col,
			field_4_header_color = field_4_header_color,
			field_5_page_break_zoom = field_5_page_break_zoom,
			field_6_normal_zoom = field_6_normal_zoom,
			field_7_reserved = field_7_reserved
		};
	}
}
