using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.Spreadsheet;

namespace NPOI.XSSF.Model;

public abstract class ParagraphPropertyFetcher
{
	public abstract bool Fetch(CT_TextParagraphProperties props);

	public abstract bool Fetch(CT_Shape props);
}
public abstract class ParagraphPropertyFetcher<T> : ParagraphPropertyFetcher
{
	private T _value;

	private int _level;

	public T GetValue()
	{
		return _value;
	}

	public void SetValue(T val)
	{
		_value = val;
	}

	public ParagraphPropertyFetcher(int level)
	{
		_level = level;
	}

	public override bool Fetch(CT_Shape shape)
	{
		if (shape != null && shape.txBody != null && shape.txBody.lstStyle != null)
		{
			CT_TextParagraphProperties cT_TextParagraphProperties = _level switch
			{
				0 => shape.txBody.lstStyle.lvl1pPr, 
				1 => shape.txBody.lstStyle.lvl2pPr, 
				2 => shape.txBody.lstStyle.lvl3pPr, 
				3 => shape.txBody.lstStyle.lvl4pPr, 
				4 => shape.txBody.lstStyle.lvl5pPr, 
				5 => shape.txBody.lstStyle.lvl6pPr, 
				6 => shape.txBody.lstStyle.lvl7pPr, 
				7 => shape.txBody.lstStyle.lvl8pPr, 
				8 => shape.txBody.lstStyle.lvl9pPr, 
				_ => null, 
			};
			if (cT_TextParagraphProperties != null)
			{
				return Fetch(cT_TextParagraphProperties);
			}
		}
		return false;
	}
}
