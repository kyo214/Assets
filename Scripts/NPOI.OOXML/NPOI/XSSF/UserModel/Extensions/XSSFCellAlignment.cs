using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel.Extensions;

public class XSSFCellAlignment
{
	private CT_CellAlignment cellAlignement;

	public VerticalAlignment Vertical
	{
		get
		{
			return (VerticalAlignment)cellAlignement.vertical;
		}
		set
		{
			cellAlignement.vertical = (ST_VerticalAlignment)value;
			cellAlignement.verticalSpecified = true;
		}
	}

	public HorizontalAlignment Horizontal
	{
		get
		{
			return (HorizontalAlignment)cellAlignement.horizontal;
		}
		set
		{
			cellAlignement.horizontal = (ST_HorizontalAlignment)value;
			cellAlignement.horizontalSpecified = true;
		}
	}

	public long Indent
	{
		get
		{
			return cellAlignement.indent;
		}
		set
		{
			cellAlignement.indent = value;
			cellAlignement.indentSpecified = true;
		}
	}

	public long TextRotation
	{
		get
		{
			return cellAlignement.textRotation;
		}
		set
		{
			long num = value;
			if (num < 0 && num >= -90)
			{
				num = 90 + -1 * num;
			}
			cellAlignement.textRotation = num;
			cellAlignement.textRotationSpecified = true;
		}
	}

	public bool WrapText
	{
		get
		{
			return cellAlignement.wrapText;
		}
		set
		{
			cellAlignement.wrapText = value;
			cellAlignement.wrapTextSpecified = true;
		}
	}

	public bool ShrinkToFit
	{
		get
		{
			return cellAlignement.shrinkToFit;
		}
		set
		{
			cellAlignement.shrinkToFit = value;
			cellAlignement.shrinkToFitSpecified = value;
		}
	}

	public XSSFCellAlignment(CT_CellAlignment cellAlignment)
	{
		cellAlignement = cellAlignment;
	}

	public CT_CellAlignment GetCTCellAlignment()
	{
		return cellAlignement;
	}
}
