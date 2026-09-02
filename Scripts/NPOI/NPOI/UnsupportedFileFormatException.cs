using System;

namespace NPOI;

public abstract class UnsupportedFileFormatException : ArgumentException
{
	public UnsupportedFileFormatException(string s)
		: base(s)
	{
	}
}
