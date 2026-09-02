using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Streaming;

public class SXSSFCreationHelper : ICreationHelper
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(SXSSFCreationHelper));

	private SXSSFWorkbook wb;

	private XSSFCreationHelper helper;

	public SXSSFCreationHelper(SXSSFWorkbook workbook)
	{
		helper = new XSSFCreationHelper(workbook.XssfWorkbook);
		wb = workbook;
	}

	public IClientAnchor CreateClientAnchor()
	{
		return helper.CreateClientAnchor();
	}

	public IDataFormat CreateDataFormat()
	{
		return helper.CreateDataFormat();
	}

	public ExtendedColor CreateExtendedColor()
	{
		return helper.CreateExtendedColor();
	}

	public IFormulaEvaluator CreateFormulaEvaluator()
	{
		return new SXSSFFormulaEvaluator(wb);
	}

	public IHyperlink CreateHyperlink(HyperlinkType type)
	{
		return helper.CreateHyperlink(type);
	}

	public IRichTextString CreateRichTextString(string text)
	{
		logger.Log(3, "SXSSF doesn't support Rich Text Strings, any formatting information will be lost");
		return new XSSFRichTextString(text);
	}
}
