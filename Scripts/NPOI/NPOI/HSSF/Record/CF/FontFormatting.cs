using System;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.Record.CF;

public class FontFormatting : ICloneable
{
	private byte[] _rawData = new byte[118];

	private const int OFFSET_FONT_NAME = 0;

	private const int OFFSET_FONT_HEIGHT = 64;

	private const int OFFSET_FONT_OPTIONS = 68;

	private const int OFFSET_FONT_WEIGHT = 72;

	private const int OFFSET_ESCAPEMENT_TYPE = 74;

	private const int OFFSET_UNDERLINE_TYPE = 76;

	private const int OFFSET_FONT_COLOR_INDEX = 80;

	private const int OFFSET_OPTION_FLAGS = 88;

	private const int OFFSET_ESCAPEMENT_TYPE_MODIFIED = 92;

	private const int OFFSET_UNDERLINE_TYPE_MODIFIED = 96;

	private const int OFFSET_FONT_WEIGHT_MODIFIED = 100;

	private const int OFFSET_NOT_USED1 = 104;

	private const int OFFSET_NOT_USED2 = 108;

	private const int OFFSET_NOT_USED3 = 112;

	private const int OFFSET_FONT_FORMATING_END = 116;

	private const int RAW_DATA_SIZE = 118;

	public const int FONT_CELL_HEIGHT_PRESERVED = -1;

	private static BitField posture = BitFieldFactory.GetInstance(2);

	private static BitField outline = BitFieldFactory.GetInstance(8);

	private static BitField shadow = BitFieldFactory.GetInstance(16);

	private static BitField cancellation = BitFieldFactory.GetInstance(128);

	private static BitField styleModified = BitFieldFactory.GetInstance(2);

	private static BitField outlineModified = BitFieldFactory.GetInstance(8);

	private static BitField shadowModified = BitFieldFactory.GetInstance(16);

	private static BitField cancellationModified = BitFieldFactory.GetInstance(128);

	private const short FONT_WEIGHT_NORMAL = 400;

	private const short FONT_WEIGHT_BOLD = 700;

	public byte[] RawRecord => _rawData;

	public int DataLength => 118;

	public int FontHeight
	{
		get
		{
			return GetInt(64);
		}
		set
		{
			SetInt(64, value);
		}
	}

	public bool IsItalic
	{
		get
		{
			return GetFontOption(posture);
		}
		set
		{
			SetFontOption(value, posture);
		}
	}

	public bool IsOutlineOn
	{
		get
		{
			return GetFontOption(outline);
		}
		set
		{
			SetFontOption(value, outline);
		}
	}

	public bool IsShadowOn
	{
		get
		{
			return GetFontOption(shadow);
		}
		set
		{
			SetFontOption(value, shadow);
		}
	}

	public bool IsStruckout
	{
		get
		{
			return GetFontOption(cancellation);
		}
		set
		{
			SetFontOption(value, cancellation);
		}
	}

	public short FontWeight
	{
		get
		{
			return GetShort(72);
		}
		set
		{
			short num = value;
			if (num < 100)
			{
				num = 100;
			}
			if (num > 1000)
			{
				num = 1000;
			}
			SetShort(72, num);
		}
	}

	public bool IsBold
	{
		get
		{
			return FontWeight == 700;
		}
		set
		{
			FontWeight = (short)(value ? 700 : 400);
		}
	}

	public FontSuperScript EscapementType
	{
		get
		{
			return (FontSuperScript)GetShort(74);
		}
		set
		{
			SetShort(74, (int)value);
		}
	}

	public FontUnderlineType UnderlineType
	{
		get
		{
			return (FontUnderlineType)GetShort(76);
		}
		set
		{
			SetShort(76, (int)value);
		}
	}

	public short FontColorIndex
	{
		get
		{
			return (short)GetInt(80);
		}
		set
		{
			SetInt(80, value);
		}
	}

	public bool IsFontStyleModified
	{
		get
		{
			return GetOptionFlag(styleModified);
		}
		set
		{
			SetOptionFlag(value, styleModified);
		}
	}

	public bool IsFontOutlineModified
	{
		get
		{
			return GetOptionFlag(outlineModified);
		}
		set
		{
			SetOptionFlag(value, outlineModified);
		}
	}

	public bool IsFontShadowModified
	{
		get
		{
			return GetOptionFlag(shadowModified);
		}
		set
		{
			SetOptionFlag(value, shadowModified);
		}
	}

	public bool IsFontCancellationModified
	{
		get
		{
			return GetOptionFlag(cancellationModified);
		}
		set
		{
			SetOptionFlag(value, cancellationModified);
		}
	}

	public bool IsEscapementTypeModified
	{
		get
		{
			return GetInt(92) == 0;
		}
		set
		{
			int value2 = ((!value) ? 1 : 0);
			SetInt(92, value2);
		}
	}

	public bool IsUnderlineTypeModified
	{
		get
		{
			return GetInt(96) == 0;
		}
		set
		{
			int value2 = ((!value) ? 1 : 0);
			SetInt(96, value2);
		}
	}

	public bool IsFontWeightModified
	{
		get
		{
			return GetInt(100) == 0;
		}
		set
		{
			int value2 = ((!value) ? 1 : 0);
			SetInt(100, value2);
		}
	}

	public FontFormatting()
	{
		FontHeight = -1;
		IsItalic = false;
		IsFontWeightModified = false;
		IsOutlineOn = false;
		IsShadowOn = false;
		IsStruckout = false;
		EscapementType = FontSuperScript.None;
		UnderlineType = FontUnderlineType.None;
		FontColorIndex = -1;
		IsFontStyleModified = false;
		IsFontOutlineModified = false;
		IsFontShadowModified = false;
		IsFontCancellationModified = false;
		IsEscapementTypeModified = false;
		IsUnderlineTypeModified = false;
		SetShort(0, 0);
		SetInt(104, 1);
		SetInt(108, 0);
		SetInt(112, int.MaxValue);
		SetShort(116, 1);
	}

	public FontFormatting(RecordInputStream in1)
	{
		for (int i = 0; i < _rawData.Length; i++)
		{
			_rawData[i] = (byte)in1.ReadByte();
		}
	}

	private short GetShort(int offset)
	{
		return LittleEndian.GetShort(_rawData, offset);
	}

	private void SetShort(int offset, int value)
	{
		LittleEndian.PutShort(_rawData, offset, (short)value);
	}

	private int GetInt(int offset)
	{
		return LittleEndian.GetInt(_rawData, offset);
	}

	private void SetInt(int offset, int value)
	{
		LittleEndian.PutInt(_rawData, offset, value);
	}

	private void SetFontOption(bool option, BitField field)
	{
		int holder = GetInt(68);
		holder = field.SetBoolean(holder, option);
		SetInt(68, holder);
	}

	private bool GetFontOption(BitField field)
	{
		int holder = GetInt(68);
		return field.IsSet(holder);
	}

	private bool GetOptionFlag(BitField field)
	{
		int holder = GetInt(88);
		if (field.GetValue(holder) != 0)
		{
			return false;
		}
		return true;
	}

	private void SetOptionFlag(bool modified, BitField field)
	{
		int value = ((!modified) ? 1 : 0);
		int holder = GetInt(88);
		holder = field.SetValue(holder, value);
		SetInt(88, holder);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("    [Font Formatting]\n");
		stringBuilder.Append("\t.font height = ").Append(FontHeight).Append(" twips\n");
		if (IsFontStyleModified)
		{
			stringBuilder.Append("\t.font posture = ").Append(IsItalic ? "Italic" : "Normal").Append("\n");
		}
		else
		{
			stringBuilder.Append("\t.font posture = ]not modified]").Append("\n");
		}
		if (IsFontOutlineModified)
		{
			stringBuilder.Append("\t.font outline = ").Append(IsOutlineOn).Append("\n");
		}
		else
		{
			stringBuilder.Append("\t.font outline Is not modified\n");
		}
		if (IsFontShadowModified)
		{
			stringBuilder.Append("\t.font shadow = ").Append(IsShadowOn).Append("\n");
		}
		else
		{
			stringBuilder.Append("\t.font shadow Is not modified\n");
		}
		if (IsFontCancellationModified)
		{
			stringBuilder.Append("\t.font strikeout = ").Append(IsStruckout).Append("\n");
		}
		else
		{
			stringBuilder.Append("\t.font strikeout Is not modified\n");
		}
		if (IsFontStyleModified)
		{
			stringBuilder.Append("\t.font weight = ").Append(FontWeight).Append((FontWeight == 400) ? "(Normal)" : ((FontWeight == 700) ? "(Bold)" : ("0x" + StringUtil.ToHexString(FontWeight))))
				.Append("\n");
		}
		else
		{
			stringBuilder.Append("\t.font weight = ]not modified]").Append("\n");
		}
		if (IsEscapementTypeModified)
		{
			stringBuilder.Append("\t.escapement type = ").Append(EscapementType).Append("\n");
		}
		else
		{
			stringBuilder.Append("\t.escapement type Is not modified\n");
		}
		if (IsUnderlineTypeModified)
		{
			stringBuilder.Append("\t.underline type = ").Append(UnderlineType).Append("\n");
		}
		else
		{
			stringBuilder.Append("\t.underline type Is not modified\n");
		}
		stringBuilder.Append("\t.color index = ").Append("0x" + StringUtil.ToHexString(FontColorIndex).ToUpper()).Append("\n");
		stringBuilder.Append("    [/Font Formatting]\n");
		return stringBuilder.ToString();
	}

	public object Clone()
	{
		FontFormatting fontFormatting = new FontFormatting();
		Array.Copy(_rawData, 0, fontFormatting._rawData, 0, _rawData.Length);
		return fontFormatting;
	}
}
