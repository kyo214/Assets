using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ExtendedFormatRecord : StandardRecord
{
	public const short sid = 224;

	public const short NULL = -16;

	public const short XF_STYLE = 1;

	public const short XF_CELL = 0;

	public const short NONE = 0;

	public const short THIN = 1;

	public const short MEDIUM = 2;

	public const short DASHED = 3;

	public const short DOTTED = 4;

	public const short THICK = 5;

	public const short DOUBLE = 6;

	public const short HAIR = 7;

	public const short MEDIUM_DASHED = 8;

	public const short DASH_DOT = 9;

	public const short MEDIUM_DASH_DOT = 10;

	public const short DASH_DOT_DOT = 11;

	public const short MEDIUM_DASH_DOT_DOT = 12;

	public const short SLANTED_DASH_DOT = 13;

	public const short GENERAL = 0;

	public const short LEFT = 1;

	public const short CENTER = 2;

	public const short RIGHT = 3;

	public const short FILL = 4;

	public const short JUSTIFY = 5;

	public const short CENTER_SELECTION = 6;

	public const short VERTICAL_TOP = 0;

	public const short VERTICAL_CENTER = 1;

	public const short VERTICAL_BOTTOM = 2;

	public const short VERTICAL_JUSTIFY = 3;

	public const short NO_FILL = 0;

	public const short SOLID_FILL = 1;

	public const short FINE_DOTS = 2;

	public const short ALT_BARS = 3;

	public const short SPARSE_DOTS = 4;

	public const short THICK_HORZ_BANDS = 5;

	public const short THICK_VERT_BANDS = 6;

	public const short THICK_BACKWARD_DIAG = 7;

	public const short THICK_FORWARD_DIAG = 8;

	public const short BIG_SPOTS = 9;

	public const short BRICKS = 10;

	public const short THIN_HORZ_BANDS = 11;

	public const short THIN_VERT_BANDS = 12;

	public const short THIN_BACKWARD_DIAG = 13;

	public const short THIN_FORWARD_DIAG = 14;

	public const short SQUARES = 15;

	public const short DIAMONDS = 16;

	private short field_1_font_index;

	private short field_2_format_index;

	private static BitField _locked = BitFieldFactory.GetInstance(1);

	private static BitField _hidden = BitFieldFactory.GetInstance(2);

	private static BitField _xf_type = BitFieldFactory.GetInstance(4);

	private static BitField _123_prefix = BitFieldFactory.GetInstance(8);

	private static BitField _parent_index = BitFieldFactory.GetInstance(65520);

	private short field_3_cell_options;

	private static BitField _alignment = BitFieldFactory.GetInstance(7);

	private static BitField _wrap_text = BitFieldFactory.GetInstance(8);

	private static BitField _vertical_alignment = BitFieldFactory.GetInstance(112);

	private static BitField _justify_last = BitFieldFactory.GetInstance(128);

	private static BitField _rotation = BitFieldFactory.GetInstance(65280);

	private short field_4_alignment_options;

	private static BitField _indent = BitFieldFactory.GetInstance(15);

	private static BitField _shrink_to_fit = BitFieldFactory.GetInstance(16);

	private static BitField _merge_cells = BitFieldFactory.GetInstance(32);

	private static BitField _Reading_order = BitFieldFactory.GetInstance(192);

	private static BitField _indent_not_parent_format = BitFieldFactory.GetInstance(1024);

	private static BitField _indent_not_parent_font = BitFieldFactory.GetInstance(2048);

	private static BitField _indent_not_parent_alignment = BitFieldFactory.GetInstance(4096);

	private static BitField _indent_not_parent_border = BitFieldFactory.GetInstance(8192);

	private static BitField _indent_not_parent_pattern = BitFieldFactory.GetInstance(16384);

	private static BitField _indent_not_parent_cell_options = BitFieldFactory.GetInstance(32768);

	private short field_5_indention_options;

	private static BitField _border_left = BitFieldFactory.GetInstance(15);

	private static BitField _border_right = BitFieldFactory.GetInstance(240);

	private static BitField _border_top = BitFieldFactory.GetInstance(3840);

	private static BitField _border_bottom = BitFieldFactory.GetInstance(61440);

	private short field_6_border_options;

	private static BitField _left_border_palette_idx = BitFieldFactory.GetInstance(127);

	private static BitField _right_border_palette_idx = BitFieldFactory.GetInstance(16256);

	private static BitField _diag = BitFieldFactory.GetInstance(49152);

	private short field_7_palette_options;

	private static BitField _top_border_palette_idx = BitFieldFactory.GetInstance(127);

	private static BitField _bottom_border_palette_idx = BitFieldFactory.GetInstance(16256);

	private static BitField _adtl_diag_border_palette_idx = BitFieldFactory.GetInstance(2080768);

	private static BitField _adtl_diag_line_style = BitFieldFactory.GetInstance(31457280);

	private static BitField _adtl_fill_pattern = BitFieldFactory.GetInstance(-67108864);

	private int field_8_adtl_palette_options;

	private static BitField _fill_foreground = BitFieldFactory.GetInstance(127);

	private static BitField _fill_background = BitFieldFactory.GetInstance(16256);

	private short field_9_fill_palette_options;

	public short FontIndex
	{
		get
		{
			return field_1_font_index;
		}
		set
		{
			field_1_font_index = value;
		}
	}

	public short FormatIndex
	{
		get
		{
			return field_2_format_index;
		}
		set
		{
			field_2_format_index = value;
		}
	}

	public short CellOptions
	{
		get
		{
			return field_3_cell_options;
		}
		set
		{
			field_3_cell_options = value;
		}
	}

	public bool IsLocked
	{
		get
		{
			return _locked.IsSet(field_3_cell_options);
		}
		set
		{
			field_3_cell_options = _locked.SetShortBoolean(field_3_cell_options, value);
		}
	}

	public bool IsHidden
	{
		get
		{
			return _hidden.IsSet(field_3_cell_options);
		}
		set
		{
			field_3_cell_options = _hidden.SetShortBoolean(field_3_cell_options, value);
		}
	}

	public short XFType
	{
		get
		{
			return _xf_type.GetShortValue(field_3_cell_options);
		}
		set
		{
			field_3_cell_options = _xf_type.SetShortValue(field_3_cell_options, value);
		}
	}

	public bool _123Prefix
	{
		get
		{
			return _123_prefix.IsSet(field_3_cell_options);
		}
		set
		{
			field_3_cell_options = _123_prefix.SetShortBoolean(field_3_cell_options, value);
		}
	}

	public short ParentIndex
	{
		get
		{
			return _parent_index.GetShortValue(field_3_cell_options);
		}
		set
		{
			field_3_cell_options = _parent_index.SetShortValue(field_3_cell_options, value);
		}
	}

	public short AlignmentOptions
	{
		get
		{
			return field_4_alignment_options;
		}
		set
		{
			field_4_alignment_options = value;
		}
	}

	public short Alignment
	{
		get
		{
			return _alignment.GetShortValue(field_4_alignment_options);
		}
		set
		{
			field_4_alignment_options = _alignment.SetShortValue(field_4_alignment_options, value);
		}
	}

	public bool WrapText
	{
		get
		{
			return _wrap_text.IsSet(field_4_alignment_options);
		}
		set
		{
			field_4_alignment_options = _wrap_text.SetShortBoolean(field_4_alignment_options, value);
		}
	}

	public short VerticalAlignment
	{
		get
		{
			return _vertical_alignment.GetShortValue(field_4_alignment_options);
		}
		set
		{
			field_4_alignment_options = _vertical_alignment.SetShortValue(field_4_alignment_options, value);
		}
	}

	public short JustifyLast
	{
		get
		{
			return _justify_last.GetShortValue(field_4_alignment_options);
		}
		set
		{
			field_4_alignment_options = _justify_last.SetShortValue(field_4_alignment_options, value);
		}
	}

	public short Rotation
	{
		get
		{
			return _rotation.GetShortValue(field_4_alignment_options);
		}
		set
		{
			field_4_alignment_options = _rotation.SetShortValue(field_4_alignment_options, value);
		}
	}

	public short IndentionOptions
	{
		get
		{
			return field_5_indention_options;
		}
		set
		{
			field_5_indention_options = value;
		}
	}

	public short Indent
	{
		get
		{
			return _indent.GetShortValue(field_5_indention_options);
		}
		set
		{
			field_5_indention_options = _indent.SetShortValue(field_5_indention_options, value);
		}
	}

	public bool ShrinkToFit
	{
		get
		{
			return _shrink_to_fit.IsSet(field_5_indention_options);
		}
		set
		{
			field_5_indention_options = _shrink_to_fit.SetShortBoolean(field_5_indention_options, value);
		}
	}

	public bool MergeCells
	{
		get
		{
			return _merge_cells.IsSet(field_5_indention_options);
		}
		set
		{
			field_5_indention_options = _merge_cells.SetShortBoolean(field_5_indention_options, value);
		}
	}

	public short ReadingOrder
	{
		get
		{
			return _Reading_order.GetShortValue(field_5_indention_options);
		}
		set
		{
			field_5_indention_options = _Reading_order.SetShortValue(field_5_indention_options, value);
		}
	}

	public bool IsIndentNotParentFormat
	{
		get
		{
			return _indent_not_parent_format.IsSet(field_5_indention_options);
		}
		set
		{
			field_5_indention_options = _indent_not_parent_format.SetShortBoolean(field_5_indention_options, value);
		}
	}

	public bool IsIndentNotParentFont
	{
		get
		{
			return _indent_not_parent_font.IsSet(field_5_indention_options);
		}
		set
		{
			field_5_indention_options = _indent_not_parent_font.SetShortBoolean(field_5_indention_options, value);
		}
	}

	public bool IsIndentNotParentAlignment
	{
		get
		{
			return _indent_not_parent_alignment.IsSet(field_5_indention_options);
		}
		set
		{
			field_5_indention_options = _indent_not_parent_alignment.SetShortBoolean(field_5_indention_options, value);
		}
	}

	public bool IsIndentNotParentBorder
	{
		get
		{
			return _indent_not_parent_border.IsSet(field_5_indention_options);
		}
		set
		{
			field_5_indention_options = _indent_not_parent_border.SetShortBoolean(field_5_indention_options, value);
		}
	}

	public bool IsIndentNotParentPattern
	{
		get
		{
			return _indent_not_parent_pattern.IsSet(field_5_indention_options);
		}
		set
		{
			field_5_indention_options = _indent_not_parent_pattern.SetShortBoolean(field_5_indention_options, value);
		}
	}

	public bool IsIndentNotParentCellOptions
	{
		get
		{
			return _indent_not_parent_cell_options.IsSet(field_5_indention_options);
		}
		set
		{
			field_5_indention_options = _indent_not_parent_cell_options.SetShortBoolean(field_5_indention_options, value);
		}
	}

	public short BorderOptions
	{
		get
		{
			return field_6_border_options;
		}
		set
		{
			field_6_border_options = value;
		}
	}

	public short BorderLeft
	{
		get
		{
			return _border_left.GetShortValue(field_6_border_options);
		}
		set
		{
			field_6_border_options = _border_left.SetShortValue(field_6_border_options, value);
		}
	}

	public short BorderRight
	{
		get
		{
			return _border_right.GetShortValue(field_6_border_options);
		}
		set
		{
			field_6_border_options = _border_right.SetShortValue(field_6_border_options, value);
		}
	}

	public short BorderTop
	{
		get
		{
			return _border_top.GetShortValue(field_6_border_options);
		}
		set
		{
			field_6_border_options = _border_top.SetShortValue(field_6_border_options, value);
		}
	}

	public short BorderBottom
	{
		get
		{
			return _border_bottom.GetShortValue(field_6_border_options);
		}
		set
		{
			field_6_border_options = _border_bottom.SetShortValue(field_6_border_options, value);
		}
	}

	public short PaletteOptions
	{
		get
		{
			return field_7_palette_options;
		}
		set
		{
			field_7_palette_options = value;
		}
	}

	public short LeftBorderPaletteIdx
	{
		get
		{
			return _left_border_palette_idx.GetShortValue(field_7_palette_options);
		}
		set
		{
			field_7_palette_options = _left_border_palette_idx.SetShortValue(field_7_palette_options, value);
		}
	}

	public short RightBorderPaletteIdx
	{
		get
		{
			return _right_border_palette_idx.GetShortValue(field_7_palette_options);
		}
		set
		{
			field_7_palette_options = _right_border_palette_idx.SetShortValue(field_7_palette_options, value);
		}
	}

	public int AdtlPaletteOptions
	{
		get
		{
			return field_8_adtl_palette_options;
		}
		set
		{
			field_8_adtl_palette_options = value;
		}
	}

	public short TopBorderPaletteIdx
	{
		get
		{
			return (short)_top_border_palette_idx.GetValue(field_8_adtl_palette_options);
		}
		set
		{
			field_8_adtl_palette_options = _top_border_palette_idx.SetValue(field_8_adtl_palette_options, value);
		}
	}

	public short BottomBorderPaletteIdx
	{
		get
		{
			return (short)_bottom_border_palette_idx.GetValue(field_8_adtl_palette_options);
		}
		set
		{
			field_8_adtl_palette_options = _bottom_border_palette_idx.SetValue(field_8_adtl_palette_options, value);
		}
	}

	public short AdtlDiagBorderPaletteIdx
	{
		get
		{
			return (short)_adtl_diag_border_palette_idx.GetValue(field_8_adtl_palette_options);
		}
		set
		{
			field_8_adtl_palette_options = _adtl_diag_border_palette_idx.SetValue(field_8_adtl_palette_options, value);
		}
	}

	public short AdtlDiagLineStyle
	{
		get
		{
			return (short)_adtl_diag_line_style.GetValue(field_8_adtl_palette_options);
		}
		set
		{
			field_8_adtl_palette_options = _adtl_diag_line_style.SetValue(field_8_adtl_palette_options, value);
		}
	}

	public short Diagonal
	{
		get
		{
			return _diag.GetShortValue(field_7_palette_options);
		}
		set
		{
			field_7_palette_options = _diag.SetShortValue(field_7_palette_options, value);
		}
	}

	public short AdtlFillPattern
	{
		get
		{
			return (short)_adtl_fill_pattern.GetValue(field_8_adtl_palette_options);
		}
		set
		{
			field_8_adtl_palette_options = _adtl_fill_pattern.SetValue(field_8_adtl_palette_options, value);
		}
	}

	public short FillPaletteOptions
	{
		get
		{
			return field_9_fill_palette_options;
		}
		set
		{
			field_9_fill_palette_options = value;
		}
	}

	public short FillForeground
	{
		get
		{
			return _fill_foreground.GetShortValue(field_9_fill_palette_options);
		}
		set
		{
			field_9_fill_palette_options = _fill_foreground.SetShortValue(field_9_fill_palette_options, value);
		}
	}

	public short FillBackground
	{
		get
		{
			return _fill_background.GetShortValue(field_9_fill_palette_options);
		}
		set
		{
			field_9_fill_palette_options = _fill_background.SetShortValue(field_9_fill_palette_options, value);
		}
	}

	protected override int DataSize => 20;

	public override short Sid => 224;

	public int[] StateSummary => new int[9] { field_1_font_index, field_2_format_index, field_3_cell_options, field_4_alignment_options, field_5_indention_options, field_6_border_options, field_7_palette_options, field_8_adtl_palette_options, field_9_fill_palette_options };

	public ExtendedFormatRecord()
	{
	}

	public ExtendedFormatRecord(RecordInputStream in1)
	{
		field_1_font_index = in1.ReadShort();
		field_2_format_index = in1.ReadShort();
		field_3_cell_options = in1.ReadShort();
		field_4_alignment_options = in1.ReadShort();
		field_5_indention_options = in1.ReadShort();
		field_6_border_options = in1.ReadShort();
		field_7_palette_options = in1.ReadShort();
		field_8_adtl_palette_options = in1.ReadInt();
		field_9_fill_palette_options = in1.ReadShort();
	}

	public void CloneStyleFrom(ExtendedFormatRecord source)
	{
		field_1_font_index = source.field_1_font_index;
		field_2_format_index = source.field_2_format_index;
		field_3_cell_options = source.field_3_cell_options;
		field_4_alignment_options = source.field_4_alignment_options;
		field_5_indention_options = source.field_5_indention_options;
		field_6_border_options = source.field_6_border_options;
		field_7_palette_options = source.field_7_palette_options;
		field_8_adtl_palette_options = source.field_8_adtl_palette_options;
		field_9_fill_palette_options = source.field_9_fill_palette_options;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[EXTENDEDFORMAT]\n");
		if (XFType == 1)
		{
			stringBuilder.Append(" STYLE_RECORD_TYPE\n");
		}
		else if (XFType == 0)
		{
			stringBuilder.Append(" CELL_RECORD_TYPE\n");
		}
		stringBuilder.Append("    .fontindex       = ").Append(StringUtil.ToHexString(FontIndex)).Append("\n");
		stringBuilder.Append("    .formatindex     = ").Append(StringUtil.ToHexString(FormatIndex)).Append("\n");
		stringBuilder.Append("    .celloptions     = ").Append(StringUtil.ToHexString(CellOptions)).Append("\n");
		stringBuilder.Append("          .Islocked  = ").Append(IsLocked).Append("\n");
		stringBuilder.Append("          .Ishidden  = ").Append(IsHidden).Append("\n");
		stringBuilder.Append("          .recordtype= ").Append(StringUtil.ToHexString(XFType)).Append("\n");
		stringBuilder.Append("          .parentidx = ").Append(StringUtil.ToHexString(ParentIndex)).Append("\n");
		stringBuilder.Append("    .alignmentoptions= ").Append(StringUtil.ToHexString(AlignmentOptions)).Append("\n");
		stringBuilder.Append("          .alignment = ").Append(Alignment).Append("\n");
		stringBuilder.Append("          .wraptext  = ").Append(WrapText).Append("\n");
		stringBuilder.Append("          .valignment= ").Append(StringUtil.ToHexString(VerticalAlignment)).Append("\n");
		stringBuilder.Append("          .justlast  = ").Append(StringUtil.ToHexString(JustifyLast)).Append("\n");
		stringBuilder.Append("          .rotation  = ").Append(StringUtil.ToHexString(Rotation)).Append("\n");
		stringBuilder.Append("    .indentionoptions= ").Append(StringUtil.ToHexString(IndentionOptions)).Append("\n");
		stringBuilder.Append("          .indent    = ").Append(StringUtil.ToHexString(Indent)).Append("\n");
		stringBuilder.Append("          .shrinktoft= ").Append(ShrinkToFit).Append("\n");
		stringBuilder.Append("          .mergecells= ").Append(MergeCells).Append("\n");
		stringBuilder.Append("          .Readngordr= ").Append(StringUtil.ToHexString(ReadingOrder)).Append("\n");
		stringBuilder.Append("          .formatflag= ").Append(IsIndentNotParentFormat).Append("\n");
		stringBuilder.Append("          .fontflag  = ").Append(IsIndentNotParentFont).Append("\n");
		stringBuilder.Append("          .prntalgnmt= ").Append(IsIndentNotParentAlignment).Append("\n");
		stringBuilder.Append("          .borderflag= ").Append(IsIndentNotParentBorder).Append("\n");
		stringBuilder.Append("          .paternflag= ").Append(IsIndentNotParentPattern).Append("\n");
		stringBuilder.Append("          .celloption= ").Append(IsIndentNotParentCellOptions).Append("\n");
		stringBuilder.Append("    .borderoptns     = ").Append(StringUtil.ToHexString(BorderOptions)).Append("\n");
		stringBuilder.Append("          .lftln     = ").Append(StringUtil.ToHexString(BorderLeft)).Append("\n");
		stringBuilder.Append("          .rgtln     = ").Append(StringUtil.ToHexString(BorderRight)).Append("\n");
		stringBuilder.Append("          .topln     = ").Append(StringUtil.ToHexString(BorderTop)).Append("\n");
		stringBuilder.Append("          .btmln     = ").Append(StringUtil.ToHexString(BorderBottom)).Append("\n");
		stringBuilder.Append("    .paleteoptns     = ").Append(StringUtil.ToHexString(PaletteOptions)).Append("\n");
		stringBuilder.Append("          .leftborder= ").Append(StringUtil.ToHexString(LeftBorderPaletteIdx)).Append("\n");
		stringBuilder.Append("          .rghtborder= ").Append(StringUtil.ToHexString(RightBorderPaletteIdx)).Append("\n");
		stringBuilder.Append("          .diag      = ").Append(StringUtil.ToHexString(Diagonal)).Append("\n");
		stringBuilder.Append("    .paleteoptn2     = ").Append(StringUtil.ToHexString(AdtlPaletteOptions)).Append("\n");
		stringBuilder.Append("          .topborder = ").Append(StringUtil.ToHexString(TopBorderPaletteIdx)).Append("\n");
		stringBuilder.Append("          .botmborder= ").Append(StringUtil.ToHexString(BottomBorderPaletteIdx)).Append("\n");
		stringBuilder.Append("          .adtldiag  = ").Append(StringUtil.ToHexString(AdtlDiagBorderPaletteIdx)).Append("\n");
		stringBuilder.Append("          .diaglnstyl= ").Append(StringUtil.ToHexString(AdtlDiagLineStyle)).Append("\n");
		stringBuilder.Append("          .Fillpattrn= ").Append(StringUtil.ToHexString(AdtlFillPattern)).Append("\n");
		stringBuilder.Append("    .Fillpaloptn     = ").Append(StringUtil.ToHexString(FillPaletteOptions)).Append("\n");
		stringBuilder.Append("          .foreground= ").Append(StringUtil.ToHexString(FillForeground)).Append("\n");
		stringBuilder.Append("          .background= ").Append(StringUtil.ToHexString(FillBackground)).Append("\n");
		stringBuilder.Append("[/EXTENDEDFORMAT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(FontIndex);
		out1.WriteShort(FormatIndex);
		out1.WriteShort(CellOptions);
		out1.WriteShort(AlignmentOptions);
		out1.WriteShort(IndentionOptions);
		out1.WriteShort(BorderOptions);
		out1.WriteShort(PaletteOptions);
		out1.WriteInt(AdtlPaletteOptions);
		out1.WriteShort(FillPaletteOptions);
	}

	public override int GetHashCode()
	{
		int num = 1;
		num = 31 * num + field_1_font_index;
		num = 31 * num + field_2_format_index;
		num = 31 * num + field_3_cell_options;
		num = 31 * num + field_4_alignment_options;
		num = 31 * num + field_5_indention_options;
		num = 31 * num + field_6_border_options;
		num = 31 * num + field_7_palette_options;
		num = 31 * num + field_8_adtl_palette_options;
		return 31 * num + field_9_fill_palette_options;
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj == null)
		{
			return false;
		}
		if (obj is ExtendedFormatRecord)
		{
			ExtendedFormatRecord extendedFormatRecord = (ExtendedFormatRecord)obj;
			if (field_1_font_index != extendedFormatRecord.field_1_font_index)
			{
				return false;
			}
			if (field_2_format_index != extendedFormatRecord.field_2_format_index)
			{
				return false;
			}
			if (field_3_cell_options != extendedFormatRecord.field_3_cell_options)
			{
				return false;
			}
			if (field_4_alignment_options != extendedFormatRecord.field_4_alignment_options)
			{
				return false;
			}
			if (field_5_indention_options != extendedFormatRecord.field_5_indention_options)
			{
				return false;
			}
			if (field_6_border_options != extendedFormatRecord.field_6_border_options)
			{
				return false;
			}
			if (field_7_palette_options != extendedFormatRecord.field_7_palette_options)
			{
				return false;
			}
			if (field_8_adtl_palette_options != extendedFormatRecord.field_8_adtl_palette_options)
			{
				return false;
			}
			if (field_9_fill_palette_options != extendedFormatRecord.field_9_fill_palette_options)
			{
				return false;
			}
			return true;
		}
		return false;
	}
}
