using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFBorderFormatting : IBorderFormatting
{
	private CT_Border _border;

	public BorderStyle BorderBottom
	{
		get
		{
			if (!_border.IsSetBottom())
			{
				return BorderStyle.None;
			}
			return (BorderStyle)_border.bottom.style;
		}
		set
		{
			CT_BorderPr cT_BorderPr = (_border.IsSetBottom() ? _border.bottom : _border.AddNewBottom());
			if (value == BorderStyle.None)
			{
				_border.UnsetBottom();
			}
			else
			{
				cT_BorderPr.style = (ST_BorderStyle)value;
			}
		}
	}

	public BorderStyle BorderDiagonal
	{
		get
		{
			if (!_border.IsSetDiagonal())
			{
				return BorderStyle.None;
			}
			return (BorderStyle)_border.diagonal.style;
		}
		set
		{
			CT_BorderPr cT_BorderPr = (_border.IsSetDiagonal() ? _border.diagonal : _border.AddNewDiagonal());
			if (value == BorderStyle.None)
			{
				_border.unsetDiagonal();
			}
			else
			{
				cT_BorderPr.style = (ST_BorderStyle)value;
			}
		}
	}

	public BorderStyle BorderLeft
	{
		get
		{
			if (!_border.IsSetLeft())
			{
				return BorderStyle.None;
			}
			return (BorderStyle)_border.left.style;
		}
		set
		{
			CT_BorderPr cT_BorderPr = (_border.IsSetLeft() ? _border.left : _border.AddNewLeft());
			if (value == BorderStyle.None)
			{
				_border.unsetLeft();
			}
			else
			{
				cT_BorderPr.style = (ST_BorderStyle)value;
			}
		}
	}

	public BorderStyle BorderRight
	{
		get
		{
			if (!_border.IsSetRight())
			{
				return BorderStyle.None;
			}
			return (BorderStyle)_border.right.style;
		}
		set
		{
			CT_BorderPr cT_BorderPr = (_border.IsSetRight() ? _border.right : _border.AddNewRight());
			if (value == BorderStyle.None)
			{
				_border.unsetRight();
			}
			else
			{
				cT_BorderPr.style = (ST_BorderStyle)value;
			}
		}
	}

	public BorderStyle BorderTop
	{
		get
		{
			if (!_border.IsSetTop())
			{
				return BorderStyle.None;
			}
			return (BorderStyle)_border.top.style;
		}
		set
		{
			CT_BorderPr cT_BorderPr = (_border.IsSetTop() ? _border.top : _border.AddNewTop());
			if (value == BorderStyle.None)
			{
				_border.unsetTop();
			}
			else
			{
				cT_BorderPr.style = (ST_BorderStyle)value;
			}
		}
	}

	public short BottomBorderColor
	{
		get
		{
			if (!(BottomBorderColorColor is XSSFColor xSSFColor))
			{
				return 0;
			}
			return xSSFColor.Indexed;
		}
		set
		{
			CT_Color cT_Color = new CT_Color();
			cT_Color.indexed = (uint)value;
			cT_Color.indexedSpecified = true;
			setBottomBorderColor(cT_Color);
		}
	}

	public short DiagonalBorderColor
	{
		get
		{
			if (!(DiagonalBorderColorColor is XSSFColor xSSFColor))
			{
				return 0;
			}
			return xSSFColor.Indexed;
		}
		set
		{
			CT_Color cT_Color = new CT_Color();
			cT_Color.indexed = (uint)value;
			cT_Color.indexedSpecified = true;
			setDiagonalBorderColor(cT_Color);
		}
	}

	public short LeftBorderColor
	{
		get
		{
			if (!(LeftBorderColorColor is XSSFColor xSSFColor))
			{
				return 0;
			}
			return xSSFColor.Indexed;
		}
		set
		{
			CT_Color cT_Color = new CT_Color();
			cT_Color.indexed = (uint)value;
			cT_Color.indexedSpecified = true;
			setLeftBorderColor(cT_Color);
		}
	}

	public short RightBorderColor
	{
		get
		{
			if (!(RightBorderColorColor is XSSFColor xSSFColor))
			{
				return 0;
			}
			return xSSFColor.Indexed;
		}
		set
		{
			CT_Color cT_Color = new CT_Color();
			cT_Color.indexed = (uint)value;
			cT_Color.indexedSpecified = true;
			setRightBorderColor(cT_Color);
		}
	}

	public short TopBorderColor
	{
		get
		{
			if (!(RightBorderColorColor is XSSFColor xSSFColor))
			{
				return 0;
			}
			return xSSFColor.Indexed;
		}
		set
		{
			CT_Color cT_Color = new CT_Color();
			cT_Color.indexed = (uint)value;
			cT_Color.indexedSpecified = true;
			setTopBorderColor(cT_Color);
		}
	}

	public IColor BottomBorderColorColor
	{
		get
		{
			if (!_border.IsSetBottom())
			{
				return null;
			}
			return new XSSFColor(_border.bottom.color);
		}
		set
		{
			XSSFColor xSSFColor = XSSFColor.ToXSSFColor(value);
			if (xSSFColor == null)
			{
				setBottomBorderColor(null);
			}
			else
			{
				setBottomBorderColor(xSSFColor.GetCTColor());
			}
		}
	}

	public IColor DiagonalBorderColorColor
	{
		get
		{
			if (!_border.IsSetDiagonal())
			{
				return null;
			}
			return new XSSFColor(_border.diagonal.color);
		}
		set
		{
			XSSFColor xSSFColor = XSSFColor.ToXSSFColor(value);
			if (xSSFColor == null)
			{
				setDiagonalBorderColor(null);
			}
			else
			{
				setDiagonalBorderColor(xSSFColor.GetCTColor());
			}
		}
	}

	public IColor LeftBorderColorColor
	{
		get
		{
			if (!_border.IsSetLeft())
			{
				return null;
			}
			return new XSSFColor(_border.left.color);
		}
		set
		{
			XSSFColor xSSFColor = XSSFColor.ToXSSFColor(value);
			if (xSSFColor == null)
			{
				setLeftBorderColor(null);
			}
			else
			{
				setLeftBorderColor(xSSFColor.GetCTColor());
			}
		}
	}

	public IColor RightBorderColorColor
	{
		get
		{
			if (!_border.IsSetRight())
			{
				return null;
			}
			return new XSSFColor(_border.right.color);
		}
		set
		{
			XSSFColor xSSFColor = XSSFColor.ToXSSFColor(value);
			if (xSSFColor == null)
			{
				setRightBorderColor(null);
			}
			else
			{
				setRightBorderColor(xSSFColor.GetCTColor());
			}
		}
	}

	public IColor TopBorderColorColor
	{
		get
		{
			if (!_border.IsSetTop())
			{
				return null;
			}
			return new XSSFColor(_border.top.color);
		}
		set
		{
			XSSFColor xSSFColor = XSSFColor.ToXSSFColor(value);
			if (xSSFColor == null)
			{
				setTopBorderColor(null);
			}
			else
			{
				setTopBorderColor(xSSFColor.GetCTColor());
			}
		}
	}

	internal XSSFBorderFormatting(CT_Border border)
	{
		_border = border;
	}

	public void setBottomBorderColor(CT_Color color)
	{
		CT_BorderPr cT_BorderPr = (_border.IsSetBottom() ? _border.bottom : _border.AddNewBottom());
		if (color == null)
		{
			cT_BorderPr.UnsetColor();
		}
		else
		{
			cT_BorderPr.color = color;
		}
	}

	public void setDiagonalBorderColor(CT_Color color)
	{
		CT_BorderPr cT_BorderPr = (_border.IsSetDiagonal() ? _border.diagonal : _border.AddNewDiagonal());
		if (color == null)
		{
			cT_BorderPr.UnsetColor();
		}
		else
		{
			cT_BorderPr.color = color;
		}
	}

	public void setLeftBorderColor(CT_Color color)
	{
		CT_BorderPr cT_BorderPr = (_border.IsSetLeft() ? _border.left : _border.AddNewLeft());
		if (color == null)
		{
			cT_BorderPr.UnsetColor();
		}
		else
		{
			cT_BorderPr.color = color;
		}
	}

	public void setRightBorderColor(CT_Color color)
	{
		CT_BorderPr cT_BorderPr = (_border.IsSetRight() ? _border.right : _border.AddNewRight());
		if (color == null)
		{
			cT_BorderPr.UnsetColor();
		}
		else
		{
			cT_BorderPr.color = color;
		}
	}

	public void setTopBorderColor(CT_Color color)
	{
		CT_BorderPr cT_BorderPr = (_border.IsSetTop() ? _border.top : _border.AddNewTop());
		if (color == null)
		{
			cT_BorderPr.UnsetColor();
		}
		else
		{
			cT_BorderPr.color = color;
		}
	}
}
