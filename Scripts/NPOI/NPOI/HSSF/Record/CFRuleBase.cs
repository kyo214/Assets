using System;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record.CF;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public abstract class CFRuleBase : StandardRecord, ICloneable
{
	public static class ComparisonOperator
	{
		public static byte NO_COMPARISON = 0;

		public static byte BETWEEN = 1;

		public static byte NOT_BETWEEN = 2;

		public static byte EQUAL = 3;

		public static byte NOT_EQUAL = 4;

		public static byte GT = 5;

		public static byte LT = 6;

		public static byte GE = 7;

		public static byte LE = 8;

		public static byte max_operator = 8;
	}

	private byte condition_type;

	public const byte CONDITION_TYPE_CELL_VALUE_IS = 1;

	public const byte CONDITION_TYPE_FORMULA = 2;

	public const byte CONDITION_TYPE_COLOR_SCALE = 3;

	public const byte CONDITION_TYPE_DATA_BAR = 4;

	public const byte CONDITION_TYPE_FILTER = 5;

	public const byte CONDITION_TYPE_ICON_SET = 6;

	private byte comparison_operator;

	public static int TEMPLATE_CELL_VALUE = 0;

	public static int TEMPLATE_FORMULA = 1;

	public static int TEMPLATE_COLOR_SCALE_FORMATTING = 2;

	public static int TEMPLATE_DATA_BAR_FORMATTING = 3;

	public static int TEMPLATE_ICON_SET_FORMATTING = 4;

	public static int TEMPLATE_FILTER = 5;

	public static int TEMPLATE_UNIQUE_VALUES = 7;

	public static int TEMPLATE_CONTAINS_TEXT = 8;

	public static int TEMPLATE_CONTAINS_BLANKS = 9;

	public static int TEMPLATE_CONTAINS_NO_BLANKS = 10;

	public static int TEMPLATE_CONTAINS_ERRORS = 11;

	public static int TEMPLATE_CONTAINS_NO_ERRORS = 12;

	public static int TEMPLATE_TODAY = 15;

	public static int TEMPLATE_TOMORROW = 16;

	public static int TEMPLATE_YESTERDAY = 17;

	public static int TEMPLATE_LAST_7_DAYS = 18;

	public static int TEMPLATE_LAST_MONTH = 19;

	public static int TEMPLATE_NEXT_MONTH = 20;

	public static int TEMPLATE_THIS_WEEK = 21;

	public static int TEMPLATE_NEXT_WEEK = 22;

	public static int TEMPLATE_LAST_WEEK = 23;

	public static int TEMPLATE_THIS_MONTH = 24;

	public static int TEMPLATE_ABOVE_AVERAGE = 25;

	public static int TEMPLATE_BELOW_AVERAGE = 26;

	public static int TEMPLATE_DUPLICATE_VALUES = 27;

	public static int TEMPLATE_ABOVE_OR_EQUAL_TO_AVERAGE = 29;

	public static int TEMPLATE_BELOW_OR_EQUAL_TO_AVERAGE = 30;

	internal static BitField modificationBits = bf(4194303L);

	internal static BitField alignHor = bf(1L);

	internal static BitField alignVer = bf(2L);

	internal static BitField alignWrap = bf(4L);

	internal static BitField alignRot = bf(8L);

	internal static BitField alignJustLast = bf(16L);

	internal static BitField alignIndent = bf(32L);

	internal static BitField alignShrin = bf(64L);

	internal static BitField mergeCell = bf(128L);

	internal static BitField protLocked = bf(256L);

	internal static BitField protHidden = bf(512L);

	internal static BitField bordLeft = bf(1024L);

	internal static BitField bordRight = bf(2048L);

	internal static BitField bordTop = bf(4096L);

	internal static BitField bordBot = bf(8192L);

	internal static BitField bordTlBr = bf(16384L);

	internal static BitField bordBlTr = bf(32768L);

	internal static BitField pattStyle = bf(65536L);

	internal static BitField pattCol = bf(131072L);

	internal static BitField pattBgCol = bf(262144L);

	internal static BitField notUsed2 = bf(3670016L);

	internal static BitField undocumented = bf(62914560L);

	internal static BitField fmtBlockBits = bf(2080374784L);

	internal static BitField font = bf(67108864L);

	internal static BitField align = bf(134217728L);

	internal static BitField bord = bf(268435456L);

	internal static BitField patt = bf(536870912L);

	internal static BitField prot = bf(1073741824L);

	internal static BitField alignTextDir = bf(2147483648L);

	protected int formatting_options;

	protected short formatting_not_used;

	protected FontFormatting _fontFormatting;

	protected BorderFormatting _borderFormatting;

	protected PatternFormatting _patternFormatting;

	private Formula formula1;

	private Formula formula2;

	public byte ConditionType
	{
		get
		{
			return condition_type;
		}
		set
		{
			if (this is CFRuleRecord && value != 1 && value != 2)
			{
				throw new ArgumentException("CFRuleRecord only accepts Value-Is and Formula types");
			}
			condition_type = value;
		}
	}

	public byte ComparisonOperation
	{
		get
		{
			return comparison_operator;
		}
		set
		{
			if (value < 0 || value > ComparisonOperator.max_operator)
			{
				throw new ArgumentException("Valid operators are only in the range 0 to " + ComparisonOperator.max_operator);
			}
			comparison_operator = value;
		}
	}

	public bool ContainsFontFormattingBlock => GetOptionFlag(font);

	public FontFormatting FontFormatting
	{
		get
		{
			if (ContainsFontFormattingBlock)
			{
				return _fontFormatting;
			}
			return null;
		}
		set
		{
			_fontFormatting = value;
			SetOptionFlag(value != null, font);
		}
	}

	public bool ContainsBorderFormattingBlock => GetOptionFlag(bord);

	public BorderFormatting BorderFormatting
	{
		get
		{
			if (ContainsBorderFormattingBlock)
			{
				return _borderFormatting;
			}
			return null;
		}
		set
		{
			_borderFormatting = value;
			SetOptionFlag(value != null, bord);
		}
	}

	public bool ContainsPatternFormattingBlock => GetOptionFlag(patt);

	public PatternFormatting PatternFormatting
	{
		get
		{
			if (ContainsPatternFormattingBlock)
			{
				return _patternFormatting;
			}
			return null;
		}
		set
		{
			_patternFormatting = value;
			SetOptionFlag(value != null, patt);
		}
	}

	public int Options => formatting_options;

	public bool IsLeftBorderModified
	{
		get
		{
			return IsModified(bordLeft);
		}
		set
		{
			SetModified(value, bordLeft);
		}
	}

	public bool IsRightBorderModified
	{
		get
		{
			return IsModified(bordRight);
		}
		set
		{
			SetModified(value, bordRight);
		}
	}

	public bool IsTopBorderModified
	{
		get
		{
			return IsModified(bordTop);
		}
		set
		{
			SetModified(value, bordTop);
		}
	}

	public bool IsBottomBorderModified
	{
		get
		{
			return IsModified(bordBot);
		}
		set
		{
			SetModified(value, bordBot);
		}
	}

	public bool IsTopLeftBottomRightBorderModified
	{
		get
		{
			return IsModified(bordTlBr);
		}
		set
		{
			SetModified(value, bordTlBr);
		}
	}

	public bool IsBottomLeftTopRightBorderModified
	{
		get
		{
			return IsModified(bordBlTr);
		}
		set
		{
			SetModified(value, bordBlTr);
		}
	}

	public bool IsPatternStyleModified
	{
		get
		{
			return IsModified(pattStyle);
		}
		set
		{
			SetModified(value, pattStyle);
		}
	}

	public bool IsPatternColorModified
	{
		get
		{
			return IsModified(pattCol);
		}
		set
		{
			SetModified(value, pattCol);
		}
	}

	public bool IsPatternBackgroundColorModified
	{
		get
		{
			return IsModified(pattBgCol);
		}
		set
		{
			SetModified(value, pattBgCol);
		}
	}

	protected int FormattingBlockSize => 6 + (ContainsFontFormattingBlock ? _fontFormatting.RawRecord.Length : 0) + (ContainsBorderFormattingBlock ? 8 : 0) + (ContainsPatternFormattingBlock ? 4 : 0);

	public Ptg[] ParsedExpression1
	{
		get
		{
			return formula1.Tokens;
		}
		set
		{
			formula1 = Formula.Create(value);
		}
	}

	protected Formula Formula1
	{
		get
		{
			return formula1;
		}
		set
		{
			formula1 = value;
		}
	}

	public Ptg[] ParsedExpression2
	{
		get
		{
			return formula2.Tokens;
		}
		set
		{
			formula2 = Formula.Create(value);
		}
	}

	protected Formula Formula2
	{
		get
		{
			return formula2;
		}
		set
		{
			formula2 = value;
		}
	}

	private static BitField bf(long i)
	{
		return BitFieldFactory.GetInstance((int)i);
	}

	protected CFRuleBase(byte conditionType, byte comparisonOperation)
	{
		ConditionType = conditionType;
		ComparisonOperation = comparisonOperation;
		formula1 = Formula.Create(Ptg.EMPTY_PTG_ARRAY);
		formula2 = Formula.Create(Ptg.EMPTY_PTG_ARRAY);
	}

	protected CFRuleBase(byte conditionType, byte comparisonOperation, Ptg[] formula1, Ptg[] formula2)
		: this(conditionType, comparisonOperation)
	{
		this.formula1 = Formula.Create(formula1);
		this.formula2 = Formula.Create(formula2);
	}

	protected CFRuleBase()
	{
	}

	protected int ReadFormatOptions(RecordInputStream in1)
	{
		formatting_options = in1.ReadInt();
		formatting_not_used = in1.ReadShort();
		int num = 6;
		if (ContainsFontFormattingBlock)
		{
			_fontFormatting = new FontFormatting(in1);
			num += _fontFormatting.DataLength;
		}
		if (ContainsBorderFormattingBlock)
		{
			_borderFormatting = new BorderFormatting(in1);
			num += _borderFormatting.DataLength;
		}
		if (ContainsPatternFormattingBlock)
		{
			_patternFormatting = new PatternFormatting(in1);
			num += _patternFormatting.DataLength;
		}
		return num;
	}

	public bool ContainsAlignFormattingBlock()
	{
		return GetOptionFlag(align);
	}

	public void SetAlignFormattingUnChanged()
	{
		SetOptionFlag(flag: false, align);
	}

	public bool ContainsProtectionFormattingBlock()
	{
		return GetOptionFlag(prot);
	}

	public void SetProtectionFormattingUnChanged()
	{
		SetOptionFlag(flag: false, prot);
	}

	private bool IsModified(BitField field)
	{
		return !field.IsSet(formatting_options);
	}

	private void SetModified(bool modified, BitField field)
	{
		formatting_options = field.SetBoolean(formatting_options, !modified);
	}

	private bool GetOptionFlag(BitField field)
	{
		return field.IsSet(formatting_options);
	}

	private void SetOptionFlag(bool flag, BitField field)
	{
		formatting_options = field.SetBoolean(formatting_options, flag);
	}

	protected void SerializeFormattingBlock(ILittleEndianOutput out1)
	{
		out1.WriteInt(formatting_options);
		out1.WriteShort(formatting_not_used);
		if (ContainsFontFormattingBlock)
		{
			byte[] rawRecord = _fontFormatting.RawRecord;
			out1.Write(rawRecord);
		}
		if (ContainsBorderFormattingBlock)
		{
			_borderFormatting.Serialize(out1);
		}
		if (ContainsPatternFormattingBlock)
		{
			_patternFormatting.Serialize(out1);
		}
	}

	protected static int GetFormulaSize(Formula formula)
	{
		return formula.EncodedTokenSize;
	}

	public static Ptg[] ParseFormula(string formula, HSSFSheet sheet)
	{
		if (formula == null)
		{
			return null;
		}
		int sheetIndex = sheet.Workbook.GetSheetIndex(sheet);
		return HSSFFormulaParser.Parse(formula, sheet.Workbook as HSSFWorkbook, FormulaType.Cell, sheetIndex);
	}

	protected void CopyTo(CFRuleBase rec)
	{
		rec.condition_type = condition_type;
		rec.comparison_operator = comparison_operator;
		rec.formatting_options = formatting_options;
		rec.formatting_not_used = formatting_not_used;
		if (ContainsFontFormattingBlock)
		{
			rec._fontFormatting = (FontFormatting)_fontFormatting.Clone();
		}
		if (ContainsBorderFormattingBlock)
		{
			rec._borderFormatting = (BorderFormatting)_borderFormatting.Clone();
		}
		if (ContainsPatternFormattingBlock)
		{
			rec._patternFormatting = (PatternFormatting)_patternFormatting.Clone();
		}
		rec.formula1 = formula1.Copy();
		rec.formula2 = formula2.Copy();
	}
}
