using System;
using System.Collections.Generic;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFCellStyle : ICellStyle
{
	private ExtendedFormatRecord _format;

	private short index;

	private InternalWorkbook _workbook;

	private static short lastDateFormat = short.MinValue;

	private static List<FormatRecord> lastFormats = null;

	private static string getDataFormatStringCache = null;

	public short Index => index;

	public HSSFCellStyle ParentStyle
	{
		get
		{
			short parentIndex = _format.ParentIndex;
			if (parentIndex == 0 || parentIndex == 4095)
			{
				return null;
			}
			return new HSSFCellStyle(parentIndex, _workbook.GetExFormatAt(parentIndex), _workbook);
		}
	}

	public short DataFormat
	{
		get
		{
			return _format.FormatIndex;
		}
		set
		{
			_format.FormatIndex = value;
		}
	}

	public short FontIndex => _format.FontIndex;

	public bool IsHidden
	{
		get
		{
			return _format.IsHidden;
		}
		set
		{
			_format.IsIndentNotParentCellOptions = true;
			_format.IsHidden = value;
		}
	}

	public bool IsLocked
	{
		get
		{
			return _format.IsLocked;
		}
		set
		{
			_format.IsIndentNotParentCellOptions = true;
			_format.IsLocked = value;
		}
	}

	public HorizontalAlignment Alignment
	{
		get
		{
			return (HorizontalAlignment)_format.Alignment;
		}
		set
		{
			_format.IsIndentNotParentAlignment = true;
			_format.Alignment = (short)value;
		}
	}

	public bool WrapText
	{
		get
		{
			return _format.WrapText;
		}
		set
		{
			_format.IsIndentNotParentAlignment = true;
			_format.WrapText = value;
		}
	}

	public VerticalAlignment VerticalAlignment
	{
		get
		{
			return (VerticalAlignment)_format.VerticalAlignment;
		}
		set
		{
			_format.VerticalAlignment = (short)value;
		}
	}

	public short Rotation
	{
		get
		{
			short num = _format.Rotation;
			if (num == 255)
			{
				return num;
			}
			if (num > 90)
			{
				num = (short)(90 - num);
			}
			return num;
		}
		set
		{
			short num = value;
			if (num != 255)
			{
				if (value < 0 && value >= -90)
				{
					num = (short)(90 - value);
				}
				else if ((num <= 90 || num > 180) && (value < -90 || value > 90))
				{
					throw new ArgumentException("The rotation must be between -90 and 90 degrees, or 0xff");
				}
			}
			_format.Rotation = num;
		}
	}

	public short Indention
	{
		get
		{
			return _format.Indent;
		}
		set
		{
			_format.Indent = value;
		}
	}

	public BorderStyle BorderLeft
	{
		get
		{
			return (BorderStyle)_format.BorderLeft;
		}
		set
		{
			_format.IsIndentNotParentBorder = true;
			_format.BorderLeft = (short)value;
		}
	}

	public BorderStyle BorderRight
	{
		get
		{
			return (BorderStyle)_format.BorderRight;
		}
		set
		{
			_format.IsIndentNotParentBorder = true;
			_format.BorderRight = (short)value;
		}
	}

	public BorderStyle BorderTop
	{
		get
		{
			return (BorderStyle)_format.BorderTop;
		}
		set
		{
			_format.IsIndentNotParentBorder = true;
			_format.BorderTop = (short)value;
		}
	}

	public BorderStyle BorderBottom
	{
		get
		{
			return (BorderStyle)_format.BorderBottom;
		}
		set
		{
			_format.IsIndentNotParentBorder = true;
			_format.BorderBottom = (short)value;
		}
	}

	public short LeftBorderColor
	{
		get
		{
			return _format.LeftBorderPaletteIdx;
		}
		set
		{
			_format.LeftBorderPaletteIdx = value;
		}
	}

	public short RightBorderColor
	{
		get
		{
			return _format.RightBorderPaletteIdx;
		}
		set
		{
			_format.RightBorderPaletteIdx = value;
		}
	}

	public short TopBorderColor
	{
		get
		{
			return _format.TopBorderPaletteIdx;
		}
		set
		{
			_format.TopBorderPaletteIdx = value;
		}
	}

	public short BottomBorderColor
	{
		get
		{
			return _format.BottomBorderPaletteIdx;
		}
		set
		{
			_format.BottomBorderPaletteIdx = value;
		}
	}

	public short BorderDiagonalColor
	{
		get
		{
			return _format.AdtlDiagBorderPaletteIdx;
		}
		set
		{
			_format.AdtlDiagBorderPaletteIdx = value;
		}
	}

	public BorderStyle BorderDiagonalLineStyle
	{
		get
		{
			return (BorderStyle)_format.AdtlDiagLineStyle;
		}
		set
		{
			_format.AdtlDiagLineStyle = (short)value;
		}
	}

	public BorderDiagonal BorderDiagonal
	{
		get
		{
			return (BorderDiagonal)_format.Diagonal;
		}
		set
		{
			_format.Diagonal = (short)value;
		}
	}

	public bool ShrinkToFit
	{
		get
		{
			return _format.ShrinkToFit;
		}
		set
		{
			_format.ShrinkToFit = value;
		}
	}

	public short ReadingOrder
	{
		get
		{
			return _format.ReadingOrder;
		}
		set
		{
			_format.ReadingOrder = value;
		}
	}

	public FillPattern FillPattern
	{
		get
		{
			return (FillPattern)_format.AdtlFillPattern;
		}
		set
		{
			_format.AdtlFillPattern = (short)value;
		}
	}

	public short FillBackgroundColor
	{
		get
		{
			short fillBackground = _format.FillBackground;
			if (fillBackground == 65)
			{
				return 64;
			}
			return fillBackground;
		}
		set
		{
			_format.FillBackground = value;
			CheckDefaultBackgroundFills();
		}
	}

	public IColor FillBackgroundColorColor => new HSSFPalette(_workbook.CustomPalette).GetColor(FillBackgroundColor);

	public short FillForegroundColor
	{
		get
		{
			return _format.FillForeground;
		}
		set
		{
			_format.FillForeground = value;
			CheckDefaultBackgroundFills();
		}
	}

	public IColor FillForegroundColorColor => new HSSFPalette(_workbook.CustomPalette).GetColor(FillForegroundColor);

	public string UserStyleName
	{
		get
		{
			StyleRecord styleRecord = _workbook.GetStyleRecord(index);
			if (styleRecord == null)
			{
				return null;
			}
			if (styleRecord.IsBuiltin)
			{
				return null;
			}
			return styleRecord.Name;
		}
		set
		{
			StyleRecord styleRecord = _workbook.GetStyleRecord(index);
			if (styleRecord == null)
			{
				styleRecord = _workbook.CreateStyleRecord(index);
			}
			if (styleRecord.IsBuiltin && index <= 20)
			{
				throw new ArgumentException("Unable to set user specified style names for built in styles!");
			}
			styleRecord.Name = value;
		}
	}

	public HSSFCellStyle(short index, ExtendedFormatRecord rec, HSSFWorkbook workbook)
		: this(index, rec, workbook.Workbook)
	{
	}

	public HSSFCellStyle(short index, ExtendedFormatRecord rec, InternalWorkbook workbook)
	{
		_workbook = workbook;
		this.index = index;
		_format = rec;
	}

	public string GetDataFormatString()
	{
		if (getDataFormatStringCache != null && lastDateFormat == DataFormat && _workbook.Formats.Equals(lastFormats))
		{
			return getDataFormatStringCache;
		}
		lastFormats = _workbook.Formats;
		lastDateFormat = DataFormat;
		getDataFormatStringCache = GetDataFormatString(_workbook);
		return getDataFormatStringCache;
	}

	public string GetDataFormatString(IWorkbook workbook)
	{
		HSSFDataFormat hSSFDataFormat = new HSSFDataFormat(((HSSFWorkbook)workbook).Workbook);
		if (DataFormat != -1)
		{
			return hSSFDataFormat.GetFormat(DataFormat);
		}
		return "General";
	}

	public string GetDataFormatString(InternalWorkbook workbook)
	{
		return new HSSFDataFormat(workbook).GetFormat(DataFormat);
	}

	public void SetFont(IFont font)
	{
		_format.IsIndentNotParentFont = true;
		short fontIndex = font.Index;
		_format.FontIndex = fontIndex;
	}

	public IFont GetFont(IWorkbook parentWorkbook)
	{
		return ((HSSFWorkbook)parentWorkbook).GetFontAt(FontIndex);
	}

	public void VerifyBelongsToWorkbook(HSSFWorkbook wb)
	{
		if (wb.Workbook != _workbook)
		{
			throw new ArgumentException("This Style does not belong to the supplied Workbook. Are you trying to assign a style from one workbook to the cell of a different workbook?");
		}
	}

	private void CheckDefaultBackgroundFills()
	{
		if (_format.FillForeground == 64)
		{
			if (_format.FillBackground != 65)
			{
				FillBackgroundColor = 65;
			}
		}
		else if (_format.FillBackground == 65 && _format.FillForeground != 64)
		{
			FillBackgroundColor = 64;
		}
	}

	public void CloneStyleFrom(ICellStyle source)
	{
		if (source is HSSFCellStyle)
		{
			CloneStyleFrom((HSSFCellStyle)source);
			return;
		}
		throw new ArgumentException("Can only clone from one HSSFCellStyle to another, not between HSSFCellStyle and XSSFCellStyle");
	}

	public void CloneStyleFrom(HSSFCellStyle source)
	{
		_format.CloneStyleFrom(source._format);
		if (_workbook != source._workbook)
		{
			lastDateFormat = short.MinValue;
			lastFormats = null;
			getDataFormatStringCache = null;
			short dataFormat = (short)_workbook.CreateFormat(source.GetDataFormatString());
			DataFormat = dataFormat;
			FontRecord fontRecord = _workbook.CreateNewFont();
			fontRecord.CloneStyleFrom(source._workbook.GetFontRecordAt(source.FontIndex));
			HSSFFont font = new HSSFFont((short)_workbook.GetFontIndex(fontRecord), fontRecord);
			SetFont(font);
		}
	}

	public override int GetHashCode()
	{
		int num = 1;
		num = 31 * num + ((_format != null) ? _format.GetHashCode() : 0);
		return 31 * num + index;
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj == null)
		{
			return false;
		}
		if (obj is HSSFCellStyle)
		{
			HSSFCellStyle hSSFCellStyle = (HSSFCellStyle)obj;
			if (_format == null)
			{
				if (hSSFCellStyle._format != null)
				{
					return false;
				}
			}
			else if (!_format.Equals(hSSFCellStyle._format))
			{
				return false;
			}
			if (index != hSSFCellStyle.index)
			{
				return false;
			}
			return true;
		}
		return false;
	}
}
