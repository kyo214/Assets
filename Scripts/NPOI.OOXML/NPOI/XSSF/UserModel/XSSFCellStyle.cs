using System;
using System.Xml;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.Model;
using NPOI.XSSF.UserModel.Extensions;

namespace NPOI.XSSF.UserModel;

public class XSSFCellStyle : ICellStyle
{
	private int _cellXfId;

	private StylesTable _stylesSource;

	private CT_Xf _cellXf;

	private CT_Xf _cellStyleXf;

	private XSSFFont _font;

	private XSSFCellAlignment _cellAlignment;

	private ThemesTable _theme;

	public HorizontalAlignment Alignment
	{
		get
		{
			return GetAlignmentEnum();
		}
		set
		{
			GetCellAlignment().Horizontal = value;
		}
	}

	public BorderStyle BorderBottom
	{
		get
		{
			if (!_cellXf.applyBorder)
			{
				return BorderStyle.None;
			}
			int borderId = (int)_cellXf.borderId;
			CT_Border cTBorder = _stylesSource.GetBorderAt(borderId).GetCTBorder();
			if (!cTBorder.IsSetBottom())
			{
				return BorderStyle.None;
			}
			return (BorderStyle)cTBorder.bottom.style;
		}
		set
		{
			CT_Border cTBorder = GetCTBorder(copy: true);
			CT_BorderPr cT_BorderPr = (cTBorder.IsSetBottom() ? cTBorder.bottom : cTBorder.AddNewBottom());
			if (value == BorderStyle.None)
			{
				cTBorder.UnsetBottom();
			}
			else
			{
				cT_BorderPr.style = (ST_BorderStyle)value;
			}
			int borderId = _stylesSource.PutBorder(new XSSFCellBorder(cTBorder, _theme));
			_cellXf.borderId = (uint)borderId;
			_cellXf.applyBorder = true;
		}
	}

	public BorderStyle BorderLeft
	{
		get
		{
			if (!_cellXf.applyBorder)
			{
				return BorderStyle.None;
			}
			int borderId = (int)_cellXf.borderId;
			CT_Border cTBorder = _stylesSource.GetBorderAt(borderId).GetCTBorder();
			if (!cTBorder.IsSetLeft())
			{
				return BorderStyle.None;
			}
			return (BorderStyle)cTBorder.left.style;
		}
		set
		{
			CT_Border cTBorder = GetCTBorder(copy: true);
			CT_BorderPr cT_BorderPr = (cTBorder.IsSetLeft() ? cTBorder.left : cTBorder.AddNewLeft());
			if (value == BorderStyle.None)
			{
				cTBorder.unsetLeft();
			}
			else
			{
				cT_BorderPr.style = (ST_BorderStyle)value;
			}
			int borderId = _stylesSource.PutBorder(new XSSFCellBorder(cTBorder, _theme));
			_cellXf.borderId = (uint)borderId;
			_cellXf.applyBorder = true;
		}
	}

	public BorderStyle BorderRight
	{
		get
		{
			if (!_cellXf.applyBorder)
			{
				return BorderStyle.None;
			}
			int borderId = (int)_cellXf.borderId;
			CT_Border cTBorder = _stylesSource.GetBorderAt(borderId).GetCTBorder();
			if (!cTBorder.IsSetRight())
			{
				return BorderStyle.None;
			}
			return (BorderStyle)cTBorder.right.style;
		}
		set
		{
			CT_Border cTBorder = GetCTBorder(copy: true);
			CT_BorderPr cT_BorderPr = (cTBorder.IsSetRight() ? cTBorder.right : cTBorder.AddNewRight());
			if (value == BorderStyle.None)
			{
				cTBorder.unsetRight();
			}
			else
			{
				cT_BorderPr.style = (ST_BorderStyle)value;
			}
			int borderId = _stylesSource.PutBorder(new XSSFCellBorder(cTBorder, _theme));
			_cellXf.borderId = (uint)borderId;
			_cellXf.applyBorder = true;
		}
	}

	public BorderStyle BorderTop
	{
		get
		{
			if (!_cellXf.applyBorder)
			{
				return BorderStyle.None;
			}
			int borderId = (int)_cellXf.borderId;
			CT_Border cTBorder = _stylesSource.GetBorderAt(borderId).GetCTBorder();
			if (!cTBorder.IsSetTop())
			{
				return BorderStyle.None;
			}
			return (BorderStyle)cTBorder.top.style;
		}
		set
		{
			CT_Border cTBorder = GetCTBorder(copy: true);
			CT_BorderPr cT_BorderPr = (cTBorder.IsSetTop() ? cTBorder.top : cTBorder.AddNewTop());
			if (value == BorderStyle.None)
			{
				cTBorder.unsetTop();
			}
			else
			{
				cT_BorderPr.style = (ST_BorderStyle)value;
			}
			int borderId = _stylesSource.PutBorder(new XSSFCellBorder(cTBorder, _theme));
			_cellXf.borderId = (uint)borderId;
			_cellXf.applyBorder = true;
		}
	}

	public short BottomBorderColor
	{
		get
		{
			return BottomBorderXSSFColor?.Indexed ?? IndexedColors.Black.Index;
		}
		set
		{
			XSSFColor xSSFColor = new XSSFColor();
			xSSFColor.Indexed = value;
			SetBottomBorderColor(xSSFColor);
		}
	}

	public XSSFColor BottomBorderXSSFColor
	{
		get
		{
			if (!_cellXf.applyBorder)
			{
				return null;
			}
			int borderId = (int)_cellXf.borderId;
			return _stylesSource.GetBorderAt(borderId).GetBorderColor(BorderSide.BOTTOM);
		}
	}

	public short DataFormat
	{
		get
		{
			return (short)_cellXf.numFmtId;
		}
		set
		{
			SetDataFormat(value & 0xFFFF);
		}
	}

	public short FillBackgroundColor
	{
		get
		{
			return ((XSSFColor)FillBackgroundColorColor)?.Indexed ?? IndexedColors.Automatic.Index;
		}
		set
		{
			XSSFColor xSSFColor = new XSSFColor();
			xSSFColor.Indexed = value;
			SetFillBackgroundColor(xSSFColor);
		}
	}

	public IColor FillBackgroundColorColor
	{
		get
		{
			return FillBackgroundXSSFColor;
		}
		set
		{
			FillBackgroundXSSFColor = (XSSFColor)value;
		}
	}

	public XSSFColor FillBackgroundXSSFColor
	{
		get
		{
			if (_cellXf.IsSetApplyFill() && !_cellXf.applyFill)
			{
				return null;
			}
			int fillId = (int)_cellXf.fillId;
			XSSFColor fillBackgroundColor = _stylesSource.GetFillAt(fillId).GetFillBackgroundColor();
			if (fillBackgroundColor != null && _theme != null)
			{
				_theme.InheritFromThemeAsRequired(fillBackgroundColor);
			}
			return fillBackgroundColor;
		}
		set
		{
			CT_Fill cTFill = GetCTFill();
			CT_PatternFill cT_PatternFill = cTFill.patternFill;
			if (value == null)
			{
				if (cT_PatternFill != null && cT_PatternFill.IsSetBgColor())
				{
					cT_PatternFill.UnsetBgColor();
				}
			}
			else
			{
				if (cT_PatternFill == null)
				{
					cT_PatternFill = cTFill.AddNewPatternFill();
				}
				cT_PatternFill.bgColor = value.GetCTColor();
			}
			AddFill(cTFill);
		}
	}

	public short FillForegroundColor
	{
		get
		{
			return ((XSSFColor)FillForegroundColorColor)?.Indexed ?? IndexedColors.Automatic.Index;
		}
		set
		{
			XSSFColor xSSFColor = new XSSFColor();
			xSSFColor.Indexed = value;
			SetFillForegroundColor(xSSFColor);
		}
	}

	public IColor FillForegroundColorColor
	{
		get
		{
			return FillForegroundXSSFColor;
		}
		set
		{
			FillForegroundXSSFColor = (XSSFColor)value;
		}
	}

	public XSSFColor FillForegroundXSSFColor
	{
		get
		{
			if (_cellXf.IsSetApplyFill() && !_cellXf.applyFill)
			{
				return null;
			}
			int fillId = (int)_cellXf.fillId;
			XSSFColor fillForegroundColor = _stylesSource.GetFillAt(fillId).GetFillForegroundColor();
			if (fillForegroundColor != null && _theme != null)
			{
				_theme.InheritFromThemeAsRequired(fillForegroundColor);
			}
			return fillForegroundColor;
		}
		set
		{
			CT_Fill cTFill = GetCTFill();
			CT_PatternFill cT_PatternFill = cTFill.patternFill;
			if (value == null)
			{
				if (cT_PatternFill != null && cT_PatternFill.IsSetFgColor())
				{
					cT_PatternFill.UnsetFgColor();
				}
			}
			else
			{
				if (cT_PatternFill == null)
				{
					cT_PatternFill = cTFill.AddNewPatternFill();
				}
				cT_PatternFill.fgColor = value.GetCTColor();
			}
			AddFill(cTFill);
		}
	}

	public FillPattern FillPattern
	{
		get
		{
			if (_cellXf.IsSetApplyFill() && !_cellXf.applyFill)
			{
				return FillPattern.NoFill;
			}
			int fillId = (int)_cellXf.fillId;
			ST_PatternType patternType = _stylesSource.GetFillAt(fillId).GetPatternType();
			if (patternType == ST_PatternType.none)
			{
				return FillPattern.NoFill;
			}
			return (FillPattern)patternType;
		}
		set
		{
			CT_Fill cTFill = GetCTFill();
			CT_PatternFill cT_PatternFill = (cTFill.IsSetPatternFill() ? cTFill.GetPatternFill() : cTFill.AddNewPatternFill());
			if (value == FillPattern.NoFill && cT_PatternFill.IsSetPatternType())
			{
				cT_PatternFill.UnsetPatternType();
			}
			else
			{
				cT_PatternFill.patternType = (ST_PatternType)value;
			}
			AddFill(cTFill);
		}
	}

	public short FontIndex => (short)FontId;

	public bool IsHidden
	{
		get
		{
			if (!_cellXf.IsSetProtection() || !_cellXf.protection.IsSetHidden())
			{
				return false;
			}
			return _cellXf.protection.hidden;
		}
		set
		{
			if (!_cellXf.IsSetProtection())
			{
				_cellXf.AddNewProtection();
			}
			_cellXf.protection.hidden = value;
		}
	}

	public short Indention
	{
		get
		{
			return (short)(_cellXf.alignment?.indent ?? 0);
		}
		set
		{
			GetCellAlignment().Indent = value;
		}
	}

	public short Index => (short)_cellXfId;

	protected internal int UIndex => _cellXfId;

	public short LeftBorderColor
	{
		get
		{
			return LeftBorderXSSFColor?.Indexed ?? IndexedColors.Black.Index;
		}
		set
		{
			XSSFColor xSSFColor = new XSSFColor();
			xSSFColor.Indexed = value;
			SetLeftBorderColor(xSSFColor);
		}
	}

	public XSSFColor DiagonalBorderXSSFColor
	{
		get
		{
			if (!_cellXf.applyBorder)
			{
				return null;
			}
			int borderId = (int)_cellXf.borderId;
			return _stylesSource.GetBorderAt(borderId).GetBorderColor(BorderSide.DIAGONAL);
		}
	}

	public XSSFColor LeftBorderXSSFColor
	{
		get
		{
			if (!_cellXf.applyBorder)
			{
				return null;
			}
			int borderId = (int)_cellXf.borderId;
			return _stylesSource.GetBorderAt(borderId).GetBorderColor(BorderSide.LEFT);
		}
	}

	public bool IsLocked
	{
		get
		{
			if (!_cellXf.IsSetProtection())
			{
				return true;
			}
			return _cellXf.protection.locked;
		}
		set
		{
			if (!_cellXf.IsSetProtection())
			{
				_cellXf.AddNewProtection();
			}
			_cellXf.protection.locked = value;
		}
	}

	public short RightBorderColor
	{
		get
		{
			return RightBorderXSSFColor?.Indexed ?? IndexedColors.Black.Index;
		}
		set
		{
			XSSFColor xSSFColor = new XSSFColor();
			xSSFColor.Indexed = value;
			SetRightBorderColor(xSSFColor);
		}
	}

	public XSSFColor RightBorderXSSFColor
	{
		get
		{
			if (!_cellXf.applyBorder)
			{
				return null;
			}
			int borderId = (int)_cellXf.borderId;
			return _stylesSource.GetBorderAt(borderId).GetBorderColor(BorderSide.RIGHT);
		}
	}

	public short Rotation
	{
		get
		{
			return (short)(_cellXf.alignment?.textRotation ?? 0);
		}
		set
		{
			GetCellAlignment().TextRotation = value;
		}
	}

	public short TopBorderColor
	{
		get
		{
			return TopBorderXSSFColor?.Indexed ?? IndexedColors.Black.Index;
		}
		set
		{
			XSSFColor xSSFColor = new XSSFColor();
			xSSFColor.Indexed = value;
			SetTopBorderColor(xSSFColor);
		}
	}

	public XSSFColor TopBorderXSSFColor
	{
		get
		{
			if (!_cellXf.applyBorder)
			{
				return null;
			}
			int borderId = (int)_cellXf.borderId;
			return _stylesSource.GetBorderAt(borderId).GetBorderColor(BorderSide.TOP);
		}
	}

	public VerticalAlignment VerticalAlignment
	{
		get
		{
			return GetVerticalAlignmentEnum();
		}
		set
		{
			GetCellAlignment().Vertical = value;
		}
	}

	public bool WrapText
	{
		get
		{
			return _cellXf.alignment?.wrapText ?? false;
		}
		set
		{
			GetCellAlignment().WrapText = value;
		}
	}

	private int FontId
	{
		get
		{
			if (_cellXf.IsSetFontId())
			{
				return (int)_cellXf.fontId;
			}
			return (int)_cellStyleXf.fontId;
		}
	}

	public bool ShrinkToFit
	{
		get
		{
			return _cellXf.alignment?.shrinkToFit ?? false;
		}
		set
		{
			GetCTCellAlignment().shrinkToFit = value;
		}
	}

	public short BorderDiagonalColor
	{
		get
		{
			return DiagonalBorderXSSFColor?.Indexed ?? IndexedColors.Black.Index;
		}
		set
		{
			XSSFColor xSSFColor = new XSSFColor();
			xSSFColor.Indexed = value;
			SetDiagonalBorderColor(xSSFColor);
		}
	}

	public BorderStyle BorderDiagonalLineStyle
	{
		get
		{
			if (!_cellXf.applyBorder)
			{
				return BorderStyle.None;
			}
			int borderId = (int)_cellXf.borderId;
			CT_Border cTBorder = _stylesSource.GetBorderAt(borderId).GetCTBorder();
			if (!cTBorder.IsSetDiagonal())
			{
				return BorderStyle.None;
			}
			return (BorderStyle)cTBorder.diagonal.style;
		}
		set
		{
			CT_Border cTBorder = GetCTBorder(copy: true);
			CT_BorderPr cT_BorderPr = (cTBorder.IsSetDiagonal() ? cTBorder.diagonal : cTBorder.AddNewDiagonal());
			if (value == BorderStyle.None)
			{
				cTBorder.unsetDiagonal();
			}
			else
			{
				cT_BorderPr.style = (ST_BorderStyle)value;
			}
			int borderId = _stylesSource.PutBorder(new XSSFCellBorder(cTBorder, _theme));
			_cellXf.borderId = (uint)borderId;
			_cellXf.applyBorder = true;
		}
	}

	public BorderDiagonal BorderDiagonal
	{
		get
		{
			CT_Border cTBorder = GetCTBorder();
			if (cTBorder.diagonalDown && cTBorder.diagonalUp)
			{
				return BorderDiagonal.Both;
			}
			if (cTBorder.diagonalDown)
			{
				return BorderDiagonal.Backward;
			}
			if (cTBorder.diagonalUp)
			{
				return BorderDiagonal.Forward;
			}
			return BorderDiagonal.None;
		}
		set
		{
			CT_Border cTBorder = GetCTBorder();
			switch (value)
			{
			case BorderDiagonal.Both:
				cTBorder.diagonalDown = true;
				cTBorder.diagonalDownSpecified = true;
				cTBorder.diagonalUp = true;
				cTBorder.diagonalUpSpecified = true;
				break;
			case BorderDiagonal.Forward:
				cTBorder.diagonalDown = false;
				cTBorder.diagonalDownSpecified = false;
				cTBorder.diagonalUp = true;
				cTBorder.diagonalUpSpecified = true;
				break;
			case BorderDiagonal.Backward:
				cTBorder.diagonalDown = true;
				cTBorder.diagonalDownSpecified = true;
				cTBorder.diagonalUp = false;
				cTBorder.diagonalUpSpecified = false;
				break;
			default:
				cTBorder.unsetDiagonal();
				cTBorder.diagonalDown = false;
				cTBorder.diagonalDownSpecified = false;
				cTBorder.diagonalUp = false;
				cTBorder.diagonalUpSpecified = false;
				break;
			}
		}
	}

	public XSSFCellStyle(int cellXfId, int cellStyleXfId, StylesTable stylesSource, ThemesTable theme)
	{
		_cellXfId = cellXfId;
		_stylesSource = stylesSource;
		_cellXf = stylesSource.GetCellXfAt(_cellXfId);
		_cellStyleXf = ((cellStyleXfId == -1) ? null : stylesSource.GetCellStyleXfAt(cellStyleXfId));
		_theme = theme;
	}

	public CT_Xf GetCoreXf()
	{
		return _cellXf;
	}

	public CT_Xf GetStyleXf()
	{
		return _cellStyleXf;
	}

	public XSSFCellStyle(StylesTable stylesSource)
	{
		_stylesSource = stylesSource;
		_cellXf = new CT_Xf();
		_cellStyleXf = null;
	}

	public void VerifyBelongsToStylesSource(StylesTable src)
	{
		if (_stylesSource != src)
		{
			throw new ArgumentException("This Style does not belong to the supplied Workbook Styles Source. Are you trying to assign a style from one workbook to the cell of a different workbook?");
		}
	}

	public void CloneStyleFrom(ICellStyle source)
	{
		if (source is XSSFCellStyle)
		{
			XSSFCellStyle xSSFCellStyle = (XSSFCellStyle)source;
			if (xSSFCellStyle._stylesSource == _stylesSource)
			{
				_cellXf = xSSFCellStyle.GetCoreXf().Copy();
				_cellStyleXf = xSSFCellStyle.GetStyleXf().Copy();
			}
			else
			{
				try
				{
					if (_cellXf.IsSetAlignment())
					{
						_cellXf.UnsetAlignment();
					}
					if (_cellXf.IsSetExtLst())
					{
						_cellXf.UnsetExtLst();
					}
					_cellXf = xSSFCellStyle.GetCoreXf().Copy();
					if (_cellXf.applyBorder)
					{
						_cellXf.borderId = FindAddBorder(xSSFCellStyle._stylesSource.GetBorderAt((int)_cellXf.borderId).GetCTBorder());
					}
					CT_Fill fill = CT_Fill.Parse(xSSFCellStyle.GetCTFill().ToString());
					AddFill(fill);
					CT_Border border = CT_Border.Parse(xSSFCellStyle.GetCTBorder().ToString());
					AddBorder(border);
					if (xSSFCellStyle._cellStyleXf.applyBorder)
					{
						_cellStyleXf.borderId = FindAddBorder(xSSFCellStyle.GetCTBorder());
					}
					_stylesSource.ReplaceCellXfAt(_cellXfId, _cellXf);
				}
				catch (XmlException ex)
				{
					throw new POIXMLException(ex);
				}
				string dataFormatString = xSSFCellStyle.GetDataFormatString();
				DataFormat = new XSSFDataFormat(_stylesSource).GetFormat(dataFormatString);
				try
				{
					XSSFFont xSSFFont = new XSSFFont(xSSFCellStyle.GetFont().GetCTFont().Clone());
					xSSFFont.RegisterTo(_stylesSource);
					SetFont(xSSFFont);
				}
				catch (XmlException ex2)
				{
					throw new POIXMLException(ex2);
				}
			}
			_font = null;
			_cellAlignment = null;
			return;
		}
		throw new ArgumentException("Can only clone from one XSSFCellStyle to another, not between HSSFCellStyle and XSSFCellStyle");
	}

	private void AddFill(CT_Fill fill)
	{
		int fillId = _stylesSource.PutFill(new XSSFCellFill(fill));
		_cellXf.fillId = (uint)fillId;
		_cellXf.applyFill = true;
	}

	private void AddBorder(CT_Border border)
	{
		int borderId = _stylesSource.PutBorder(new XSSFCellBorder(border, _theme));
		_cellXf.borderId = (uint)borderId;
		_cellXf.applyBorder = true;
	}

	private uint FindAddBorder(CT_Border border)
	{
		int hashCode = border.ToString().GetHashCode();
		uint num = 0u;
		foreach (XSSFCellBorder border2 in _stylesSource.GetBorders())
		{
			if (hashCode == border2.GetCTBorder().ToString().GetHashCode())
			{
				return num;
			}
			num++;
		}
		return (uint)_stylesSource.PutBorder(new XSSFCellBorder(border.Copy()));
	}

	internal HorizontalAlignment GetAlignmentEnum()
	{
		CT_CellAlignment alignment = _cellXf.alignment;
		if (alignment != null && alignment.IsSetHorizontal())
		{
			return (HorizontalAlignment)alignment.horizontal;
		}
		return HorizontalAlignment.General;
	}

	public void SetDataFormat(int fmt)
	{
		_cellXf.applyNumberFormat = true;
		_cellXf.numFmtId = (uint)fmt;
	}

	public string GetDataFormatString()
	{
		int dataFormat = DataFormat;
		return new XSSFDataFormat(_stylesSource).GetFormat((short)dataFormat);
	}

	public XSSFFont GetFont()
	{
		if (_font == null)
		{
			_font = _stylesSource.GetFontAt(FontId);
		}
		return _font;
	}

	internal VerticalAlignment GetVerticalAlignmentEnum()
	{
		CT_CellAlignment alignment = _cellXf.alignment;
		if (alignment != null && alignment.IsSetVertical())
		{
			return (VerticalAlignment)alignment.vertical;
		}
		return VerticalAlignment.Bottom;
	}

	public void SetBottomBorderColor(XSSFColor color)
	{
		CT_Border cTBorder = GetCTBorder(copy: true);
		if (color != null || cTBorder.IsSetBottom())
		{
			CT_BorderPr cT_BorderPr = (cTBorder.IsSetBottom() ? cTBorder.bottom : cTBorder.AddNewBottom());
			if (color != null)
			{
				cT_BorderPr.SetColor(color.GetCTColor());
			}
			else
			{
				cT_BorderPr.UnsetColor();
			}
			int borderId = _stylesSource.PutBorder(new XSSFCellBorder(cTBorder, _theme));
			_cellXf.borderId = (uint)borderId;
			_cellXf.applyBorder = true;
		}
	}

	public void SetFillBackgroundColor(XSSFColor color)
	{
		CT_Fill cTFill = GetCTFill();
		CT_PatternFill cT_PatternFill = cTFill.GetPatternFill();
		if (color == null)
		{
			cT_PatternFill?.UnsetBgColor();
		}
		else
		{
			if (cT_PatternFill == null)
			{
				cT_PatternFill = cTFill.AddNewPatternFill();
			}
			cT_PatternFill.bgColor = color.GetCTColor();
		}
		int fillId = _stylesSource.PutFill(new XSSFCellFill(cTFill));
		_cellXf.fillId = (uint)fillId;
		_cellXf.applyFill = true;
	}

	public void SetFillForegroundColor(XSSFColor color)
	{
		CT_Fill cTFill = GetCTFill();
		CT_PatternFill cT_PatternFill = cTFill.GetPatternFill();
		if (color == null)
		{
			cT_PatternFill?.UnsetFgColor();
		}
		else
		{
			if (cT_PatternFill == null)
			{
				cT_PatternFill = cTFill.AddNewPatternFill();
			}
			cT_PatternFill.fgColor = color.GetCTColor();
		}
		int fillId = _stylesSource.PutFill(new XSSFCellFill(cTFill));
		_cellXf.fillId = (uint)fillId;
		_cellXf.applyFill = true;
	}

	public CT_Fill GetCTFill()
	{
		if (!_cellXf.IsSetApplyFill() || _cellXf.applyFill)
		{
			int fillId = (int)_cellXf.fillId;
			return _stylesSource.GetFillAt(fillId).GetCTFill().Copy();
		}
		return new CT_Fill();
	}

	public CT_Border GetCTBorder(bool copy = false)
	{
		CT_Border cT_Border;
		if (_cellXf.applyBorder)
		{
			int borderId = (int)_cellXf.borderId;
			cT_Border = _stylesSource.GetBorderAt(borderId).GetCTBorder();
			if (copy)
			{
				cT_Border = cT_Border.Copy();
			}
		}
		else
		{
			cT_Border = new CT_Border();
			cT_Border.AddNewLeft();
			cT_Border.AddNewRight();
			cT_Border.AddNewTop();
			cT_Border.AddNewBottom();
			cT_Border.AddNewDiagonal();
		}
		return cT_Border;
	}

	public void SetFont(IFont font)
	{
		if (font != null)
		{
			long num = font.Index;
			_cellXf.fontId = (uint)num;
			_cellXf.fontIdSpecified = true;
			_cellXf.applyFont = true;
		}
		else
		{
			_cellXf.applyFont = false;
		}
	}

	public void SetDiagonalBorderColor(XSSFColor color)
	{
		CT_Border cTBorder = GetCTBorder(copy: true);
		if (color != null || cTBorder.IsSetDiagonal())
		{
			CT_BorderPr cT_BorderPr = (cTBorder.IsSetDiagonal() ? cTBorder.diagonal : cTBorder.AddNewDiagonal());
			if (color != null)
			{
				cT_BorderPr.color = color.GetCTColor();
			}
			else
			{
				cT_BorderPr.UnsetColor();
			}
			int borderId = _stylesSource.PutBorder(new XSSFCellBorder(cTBorder, _theme));
			_cellXf.borderId = (uint)borderId;
			_cellXf.applyBorder = true;
		}
	}

	public void SetLeftBorderColor(XSSFColor color)
	{
		CT_Border cTBorder = GetCTBorder(copy: true);
		if (color != null || cTBorder.IsSetLeft())
		{
			CT_BorderPr cT_BorderPr = (cTBorder.IsSetLeft() ? cTBorder.left : cTBorder.AddNewLeft());
			if (color != null)
			{
				cT_BorderPr.color = color.GetCTColor();
			}
			else
			{
				cT_BorderPr.UnsetColor();
			}
			int borderId = _stylesSource.PutBorder(new XSSFCellBorder(cTBorder, _theme));
			_cellXf.borderId = (uint)borderId;
			_cellXf.applyBorder = true;
		}
	}

	public void SetRightBorderColor(XSSFColor color)
	{
		CT_Border cTBorder = GetCTBorder(copy: true);
		if (color != null || cTBorder.IsSetRight())
		{
			CT_BorderPr cT_BorderPr = (cTBorder.IsSetRight() ? cTBorder.right : cTBorder.AddNewRight());
			if (color != null)
			{
				cT_BorderPr.color = color.GetCTColor();
			}
			else
			{
				cT_BorderPr.UnsetColor();
			}
			int borderId = _stylesSource.PutBorder(new XSSFCellBorder(cTBorder, _theme));
			_cellXf.borderId = (uint)borderId;
			_cellXf.applyBorder = true;
		}
	}

	public void SetTopBorderColor(XSSFColor color)
	{
		CT_Border cTBorder = GetCTBorder(copy: true);
		if (color != null || cTBorder.IsSetTop())
		{
			CT_BorderPr cT_BorderPr = (cTBorder.IsSetTop() ? cTBorder.top : cTBorder.AddNewTop());
			if (color != null)
			{
				cT_BorderPr.color = color.GetCTColor();
			}
			else
			{
				cT_BorderPr.UnsetColor();
			}
			int borderId = _stylesSource.PutBorder(new XSSFCellBorder(cTBorder, _theme));
			_cellXf.borderId = (uint)borderId;
			_cellXf.applyBorder = true;
		}
	}

	public void SetVerticalAlignment(short align)
	{
		GetCellAlignment().Vertical = (VerticalAlignment)align;
	}

	public XSSFColor GetBorderColor(BorderSide side)
	{
		return side switch
		{
			BorderSide.BOTTOM => BottomBorderXSSFColor, 
			BorderSide.RIGHT => RightBorderXSSFColor, 
			BorderSide.TOP => TopBorderXSSFColor, 
			BorderSide.LEFT => LeftBorderXSSFColor, 
			_ => throw new ArgumentException("Unknown border: " + side), 
		};
	}

	public void SetBorderColor(BorderSide side, XSSFColor color)
	{
		switch (side)
		{
		case BorderSide.BOTTOM:
			SetBottomBorderColor(color);
			break;
		case BorderSide.RIGHT:
			SetRightBorderColor(color);
			break;
		case BorderSide.TOP:
			SetTopBorderColor(color);
			break;
		case BorderSide.LEFT:
			SetLeftBorderColor(color);
			break;
		}
	}

	internal XSSFCellAlignment GetCellAlignment()
	{
		if (_cellAlignment == null)
		{
			_cellAlignment = new XSSFCellAlignment(GetCTCellAlignment());
		}
		return _cellAlignment;
	}

	internal CT_CellAlignment GetCTCellAlignment()
	{
		if (_cellXf.alignment == null)
		{
			_cellXf.alignment = new CT_CellAlignment();
		}
		return _cellXf.alignment;
	}

	public override int GetHashCode()
	{
		return _cellXf.ToString().GetHashCode();
	}

	public override bool Equals(object o)
	{
		if (o == null || !(o is XSSFCellStyle))
		{
			return false;
		}
		XSSFCellStyle xSSFCellStyle = (XSSFCellStyle)o;
		return _cellXf.ToString().Equals(xSSFCellStyle.GetCoreXf().ToString());
	}

	public object Clone()
	{
		CT_Xf cellXf = _cellXf.Copy();
		int styleXfsSize = _stylesSource.StyleXfsSize;
		return new XSSFCellStyle(_stylesSource.PutCellXf(cellXf) - 1, styleXfsSize - 1, _stylesSource, _theme);
	}

	public IFont GetFont(IWorkbook parentWorkbook)
	{
		return GetFont();
	}
}
