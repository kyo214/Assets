using System;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.CF;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFConditionalFormattingRule : IConditionalFormattingRule
{
	private const byte CELL_COMPARISON = 1;

	private CFRuleBase cfRuleRecord;

	private HSSFWorkbook workbook;

	private HSSFSheet sheet;

	public CFRuleBase CfRuleRecord => cfRuleRecord;

	public IFontFormatting FontFormatting => GetFontFormatting(Create: false);

	public IBorderFormatting BorderFormatting => GetBorderFormatting(Create: false);

	public IPatternFormatting PatternFormatting => GetPatternFormatting(Create: false);

	public IDataBarFormatting DataBarFormatting => GetDataBarFormatting(create: false);

	public IIconMultiStateFormatting MultiStateFormatting => GetMultiStateFormatting(create: false);

	public IColorScaleFormatting ColorScaleFormatting => GetColorScaleFormatting(create: false);

	public ConditionType ConditionType => ConditionType.ForId(cfRuleRecord.ConditionType);

	public ComparisonOperator ComparisonOperation => (ComparisonOperator)cfRuleRecord.ComparisonOperation;

	public string Formula1 => ToFormulaString(cfRuleRecord.ParsedExpression1);

	public string Formula2
	{
		get
		{
			if (cfRuleRecord.ConditionType == 1)
			{
				byte comparisonOperation = cfRuleRecord.ComparisonOperation;
				if ((uint)(comparisonOperation - 1) <= 1u)
				{
					return ToFormulaString(cfRuleRecord.ParsedExpression2);
				}
			}
			return null;
		}
	}

	public HSSFConditionalFormattingRule(HSSFSheet pSheet, CFRuleBase pRuleRecord)
	{
		if (pSheet == null)
		{
			throw new ArgumentException("pSheet must not be null");
		}
		if (pRuleRecord == null)
		{
			throw new ArgumentException("pRuleRecord must not be null");
		}
		sheet = pSheet;
		workbook = pSheet.Workbook as HSSFWorkbook;
		cfRuleRecord = pRuleRecord;
	}

	private CFRule12Record GetCFRule12Record(bool create)
	{
		if (!(cfRuleRecord is CFRule12Record))
		{
			if (create)
			{
				throw new ArgumentException("Can't convert a CF into a CF12 record");
			}
			return null;
		}
		return (CFRule12Record)cfRuleRecord;
	}

	private HSSFFontFormatting GetFontFormatting(bool Create)
	{
		FontFormatting fontFormatting = cfRuleRecord.FontFormatting;
		if (fontFormatting != null)
		{
			cfRuleRecord.FontFormatting = fontFormatting;
			return new HSSFFontFormatting(cfRuleRecord, workbook);
		}
		if (Create)
		{
			fontFormatting = new FontFormatting();
			cfRuleRecord.FontFormatting = fontFormatting;
			return new HSSFFontFormatting(cfRuleRecord, workbook);
		}
		return null;
	}

	public IFontFormatting CreateFontFormatting()
	{
		return GetFontFormatting(Create: true);
	}

	private HSSFBorderFormatting GetBorderFormatting(bool Create)
	{
		BorderFormatting borderFormatting = cfRuleRecord.BorderFormatting;
		if (borderFormatting != null)
		{
			cfRuleRecord.BorderFormatting = borderFormatting;
			return new HSSFBorderFormatting(cfRuleRecord, workbook);
		}
		if (Create)
		{
			borderFormatting = new BorderFormatting();
			cfRuleRecord.BorderFormatting = borderFormatting;
			return new HSSFBorderFormatting(cfRuleRecord, workbook);
		}
		return null;
	}

	public IBorderFormatting CreateBorderFormatting()
	{
		return GetBorderFormatting(Create: true);
	}

	private HSSFPatternFormatting GetPatternFormatting(bool Create)
	{
		PatternFormatting patternFormatting = cfRuleRecord.PatternFormatting;
		if (patternFormatting != null)
		{
			cfRuleRecord.PatternFormatting = patternFormatting;
			return new HSSFPatternFormatting(cfRuleRecord, workbook);
		}
		if (Create)
		{
			patternFormatting = new PatternFormatting();
			cfRuleRecord.PatternFormatting = patternFormatting;
			return new HSSFPatternFormatting(cfRuleRecord, workbook);
		}
		return null;
	}

	public IPatternFormatting CreatePatternFormatting()
	{
		return GetPatternFormatting(Create: true);
	}

	private HSSFDataBarFormatting GetDataBarFormatting(bool create)
	{
		CFRule12Record cFRule12Record = GetCFRule12Record(create);
		if (cFRule12Record.DataBarFormatting != null)
		{
			return new HSSFDataBarFormatting(cFRule12Record, sheet);
		}
		if (create)
		{
			cFRule12Record.CreateDataBarFormatting();
			return new HSSFDataBarFormatting(cFRule12Record, sheet);
		}
		return null;
	}

	public HSSFDataBarFormatting CreateDataBarFormatting()
	{
		return GetDataBarFormatting(create: true);
	}

	private HSSFIconMultiStateFormatting GetMultiStateFormatting(bool create)
	{
		CFRule12Record cFRule12Record = GetCFRule12Record(create);
		if (cFRule12Record.MultiStateFormatting != null)
		{
			return new HSSFIconMultiStateFormatting(cFRule12Record, sheet);
		}
		if (create)
		{
			cFRule12Record.CreateMultiStateFormatting();
			return new HSSFIconMultiStateFormatting(cFRule12Record, sheet);
		}
		return null;
	}

	public HSSFIconMultiStateFormatting CreateMultiStateFormatting()
	{
		return GetMultiStateFormatting(create: true);
	}

	private HSSFColorScaleFormatting GetColorScaleFormatting(bool create)
	{
		CFRule12Record cFRule12Record = GetCFRule12Record(create);
		if (cFRule12Record.ColorGradientFormatting != null)
		{
			return new HSSFColorScaleFormatting(cFRule12Record, sheet);
		}
		if (create)
		{
			cFRule12Record.CreateColorGradientFormatting();
			return new HSSFColorScaleFormatting(cFRule12Record, sheet);
		}
		return null;
	}

	public HSSFColorScaleFormatting CreateColorScaleFormatting()
	{
		return GetColorScaleFormatting(create: true);
	}

	protected internal string ToFormulaString(Ptg[] ParsedExpression)
	{
		if (ParsedExpression == null)
		{
			return null;
		}
		return ToFormulaString(ParsedExpression, workbook);
	}

	protected internal static string ToFormulaString(Ptg[] parsedExpression, HSSFWorkbook workbook)
	{
		if (parsedExpression == null || parsedExpression.Length == 0)
		{
			return null;
		}
		return HSSFFormulaParser.ToFormulaString(workbook, parsedExpression);
	}
}
