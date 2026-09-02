using System;
using System.IO;

namespace NPOI;

public class EmptyFileException : IOException
{
	private const long serialVersionUID = 1536449292174360166L;

	public EmptyFileException()
		: base("The supplied file was empty (zero bytes long)")
	{
	}

	public EmptyFileException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
