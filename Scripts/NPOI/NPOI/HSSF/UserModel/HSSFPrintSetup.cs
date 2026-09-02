using NPOI.HSSF.Record;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFPrintSetup : IPrintSetup
{
	private PrintSetupRecord printSetupRecord;

	public short PaperSize
	{
		get
		{
			return printSetupRecord.PaperSize;
		}
		set
		{
			printSetupRecord.PaperSize = value;
		}
	}

	public short Scale
	{
		get
		{
			return printSetupRecord.Scale;
		}
		set
		{
			printSetupRecord.Scale = value;
		}
	}

	public short PageStart
	{
		get
		{
			return printSetupRecord.PageStart;
		}
		set
		{
			printSetupRecord.PageStart = value;
		}
	}

	public short FitWidth
	{
		get
		{
			return printSetupRecord.FitWidth;
		}
		set
		{
			printSetupRecord.FitWidth = value;
		}
	}

	public short FitHeight
	{
		get
		{
			return printSetupRecord.FitHeight;
		}
		set
		{
			printSetupRecord.FitHeight = value;
		}
	}

	public short Options
	{
		get
		{
			return printSetupRecord.Options;
		}
		set
		{
			printSetupRecord.Options = value;
		}
	}

	public bool LeftToRight
	{
		get
		{
			return printSetupRecord.LeftToRight;
		}
		set
		{
			printSetupRecord.LeftToRight = value;
		}
	}

	public bool Landscape
	{
		get
		{
			return !printSetupRecord.Landscape;
		}
		set
		{
			printSetupRecord.Landscape = !value;
		}
	}

	public bool ValidSettings
	{
		get
		{
			return printSetupRecord.ValidSettings;
		}
		set
		{
			printSetupRecord.ValidSettings = value;
		}
	}

	public bool NoColor
	{
		get
		{
			return printSetupRecord.NoColor;
		}
		set
		{
			printSetupRecord.NoColor = value;
		}
	}

	public bool EndNote
	{
		get
		{
			return printSetupRecord.EndNote;
		}
		set
		{
			printSetupRecord.EndNote = value;
		}
	}

	public DisplayCellErrorType CellError
	{
		get
		{
			return (DisplayCellErrorType)printSetupRecord.CellError;
		}
		set
		{
			printSetupRecord.CellError = (short)value;
		}
	}

	public bool Draft
	{
		get
		{
			return printSetupRecord.Draft;
		}
		set
		{
			printSetupRecord.Draft = value;
		}
	}

	public bool Notes
	{
		get
		{
			return printSetupRecord.Notes;
		}
		set
		{
			printSetupRecord.Notes = value;
		}
	}

	public bool NoOrientation
	{
		get
		{
			return printSetupRecord.NoOrientation;
		}
		set
		{
			printSetupRecord.NoOrientation = value;
		}
	}

	public bool UsePage
	{
		get
		{
			return printSetupRecord.UsePage;
		}
		set
		{
			printSetupRecord.UsePage = value;
		}
	}

	public short HResolution
	{
		get
		{
			return printSetupRecord.HResolution;
		}
		set
		{
			printSetupRecord.HResolution = value;
		}
	}

	public short VResolution
	{
		get
		{
			return printSetupRecord.VResolution;
		}
		set
		{
			printSetupRecord.VResolution = value;
		}
	}

	public double HeaderMargin
	{
		get
		{
			return printSetupRecord.HeaderMargin;
		}
		set
		{
			printSetupRecord.HeaderMargin = value;
		}
	}

	public double FooterMargin
	{
		get
		{
			return printSetupRecord.FooterMargin;
		}
		set
		{
			printSetupRecord.FooterMargin = value;
		}
	}

	public short Copies
	{
		get
		{
			return printSetupRecord.Copies;
		}
		set
		{
			printSetupRecord.Copies = value;
		}
	}

	public HSSFPrintSetup(PrintSetupRecord printSetupRecord)
	{
		this.printSetupRecord = printSetupRecord;
	}
}
