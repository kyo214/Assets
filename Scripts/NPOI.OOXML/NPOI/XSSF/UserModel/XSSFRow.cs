using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS;
using NPOI.SS.Formula;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.Model;
using NPOI.XSSF.UserModel.Helpers;

namespace NPOI.XSSF.UserModel;

public class XSSFRow : IRow, IEnumerable<ICell>, IEnumerable, IComparable<XSSFRow>
{
	private static POILogger _logger = POILogFactory.GetLogger(typeof(XSSFRow));

	private CT_Row _row;

	private SortedDictionary<int, ICell> _cells;

	private XSSFSheet _sheet;

	public ISheet Sheet => _sheet;

	public short FirstCellNum => (short)((_cells.Count == 0) ? (-1) : GetFirstKey());

	public short LastCellNum => (short)((_cells.Count == 0) ? (-1) : (GetLastKey() + 1));

	public short Height
	{
		get
		{
			return (short)(HeightInPoints * 20f);
		}
		set
		{
			if (value < 0)
			{
				if (_row.IsSetHt())
				{
					_row.UnsetHt();
				}
				if (_row.IsSetCustomHeight())
				{
					_row.UnsetCustomHeight();
				}
			}
			else
			{
				_row.ht = (double)value / 20.0;
				_row.customHeight = true;
			}
		}
	}

	public float HeightInPoints
	{
		get
		{
			if (_row.IsSetHt())
			{
				return (float)_row.ht;
			}
			return _sheet.DefaultRowHeightInPoints;
		}
		set
		{
			Height = (short)((value == -1f) ? (-1f) : (value * 20f));
		}
	}

	public int PhysicalNumberOfCells => _cells.Count;

	public int RowNum
	{
		get
		{
			return (int)(_row.r - 1);
		}
		set
		{
			int lastRowIndex = SpreadsheetVersion.EXCEL2007.LastRowIndex;
			if (value < 0 || value > lastRowIndex)
			{
				throw new ArgumentException("Invalid row number (" + value + ") outside allowable range (0.." + lastRowIndex + ")");
			}
			_row.r = (uint)(value + 1);
		}
	}

	public bool ZeroHeight
	{
		get
		{
			return _row.hidden;
		}
		set
		{
			_row.hidden = value;
		}
	}

	public bool IsFormatted => _row.IsSetS();

	public ICellStyle RowStyle
	{
		get
		{
			if (!IsFormatted)
			{
				return null;
			}
			StylesTable stylesSource = ((XSSFWorkbook)Sheet.Workbook).GetStylesSource();
			if (stylesSource.NumCellStyles > 0)
			{
				return stylesSource.GetStyleAt((int)_row.s);
			}
			return null;
		}
		set
		{
			if (value == null)
			{
				if (_row.IsSetS())
				{
					_row.UnsetS();
					_row.UnsetCustomFormat();
				}
			}
			else
			{
				StylesTable stylesSource = ((XSSFWorkbook)Sheet.Workbook).GetStylesSource();
				XSSFCellStyle xSSFCellStyle = (XSSFCellStyle)value;
				xSSFCellStyle.VerifyBelongsToStylesSource(stylesSource);
				long num = stylesSource.PutStyle(xSSFCellStyle);
				_row.s = (uint)num;
				_row.customFormat = true;
			}
		}
	}

	public List<ICell> Cells
	{
		get
		{
			List<ICell> list = new List<ICell>();
			foreach (ICell value in _cells.Values)
			{
				list.Add(value);
			}
			return list;
		}
	}

	public int OutlineLevel
	{
		get
		{
			return _row.outlineLevel;
		}
		set
		{
			_row.outlineLevel = (byte)value;
		}
	}

	public bool? Hidden
	{
		get
		{
			return _row.hidden;
		}
		set
		{
			_row.hidden = value == true;
		}
	}

	public bool? Collapsed
	{
		get
		{
			return _row.collapsed;
		}
		set
		{
			_row.collapsed = value == true;
		}
	}

	public XSSFRow(CT_Row row, XSSFSheet sheet)
	{
		_row = row;
		_sheet = sheet;
		_cells = new SortedDictionary<int, ICell>();
		if (0 < row.SizeOfCArray())
		{
			foreach (CT_Cell item in row.c)
			{
				XSSFCell xSSFCell = new XSSFCell(this, item);
				_cells.Add(xSSFCell.ColumnIndex, xSSFCell);
				sheet.OnReadCell(xSSFCell);
			}
		}
		if (!row.IsSetR())
		{
			int num = sheet.LastRowNum + 2;
			if (num == 2 && sheet.PhysicalNumberOfRows == 0)
			{
				num = 1;
			}
			row.r = (uint)num;
		}
	}

	public SortedDictionary<int, ICell>.ValueCollection.Enumerator CellIterator()
	{
		return _cells.Values.GetEnumerator();
	}

	public IEnumerator<ICell> GetEnumerator()
	{
		return CellIterator();
	}

	public int CompareTo(XSSFRow other)
	{
		if (Sheet != other.Sheet)
		{
			throw new ArgumentException("The compared rows must belong to the same sheet");
		}
		return RowNum.CompareTo(other.RowNum);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is XSSFRow))
		{
			return false;
		}
		XSSFRow xSSFRow = (XSSFRow)obj;
		if (RowNum == xSSFRow.RowNum)
		{
			return Sheet == xSSFRow.Sheet;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _row.GetHashCode();
	}

	public ICell CreateCell(int columnIndex)
	{
		return CreateCell(columnIndex, CellType.Blank);
	}

	public ICell CreateCell(int columnIndex, CellType type)
	{
		XSSFCell xSSFCell = (_cells.ContainsKey(columnIndex) ? ((XSSFCell)_cells[columnIndex]) : null);
		CT_Cell cT_Cell;
		if (xSSFCell != null)
		{
			cT_Cell = xSSFCell.GetCTCell();
			cT_Cell.Set(new CT_Cell());
		}
		else
		{
			cT_Cell = _row.AddNewC();
		}
		XSSFCell xSSFCell2 = new XSSFCell(this, cT_Cell);
		xSSFCell2.SetCellNum(columnIndex);
		if (type != CellType.Blank)
		{
			xSSFCell2.SetCellType(type);
		}
		_cells[columnIndex] = xSSFCell2;
		return xSSFCell2;
	}

	public ICell GetCell(int cellnum)
	{
		return GetCell(cellnum, _sheet.Workbook.MissingCellPolicy);
	}

	private ICell RetrieveCell(int cellnum)
	{
		if (!_cells.ContainsKey(cellnum))
		{
			return null;
		}
		return _cells[cellnum];
	}

	public ICell GetCell(int cellnum, MissingCellPolicy policy)
	{
		if (cellnum < 0)
		{
			throw new ArgumentException("Cell index must be >= 0");
		}
		XSSFCell xSSFCell = (XSSFCell)RetrieveCell(cellnum);
		switch (policy)
		{
		case MissingCellPolicy.RETURN_NULL_AND_BLANK:
			return xSSFCell;
		case MissingCellPolicy.RETURN_BLANK_AS_NULL:
			if (xSSFCell == null || xSSFCell.CellType != CellType.Blank)
			{
				return xSSFCell;
			}
			return null;
		case MissingCellPolicy.CREATE_NULL_AS_BLANK:
			if (xSSFCell != null)
			{
				return xSSFCell;
			}
			return CreateCell(cellnum, CellType.Blank);
		default:
			throw new ArgumentException("Illegal policy " + policy.ToString() + " (" + policy.ToString() + ")");
		}
	}

	private int GetFirstKey()
	{
		return _cells.Keys.Min();
	}

	private int GetLastKey()
	{
		return _cells.Keys.Max();
	}

	public void SetRowStyle(ICellStyle style)
	{
	}

	public void RemoveCell(ICell cell)
	{
		if (cell.Row != this)
		{
			throw new ArgumentException("Specified cell does not belong to this row");
		}
		XSSFCell xSSFCell = (XSSFCell)cell;
		if (xSSFCell.IsPartOfArrayFormulaGroup)
		{
			xSSFCell.NotifyArrayFormulaChanging();
		}
		if (cell.CellType == CellType.Formula)
		{
			((XSSFWorkbook)_sheet.Workbook).OnDeleteFormula(xSSFCell);
		}
		_cells.Remove(cell.ColumnIndex);
	}

	public CT_Row GetCTRow()
	{
		return _row;
	}

	internal void OnDocumentWrite()
	{
		bool flag = true;
		if (_row.SizeOfCArray() != _cells.Count)
		{
			flag = false;
		}
		else
		{
			int num = 0;
			foreach (XSSFCell value in _cells.Values)
			{
				CT_Cell cTCell = value.GetCTCell();
				CT_Cell cArray = _row.GetCArray(num++);
				string r = cTCell.r;
				string r2 = cArray.r;
				if (!(r?.Equals(r2) ?? (r2 == null)))
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			return;
		}
		CT_Cell[] array = new CT_Cell[_cells.Count];
		int num2 = 0;
		foreach (XSSFCell value2 in _cells.Values)
		{
			array[num2++] = value2.GetCTCell();
		}
		_row.SetCArray(array);
	}

	public override string ToString()
	{
		return _row.ToString();
	}

	internal void Shift(int n)
	{
		int num = RowNum + n;
		CalculationChain calculationChain = ((XSSFWorkbook)_sheet.Workbook).GetCalculationChain();
		int sheetId = (int)_sheet.sheet.sheetId;
		string msg = "Row[rownum=" + RowNum + "] contains cell(s) included in a multi-cell array formula. You cannot change part of an array.";
		using (IEnumerator<ICell> enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				XSSFCell xSSFCell = (XSSFCell)enumerator.Current;
				if (xSSFCell.IsPartOfArrayFormulaGroup)
				{
					xSSFCell.NotifyArrayFormulaChanging(msg);
				}
				calculationChain?.RemoveItem(sheetId, xSSFCell.GetReference());
				CT_Cell cTCell = xSSFCell.GetCTCell();
				string r = new CellReference(num, xSSFCell.ColumnIndex).FormatAsString();
				cTCell.r = r;
			}
		}
		RowNum = num;
	}

	public void CopyRowFrom(IRow srcRow, CellCopyPolicy policy)
	{
		if (srcRow == null)
		{
			using (IEnumerator<ICell> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ICell current = enumerator.Current;
					XSSFCell srcCell = null;
					((XSSFCell)current).CopyCellFrom(srcCell, policy);
				}
			}
			if (policy.IsCopyMergedRegions)
			{
				int rowNum = RowNum;
				int num = 0;
				HashSet<int> hashSet = new HashSet<int>();
				foreach (CellRangeAddress mergedRegion in Sheet.MergedRegions)
				{
					if (rowNum == mergedRegion.FirstRow && rowNum == mergedRegion.LastRow)
					{
						hashSet.Add(num);
					}
					num++;
				}
				(Sheet as XSSFSheet).RemoveMergedRegions(hashSet.ToList());
			}
			if (policy.IsCopyRowHeight)
			{
				Height = -1;
			}
			return;
		}
		foreach (XSSFCell item in srcRow)
		{
			(CreateCell(item.ColumnIndex, item.CellType) as XSSFCell).CopyCellFrom(item, policy);
		}
		XSSFRowShifter xSSFRowShifter = new XSSFRowShifter(_sheet);
		int sheetIndex = _sheet.Workbook.GetSheetIndex(_sheet);
		string sheetName = _sheet.Workbook.GetSheetName(sheetIndex);
		int rowNum2 = srcRow.RowNum;
		int rowNum3 = RowNum;
		int numberOfRowsToMove = rowNum3 - rowNum2;
		FormulaShifter shifter = FormulaShifter.CreateForRowCopy(sheetIndex, sheetName, rowNum2, rowNum2, numberOfRowsToMove, SpreadsheetVersion.EXCEL2007);
		xSSFRowShifter.UpdateRowFormulas(this, shifter);
		if (policy.IsCopyMergedRegions)
		{
			foreach (CellRangeAddress mergedRegion2 in srcRow.Sheet.MergedRegions)
			{
				if (rowNum2 == mergedRegion2.FirstRow && rowNum2 == mergedRegion2.LastRow)
				{
					CellRangeAddress cellRangeAddress = mergedRegion2.Copy();
					cellRangeAddress.FirstRow = rowNum3;
					cellRangeAddress.LastRow = rowNum3;
					Sheet.AddMergedRegion(cellRangeAddress);
				}
			}
		}
		if (policy.IsCopyRowHeight)
		{
			Height = srcRow.Height;
		}
	}

	public void MoveCell(ICell cell, int newColumn)
	{
		throw new NotImplementedException();
	}

	public IRow CopyRowTo(int targetIndex)
	{
		return Sheet.CopyRow(RowNum, targetIndex);
	}

	public ICell CopyCell(int sourceIndex, int targetIndex)
	{
		return CellUtil.CopyCell(this, sourceIndex, targetIndex);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool HasCustomHeight()
	{
		throw new NotImplementedException();
	}
}
