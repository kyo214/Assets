using System.Text;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class FontRecord : StandardRecord
{
	public const short sid = 49;

	private short field_1_font_height;

	private short field_2_attributes;

	private static BitField italic = BitFieldFactory.GetInstance(2);

	private static BitField strikeout = BitFieldFactory.GetInstance(8);

	private static BitField macoutline = BitFieldFactory.GetInstance(16);

	private static BitField macshadow = BitFieldFactory.GetInstance(32);

	private short field_3_color_palette_index;

	private short field_4_bold_weight;

	private short field_5_base_sub_script;

	private byte field_6_underline;

	private byte field_7_family;

	private byte field_8_charset;

	private byte field_9_zero;

	private string field_11_font_name;

	public bool IsItalic
	{
		get
		{
			return italic.IsSet(field_2_attributes);
		}
		set
		{
			field_2_attributes = italic.SetShortBoolean(field_2_attributes, value);
		}
	}

	public bool IsStrikeout
	{
		get
		{
			return strikeout.IsSet(field_2_attributes);
		}
		set
		{
			field_2_attributes = strikeout.SetShortBoolean(field_2_attributes, value);
		}
	}

	public bool IsMacoutlined
	{
		get
		{
			return macoutline.IsSet(field_2_attributes);
		}
		set
		{
			field_2_attributes = macoutline.SetShortBoolean(field_2_attributes, value);
		}
	}

	public bool IsMacshadowed
	{
		get
		{
			return macshadow.IsSet(field_2_attributes);
		}
		set
		{
			field_2_attributes = macshadow.SetShortBoolean(field_2_attributes, value);
		}
	}

	public FontUnderlineType Underline
	{
		get
		{
			return (FontUnderlineType)field_6_underline;
		}
		set
		{
			field_6_underline = (byte)value;
		}
	}

	public byte Family
	{
		get
		{
			return field_7_family;
		}
		set
		{
			field_7_family = value;
		}
	}

	public byte Charset
	{
		get
		{
			return field_8_charset;
		}
		set
		{
			field_8_charset = value;
		}
	}

	public string FontName
	{
		get
		{
			return field_11_font_name;
		}
		set
		{
			field_11_font_name = value;
		}
	}

	public short FontHeight
	{
		get
		{
			return field_1_font_height;
		}
		set
		{
			field_1_font_height = value;
		}
	}

	public short Attributes
	{
		get
		{
			return field_2_attributes;
		}
		set
		{
			field_2_attributes = value;
		}
	}

	public short ColorPaletteIndex
	{
		get
		{
			return field_3_color_palette_index;
		}
		set
		{
			field_3_color_palette_index = value;
		}
	}

	public short BoldWeight
	{
		get
		{
			return field_4_bold_weight;
		}
		set
		{
			field_4_bold_weight = value;
		}
	}

	public FontSuperScript SuperSubScript
	{
		get
		{
			return (FontSuperScript)field_5_base_sub_script;
		}
		set
		{
			field_5_base_sub_script = (short)value;
		}
	}

	protected override int DataSize
	{
		get
		{
			int num = 16;
			int length = field_11_font_name.Length;
			if (length < 1)
			{
				return num;
			}
			bool flag = StringUtil.HasMultibyte(field_11_font_name);
			return num + length * ((!flag) ? 1 : 2);
		}
	}

	public override short Sid => 49;

	public FontRecord()
	{
	}

	public FontRecord(RecordInputStream in1)
	{
		field_1_font_height = in1.ReadShort();
		field_2_attributes = in1.ReadShort();
		field_3_color_palette_index = in1.ReadShort();
		field_4_bold_weight = in1.ReadShort();
		field_5_base_sub_script = in1.ReadShort();
		field_6_underline = (byte)in1.ReadByte();
		field_7_family = (byte)in1.ReadByte();
		field_8_charset = (byte)in1.ReadByte();
		field_9_zero = (byte)in1.ReadByte();
		int num = (byte)in1.ReadByte();
		int num2 = in1.ReadUByte();
		if (num > 0)
		{
			if (num2 == 0)
			{
				field_11_font_name = in1.ReadCompressedUnicode(num);
			}
			else
			{
				field_11_font_name = in1.ReadUnicodeLEString(num);
			}
		}
		else
		{
			field_11_font_name = "";
		}
	}

	public void CloneStyleFrom(FontRecord source)
	{
		field_1_font_height = source.field_1_font_height;
		field_2_attributes = source.field_2_attributes;
		field_3_color_palette_index = source.field_3_color_palette_index;
		field_4_bold_weight = source.field_4_bold_weight;
		field_5_base_sub_script = source.field_5_base_sub_script;
		field_6_underline = source.field_6_underline;
		field_7_family = source.field_7_family;
		field_8_charset = source.field_8_charset;
		field_9_zero = source.field_9_zero;
		field_11_font_name = source.field_11_font_name;
	}

	public bool SameProperties(FontRecord other)
	{
		if (field_1_font_height == other.field_1_font_height && field_2_attributes == other.field_2_attributes && field_3_color_palette_index == other.field_3_color_palette_index && field_4_bold_weight == other.field_4_bold_weight && field_5_base_sub_script == other.field_5_base_sub_script && field_6_underline == other.field_6_underline && field_7_family == other.field_7_family && field_8_charset == other.field_8_charset && field_9_zero == other.field_9_zero)
		{
			return field_11_font_name.Equals(other.field_11_font_name);
		}
		return false;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FONT]\n");
		stringBuilder.Append("    .fontheight      = ").Append(StringUtil.ToHexString(FontHeight)).Append("\n");
		stringBuilder.Append("    .attributes      = ").Append(StringUtil.ToHexString(Attributes)).Append("\n");
		stringBuilder.Append("         .italic     = ").Append(IsItalic).Append("\n");
		stringBuilder.Append("         .strikout   = ").Append(IsStrikeout).Append("\n");
		stringBuilder.Append("         .macoutlined= ").Append(IsMacoutlined).Append("\n");
		stringBuilder.Append("         .macshadowed= ").Append(IsMacshadowed).Append("\n");
		stringBuilder.Append("    .colorpalette    = ").Append(StringUtil.ToHexString(ColorPaletteIndex)).Append("\n");
		stringBuilder.Append("    .boldweight      = ").Append(StringUtil.ToHexString(BoldWeight)).Append("\n");
		stringBuilder.Append("    .basesubscript  = ").Append(StringUtil.ToHexString((short)SuperSubScript)).Append("\n");
		stringBuilder.Append("    .underline       = ").Append(StringUtil.ToHexString((short)Underline)).Append("\n");
		stringBuilder.Append("    .family          = ").Append(StringUtil.ToHexString(Family)).Append("\n");
		stringBuilder.Append("    .charset         = ").Append(StringUtil.ToHexString(Charset)).Append("\n");
		stringBuilder.Append("    .fontname        = ").Append(FontName).Append("\n");
		stringBuilder.Append("[/FONT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(FontHeight);
		out1.WriteShort(Attributes);
		out1.WriteShort(ColorPaletteIndex);
		out1.WriteShort(BoldWeight);
		out1.WriteShort((int)SuperSubScript);
		out1.WriteByte((int)Underline);
		out1.WriteByte(Family);
		out1.WriteByte(Charset);
		out1.WriteByte(field_9_zero);
		int length = field_11_font_name.Length;
		out1.WriteByte(length);
		bool flag = StringUtil.HasMultibyte(field_11_font_name);
		out1.WriteByte(flag ? 1 : 0);
		if (length > 0)
		{
			if (flag)
			{
				StringUtil.PutUnicodeLE(field_11_font_name, out1);
			}
			else
			{
				StringUtil.PutCompressedUnicode(field_11_font_name, out1);
			}
		}
	}

	public override int GetHashCode()
	{
		int num = 1;
		num = 31 * num + ((field_11_font_name != null) ? field_11_font_name.GetHashCode() : 0);
		num = 31 * num + field_1_font_height;
		num = 31 * num + field_2_attributes;
		num = 31 * num + field_3_color_palette_index;
		num = 31 * num + field_4_bold_weight;
		num = 31 * num + field_5_base_sub_script;
		num = 31 * num + field_6_underline;
		num = 31 * num + field_7_family;
		num = 31 * num + field_8_charset;
		return 31 * num + field_9_zero;
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		return false;
	}
}
