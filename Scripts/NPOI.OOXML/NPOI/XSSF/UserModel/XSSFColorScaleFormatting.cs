using System.Collections.Generic;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFColorScaleFormatting : IColorScaleFormatting
{
	private CT_ColorScale _scale;

	public int NumControlPoints
	{
		get
		{
			return _scale.SizeOfCfvoArray();
		}
		set
		{
			while (value < _scale.SizeOfCfvoArray())
			{
				_scale.RemoveCfvo(_scale.SizeOfCfvoArray() - 1);
				_scale.RemoveColor(_scale.SizeOfColorArray() - 1);
			}
			while (value > _scale.SizeOfCfvoArray())
			{
				_scale.AddNewCfvo();
				_scale.AddNewColor();
			}
		}
	}

	public IColor[] Colors
	{
		get
		{
			CT_Color[] array = _scale.color.ToArray();
			XSSFColor[] array2 = new XSSFColor[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = new XSSFColor(array[i]);
			}
			return array2;
		}
		set
		{
			CT_Color[] array = new CT_Color[value.Length];
			for (int i = 0; i < value.Length; i++)
			{
				array[i] = ((XSSFColor)value[i]).GetCTColor();
			}
			_scale.color = new List<CT_Color>(array);
		}
	}

	public IConditionalFormattingThreshold[] Thresholds
	{
		get
		{
			CT_Cfvo[] array = _scale.cfvo.ToArray();
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
			_scale.cfvo = new List<CT_Cfvo>(array);
		}
	}

	public XSSFColorScaleFormatting(CT_ColorScale scale)
	{
		_scale = scale;
	}

	public XSSFColor CreateColor()
	{
		return new XSSFColor(_scale.AddNewColor());
	}

	public IConditionalFormattingThreshold CreateThreshold()
	{
		return new XSSFConditionalFormattingThreshold(_scale.AddNewCfvo());
	}
}
