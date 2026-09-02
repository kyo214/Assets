using System;
using System.Runtime.Serialization;

namespace NPOI.POIFS.Crypt.Dsig.Services;

[Serializable]
internal class CRLException : Exception
{
	public CRLException()
	{
	}

	public CRLException(string message)
		: base(message)
	{
	}

	public CRLException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected CRLException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
