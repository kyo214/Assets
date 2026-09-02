namespace NPOI.SS.Formula.Functions;

public class Atanh : OneArg
{
	public override double Evaluate(double d)
	{
		return MathX.Atanh(d);
	}
}
