using System.Collections.Generic;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFIconMultiStateFormatting : IIconMultiStateFormatting
{
	private CT_IconSet _iconset;

	public IconSet IconSet
	{
		get
		{
			return IconSet.ByOOXMLName(_iconset.iconSet.ToString());
		}
		set
		{
			ST_IconSetType iconSet = XmlEnumParser<ST_IconSetType>.ForName(value.name, ST_IconSetType.Item3TrafficLights1);
			_iconset.iconSet = iconSet;
		}
	}

	public bool IsIconOnly
	{
		get
		{
			if (_iconset.IsSetShowValue())
			{
				return !_iconset.showValue;
			}
			return false;
		}
		set
		{
			_iconset.showValue = !value;
		}
	}

	public bool IsReversed
	{
		get
		{
			if (_iconset.reverse)
			{
				return _iconset.reverse;
			}
			return false;
		}
		set
		{
			_iconset.reverse = value;
		}
	}

	public IConditionalFormattingThreshold[] Thresholds
	{
		get
		{
			CT_Cfvo[] array = _iconset.cfvo.ToArray();
			XSSFConditionalFormattingThreshold[] array2 = new XSSFConditionalFormattingThreshold[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = new XSSFConditionalFormattingThreshold(array[i]);
			}
			return array2;
		}
		set
		{
			CT_Cfvo[] array = new CT_Cfvo[value.Length];
			for (int i = 0; i < value.Length; i++)
			{
				array[i] = ((XSSFConditionalFormattingThreshold)value[i]).CTCfvo;
			}
			_iconset.cfvo = new List<CT_Cfvo>(array);
		}
	}

	internal XSSFIconMultiStateFormatting(CT_IconSet iconset)
	{
		_iconset = iconset;
	}

	public IConditionalFormattingThreshold CreateThreshold()
	{
		return new XSSFConditionalFormattingThreshold(_iconset.AddNewCfvo());
	}
}
