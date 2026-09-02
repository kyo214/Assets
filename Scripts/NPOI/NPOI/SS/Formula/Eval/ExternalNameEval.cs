using System.Text;

namespace NPOI.SS.Formula.Eval;

public class ExternalNameEval : ValueEval
{
	private IEvaluationName _name;

	public IEvaluationName Name => _name;

	public ExternalNameEval(IEvaluationName name)
	{
		_name = name;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append(_name.NameText);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
