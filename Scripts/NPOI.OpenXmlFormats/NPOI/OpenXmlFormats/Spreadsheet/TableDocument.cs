using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Spreadsheet;

public class TableDocument
{
	private CT_Table ctTable;

	public TableDocument()
	{
	}

	public TableDocument(CT_Table table)
	{
		ctTable = table;
	}

	public static TableDocument Parse(XmlDocument xmldoc, XmlNamespaceManager namespaceMgr)
	{
		return new TableDocument(CT_Table.Parse(xmldoc.DocumentElement, namespaceMgr));
	}

	public CT_Table GetTable()
	{
		return ctTable;
	}

	public void SetTable(CT_Table table)
	{
		ctTable = table;
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		ctTable.Write(sw);
	}
}
