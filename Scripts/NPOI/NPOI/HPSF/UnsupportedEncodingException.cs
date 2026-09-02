using System.IO;

namespace NPOI.HPSF;

public class UnsupportedEncodingException : IOException
{
	public UnsupportedEncodingException()
	{
	}

	public UnsupportedEncodingException(string s)
		: base(s)
	{
	}
}
