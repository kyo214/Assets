using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class NotImplementedFunction : Function
{
	private string _functionName;

	public string FunctionName => _functionName;

	internal NotImplementedFunction()
	{
		_functionName = GetType().Name;
	}

	public NotImplementedFunction(string name)
	{
		_functionName = name;
	}

	public ValueEval Evaluate(ValueEval[] operands, int srcRow, int srcCol)
	{
		throw new NotImplementedFunctionException(_functionName);
	}
}
