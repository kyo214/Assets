using System;

namespace NPOI.SS.Formula;

[Serializable]
public class FormulaParseException : Exception
{
	public FormulaParseException(string msg)
		: base(msg)
	{
	}
}
