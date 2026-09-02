using System;
using System.Collections.Generic;
using System.Text;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFTable : IBodyElement, ISDTContents
{
	public enum XWPFBorderType
	{
		NIL = 0,
		NONE = 1,
		SINGLE = 2,
		THICK = 3,
		DOUBLE = 4,
		DOTTED = 5,
		DASHED = 6,
		DOT_DASH = 7
	}

	protected StringBuilder text = new StringBuilder();

	private CT_Tbl ctTbl;

	protected List<XWPFTableRow> tableRows;

	internal static Dictionary<XWPFBorderType, ST_Border> xwpfBorderTypeMap;

	internal static Dictionary<ST_Border, XWPFBorderType> stBorderTypeMap;

	protected IBody part;

	public string Text => text.ToString();

	public int Width
	{
		get
		{
			CT_TblPr trPr = GetTrPr();
			if (!trPr.IsSetTblW())
			{
				return -1;
			}
			return int.Parse(trPr.tblW.w);
		}
		set
		{
			CT_TblPr trPr = GetTrPr();
			CT_TblWidth obj = (trPr.IsSetTblW() ? trPr.tblW : trPr.AddNewTblW());
			obj.w = value.ToString();
			obj.type = ST_TblWidth.pct;
		}
	}

	public int NumberOfRows => ctTbl.SizeOfTrArray();

	public string StyleID
	{
		get
		{
			string result = null;
			CT_TblPr tblPr = ctTbl.tblPr;
			if (tblPr != null)
			{
				CT_String tblStyle = tblPr.tblStyle;
				if (tblStyle != null)
				{
					result = tblStyle.val;
				}
			}
			return result;
		}
		set
		{
			CT_TblPr trPr = GetTrPr();
			CT_String cT_String = trPr.tblStyle;
			if (cT_String == null)
			{
				cT_String = trPr.AddNewTblStyle();
			}
			cT_String.val = value;
		}
	}

	public XWPFBorderType InsideHBorderType
	{
		get
		{
			XWPFBorderType result = XWPFBorderType.NONE;
			CT_TblPr trPr = GetTrPr();
			if (trPr.IsSetTblBorders())
			{
				CT_TblBorders tblBorders = trPr.tblBorders;
				if (tblBorders.IsSetInsideH())
				{
					CT_Border insideH = tblBorders.insideH;
					result = stBorderTypeMap[insideH.val];
				}
			}
			return result;
		}
	}

	public int InsideHBorderSize
	{
		get
		{
			int result = -1;
			CT_TblPr trPr = GetTrPr();
			if (trPr.IsSetTblBorders())
			{
				CT_TblBorders tblBorders = trPr.tblBorders;
				if (tblBorders.IsSetInsideH())
				{
					result = (int)tblBorders.insideH.sz.Value;
				}
			}
			return result;
		}
	}

	public int InsideHBorderSpace
	{
		get
		{
			int result = -1;
			CT_TblPr trPr = GetTrPr();
			if (trPr.IsSetTblBorders())
			{
				CT_TblBorders tblBorders = trPr.tblBorders;
				if (tblBorders.IsSetInsideH())
				{
					result = (int)tblBorders.insideH.space.Value;
				}
			}
			return result;
		}
	}

	public string InsideHBorderColor
	{
		get
		{
			string result = null;
			CT_TblPr trPr = GetTrPr();
			if (trPr.IsSetTblBorders())
			{
				CT_TblBorders tblBorders = trPr.tblBorders;
				if (tblBorders.IsSetInsideH())
				{
					result = tblBorders.insideH.color;
				}
			}
			return result;
		}
	}

	public XWPFBorderType InsideVBorderType
	{
		get
		{
			XWPFBorderType result = XWPFBorderType.NONE;
			CT_TblPr trPr = GetTrPr();
			if (trPr.IsSetTblBorders())
			{
				CT_TblBorders tblBorders = trPr.tblBorders;
				if (tblBorders.IsSetInsideV())
				{
					CT_Border insideV = tblBorders.insideV;
					result = stBorderTypeMap[insideV.val];
				}
			}
			return result;
		}
	}

	public int InsideVBorderSize
	{
		get
		{
			int result = -1;
			CT_TblPr trPr = GetTrPr();
			if (trPr.IsSetTblBorders())
			{
				CT_TblBorders tblBorders = trPr.tblBorders;
				if (tblBorders.IsSetInsideV())
				{
					result = (int)tblBorders.insideV.sz.Value;
				}
			}
			return result;
		}
	}

	public int InsideVBorderSpace
	{
		get
		{
			int result = -1;
			CT_TblPr trPr = GetTrPr();
			if (trPr.IsSetTblBorders())
			{
				CT_TblBorders tblBorders = trPr.tblBorders;
				if (tblBorders.IsSetInsideV())
				{
					result = (int)tblBorders.insideV.space.Value;
				}
			}
			return result;
		}
	}

	public string InsideVBorderColor
	{
		get
		{
			string result = null;
			CT_TblPr trPr = GetTrPr();
			if (trPr.IsSetTblBorders())
			{
				CT_TblBorders tblBorders = trPr.tblBorders;
				if (tblBorders.IsSetInsideV())
				{
					result = tblBorders.insideV.color;
				}
			}
			return result;
		}
	}

	public int RowBandSize
	{
		get
		{
			int result = 0;
			CT_TblPr trPr = GetTrPr();
			if (trPr.IsSetTblStyleRowBandSize())
			{
				int.TryParse(trPr.tblStyleRowBandSize.val, out result);
			}
			return result;
		}
		set
		{
			CT_TblPr trPr = GetTrPr();
			(trPr.IsSetTblStyleRowBandSize() ? trPr.tblStyleRowBandSize : trPr.AddNewTblStyleRowBandSize()).val = value.ToString();
		}
	}

	public int ColBandSize
	{
		get
		{
			int result = 0;
			CT_TblPr trPr = GetTrPr();
			if (trPr.IsSetTblStyleColBandSize())
			{
				int.TryParse(trPr.tblStyleColBandSize.val, out result);
			}
			return result;
		}
		set
		{
			CT_TblPr trPr = GetTrPr();
			(trPr.IsSetTblStyleColBandSize() ? trPr.tblStyleColBandSize : trPr.AddNewTblStyleColBandSize()).val = value.ToString();
		}
	}

	public int CellMarginTop
	{
		get
		{
			int result = 0;
			CT_TblCellMar tblCellMar = GetTrPr().tblCellMar;
			if (tblCellMar != null)
			{
				CT_TblWidth top = tblCellMar.top;
				if (top != null)
				{
					int.TryParse(top.w, out result);
				}
			}
			return result;
		}
	}

	public int CellMarginLeft
	{
		get
		{
			int result = 0;
			CT_TblCellMar tblCellMar = GetTrPr().tblCellMar;
			if (tblCellMar != null)
			{
				CT_TblWidth left = tblCellMar.left;
				if (left != null)
				{
					int.TryParse(left.w, out result);
				}
			}
			return result;
		}
	}

	public int CellMarginBottom
	{
		get
		{
			int result = 0;
			CT_TblCellMar tblCellMar = GetTrPr().tblCellMar;
			if (tblCellMar != null)
			{
				CT_TblWidth bottom = tblCellMar.bottom;
				if (bottom != null)
				{
					int.TryParse(bottom.w, out result);
				}
			}
			return result;
		}
	}

	public int CellMarginRight
	{
		get
		{
			int result = 0;
			CT_TblCellMar tblCellMar = GetTrPr().tblCellMar;
			if (tblCellMar != null)
			{
				CT_TblWidth right = tblCellMar.right;
				if (right != null)
				{
					int.TryParse(right.w, out result);
				}
			}
			return result;
		}
	}

	public string TableCaption
	{
		get
		{
			CT_TblPr trPr = GetTrPr();
			if (trPr.tblCaption != null)
			{
				return trPr.tblCaption.val;
			}
			return string.Empty;
		}
		set
		{
			CT_TblPr trPr = GetTrPr();
			if (trPr.tblCaption == null)
			{
				CT_String cT_String = new CT_String();
				cT_String.val = value;
				trPr.tblCaption = cT_String;
			}
			else
			{
				trPr.tblCaption.val = value;
			}
		}
	}

	public string TableDescription
	{
		get
		{
			CT_TblPr trPr = GetTrPr();
			if (trPr.tblDescription != null)
			{
				return trPr.tblDescription.val;
			}
			return string.Empty;
		}
		set
		{
			CT_TblPr trPr = GetTrPr();
			if (trPr.tblDescription == null)
			{
				CT_String cT_String = new CT_String();
				cT_String.val = value;
				trPr.tblDescription = cT_String;
			}
			else
			{
				trPr.tblDescription.val = value;
			}
		}
	}

	public List<XWPFTableRow> Rows => tableRows;

	public BodyElementType ElementType => BodyElementType.TABLE;

	public IBody Body => part;

	public POIXMLDocumentPart Part
	{
		get
		{
			if (part != null)
			{
				return part.Part;
			}
			return null;
		}
	}

	public BodyType PartType => part.PartType;

	static XWPFTable()
	{
		xwpfBorderTypeMap = new Dictionary<XWPFBorderType, ST_Border>();
		xwpfBorderTypeMap.Add(XWPFBorderType.NIL, ST_Border.nil);
		xwpfBorderTypeMap.Add(XWPFBorderType.NONE, ST_Border.none);
		xwpfBorderTypeMap.Add(XWPFBorderType.SINGLE, ST_Border.single);
		xwpfBorderTypeMap.Add(XWPFBorderType.THICK, ST_Border.thick);
		xwpfBorderTypeMap.Add(XWPFBorderType.DOUBLE, ST_Border.@double);
		xwpfBorderTypeMap.Add(XWPFBorderType.DOTTED, ST_Border.dotted);
		xwpfBorderTypeMap.Add(XWPFBorderType.DASHED, ST_Border.dashed);
		xwpfBorderTypeMap.Add(XWPFBorderType.DOT_DASH, ST_Border.dotDash);
		stBorderTypeMap = new Dictionary<ST_Border, XWPFBorderType>();
		stBorderTypeMap.Add(ST_Border.nil, XWPFBorderType.NIL);
		stBorderTypeMap.Add(ST_Border.none, XWPFBorderType.NONE);
		stBorderTypeMap.Add(ST_Border.single, XWPFBorderType.SINGLE);
		stBorderTypeMap.Add(ST_Border.thick, XWPFBorderType.THICK);
		stBorderTypeMap.Add(ST_Border.@double, XWPFBorderType.DOUBLE);
		stBorderTypeMap.Add(ST_Border.dotted, XWPFBorderType.DOTTED);
		stBorderTypeMap.Add(ST_Border.dashed, XWPFBorderType.DASHED);
		stBorderTypeMap.Add(ST_Border.dotDash, XWPFBorderType.DOT_DASH);
	}

	public XWPFTable(CT_Tbl table, IBody part, int row, int col)
		: this(table, part)
	{
		CT_TblGrid cT_TblGrid = table.AddNewTblGrid();
		for (int i = 0; i < col; i++)
		{
			cT_TblGrid.AddNewGridCol().w = 300uL;
		}
		for (int j = 0; j < row; j++)
		{
			XWPFTableRow xWPFTableRow = ((GetRow(j) == null) ? CreateRow() : GetRow(j));
			for (int k = 0; k < col; k++)
			{
				if (xWPFTableRow.GetCell(k) == null)
				{
					xWPFTableRow.CreateCell();
				}
			}
		}
	}

	public void SetColumnWidth(int columnIndex, ulong width)
	{
		if (ctTbl.tblGrid != null)
		{
			if (columnIndex > ctTbl.tblGrid.gridCol.Count)
			{
				throw new ArgumentOutOfRangeException($"Column index {columnIndex} doesn't exist.");
			}
			ctTbl.tblGrid.gridCol[columnIndex].w = width;
		}
	}

	public XWPFTable(CT_Tbl table, IBody part)
	{
		this.part = part;
		ctTbl = table;
		tableRows = new List<XWPFTableRow>();
		if (table.SizeOfTrArray() == 0)
		{
			CreateEmptyTable(table);
		}
		foreach (CT_Row tr in table.GetTrList())
		{
			StringBuilder stringBuilder = new StringBuilder();
			tr.Table = table;
			XWPFTableRow item = new XWPFTableRow(tr, this);
			tableRows.Add(item);
			foreach (CT_Tc tc in tr.GetTcList())
			{
				foreach (CT_P p in tc.GetPList())
				{
					XWPFParagraph xWPFParagraph = new XWPFParagraph(p, part);
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append('\t');
					}
					stringBuilder.Append(xWPFParagraph.Text);
				}
			}
			if (stringBuilder.Length > 0)
			{
				text.Append((object)stringBuilder);
				text.Append('\n');
			}
		}
	}

	private void CreateEmptyTable(CT_Tbl table)
	{
		table.AddNewTr().AddNewTc().AddNewP();
		CT_TblPr cT_TblPr = table.AddNewTblPr();
		if (!cT_TblPr.IsSetTblW())
		{
			cT_TblPr.AddNewTblW().w = "0";
		}
		cT_TblPr.tblW.type = ST_TblWidth.auto;
		cT_TblPr.AddNewTblLayout().type = ST_TblLayoutType.autofit;
		CT_TblBorders cT_TblBorders = cT_TblPr.AddNewTblBorders();
		cT_TblBorders.AddNewBottom().val = ST_Border.single;
		cT_TblBorders.AddNewInsideH().val = ST_Border.single;
		cT_TblBorders.AddNewInsideV().val = ST_Border.single;
		cT_TblBorders.AddNewLeft().val = ST_Border.single;
		cT_TblBorders.AddNewRight().val = ST_Border.single;
		cT_TblBorders.AddNewTop().val = ST_Border.single;
	}

	public CT_Tbl GetCTTbl()
	{
		return ctTbl;
	}

	public void AddNewRowBetween(int start, int end)
	{
		throw new NotImplementedException();
	}

	public void AddNewCol()
	{
		if (ctTbl.SizeOfTrArray() == 0)
		{
			CreateRow();
		}
		for (int i = 0; i < ctTbl.SizeOfTrArray(); i++)
		{
			new XWPFTableRow(ctTbl.GetTrArray(i), this).CreateCell();
		}
	}

	public XWPFTableRow CreateRow()
	{
		int sizeCol = ((ctTbl.SizeOfTrArray() > 0) ? ctTbl.GetTrArray(0).SizeOfTcArray() : 0);
		XWPFTableRow xWPFTableRow = new XWPFTableRow(ctTbl.AddNewTr(), this);
		AddColumn(xWPFTableRow, sizeCol);
		tableRows.Add(xWPFTableRow);
		return xWPFTableRow;
	}

	public XWPFTableRow GetRow(int pos)
	{
		if (pos >= 0 && pos < ctTbl.SizeOfTrArray())
		{
			return Rows[pos];
		}
		return null;
	}

	public CT_TblPr GetTrPr()
	{
		if (ctTbl.tblPr == null)
		{
			return ctTbl.AddNewTblPr();
		}
		return ctTbl.tblPr;
	}

	private void AddColumn(XWPFTableRow tabRow, int sizeCol)
	{
		if (sizeCol > 0)
		{
			for (int i = 0; i < sizeCol; i++)
			{
				tabRow.CreateCell();
			}
		}
	}

	public void SetTopBorder(XWPFBorderType type, int size, int space, string rgbColor)
	{
		CT_TblPr trPr = GetTrPr();
		CT_TblBorders cT_TblBorders = (trPr.IsSetTblBorders() ? trPr.tblBorders : trPr.AddNewTblBorders());
		CT_Border obj = ((cT_TblBorders.top != null) ? cT_TblBorders.top : cT_TblBorders.AddNewTop());
		obj.val = xwpfBorderTypeMap[type];
		obj.sz = (ulong)size;
		obj.space = (ulong)space;
		obj.color = rgbColor;
	}

	public void SetBottomBorder(XWPFBorderType type, int size, int space, string rgbColor)
	{
		CT_TblPr trPr = GetTrPr();
		CT_TblBorders cT_TblBorders = (trPr.IsSetTblBorders() ? trPr.tblBorders : trPr.AddNewTblBorders());
		CT_Border obj = ((cT_TblBorders.bottom != null) ? cT_TblBorders.bottom : cT_TblBorders.AddNewBottom());
		obj.val = xwpfBorderTypeMap[type];
		obj.sz = (ulong)size;
		obj.space = (ulong)space;
		obj.color = rgbColor;
	}

	public void SetLeftBorder(XWPFBorderType type, int size, int space, string rgbColor)
	{
		CT_TblPr trPr = GetTrPr();
		CT_TblBorders cT_TblBorders = (trPr.IsSetTblBorders() ? trPr.tblBorders : trPr.AddNewTblBorders());
		CT_Border obj = ((cT_TblBorders.left != null) ? cT_TblBorders.left : cT_TblBorders.AddNewLeft());
		obj.val = xwpfBorderTypeMap[type];
		obj.sz = (ulong)size;
		obj.space = (ulong)space;
		obj.color = rgbColor;
	}

	public void SetRightBorder(XWPFBorderType type, int size, int space, string rgbColor)
	{
		CT_TblPr trPr = GetTrPr();
		CT_TblBorders cT_TblBorders = (trPr.IsSetTblBorders() ? trPr.tblBorders : trPr.AddNewTblBorders());
		CT_Border obj = ((cT_TblBorders.right != null) ? cT_TblBorders.right : cT_TblBorders.AddNewRight());
		obj.val = xwpfBorderTypeMap[type];
		obj.sz = (ulong)size;
		obj.space = (ulong)space;
		obj.color = rgbColor;
	}

	public void SetInsideHBorder(XWPFBorderType type, int size, int space, string rgbColor)
	{
		CT_TblPr trPr = GetTrPr();
		CT_TblBorders cT_TblBorders = (trPr.IsSetTblBorders() ? trPr.tblBorders : trPr.AddNewTblBorders());
		CT_Border obj = (cT_TblBorders.IsSetInsideH() ? cT_TblBorders.insideH : cT_TblBorders.AddNewInsideH());
		obj.val = xwpfBorderTypeMap[type];
		obj.sz = (ulong)size;
		obj.space = (ulong)space;
		obj.color = rgbColor;
	}

	public void SetInsideVBorder(XWPFBorderType type, int size, int space, string rgbColor)
	{
		CT_TblPr trPr = GetTrPr();
		CT_TblBorders cT_TblBorders = (trPr.IsSetTblBorders() ? trPr.tblBorders : trPr.AddNewTblBorders());
		CT_Border obj = (cT_TblBorders.IsSetInsideV() ? cT_TblBorders.insideV : cT_TblBorders.AddNewInsideV());
		obj.val = xwpfBorderTypeMap[type];
		obj.sz = (ulong)size;
		obj.space = (ulong)space;
		obj.color = rgbColor;
	}

	public void SetCellMargins(int top, int left, int bottom, int right)
	{
		CT_TblPr trPr = GetTrPr();
		CT_TblCellMar cT_TblCellMar = (trPr.IsSetTblCellMar() ? trPr.tblCellMar : trPr.AddNewTblCellMar());
		CT_TblWidth obj = (cT_TblCellMar.IsSetLeft() ? cT_TblCellMar.left : cT_TblCellMar.AddNewLeft());
		obj.type = ST_TblWidth.dxa;
		obj.w = left.ToString();
		CT_TblWidth obj2 = (cT_TblCellMar.IsSetTop() ? cT_TblCellMar.top : cT_TblCellMar.AddNewTop());
		obj2.type = ST_TblWidth.dxa;
		obj2.w = top.ToString();
		CT_TblWidth obj3 = (cT_TblCellMar.IsSetBottom() ? cT_TblCellMar.bottom : cT_TblCellMar.AddNewBottom());
		obj3.type = ST_TblWidth.dxa;
		obj3.w = bottom.ToString();
		CT_TblWidth obj4 = (cT_TblCellMar.IsSetRight() ? cT_TblCellMar.right : cT_TblCellMar.AddNewRight());
		obj4.type = ST_TblWidth.dxa;
		obj4.w = right.ToString();
	}

	public void AddRow(XWPFTableRow row)
	{
		ctTbl.AddNewTr();
		ctTbl.SetTrArray(NumberOfRows - 1, row.GetCTRow());
		tableRows.Add(row);
	}

	public bool AddRow(XWPFTableRow row, int pos)
	{
		if (pos >= 0 && pos <= tableRows.Count)
		{
			ctTbl.InsertNewTr(pos);
			ctTbl.SetTrArray(pos, row.GetCTRow());
			tableRows.Insert(pos, row);
			return true;
		}
		return false;
	}

	public XWPFTableRow InsertNewTableRow(int pos)
	{
		if (pos >= 0 && pos <= tableRows.Count)
		{
			XWPFTableRow xWPFTableRow = new XWPFTableRow(ctTbl.InsertNewTr(pos), this);
			tableRows.Insert(pos, xWPFTableRow);
			return xWPFTableRow;
		}
		return null;
	}

	public bool RemoveRow(int pos)
	{
		if (pos >= 0 && pos < tableRows.Count)
		{
			if (ctTbl.SizeOfTrArray() > 0)
			{
				ctTbl.RemoveTr(pos);
			}
			tableRows.RemoveAt(pos);
			return true;
		}
		return false;
	}

	public XWPFTableRow GetRow(CT_Row row)
	{
		for (int i = 0; i < Rows.Count; i++)
		{
			if (Rows[i].GetCTRow() == row)
			{
				return GetRow(i);
			}
		}
		return null;
	}
}
