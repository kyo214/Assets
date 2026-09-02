using NPOI.OpenXmlFormats.Dml.Spreadsheet;

namespace NPOI.XSSF.UserModel;

public class XSSFTextBox : XSSFSimpleShape
{
	internal XSSFTextBox(XSSFDrawing drawing, CT_Shape ctShape)
		: base(drawing, ctShape)
	{
	}
}
