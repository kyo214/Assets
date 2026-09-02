using System.Text;

namespace NPOI.SS.Formula.Eval;

public class FunctionNameEval : ValueEval
{
	private string _functionName;

	public string FunctionName => _functionName;

	public FunctionNameEval(string functionName)
	{
		_functionName = functionName;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append(_functionName);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
