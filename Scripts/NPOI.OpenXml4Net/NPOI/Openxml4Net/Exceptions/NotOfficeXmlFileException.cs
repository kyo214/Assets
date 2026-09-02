namespace NPOI.Openxml4Net.Exceptions;

public class NotOfficeXmlFileException : UnsupportedFileFormatException
{
	public NotOfficeXmlFileException(string message)
		: base(message)
	{
	}
}
