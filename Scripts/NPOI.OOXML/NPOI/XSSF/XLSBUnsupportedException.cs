namespace NPOI.XSSF;

public class XLSBUnsupportedException : UnsupportedFileFormatException
{
	public static string MESSAGE = ".XLSB Binary Workbooks are not supported";

	public XLSBUnsupportedException()
		: base(MESSAGE)
	{
	}
}
