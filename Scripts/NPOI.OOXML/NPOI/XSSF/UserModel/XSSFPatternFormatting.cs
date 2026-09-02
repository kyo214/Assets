using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFPatternFormatting : IPatternFormatting
{
	private CT_Fill _fill;

	public IColor FillBackgroundColorColor
	{
		get
		{
			if (!_fill.IsSetPatternFill())
			{
				return null;
			}
			return new XSSFColor(_fill.GetPatternFill().bgColor);
		}
		set
		{
			XSSFColor xSSFColor = XSSFColor.ToXSSFColor(value);
			if (value == null)
			{
				SetFillBackgroundColor(null);
			}
			else
			{
				SetFillBackgroundColor(xSSFColor.GetCTColor());
			}
		}
	}

	public IColor FillForegroundColorColor
	{
		get
		{
			if (!_fill.IsSetPatternFill() || !_fill.GetPatternFill().IsSetFgColor())
			{
				return null;
			}
			return new XSSFColor(_fill.GetPatternFill().bgColor);
		}
		set
		{
			XSSFColor xSSFColor = XSSFColor.ToXSSFColor(value);
			if (value == null)
			{
				SetFillForegroundColor(null);
			}
			else
			{
				SetFillForegroundColor(xSSFColor.GetCTColor());
			}
		}
	}

	public short FillBackgroundColor
	{
		get
		{
			if (!_fill.IsSetPatternFill())
			{
				return 0;
			}
			if (!_fill.GetPatternFill().bgColor.indexedSpecified)
			{
				return 0;
			}
			return (short)_fill.GetPatternFill().bgColor.indexed;
		}
		set
		{
			(_fill.IsSetPatternFill() ? _fill.GetPatternFill() : _fill.AddNewPatternFill()).bgColor = new CT_Color
			{
				indexed = (uint)value,
				indexedSpecified = true
			};
		}
	}

	public short FillForegroundColor
	{
		get
		{
			if (!_fill.IsSetPatternFill() || !_fill.GetPatternFill().IsSetFgColor())
			{
				return 0;
			}
			if (!_fill.GetPatternFill().fgColor.indexedSpecified)
			{
				return 0;
			}
			return (short)_fill.GetPatternFill().fgColor.indexed;
		}
		set
		{
			(_fill.IsSetPatternFill() ? _fill.GetPatternFill() : _fill.AddNewPatternFill()).fgColor = new CT_Color
			{
				indexed = (uint)value,
				indexedSpecified = true
			};
		}
	}

	public FillPattern FillPattern
	{
		get
		{
			if (!_fill.IsSetPatternFill() || !_fill.GetPatternFill().IsSetPatternType())
			{
				return FillPattern.NoFill;
			}
			return (FillPattern)_fill.GetPatternFill().patternType.Value;
		}
		set
		{
			(_fill.IsSetPatternFill() ? _fill.GetPatternFill() : _fill.AddNewPatternFill()).patternType = (ST_PatternType)value;
		}
	}

	public XSSFPatternFormatting(CT_Fill fill)
	{
		_fill = fill;
	}

	private void SetFillBackgroundColor(CT_Color color)
	{
		CT_PatternFill cT_PatternFill = (_fill.IsSetPatternFill() ? _fill.patternFill : _fill.AddNewPatternFill());
		if (color == null)
		{
			cT_PatternFill.UnsetBgColor();
		}
		else
		{
			cT_PatternFill.bgColor = color;
		}
	}

	private void SetFillForegroundColor(CT_Color color)
	{
		CT_PatternFill cT_PatternFill = (_fill.IsSetPatternFill() ? _fill.GetPatternFill() : _fill.AddNewPatternFill());
		if (color == null)
		{
			cT_PatternFill.UnsetFgColor();
		}
		else
		{
			cT_PatternFill.fgColor = color;
		}
	}
}
