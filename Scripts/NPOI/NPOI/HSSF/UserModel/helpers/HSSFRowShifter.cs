using System;
using NPOI.SS.Formula;
using NPOI.SS.UserModel;
using NPOI.SS.UserModel.Helpers;
using NPOI.Util;

namespace NPOI.HSSF.UserModel.helpers;

public class HSSFRowShifter : RowShifter
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(HSSFRowShifter));

	public HSSFRowShifter(HSSFSheet sh)
		: base(sh)
	{
	}

	public override void UpdateNamedRanges(FormulaShifter Shifter)
	{
		throw new NotImplementedException("HSSFRowShifter.updateNamedRanges");
	}

	public override void UpdateFormulas(FormulaShifter Shifter)
	{
		throw new NotImplementedException("updateFormulas");
	}

	public override void UpdateRowFormulas(IRow row, FormulaShifter Shifter)
	{
		throw new NotImplementedException("updateRowFormulas");
	}

	public override void UpdateConditionalFormatting(FormulaShifter Shifter)
	{
		throw new NotImplementedException("updateConditionalFormatting");
	}

	public override void UpdateHyperlinks(FormulaShifter Shifter)
	{
		throw new NotImplementedException("updateHyperlinks");
	}
}
