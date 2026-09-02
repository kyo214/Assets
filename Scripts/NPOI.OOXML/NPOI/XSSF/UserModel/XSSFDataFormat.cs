using NPOI.SS.UserModel;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.UserModel;

public class XSSFDataFormat : IDataFormat
{
	private StylesTable stylesSource;

	public XSSFDataFormat(StylesTable stylesSource)
	{
		this.stylesSource = stylesSource;
	}

	public short GetFormat(string format)
	{
		int num = BuiltinFormats.GetBuiltinFormat(format);
		if (num == -1)
		{
			num = stylesSource.PutNumberFormat(format);
		}
		return (short)num;
	}

	public string GetFormat(short index)
	{
		return GetFormat(index & 0xFFFF);
	}

	public string GetFormat(int index)
	{
		string text = stylesSource.GetNumberFormatAt(index);
		if (text == null)
		{
			text = BuiltinFormats.GetBuiltinFormat(index);
		}
		return text;
	}

	public void PutFormat(short index, string format)
	{
		stylesSource.PutNumberFormat(index, format);
	}
}
