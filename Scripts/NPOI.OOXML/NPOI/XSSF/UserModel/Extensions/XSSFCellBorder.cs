using System;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.UserModel.Extensions;

public class XSSFCellBorder
{
	private ThemesTable _theme;

	private CT_Border border;

	public XSSFCellBorder(CT_Border border, ThemesTable theme)
		: this(border)
	{
		_theme = theme;
	}

	public XSSFCellBorder(CT_Border border)
	{
		this.border = border;
	}

	public XSSFCellBorder()
	{
		border = new CT_Border();
	}

	public void SetThemesTable(ThemesTable themes)
	{
		_theme = themes;
	}

	public CT_Border GetCTBorder()
	{
		return border;
	}

	public BorderStyle GetBorderStyle(BorderSide side)
	{
		ST_BorderStyle? sT_BorderStyle = GetBorder(side)?.style ?? ST_BorderStyle.none;
		return (BorderStyle)sT_BorderStyle.Value;
	}

	public void SetBorderStyle(BorderSide side, BorderStyle style)
	{
		GetBorder(side, ensure: true).style = (ST_BorderStyle)Enum.GetValues(typeof(ST_BorderStyle)).GetValue((int)(style + 1));
	}

	public XSSFColor GetBorderColor(BorderSide side)
	{
		CT_BorderPr cT_BorderPr = GetBorder(side);
		if (cT_BorderPr != null && cT_BorderPr.IsSetColor())
		{
			XSSFColor xSSFColor = new XSSFColor(cT_BorderPr.color);
			if (_theme != null)
			{
				_theme.InheritFromThemeAsRequired(xSSFColor);
			}
			return xSSFColor;
		}
		return null;
	}

	public void SetBorderColor(BorderSide side, XSSFColor color)
	{
		CT_BorderPr cT_BorderPr = GetBorder(side, ensure: true);
		if (color == null)
		{
			cT_BorderPr.UnsetColor();
		}
		else
		{
			cT_BorderPr.color = color.GetCTColor();
		}
	}

	private CT_BorderPr GetBorder(BorderSide side)
	{
		return GetBorder(side, ensure: false);
	}

	private CT_BorderPr GetBorder(BorderSide side, bool ensure)
	{
		CT_BorderPr cT_BorderPr;
		switch (side)
		{
		case BorderSide.TOP:
			cT_BorderPr = border.top;
			if (ensure && cT_BorderPr == null)
			{
				cT_BorderPr = border.AddNewTop();
			}
			break;
		case BorderSide.RIGHT:
			cT_BorderPr = border.right;
			if (ensure && cT_BorderPr == null)
			{
				cT_BorderPr = border.AddNewRight();
			}
			break;
		case BorderSide.BOTTOM:
			cT_BorderPr = border.bottom;
			if (ensure && cT_BorderPr == null)
			{
				cT_BorderPr = border.AddNewBottom();
			}
			break;
		case BorderSide.LEFT:
			cT_BorderPr = border.left;
			if (ensure && cT_BorderPr == null)
			{
				cT_BorderPr = border.AddNewLeft();
			}
			break;
		case BorderSide.DIAGONAL:
			cT_BorderPr = border.diagonal;
			if (ensure && cT_BorderPr == null)
			{
				cT_BorderPr = border.AddNewDiagonal();
			}
			break;
		default:
			throw new ArgumentException("No suitable side specified for the border");
		}
		return cT_BorderPr;
	}

	public override int GetHashCode()
	{
		return border.ToString().GetHashCode();
	}

	public override bool Equals(object o)
	{
		if (!(o is XSSFCellBorder))
		{
			return false;
		}
		XSSFCellBorder xSSFCellBorder = (XSSFCellBorder)o;
		return border.ToString().Equals(xSSFCellBorder.GetCTBorder().ToString());
	}
}
