using System;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFPrintSetup : IPrintSetup
{
	private CT_Worksheet ctWorksheet;

	private CT_PageSetup pageSetup;

	private CT_PageMargins pageMargins;

	public PrintOrientation Orientation
	{
		get
		{
			ST_Orientation? sT_Orientation = pageSetup.orientation;
			if (sT_Orientation.HasValue)
			{
				return PrintOrientation.ValueOf((int)sT_Orientation.Value);
			}
			return PrintOrientation.DEFAULT;
		}
		set
		{
			ST_Orientation value2 = (ST_Orientation)value.Value;
			pageSetup.orientation = value2;
		}
	}

	public PageOrder PageOrder
	{
		get
		{
			return PageOrder.ValueOf((int)pageSetup.pageOrder);
		}
		set
		{
			ST_PageOrder value2 = (ST_PageOrder)value.Value;
			pageSetup.pageOrder = value2;
		}
	}

	public short PaperSize
	{
		get
		{
			return (short)pageSetup.paperSize;
		}
		set
		{
			pageSetup.paperSize = (uint)value;
		}
	}

	public short Scale
	{
		get
		{
			if (pageSetup.scale == 0)
			{
				return 100;
			}
			return (short)pageSetup.scale;
		}
		set
		{
			if (value < 10 || value > 400)
			{
				throw new POIXMLException("Scale value not accepted: you must choose a value between 10 and 400.");
			}
			pageSetup.scale = (uint)value;
		}
	}

	public short PageStart
	{
		get
		{
			return (short)pageSetup.firstPageNumber;
		}
		set
		{
			pageSetup.firstPageNumber = (uint)value;
		}
	}

	public short FitWidth
	{
		get
		{
			return (short)pageSetup.fitToWidth;
		}
		set
		{
			pageSetup.fitToWidth = (uint)value;
		}
	}

	public short FitHeight
	{
		get
		{
			return (short)pageSetup.fitToHeight;
		}
		set
		{
			pageSetup.fitToHeight = (uint)value;
		}
	}

	public bool LeftToRight
	{
		get
		{
			return PageOrder == PageOrder.OVER_THEN_DOWN;
		}
		set
		{
			if (value)
			{
				PageOrder = PageOrder.OVER_THEN_DOWN;
			}
			else
			{
				PageOrder = PageOrder.DOWN_THEN_OVER;
			}
		}
	}

	public bool Landscape
	{
		get
		{
			return Orientation == PrintOrientation.LANDSCAPE;
		}
		set
		{
			if (value)
			{
				Orientation = PrintOrientation.LANDSCAPE;
			}
			else
			{
				Orientation = PrintOrientation.PORTRAIT;
			}
		}
	}

	public bool ValidSettings
	{
		get
		{
			return pageSetup.usePrinterDefaults;
		}
		set
		{
			pageSetup.usePrinterDefaults = value;
		}
	}

	public bool NoColor
	{
		get
		{
			return pageSetup.blackAndWhite;
		}
		set
		{
			pageSetup.blackAndWhite = value;
		}
	}

	public bool Draft
	{
		get
		{
			return pageSetup.draft;
		}
		set
		{
			pageSetup.draft = value;
		}
	}

	public bool Notes
	{
		get
		{
			return GetCellComment() == PrintCellComments.AS_DISPLAYED;
		}
		set
		{
			if (value)
			{
				pageSetup.cellComments = ST_CellComments.asDisplayed;
			}
		}
	}

	public bool NoOrientation
	{
		get
		{
			return Orientation == PrintOrientation.DEFAULT;
		}
		set
		{
			if (value)
			{
				Orientation = PrintOrientation.DEFAULT;
			}
		}
	}

	public bool UsePage
	{
		get
		{
			return pageSetup.useFirstPageNumber;
		}
		set
		{
			pageSetup.useFirstPageNumber = value;
		}
	}

	public short HResolution
	{
		get
		{
			return (short)pageSetup.horizontalDpi;
		}
		set
		{
			pageSetup.horizontalDpi = (uint)value;
		}
	}

	public short VResolution
	{
		get
		{
			return (short)pageSetup.verticalDpi;
		}
		set
		{
			pageSetup.verticalDpi = (uint)value;
		}
	}

	public double HeaderMargin
	{
		get
		{
			return pageMargins.header;
		}
		set
		{
			pageMargins.header = value;
		}
	}

	public double FooterMargin
	{
		get
		{
			return pageMargins.footer;
		}
		set
		{
			pageMargins.footer = value;
		}
	}

	public short Copies
	{
		get
		{
			return (short)pageSetup.copies;
		}
		set
		{
			pageSetup.copies = (uint)value;
		}
	}

	public DisplayCellErrorType CellError
	{
		get
		{
			return (DisplayCellErrorType)pageSetup.errors;
		}
		set
		{
			pageSetup.errors = (ST_PrintError)value;
		}
	}

	public bool EndNote
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

	public XSSFPrintSetup(CT_Worksheet worksheet)
	{
		ctWorksheet = worksheet;
		if (ctWorksheet.IsSetPageSetup())
		{
			pageSetup = ctWorksheet.pageSetup;
		}
		else
		{
			pageSetup = ctWorksheet.AddNewPageSetup();
		}
		if (ctWorksheet.IsSetPageMargins())
		{
			pageMargins = ctWorksheet.pageMargins;
		}
		else
		{
			pageMargins = ctWorksheet.AddNewPageMargins();
		}
	}

	public void SetPaperSize(PaperSize size)
	{
		PaperSize = (short)(size + 1);
	}

	public PrintCellComments GetCellComment()
	{
		ST_CellComments? sT_CellComments = pageSetup.cellComments;
		if (sT_CellComments.HasValue)
		{
			return PrintCellComments.ValueOf((int)sT_CellComments.Value);
		}
		return PrintCellComments.NONE;
	}

	public PaperSize GetPaperSizeEnum()
	{
		return (PaperSize)(PaperSize - 1);
	}
}
