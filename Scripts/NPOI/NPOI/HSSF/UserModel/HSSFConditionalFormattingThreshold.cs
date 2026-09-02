using NPOI.HSSF.Record;
using NPOI.HSSF.Record.CF;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFConditionalFormattingThreshold : IConditionalFormattingThreshold
{
	private Threshold threshold;

	private HSSFSheet sheet;

	private HSSFWorkbook workbook;

	protected internal Threshold Threshold => threshold;

	public RangeType RangeType
	{
		get
		{
			return RangeType.ById(threshold.Type);
		}
		set
		{
			threshold.Type = (byte)value.id;
		}
	}

	public string Formula
	{
		get
		{
			return HSSFConditionalFormattingRule.ToFormulaString(threshold.ParsedExpression, workbook);
		}
		set
		{
			threshold.ParsedExpression = CFRuleBase.ParseFormula(value, sheet);
		}
	}

	public double? Value
	{
		get
		{
			return threshold.Value;
		}
		set
		{
			threshold.Value = value;
		}
	}

	protected internal HSSFConditionalFormattingThreshold(Threshold threshold, HSSFSheet sheet)
	{
		this.threshold = threshold;
		this.sheet = sheet;
		workbook = sheet.Workbook as HSSFWorkbook;
	}
}
