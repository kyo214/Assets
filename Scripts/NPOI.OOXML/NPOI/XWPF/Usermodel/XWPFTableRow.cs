using System;
using System.Collections.Generic;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFTableRow
{
	private CT_Row ctRow;

	private XWPFTable table;

	private List<XWPFTableCell> tableCells;

	public int Height
	{
		get
		{
			CT_TrPr trPr = GetTrPr();
			if (trPr.SizeOfTrHeightArray() != 0)
			{
				return (int)trPr.GetTrHeightArray(0).val;
			}
			return 0;
		}
		set
		{
			CT_TrPr trPr = GetTrPr();
			((trPr.SizeOfTrHeightArray() == 0) ? trPr.AddNewTrHeight() : trPr.GetTrHeightArray(0)).val = (ulong)value;
		}
	}

	public bool IsCantSplitRow
	{
		get
		{
			bool result = false;
			CT_TrPr trPr = GetTrPr();
			if (trPr.SizeOfCantSplitArray() > 0)
			{
				result = trPr.GetCantSplitList()[0].val;
			}
			return result;
		}
		set
		{
			GetTrPr().AddNewCantSplit().val = value;
		}
	}

	public bool IsRepeatHeader
	{
		get
		{
			bool result = false;
			CT_TrPr trPr = GetTrPr();
			if (trPr.SizeOfTblHeaderArray() > 0)
			{
				result = trPr.GetTblHeaderList()[0].val;
			}
			return result;
		}
		set
		{
			GetTrPr().AddNewTblHeader().val = value;
		}
	}

	public XWPFTableRow(CT_Row row, XWPFTable table)
	{
		this.table = table;
		ctRow = row;
		GetTableCells();
	}

	public CT_Row GetCTRow()
	{
		return ctRow;
	}

	public XWPFTableCell CreateCell()
	{
		XWPFTableCell xWPFTableCell = new XWPFTableCell(ctRow.AddNewTc(), this, table.Body);
		tableCells.Add(xWPFTableCell);
		return xWPFTableCell;
	}

	public void MergeCells(int startIndex, int endIndex)
	{
		if (startIndex >= endIndex)
		{
			throw new ArgumentOutOfRangeException("Start index must be smaller than end index");
		}
		if (startIndex < 0 || endIndex >= tableCells.Count)
		{
			throw new ArgumentOutOfRangeException("Invalid start index and end index");
		}
		XWPFTableCell cell = GetCell(startIndex);
		for (int num = endIndex; num > startIndex; num--)
		{
			RemoveCell(num);
		}
		if (!cell.GetCTTc().IsSetTcPr())
		{
			cell.GetCTTc().AddNewTcPr();
		}
		CT_TcPr tcPr = cell.GetCTTc().tcPr;
		if (tcPr.gridSpan == null)
		{
			tcPr.AddNewGridspan();
		}
		tcPr.gridSpan.val = (endIndex - startIndex + 1).ToString();
	}

	public XWPFTableCell GetCell(int pos)
	{
		if (pos >= 0 && pos < ctRow.SizeOfTcArray())
		{
			return GetTableCells()[pos];
		}
		return null;
	}

	public void RemoveCell(int pos)
	{
		if (pos >= 0 && pos < ctRow.SizeOfTcArray())
		{
			tableCells.RemoveAt(pos);
			ctRow.RemoveTc(pos);
		}
	}

	public XWPFTableCell AddNewTableCell()
	{
		XWPFTableCell xWPFTableCell = new XWPFTableCell(ctRow.AddNewTc(), this, table.Body);
		tableCells.Add(xWPFTableCell);
		return xWPFTableCell;
	}

	public XWPFTableRow CloneRow()
	{
		XWPFTableRow xWPFTableRow = new XWPFTableRow(ctRow.Copy(), table);
		table.AddRow(xWPFTableRow);
		return xWPFTableRow;
	}

	private CT_TrPr GetTrPr()
	{
		if (!ctRow.IsSetTrPr())
		{
			return ctRow.AddNewTrPr();
		}
		return ctRow.trPr;
	}

	public XWPFTable GetTable()
	{
		return table;
	}

	public List<ICell> GetTableICells()
	{
		List<ICell> list = new List<ICell>();
		foreach (object item in ctRow.Items)
		{
			if (item is CT_Tc)
			{
				list.Add(new XWPFTableCell((CT_Tc)item, this, table.Body));
			}
			else if (item is CT_SdtCell)
			{
				list.Add(new XWPFSDTCell((CT_SdtCell)item, this, table.Body));
			}
		}
		return list;
	}

	public List<XWPFTableCell> GetTableCells()
	{
		if (tableCells == null)
		{
			List<XWPFTableCell> list = new List<XWPFTableCell>();
			foreach (CT_Tc tc in ctRow.GetTcList())
			{
				list.Add(new XWPFTableCell(tc, this, table.Body));
			}
			tableCells = list;
		}
		return tableCells;
	}

	public XWPFTableCell GetTableCell(CT_Tc cell)
	{
		for (int i = 0; i < tableCells.Count; i++)
		{
			if (tableCells[i].GetCTTc() == cell)
			{
				return tableCells[i];
			}
		}
		return null;
	}
}
