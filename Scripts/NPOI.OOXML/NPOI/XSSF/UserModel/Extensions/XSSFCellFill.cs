using NPOI.OpenXmlFormats.Spreadsheet;

namespace NPOI.XSSF.UserModel.Extensions;

public class XSSFCellFill
{
	private CT_Fill _fill;

	public XSSFCellFill(CT_Fill fill)
	{
		_fill = fill;
	}

	public XSSFCellFill()
	{
		_fill = new CT_Fill();
	}

	public XSSFColor GetFillBackgroundColor()
	{
		CT_PatternFill patternFill = _fill.GetPatternFill();
		if (patternFill == null)
		{
			return null;
		}
		CT_Color bgColor = patternFill.bgColor;
		if (bgColor != null)
		{
			return new XSSFColor(bgColor);
		}
		return null;
	}

	public void SetFillBackgroundColor(int index)
	{
		CT_PatternFill cT_PatternFill = EnsureCTPatternFill();
		CT_Color obj = (cT_PatternFill.IsSetBgColor() ? cT_PatternFill.bgColor : cT_PatternFill.AddNewBgColor());
		obj.indexed = (uint)index;
		obj.indexedSpecified = true;
	}

	public void SetFillBackgroundColor(XSSFColor color)
	{
		EnsureCTPatternFill().bgColor = color.GetCTColor();
	}

	public XSSFColor GetFillForegroundColor()
	{
		CT_PatternFill patternFill = _fill.GetPatternFill();
		if (patternFill == null)
		{
			return null;
		}
		CT_Color fgColor = patternFill.fgColor;
		if (fgColor != null)
		{
			return new XSSFColor(fgColor);
		}
		return null;
	}

	public void SetFillForegroundColor(int index)
	{
		CT_PatternFill cT_PatternFill = EnsureCTPatternFill();
		(cT_PatternFill.IsSetFgColor() ? cT_PatternFill.fgColor : cT_PatternFill.AddNewFgColor()).indexed = (uint)index;
	}

	public void SetFillForegroundColor(XSSFColor color)
	{
		EnsureCTPatternFill().fgColor = color.GetCTColor();
	}

	public ST_PatternType GetPatternType()
	{
		return _fill.GetPatternFill()?.patternType.Value ?? ST_PatternType.none;
	}

	public void SetPatternType(ST_PatternType patternType)
	{
		EnsureCTPatternFill().patternType = patternType;
	}

	private CT_PatternFill EnsureCTPatternFill()
	{
		CT_PatternFill cT_PatternFill = _fill.GetPatternFill();
		if (cT_PatternFill == null)
		{
			cT_PatternFill = _fill.AddNewPatternFill();
		}
		return cT_PatternFill;
	}

	internal CT_Fill GetCTFill()
	{
		return _fill;
	}

	public override int GetHashCode()
	{
		return _fill.ToString().GetHashCode();
	}

	public override bool Equals(object o)
	{
		if (!(o is XSSFCellFill))
		{
			return false;
		}
		XSSFCellFill xSSFCellFill = (XSSFCellFill)o;
		return _fill.ToString().Equals(xSSFCellFill.GetCTFill().ToString());
	}
}
