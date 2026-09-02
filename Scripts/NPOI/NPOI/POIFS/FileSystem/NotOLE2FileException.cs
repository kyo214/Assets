using System.IO;

namespace NPOI.POIFS.FileSystem;

public class NotOLE2FileException : IOException
{
	public NotOLE2FileException(string s)
		: base(s)
	{
	}
}
