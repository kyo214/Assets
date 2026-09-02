namespace NPOI.SS.Formula.Functions;

public class Acosh : OneArg
{
	public override double Evaluate(double d)
	{
		return MathX.Acosh(d);
	}
}
