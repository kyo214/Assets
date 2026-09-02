using System;

namespace NPOI.POIFS.FileSystem;

[Serializable]
public class OfficeXmlFileException : UnsupportedFileFormatException
{
	public OfficeXmlFileException(string s)
		: base(s)
	{
	}
}
