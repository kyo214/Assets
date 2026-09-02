using System;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.Spreadsheet;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public abstract class XSSFShape : IShape
{
	public static int EMU_PER_PIXEL = 9525;

	public static int EMU_PER_POINT = 12700;

	public static int POINT_DPI = 72;

	public static int PIXEL_DPI = 96;

	protected XSSFDrawing drawing;

	public XSSFShapeGroup parent;

	internal XSSFAnchor anchor;

	public IShape Parent => parent;

	public bool IsNoFill
	{
		get
		{
			return GetShapeProperties().noFill != null;
		}
		set
		{
			NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties shapeProperties = GetShapeProperties();
			if (shapeProperties.IsSetPattFill())
			{
				shapeProperties.unsetPattFill();
			}
			if (shapeProperties.IsSetSolidFill())
			{
				shapeProperties.unsetSolidFill();
			}
			shapeProperties.noFill = new CT_NoFillProperties();
		}
	}

	public int CountOfAllChildren
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public int FillColor
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public virtual LineStyle LineStyle
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties shapeProperties = GetShapeProperties();
			(shapeProperties.IsSetLn() ? shapeProperties.ln : shapeProperties.AddNewLn()).prstDash = new CT_PresetLineDashProperties
			{
				val = (ST_PresetLineDashVal)(value + 1)
			};
		}
	}

	public virtual int LineStyleColor
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public virtual double LineWidth
	{
		get
		{
			NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties shapeProperties = GetShapeProperties();
			if (shapeProperties.IsSetLn())
			{
				return (double)shapeProperties.ln.w * 1.0 / (double)EMU_PER_POINT;
			}
			return 0.0;
		}
		set
		{
			NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties shapeProperties = GetShapeProperties();
			(shapeProperties.IsSetLn() ? shapeProperties.ln : shapeProperties.AddNewLn()).w = (int)(value * (double)EMU_PER_POINT);
		}
	}

	public XSSFDrawing GetDrawing()
	{
		return drawing;
	}

	public XSSFAnchor GetAnchor()
	{
		return anchor;
	}

	protected internal abstract NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties GetShapeProperties();

	public void SetFillColor(int red, int green, int blue)
	{
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties shapeProperties = GetShapeProperties();
		CT_SolidColorFillProperties cT_SolidColorFillProperties = (shapeProperties.IsSetSolidFill() ? shapeProperties.solidFill : shapeProperties.AddNewSolidFill());
		CT_SRgbColor cT_SRgbColor = new CT_SRgbColor();
		cT_SRgbColor.val = new byte[3]
		{
			(byte)red,
			(byte)green,
			(byte)blue
		};
		cT_SolidColorFillProperties.srgbClr = cT_SRgbColor;
	}

	public void SetLineStyleColor(int red, int green, int blue)
	{
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties shapeProperties = GetShapeProperties();
		CT_LineProperties cT_LineProperties = (shapeProperties.IsSetLn() ? shapeProperties.ln : shapeProperties.AddNewLn());
		CT_SolidColorFillProperties cT_SolidColorFillProperties = (cT_LineProperties.IsSetSolidFill() ? cT_LineProperties.solidFill : cT_LineProperties.AddNewSolidFill());
		CT_SRgbColor cT_SRgbColor = new CT_SRgbColor();
		cT_SRgbColor.val = new byte[3]
		{
			(byte)red,
			(byte)green,
			(byte)blue
		};
		cT_SolidColorFillProperties.srgbClr = cT_SRgbColor;
	}

	public void SetLineStyleColor(int lineStyleColor)
	{
		throw new NotImplementedException();
	}
}
