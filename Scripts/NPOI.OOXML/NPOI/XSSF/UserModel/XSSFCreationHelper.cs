using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFCreationHelper : ICreationHelper
{
	private XSSFWorkbook workbook;

	public XSSFCreationHelper(XSSFWorkbook wb)
	{
		workbook = wb;
	}

	public IRichTextString CreateRichTextString(string text)
	{
		XSSFRichTextString xSSFRichTextString = new XSSFRichTextString(text);
		xSSFRichTextString.SetStylesTableReference(workbook.GetStylesSource());
		return xSSFRichTextString;
	}

	public IDataFormat CreateDataFormat()
	{
		return workbook.CreateDataFormat();
	}

	public IHyperlink CreateHyperlink(HyperlinkType type)
	{
		return new XSSFHyperlink(type);
	}

	public IFormulaEvaluator CreateFormulaEvaluator()
	{
		return new XSSFFormulaEvaluator(workbook);
	}

	public IClientAnchor CreateClientAnchor()
	{
		return new XSSFClientAnchor();
	}

	public ExtendedColor CreateExtendedColor()
	{
		return new XSSFColor();
	}
}
