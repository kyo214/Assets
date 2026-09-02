using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Common;

public class FeatFormulaErr2 : SharedFeature
{
	private static BitField CHECK_CALCULATION_ERRORS = BitFieldFactory.GetInstance(1);

	private static BitField CHECK_EMPTY_CELL_REF = BitFieldFactory.GetInstance(2);

	private static BitField CHECK_NUMBERS_AS_TEXT = BitFieldFactory.GetInstance(4);

	private static BitField CHECK_INCONSISTENT_RANGES = BitFieldFactory.GetInstance(8);

	private static BitField CHECK_INCONSISTENT_FORMULAS = BitFieldFactory.GetInstance(16);

	private static BitField CHECK_DATETIME_FORMATS = BitFieldFactory.GetInstance(32);

	private static BitField CHECK_UNPROTECTED_FORMULAS = BitFieldFactory.GetInstance(64);

	private static BitField PERFORM_DATA_VALIDATION = BitFieldFactory.GetInstance(128);

	private int errorCheck;

	public int DataSize => 4;

	public int RawErrorCheckValue => errorCheck;

	public bool CheckCalculationErrors
	{
		get
		{
			return CHECK_CALCULATION_ERRORS.IsSet(errorCheck);
		}
		set
		{
			errorCheck = CHECK_CALCULATION_ERRORS.SetBoolean(errorCheck, value);
		}
	}

	public bool CheckEmptyCellRef
	{
		get
		{
			return CHECK_EMPTY_CELL_REF.IsSet(errorCheck);
		}
		set
		{
			errorCheck = CHECK_EMPTY_CELL_REF.SetBoolean(errorCheck, value);
		}
	}

	public bool CheckNumbersAsText
	{
		get
		{
			return CHECK_NUMBERS_AS_TEXT.IsSet(errorCheck);
		}
		set
		{
			errorCheck = CHECK_NUMBERS_AS_TEXT.SetBoolean(errorCheck, value);
		}
	}

	public bool CheckInconsistentRanges
	{
		get
		{
			return CHECK_INCONSISTENT_RANGES.IsSet(errorCheck);
		}
		set
		{
			errorCheck = CHECK_INCONSISTENT_RANGES.SetBoolean(errorCheck, value);
		}
	}

	public bool CheckInconsistentFormulas
	{
		get
		{
			return CHECK_INCONSISTENT_FORMULAS.IsSet(errorCheck);
		}
		set
		{
			errorCheck = CHECK_INCONSISTENT_FORMULAS.SetBoolean(errorCheck, value);
		}
	}

	public bool CheckDateTimeFormats
	{
		get
		{
			return CHECK_DATETIME_FORMATS.IsSet(errorCheck);
		}
		set
		{
			errorCheck = CHECK_DATETIME_FORMATS.SetBoolean(errorCheck, value);
		}
	}

	public bool CheckUnprotectedFormulas
	{
		get
		{
			return CHECK_UNPROTECTED_FORMULAS.IsSet(errorCheck);
		}
		set
		{
			errorCheck = CHECK_UNPROTECTED_FORMULAS.SetBoolean(errorCheck, value);
		}
	}

	public bool PerformDataValidation
	{
		get
		{
			return PERFORM_DATA_VALIDATION.IsSet(errorCheck);
		}
		set
		{
			errorCheck = PERFORM_DATA_VALIDATION.SetBoolean(errorCheck, value);
		}
	}

	public FeatFormulaErr2()
	{
	}

	public FeatFormulaErr2(RecordInputStream in1)
	{
		errorCheck = in1.ReadInt();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(" [FEATURE FORMULA ERRORS]\n");
		stringBuilder.Append("  checkCalculationErrors    = ");
		stringBuilder.Append("  checkEmptyCellRef         = ");
		stringBuilder.Append("  checkNumbersAsText        = ");
		stringBuilder.Append("  checkInconsistentRanges   = ");
		stringBuilder.Append("  checkInconsistentFormulas = ");
		stringBuilder.Append("  checkDateTimeFormats      = ");
		stringBuilder.Append("  checkUnprotectedFormulas  = ");
		stringBuilder.Append("  performDataValidation     = ");
		stringBuilder.Append(" [/FEATURE FORMULA ERRORS]\n");
		return stringBuilder.ToString();
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteInt(errorCheck);
	}
}
