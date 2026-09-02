using System;
using System.Text;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class DVRecord : StandardRecord, ICloneable
{
	private static readonly UnicodeString NULL_TEXT_STRING = new UnicodeString("\0");

	public const short sid = 446;

	private int _option_flags;

	private UnicodeString _promptTitle;

	private UnicodeString _errorTitle;

	private UnicodeString _promptText;

	private UnicodeString _errorText;

	private short _not_used_1 = 16352;

	private Formula _formula1;

	private short _not_used_2;

	private Formula _formula2;

	private CellRangeAddressList _regions;

	public const int STRING_PROMPT_TITLE = 0;

	public const int STRING_ERROR_TITLE = 1;

	public const int STRING_PROMPT_TEXT = 2;

	public const int STRING_ERROR_TEXT = 3;

	private BitField opt_data_type = new BitField(15);

	private BitField opt_error_style = new BitField(112);

	private BitField opt_string_list_formula = new BitField(128);

	private BitField opt_empty_cell_allowed = new BitField(256);

	private BitField opt_suppress_dropdown_arrow = new BitField(512);

	private BitField opt_show_prompt_on_cell_selected = new BitField(262144);

	private BitField opt_show_error_on_invalid_value = new BitField(524288);

	private BitField opt_condition_operator = new BitField(15728640);

	public int DataType
	{
		get
		{
			return opt_data_type.GetValue(_option_flags);
		}
		set
		{
			_option_flags = opt_data_type.SetValue(_option_flags, value);
		}
	}

	public int ErrorStyle
	{
		get
		{
			return opt_error_style.GetValue(_option_flags);
		}
		set
		{
			_option_flags = opt_error_style.SetValue(_option_flags, value);
		}
	}

	public bool ListExplicitFormula
	{
		get
		{
			return opt_string_list_formula.IsSet(_option_flags);
		}
		set
		{
			_option_flags = opt_string_list_formula.SetBoolean(_option_flags, value);
		}
	}

	public bool EmptyCellAllowed
	{
		get
		{
			return opt_empty_cell_allowed.IsSet(_option_flags);
		}
		set
		{
			_option_flags = opt_empty_cell_allowed.SetBoolean(_option_flags, value);
		}
	}

	public bool SuppressDropdownArrow => opt_suppress_dropdown_arrow.IsSet(_option_flags);

	public bool ShowPromptOnCellSelected => opt_show_prompt_on_cell_selected.IsSet(_option_flags);

	public bool ShowErrorOnInvalidValue
	{
		get
		{
			return opt_show_error_on_invalid_value.IsSet(_option_flags);
		}
		set
		{
			_option_flags = opt_show_error_on_invalid_value.SetBoolean(_option_flags, value);
		}
	}

	public int ConditionOperator
	{
		get
		{
			return opt_condition_operator.GetValue(_option_flags);
		}
		set
		{
			_option_flags = opt_condition_operator.SetValue(_option_flags, value);
		}
	}

	public string PromptTitle => ResolveTitleString(_promptTitle);

	public string ErrorTitle => ResolveTitleString(_errorTitle);

	public string PromptText => ResolveTitleString(_promptText);

	public string ErrorText => ResolveTitleString(_errorText);

	public Ptg[] Formula1 => Formula.GetTokens(_formula1);

	public Ptg[] Formula2 => Formula.GetTokens(_formula2);

	public CellRangeAddressList CellRangeAddress
	{
		get
		{
			return _regions;
		}
		set
		{
			_regions = value;
		}
	}

	public int OptionFlags => _option_flags;

	protected override int DataSize => 12 + GetUnicodeStringSize(_promptTitle) + GetUnicodeStringSize(_errorTitle) + GetUnicodeStringSize(_promptText) + GetUnicodeStringSize(_errorText) + _formula1.EncodedTokenSize + _formula2.EncodedTokenSize + _regions.Size;

	public override short Sid => 446;

	public DVRecord()
	{
	}

	public DVRecord(int validationType, int operator1, int errorStyle, bool emptyCellAllowed, bool suppressDropDownArrow, bool isExplicitList, bool showPromptBox, string promptTitle, string promptText, bool showErrorBox, string errorTitle, string errorText, Ptg[] formula1, Ptg[] formula2, CellRangeAddressList regions)
	{
		int holder = 0;
		holder = opt_data_type.SetValue(holder, validationType);
		holder = opt_condition_operator.SetValue(holder, operator1);
		holder = opt_error_style.SetValue(holder, errorStyle);
		holder = opt_empty_cell_allowed.SetBoolean(holder, emptyCellAllowed);
		holder = opt_suppress_dropdown_arrow.SetBoolean(holder, suppressDropDownArrow);
		holder = opt_string_list_formula.SetBoolean(holder, isExplicitList);
		holder = opt_show_prompt_on_cell_selected.SetBoolean(holder, showPromptBox);
		holder = opt_show_error_on_invalid_value.SetBoolean(holder, showErrorBox);
		_option_flags = holder;
		_promptTitle = ResolveTitleText(promptTitle);
		_promptText = ResolveTitleText(promptText);
		_errorTitle = ResolveTitleText(errorTitle);
		_errorText = ResolveTitleText(errorText);
		_formula1 = Formula.Create(formula1);
		_formula2 = Formula.Create(formula2);
		_regions = regions;
	}

	public DVRecord(RecordInputStream in1)
	{
		_option_flags = in1.ReadInt();
		_promptTitle = ReadUnicodeString(in1);
		_errorTitle = ReadUnicodeString(in1);
		_promptText = ReadUnicodeString(in1);
		_errorText = ReadUnicodeString(in1);
		int encodedTokenLen = in1.ReadUShort();
		_not_used_1 = in1.ReadShort();
		_formula1 = Formula.Read(encodedTokenLen, in1);
		int encodedTokenLen2 = in1.ReadUShort();
		_not_used_2 = in1.ReadShort();
		_formula2 = Formula.Read(encodedTokenLen2, in1);
		_regions = new CellRangeAddressList(in1);
	}

	private static UnicodeString ResolveTitleText(string str)
	{
		if (str == null || str.Length < 1)
		{
			return NULL_TEXT_STRING;
		}
		return new UnicodeString(str);
	}

	private static string ResolveTitleString(UnicodeString us)
	{
		if (us == null || us.Equals(NULL_TEXT_STRING))
		{
			return null;
		}
		return us.String;
	}

	private static UnicodeString ReadUnicodeString(RecordInputStream in1)
	{
		return new UnicodeString(in1);
	}

	public override string ToString()
	{
		return new StringBuilder().ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteInt(_option_flags);
		SerializeUnicodeString(_promptTitle, out1);
		SerializeUnicodeString(_errorTitle, out1);
		SerializeUnicodeString(_promptText, out1);
		SerializeUnicodeString(_errorText, out1);
		out1.WriteShort(_formula1.EncodedTokenSize);
		out1.WriteShort(_not_used_1);
		_formula1.SerializeTokens(out1);
		out1.WriteShort(_formula2.EncodedTokenSize);
		out1.WriteShort(_not_used_2);
		_formula2.SerializeTokens(out1);
		_regions.Serialize(out1);
	}

	private static void SerializeUnicodeString(UnicodeString us, ILittleEndianOutput out1)
	{
		StringUtil.WriteUnicodeString(out1, us.String);
	}

	private static int GetUnicodeStringSize(UnicodeString us)
	{
		string text = us.String;
		return 3 + text.Length * ((!StringUtil.HasMultibyte(text)) ? 1 : 2);
	}

	public override object Clone()
	{
		return CloneViaReserialise();
	}
}
