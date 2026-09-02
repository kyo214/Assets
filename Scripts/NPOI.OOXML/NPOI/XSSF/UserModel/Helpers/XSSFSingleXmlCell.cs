using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.UserModel.Helpers;

public class XSSFSingleXmlCell
{
	private CT_SingleXmlCell SingleXmlCell;

	private SingleXmlCells parent;

	public XSSFSingleXmlCell(CT_SingleXmlCell SingleXmlCell, SingleXmlCells parent)
	{
		this.SingleXmlCell = SingleXmlCell;
		this.parent = parent;
	}

	public ICell GetReferencedCell()
	{
		ICell cell = null;
		CellReference cellReference = new CellReference(SingleXmlCell.r);
		IRow row = parent.GetXSSFSheet().GetRow(cellReference.Row);
		if (row == null)
		{
			row = parent.GetXSSFSheet().CreateRow(cellReference.Row);
		}
		cell = row.GetCell(cellReference.Col);
		if (cell == null)
		{
			cell = row.CreateCell(cellReference.Col);
		}
		return cell;
	}

	public string GetXpath()
	{
		return SingleXmlCell.xmlCellPr.xmlPr.xpath;
	}

	public long GetMapId()
	{
		return SingleXmlCell.xmlCellPr.xmlPr.mapId;
	}

	public ST_XmlDataType GetXmlDataType()
	{
		return SingleXmlCell.xmlCellPr.xmlPr.xmlDataType;
	}
}
