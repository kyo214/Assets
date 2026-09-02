using NPOI.HSSF.Record.Common;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFCreationHelper : ICreationHelper
{
	private HSSFWorkbook workbook;

	private HSSFDataFormat dataFormat;

	public HSSFCreationHelper(HSSFWorkbook wb)
	{
		workbook = wb;
		dataFormat = new HSSFDataFormat(workbook.Workbook);
	}

	public IRichTextString CreateRichTextString(string text)
	{
		return new HSSFRichTextString(text);
	}

	public IDataFormat CreateDataFormat()
	{
		return dataFormat;
	}

	public IHyperlink CreateHyperlink(HyperlinkType type)
	{
		return new HSSFHyperlink(type);
	}

	public IFormulaEvaluator CreateFormulaEvaluator()
	{
		return new HSSFFormulaEvaluator(workbook);
	}

	public IClientAnchor CreateClientAnchor()
	{
		return new HSSFClientAnchor();
	}

	public NPOI.SS.UserModel.ExtendedColor CreateExtendedColor()
	{
		return new HSSFExtendedColor(new NPOI.HSSF.Record.Common.ExtendedColor());
	}
}
