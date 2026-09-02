using System;
using System.Globalization;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFConditionalFormattingThreshold : IConditionalFormattingThreshold
{
	private CT_Cfvo cfvo;

	protected internal CT_Cfvo CTCfvo => cfvo;

	public RangeType RangeType
	{
		get
		{
			return RangeType.ByName(cfvo.type.ToString());
		}
		set
		{
			ST_CfvoType type = (ST_CfvoType)Enum.Parse(typeof(ST_CfvoType), value.name);
			cfvo.type = type;
		}
	}

	public string Formula
	{
		get
		{
			if (cfvo.type == ST_CfvoType.formula)
			{
				return cfvo.val;
			}
			return null;
		}
		set
		{
			cfvo.val = value;
		}
	}

	public double? Value
	{
		get
		{
			if (cfvo.type == ST_CfvoType.formula || cfvo.type == ST_CfvoType.min || cfvo.type == ST_CfvoType.max || cfvo.type == ST_CfvoType.autoMax || cfvo.type == ST_CfvoType.autoMin)
			{
				return null;
			}
			if (cfvo.IsSetVal())
			{
				return double.Parse(cfvo.val, CultureInfo.InvariantCulture);
			}
			return null;
		}
		set
		{
			if (!value.HasValue)
			{
				cfvo.UnsetVal();
			}
			else
			{
				cfvo.val = value.Value.ToString(CultureInfo.InvariantCulture);
			}
		}
	}

	protected internal XSSFConditionalFormattingThreshold(CT_Cfvo cfvo)
	{
		this.cfvo = cfvo;
	}
}
