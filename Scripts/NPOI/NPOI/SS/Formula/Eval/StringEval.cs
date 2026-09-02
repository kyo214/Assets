using System;
using System.Text;
using NPOI.SS.Formula.PTG;

namespace NPOI.SS.Formula.Eval;

public class StringEval : StringValueEval, ValueEval
{
	public static readonly StringEval EMPTY_INSTANCE = new StringEval("");

	private string value;

	public string StringValue => value;

	public StringEval(Ptg ptg)
		: this(((StringPtg)ptg).Value)
	{
	}

	public StringEval(string value)
	{
		if (value == null)
		{
			throw new ArgumentException("value must not be null");
		}
		this.value = value;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append(value);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
