using System;

namespace NPOI;

[Serializable]
public class OldFileFormatException : UnsupportedFileFormatException
{
	public OldFileFormatException(string s)
		: base(s)
	{
	}
}
