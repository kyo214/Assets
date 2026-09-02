namespace NPOI.SS.Formula.Functions;

public class Ceiling : TwoArg
{
	public override double Evaluate(double d0, double d1)
	{
		return MathX.Ceiling(d0, d1);
	}
}
