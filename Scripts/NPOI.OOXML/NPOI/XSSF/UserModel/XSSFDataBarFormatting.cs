using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFDataBarFormatting : IDataBarFormatting
{
	private CT_DataBar _databar;

	public bool IsIconOnly
	{
		get
		{
			if (_databar.IsSetShowValue())
			{
				return !_databar.showValue;
			}
			return false;
		}
		set
		{
			_databar.showValue = value;
		}
	}

	public bool IsLeftToRight
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	public int WidthMin
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public int WidthMax
	{
		get
		{
			return 100;
		}
		set
		{
		}
	}

	public IColor Color
	{
		get
		{
			return new XSSFColor(_databar.color);
		}
		set
		{
			_databar.color = ((XSSFColor)value).GetCTColor();
		}
	}

	public IConditionalFormattingThreshold MinThreshold => new XSSFConditionalFormattingThreshold(_databar.cfvo[0]);

	public IConditionalFormattingThreshold MaxThreshold => new XSSFConditionalFormattingThreshold(_databar.cfvo[1]);

	public XSSFDataBarFormatting(CT_DataBar databar)
	{
		_databar = databar;
	}

	public XSSFConditionalFormattingThreshold CreateThreshold()
	{
		return new XSSFConditionalFormattingThreshold(_databar.AddNewCfvo());
	}
}
