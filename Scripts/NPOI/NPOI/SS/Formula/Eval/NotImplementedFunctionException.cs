using System;

namespace NPOI.SS.Formula.Eval;

public class NotImplementedFunctionException : NotImplementedException
{
	private string functionName;

	public string FunctionName => functionName;

	public NotImplementedFunctionException(string functionName)
		: base(functionName)
	{
		this.functionName = functionName;
	}

	public NotImplementedFunctionException(string functionName, NotImplementedException cause)
		: base(functionName, cause)
	{
		this.functionName = functionName;
	}
}
