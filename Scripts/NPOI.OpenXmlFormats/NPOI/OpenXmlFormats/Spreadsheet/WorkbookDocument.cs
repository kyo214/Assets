using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Spreadsheet;

public class WorkbookDocument
{
	private CT_Workbook workbook;

	public CT_Workbook Workbook => workbook;

	public WorkbookDocument()
	{
		workbook = new CT_Workbook();
	}

	public static WorkbookDocument Parse(XmlDocument xmlDoc, XmlNamespaceManager NameSpaceManager)
	{
		return new WorkbookDocument(CT_Workbook.Parse(xmlDoc.DocumentElement, NameSpaceManager));
	}

	public WorkbookDocument(CT_Workbook workbook)
	{
		this.workbook = workbook;
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		workbook.Write(sw);
	}
}
