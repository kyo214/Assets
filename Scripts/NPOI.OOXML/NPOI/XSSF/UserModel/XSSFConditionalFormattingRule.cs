using System;
using System.Collections.Generic;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.Model;
using NPOI.XSSF.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFConditionalFormattingRule : IConditionalFormattingRule
{
	private CT_CfRule _cfRule;

	private XSSFSheet _sh;

	private static Dictionary<ST_CfType, ConditionType> typeLookup;

	public IBorderFormatting BorderFormatting
	{
		get
		{
			CT_Dxf dxf = GetDxf(create: false);
			if (dxf == null || !dxf.IsSetBorder())
			{
				return null;
			}
			return new XSSFBorderFormatting(dxf.border);
		}
	}

	public IFontFormatting FontFormatting
	{
		get
		{
			CT_Dxf dxf = GetDxf(create: false);
			if (dxf == null || !dxf.IsSetFont())
			{
				return null;
			}
			return new XSSFFontFormatting(dxf.font);
		}
	}

	public IPatternFormatting PatternFormatting
	{
		get
		{
			CT_Dxf dxf = GetDxf(create: false);
			if (dxf == null || !dxf.IsSetFill())
			{
				return null;
			}
			return new XSSFPatternFormatting(dxf.fill);
		}
	}

	public IDataBarFormatting DataBarFormatting
	{
		get
		{
			if (_cfRule.IsSetDataBar())
			{
				return new XSSFDataBarFormatting(_cfRule.dataBar);
			}
			return null;
		}
	}

	public IIconMultiStateFormatting MultiStateFormatting
	{
		get
		{
			if (_cfRule.IsSetIconSet())
			{
				return new XSSFIconMultiStateFormatting(_cfRule.iconSet);
			}
			return null;
		}
	}

	public IColorScaleFormatting ColorScaleFormatting
	{
		get
		{
			if (_cfRule.IsSetColorScale())
			{
				return new XSSFColorScaleFormatting(_cfRule.colorScale);
			}
			return null;
		}
	}

	public ConditionType ConditionType => typeLookup[_cfRule.type];

	public ComparisonOperator ComparisonOperation
	{
		get
		{
			ST_ConditionalFormattingOperator? sT_ConditionalFormattingOperator = _cfRule.@operator;
			if (!sT_ConditionalFormattingOperator.HasValue)
			{
				return ComparisonOperator.NoComparison;
			}
			return sT_ConditionalFormattingOperator switch
			{
				ST_ConditionalFormattingOperator.lessThan => ComparisonOperator.LessThan, 
				ST_ConditionalFormattingOperator.lessThanOrEqual => ComparisonOperator.LessThanOrEqual, 
				ST_ConditionalFormattingOperator.greaterThan => ComparisonOperator.GreaterThan, 
				ST_ConditionalFormattingOperator.greaterThanOrEqual => ComparisonOperator.GreaterThanOrEqual, 
				ST_ConditionalFormattingOperator.equal => ComparisonOperator.Equal, 
				ST_ConditionalFormattingOperator.notEqual => ComparisonOperator.NotEqual, 
				ST_ConditionalFormattingOperator.between => ComparisonOperator.Between, 
				ST_ConditionalFormattingOperator.notBetween => ComparisonOperator.NotBetween, 
				_ => ComparisonOperator.NoComparison, 
			};
		}
	}

	public string Formula1
	{
		get
		{
			if (_cfRule.SizeOfFormulaArray() <= 0)
			{
				return null;
			}
			return _cfRule.GetFormulaArray(0);
		}
	}

	public string Formula2
	{
		get
		{
			if (_cfRule.SizeOfFormulaArray() != 2)
			{
				return null;
			}
			return _cfRule.GetFormulaArray(1);
		}
	}

	static XSSFConditionalFormattingRule()
	{
		typeLookup = new Dictionary<ST_CfType, ConditionType>();
		typeLookup.Add(ST_CfType.cellIs, ConditionType.CellValueIs);
		typeLookup.Add(ST_CfType.expression, ConditionType.Formula);
		typeLookup.Add(ST_CfType.colorScale, ConditionType.ColorScale);
		typeLookup.Add(ST_CfType.dataBar, ConditionType.DataBar);
		typeLookup.Add(ST_CfType.iconSet, ConditionType.IconSet);
		typeLookup.Add(ST_CfType.top10, ConditionType.Filter);
		typeLookup.Add(ST_CfType.uniqueValues, ConditionType.Filter);
		typeLookup.Add(ST_CfType.duplicateValues, ConditionType.Filter);
		typeLookup.Add(ST_CfType.containsText, ConditionType.Filter);
		typeLookup.Add(ST_CfType.notContainsText, ConditionType.Filter);
		typeLookup.Add(ST_CfType.beginsWith, ConditionType.Filter);
		typeLookup.Add(ST_CfType.endsWith, ConditionType.Filter);
		typeLookup.Add(ST_CfType.containsBlanks, ConditionType.Filter);
		typeLookup.Add(ST_CfType.notContainsBlanks, ConditionType.Filter);
		typeLookup.Add(ST_CfType.containsErrors, ConditionType.Filter);
		typeLookup.Add(ST_CfType.notContainsErrors, ConditionType.Filter);
		typeLookup.Add(ST_CfType.timePeriod, ConditionType.Filter);
		typeLookup.Add(ST_CfType.aboveAverage, ConditionType.Filter);
	}

	public XSSFConditionalFormattingRule(XSSFSheet sh)
	{
		_cfRule = new CT_CfRule();
		_sh = sh;
	}

	internal XSSFConditionalFormattingRule(XSSFSheet sh, CT_CfRule cfRule)
	{
		_cfRule = cfRule;
		_sh = sh;
	}

	internal CT_CfRule GetCTCfRule()
	{
		return _cfRule;
	}

	internal CT_Dxf GetDxf(bool create)
	{
		StylesTable stylesSource = ((XSSFWorkbook)_sh.Workbook).GetStylesSource();
		CT_Dxf cT_Dxf = null;
		if (stylesSource.DXfsSize > 0 && _cfRule.IsSetDxfId())
		{
			int dxfId = (int)_cfRule.dxfId;
			cT_Dxf = stylesSource.GetDxfAt(dxfId);
		}
		if (create && cT_Dxf == null)
		{
			cT_Dxf = new CT_Dxf();
			int num = stylesSource.PutDxf(cT_Dxf);
			_cfRule.dxfId = (uint)(num - 1);
		}
		return cT_Dxf;
	}

	public IBorderFormatting CreateBorderFormatting()
	{
		CT_Dxf dxf = GetDxf(create: true);
		CT_Border border = (dxf.IsSetBorder() ? dxf.border : dxf.AddNewBorder());
		return new XSSFBorderFormatting(border);
	}

	public IFontFormatting CreateFontFormatting()
	{
		CT_Dxf dxf = GetDxf(create: true);
		CT_Font font = (dxf.IsSetFont() ? dxf.font : dxf.AddNewFont());
		return new XSSFFontFormatting(font);
	}

	public IPatternFormatting CreatePatternFormatting()
	{
		CT_Dxf dxf = GetDxf(create: true);
		CT_Fill fill = (dxf.IsSetFill() ? dxf.fill : dxf.AddNewFill());
		return new XSSFPatternFormatting(fill);
	}

	public XSSFDataBarFormatting CreateDataBarFormatting(XSSFColor color)
	{
		if (_cfRule.IsSetDataBar() && _cfRule.type == ST_CfType.dataBar)
		{
			return DataBarFormatting as XSSFDataBarFormatting;
		}
		_cfRule.type = ST_CfType.dataBar;
		CT_DataBar cT_DataBar = null;
		cT_DataBar = ((!_cfRule.IsSetDataBar()) ? _cfRule.AddNewDataBar() : _cfRule.dataBar);
		cT_DataBar.color = color.GetCTColor();
		cT_DataBar.AddNewCfvo().type = (ST_CfvoType)Enum.Parse(typeof(ST_CfvoType), RangeType.MIN.name);
		cT_DataBar.AddNewCfvo().type = (ST_CfvoType)Enum.Parse(typeof(ST_CfvoType), RangeType.MAX.name);
		return new XSSFDataBarFormatting(cT_DataBar);
	}

	public XSSFIconMultiStateFormatting CreateMultiStateFormatting(IconSet iconSet)
	{
		if (_cfRule.IsSetIconSet() && _cfRule.type == ST_CfType.iconSet)
		{
			return MultiStateFormatting as XSSFIconMultiStateFormatting;
		}
		_cfRule.type = ST_CfType.iconSet;
		CT_IconSet cT_IconSet = null;
		cT_IconSet = ((!_cfRule.IsSetIconSet()) ? _cfRule.AddNewIconSet() : _cfRule.iconSet);
		if (iconSet.name != null)
		{
			ST_IconSetType iconSet2 = XmlEnumParser<ST_IconSetType>.ForName(iconSet.name, ST_IconSetType.Item3TrafficLights1);
			cT_IconSet.iconSet = iconSet2;
		}
		int num = 100 / iconSet.num;
		ST_CfvoType type = (ST_CfvoType)Enum.Parse(typeof(ST_CfvoType), RangeType.PERCENT.name);
		for (int i = 0; i < iconSet.num; i++)
		{
			CT_Cfvo cT_Cfvo = cT_IconSet.AddNewCfvo();
			cT_Cfvo.type = type;
			cT_Cfvo.val = (i * num).ToString();
		}
		return new XSSFIconMultiStateFormatting(cT_IconSet);
	}

	public XSSFColorScaleFormatting CreateColorScaleFormatting()
	{
		if (_cfRule.IsSetColorScale() && _cfRule.type == ST_CfType.colorScale)
		{
			return ColorScaleFormatting as XSSFColorScaleFormatting;
		}
		_cfRule.type = ST_CfType.colorScale;
		CT_ColorScale cT_ColorScale = null;
		cT_ColorScale = ((!_cfRule.IsSetColorScale()) ? _cfRule.AddNewColorScale() : _cfRule.colorScale);
		if (cT_ColorScale.SizeOfCfvoArray() == 0)
		{
			cT_ColorScale.AddNewCfvo().type = (ST_CfvoType)Enum.Parse(typeof(ST_CfvoType), RangeType.MIN.name);
			CT_Cfvo cT_Cfvo = cT_ColorScale.AddNewCfvo();
			cT_Cfvo.type = (ST_CfvoType)Enum.Parse(typeof(ST_CfvoType), RangeType.PERCENTILE.name);
			cT_Cfvo.val = "50";
			cT_ColorScale.AddNewCfvo().type = (ST_CfvoType)Enum.Parse(typeof(ST_CfvoType), RangeType.MAX.name);
			for (int i = 0; i < 3; i++)
			{
				cT_ColorScale.AddNewColor();
			}
		}
		return new XSSFColorScaleFormatting(cT_ColorScale);
	}
}
