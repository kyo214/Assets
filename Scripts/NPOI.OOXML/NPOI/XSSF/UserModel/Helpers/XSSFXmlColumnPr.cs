using System.Text;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace NPOI.XSSF.UserModel.Helpers;

public class XSSFXmlColumnPr
{
	private XSSFTable table;

	private CT_TableColumn ctTableColumn;

	private CT_XmlColumnPr ctXmlColumnPr;

	public XSSFXmlColumnPr(XSSFTable table, CT_TableColumn ctTableColum, CT_XmlColumnPr CT_XmlColumnPr)
	{
		this.table = table;
		ctTableColumn = ctTableColum;
		ctXmlColumnPr = CT_XmlColumnPr;
	}

	public long GetMapId()
	{
		return ctXmlColumnPr.mapId;
	}

	public string GetXPath()
	{
		return ctXmlColumnPr.xpath;
	}

	public long GetId()
	{
		return ctTableColumn.id;
	}

	public string GetLocalXPath()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = table.GetCommonXpath().Split(new char[1] { '/' }).Length - 1;
		string[] array = ctXmlColumnPr.xpath.Split(new char[1] { '/' });
		for (int i = num; i < array.Length; i++)
		{
			stringBuilder.Append("/" + array[i]);
		}
		return stringBuilder.ToString();
	}

	public ST_XmlDataType GetXmlDataType()
	{
		return ctXmlColumnPr.xmlDataType;
	}
}
