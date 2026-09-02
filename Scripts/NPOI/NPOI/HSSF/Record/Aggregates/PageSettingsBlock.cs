using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.Model;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.Record.Aggregates;

public class PageSettingsBlock : RecordAggregate
{
	private class CustomRecordVisitor1 : RecordVisitor
	{
		private CustomViewSettingsRecordAggregate _cv;

		private HeaderFooterRecord _hf;

		private List<HeaderFooterRecord> _sviewHeaderFooters;

		private Dictionary<string, HeaderFooterRecord> _hfGuidMap;

		public CustomRecordVisitor1(CustomViewSettingsRecordAggregate cv, HeaderFooterRecord hf, List<HeaderFooterRecord> sviewHeaderFooter, Dictionary<string, HeaderFooterRecord> hfGuidMap)
		{
			_cv = cv;
			_hf = hf;
			_sviewHeaderFooters = sviewHeaderFooter;
			_hfGuidMap = hfGuidMap;
		}

		public void VisitRecord(Record r)
		{
			if (r.Sid == 426)
			{
				string key = HexDump.ToHex(((UserSViewBegin)r).Guid);
				if (_hfGuidMap[key] != null)
				{
					_cv.Append(_hf);
					_sviewHeaderFooters.Remove(_hf);
				}
			}
		}
	}

	private PageBreakRecord _rowBreaksRecord;

	private PageBreakRecord _columnBreaksRecord;

	private HeaderRecord header;

	private FooterRecord footer;

	private HCenterRecord _hCenter;

	private VCenterRecord _vCenter;

	private LeftMarginRecord _leftMargin;

	private RightMarginRecord _rightMargin;

	private TopMarginRecord _topMargin;

	private BottomMarginRecord _bottomMargin;

	private PrintSetupRecord printSetup;

	private Record _bitmap;

	private HeaderFooterRecord _headerFooter;

	private List<HeaderFooterRecord> _sviewHeaderFooters = new List<HeaderFooterRecord>();

	private List<PLSAggregate> _plsRecords;

	private Record _printSize;

	private PageBreakRecord RowBreaksRecord
	{
		get
		{
			if (_rowBreaksRecord == null)
			{
				_rowBreaksRecord = new HorizontalPageBreakRecord();
			}
			return _rowBreaksRecord;
		}
	}

	private PageBreakRecord ColumnBreaksRecord
	{
		get
		{
			if (_columnBreaksRecord == null)
			{
				_columnBreaksRecord = new VerticalPageBreakRecord();
			}
			return _columnBreaksRecord;
		}
	}

	public HeaderRecord Header
	{
		get
		{
			return header;
		}
		set
		{
			header = value;
		}
	}

	public FooterRecord Footer
	{
		get
		{
			return footer;
		}
		set
		{
			footer = value;
		}
	}

	public PrintSetupRecord PrintSetup
	{
		get
		{
			return printSetup;
		}
		set
		{
			printSetup = value;
		}
	}

	public int[] RowBreaks => RowBreaksRecord.GetBreaks();

	public int NumRowBreaks => RowBreaksRecord.NumBreaks;

	public int[] ColumnBreaks => ColumnBreaksRecord.GetBreaks();

	public int NumColumnBreaks => ColumnBreaksRecord.NumBreaks;

	public VCenterRecord VCenter => _vCenter;

	public HCenterRecord HCenter => _hCenter;

	public PageSettingsBlock(RecordStream rs)
	{
		_plsRecords = new List<PLSAggregate>();
		while (ReadARecord(rs))
		{
		}
	}

	public PageSettingsBlock()
	{
		_plsRecords = new List<PLSAggregate>();
		_rowBreaksRecord = new HorizontalPageBreakRecord();
		_columnBreaksRecord = new VerticalPageBreakRecord();
		header = new HeaderRecord(string.Empty);
		footer = new FooterRecord(string.Empty);
		_hCenter = CreateHCenter();
		_vCenter = CreateVCenter();
		printSetup = CreatePrintSetup();
	}

	public static bool IsComponentRecord(int sid)
	{
		switch (sid)
		{
		case 20:
		case 21:
		case 26:
		case 27:
		case 38:
		case 39:
		case 40:
		case 41:
		case 51:
		case 77:
		case 131:
		case 132:
		case 161:
		case 233:
		case 2204:
			return true;
		default:
			return false;
		}
	}

	private bool ReadARecord(RecordStream rs)
	{
		switch (rs.PeekNextSid())
		{
		case 27:
			CheckNotPresent(_rowBreaksRecord);
			_rowBreaksRecord = (PageBreakRecord)rs.GetNext();
			break;
		case 26:
			CheckNotPresent(_columnBreaksRecord);
			_columnBreaksRecord = (PageBreakRecord)rs.GetNext();
			break;
		case 20:
			CheckNotPresent(header);
			header = (HeaderRecord)rs.GetNext();
			break;
		case 21:
			CheckNotPresent(footer);
			footer = (FooterRecord)rs.GetNext();
			break;
		case 131:
			CheckNotPresent(_hCenter);
			_hCenter = (HCenterRecord)rs.GetNext();
			break;
		case 132:
			CheckNotPresent(_vCenter);
			_vCenter = (VCenterRecord)rs.GetNext();
			break;
		case 38:
			CheckNotPresent(_leftMargin);
			_leftMargin = (LeftMarginRecord)rs.GetNext();
			break;
		case 39:
			CheckNotPresent(_rightMargin);
			_rightMargin = (RightMarginRecord)rs.GetNext();
			break;
		case 40:
			CheckNotPresent(_topMargin);
			_topMargin = (TopMarginRecord)rs.GetNext();
			break;
		case 41:
			CheckNotPresent(_bottomMargin);
			_bottomMargin = (BottomMarginRecord)rs.GetNext();
			break;
		case 77:
			_plsRecords.Add(new PLSAggregate(rs));
			break;
		case 161:
			CheckNotPresent(printSetup);
			printSetup = (PrintSetupRecord)rs.GetNext();
			break;
		case 233:
			CheckNotPresent(_bitmap);
			_bitmap = rs.GetNext();
			break;
		case 51:
			CheckNotPresent(_printSize);
			_printSize = rs.GetNext();
			break;
		case 2204:
		{
			HeaderFooterRecord headerFooterRecord = (HeaderFooterRecord)rs.GetNext();
			if (headerFooterRecord.IsCurrentSheet)
			{
				_headerFooter = headerFooterRecord;
			}
			else
			{
				_sviewHeaderFooters.Add(headerFooterRecord);
			}
			break;
		}
		default:
			return false;
		}
		return true;
	}

	private void CheckNotPresent(Record rec)
	{
		if (rec != null)
		{
			throw new RecordFormatException("Duplicate PageSettingsBlock record (sid=0x" + StringUtil.ToHexString(rec.Sid) + ")");
		}
	}

	public IEnumerator GetEnumerator()
	{
		return _plsRecords.GetEnumerator();
	}

	public void SetColumnBreak(int column, int fromRow, int toRow)
	{
		ColumnBreaksRecord.AddBreak(column, fromRow, toRow);
	}

	public void RemoveColumnBreak(int column)
	{
		ColumnBreaksRecord.RemoveBreak(column);
	}

	public override void VisitContainedRecords(RecordVisitor rv)
	{
		VisitIfPresent(_rowBreaksRecord, rv);
		VisitIfPresent(_columnBreaksRecord, rv);
		if (header == null)
		{
			rv.VisitRecord(new HeaderRecord(""));
		}
		else
		{
			rv.VisitRecord(header);
		}
		if (footer == null)
		{
			rv.VisitRecord(new FooterRecord(""));
		}
		else
		{
			rv.VisitRecord(footer);
		}
		VisitIfPresent(_hCenter, rv);
		VisitIfPresent(_vCenter, rv);
		VisitIfPresent(_leftMargin, rv);
		VisitIfPresent(_rightMargin, rv);
		VisitIfPresent(_topMargin, rv);
		VisitIfPresent(_bottomMargin, rv);
		foreach (PLSAggregate plsRecord in _plsRecords)
		{
			plsRecord.VisitContainedRecords(rv);
		}
		VisitIfPresent(printSetup, rv);
		VisitIfPresent(_printSize, rv);
		VisitIfPresent(_headerFooter, rv);
		VisitIfPresent(_bitmap, rv);
	}

	private static void VisitIfPresent(Record r, RecordVisitor rv)
	{
		if (r != null)
		{
			rv.VisitRecord(r);
		}
	}

	private static void VisitIfPresent(PageBreakRecord r, RecordVisitor rv)
	{
		if (r != null && !r.IsEmpty)
		{
			rv.VisitRecord(r);
		}
	}

	private static HCenterRecord CreateHCenter()
	{
		return new HCenterRecord
		{
			HCenter = false
		};
	}

	private static VCenterRecord CreateVCenter()
	{
		return new VCenterRecord
		{
			VCenter = false
		};
	}

	private static PrintSetupRecord CreatePrintSetup()
	{
		return new PrintSetupRecord
		{
			PaperSize = 1,
			Scale = 100,
			PageStart = 1,
			FitWidth = 1,
			FitHeight = 1,
			Options = 2,
			HResolution = 300,
			VResolution = 300,
			HeaderMargin = 0.5,
			FooterMargin = 0.5,
			Copies = 1
		};
	}

	private IMargin GetMarginRec(MarginType margin)
	{
		switch (margin)
		{
		case MarginType.LeftMargin:
			return _leftMargin;
		case MarginType.RightMargin:
			return _rightMargin;
		case MarginType.TopMargin:
			return _topMargin;
		case MarginType.BottomMargin:
			return _bottomMargin;
		default:
		{
			short num = (short)margin;
			throw new InvalidOperationException("Unknown margin constant:  " + num);
		}
		}
	}

	public double GetMargin(MarginType margin)
	{
		return GetMarginRec(margin)?.Margin ?? (margin switch
		{
			MarginType.LeftMargin => 0.75, 
			MarginType.RightMargin => 0.75, 
			MarginType.TopMargin => 1.0, 
			MarginType.BottomMargin => 1.0, 
			_ => throw new InvalidOperationException("Unknown margin constant:  " + margin), 
		});
	}

	public void SetMargin(MarginType margin, double size)
	{
		IMargin margin2 = GetMarginRec(margin);
		if (margin2 == null)
		{
			switch (margin)
			{
			case MarginType.LeftMargin:
				_leftMargin = new LeftMarginRecord();
				margin2 = _leftMargin;
				break;
			case MarginType.RightMargin:
				_rightMargin = new RightMarginRecord();
				margin2 = _rightMargin;
				break;
			case MarginType.TopMargin:
				_topMargin = new TopMarginRecord();
				margin2 = _topMargin;
				break;
			case MarginType.BottomMargin:
				_bottomMargin = new BottomMarginRecord();
				margin2 = _bottomMargin;
				break;
			default:
				throw new InvalidOperationException("Unknown margin constant:  " + margin);
			}
		}
		margin2.Margin = size;
	}

	private static void ShiftBreaks(PageBreakRecord breaks, int start, int stop, int count)
	{
		IEnumerator breaksEnumerator = breaks.GetBreaksEnumerator();
		IList list = new ArrayList();
		while (breaksEnumerator.MoveNext())
		{
			PageBreakRecord.Break obj = (PageBreakRecord.Break)breaksEnumerator.Current;
			int main = obj.main;
			bool flag = main >= start;
			bool flag2 = main <= stop;
			if (flag & flag2)
			{
				list.Add(obj);
			}
		}
		breaksEnumerator = list.GetEnumerator();
		while (breaksEnumerator.MoveNext())
		{
			PageBreakRecord.Break obj2 = (PageBreakRecord.Break)breaksEnumerator.Current;
			breaks.RemoveBreak(obj2.main);
			breaks.AddBreak((short)(obj2.main + count), obj2.subFrom, obj2.subTo);
		}
	}

	public void SetRowBreak(int row, short fromCol, short toCol)
	{
		RowBreaksRecord.AddBreak((short)row, fromCol, toCol);
	}

	public void RemoveRowBreak(int row)
	{
		if (RowBreaksRecord.GetBreaks().Length < 1)
		{
			throw new ArgumentException("Sheet does not define any row breaks");
		}
		RowBreaksRecord.RemoveBreak((short)row);
	}

	public bool IsRowBroken(int row)
	{
		return RowBreaksRecord.GetBreak(row) != null;
	}

	public bool IsColumnBroken(int column)
	{
		return ColumnBreaksRecord.GetBreak(column) != null;
	}

	public void ShiftRowBreaks(int startingRow, int endingRow, int count)
	{
		ShiftBreaks(RowBreaksRecord, startingRow, endingRow, count);
	}

	public void ShiftColumnBreaks(short startingCol, short endingCol, short count)
	{
		ShiftBreaks(ColumnBreaksRecord, startingCol, endingCol, count);
	}

	public void AddLateHeaderFooter(HeaderFooterRecord rec)
	{
		if (_headerFooter != null)
		{
			throw new ArgumentNullException("This page settings block already has a header/footer record");
		}
		if (rec.Sid != 2204)
		{
			throw new RecordFormatException("Unexpected header-footer record sid: 0x" + StringUtil.ToHexString(rec.Sid));
		}
		_headerFooter = rec;
	}

	public void AddLateRecords(RecordStream rs)
	{
		while (ReadARecord(rs))
		{
		}
	}

	public void PositionRecords(List<RecordBase> sheetRecords)
	{
		List<HeaderFooterRecord> list = new List<HeaderFooterRecord>(_sviewHeaderFooters);
		Dictionary<string, HeaderFooterRecord> dictionary = new Dictionary<string, HeaderFooterRecord>();
		foreach (HeaderFooterRecord item in list)
		{
			string key = HexDump.ToHex(item.Guid);
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = item;
			}
			else
			{
				dictionary.Add(HexDump.ToHex(item.Guid), item);
			}
		}
		foreach (HeaderFooterRecord item2 in list)
		{
			foreach (RecordBase sheetRecord in sheetRecords)
			{
				if (sheetRecord is CustomViewSettingsRecordAggregate)
				{
					CustomViewSettingsRecordAggregate obj = (CustomViewSettingsRecordAggregate)sheetRecord;
					obj.VisitContainedRecords(new CustomRecordVisitor1(obj, item2, _sviewHeaderFooters, dictionary));
				}
			}
		}
	}
}
