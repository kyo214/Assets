using System.Text;
using NPOI.SS.Formula.PTG;

namespace NPOI.SS.Formula.Eval;

public class BoolEval : NumericValueEval, ValueEval, StringValueEval
{
	private bool value;

	public static readonly BoolEval FALSE = new BoolEval(value: false);

	public static readonly BoolEval TRUE = new BoolEval(value: true);

	public bool BooleanValue => value;

	public double NumberValue => value ? 1 : 0;

	public string StringValue
	{
		get
		{
			if (!value)
			{
				return "FALSE";
			}
			return "TRUE";
		}
	}

	public static BoolEval ValueOf(bool b)
	{
		if (!b)
		{
			return FALSE;
		}
		return TRUE;
	}

	public BoolEval(Ptg ptg)
	{
		value = ((BoolPtg)ptg).Value;
	}

	private BoolEval(bool value)
	{
		this.value = value;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append(StringValue);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
