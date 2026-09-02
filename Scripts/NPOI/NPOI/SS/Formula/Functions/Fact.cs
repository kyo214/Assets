namespace NPOI.SS.Formula.Functions;

public class Fact : OneArg
{
	public override double Evaluate(double d)
	{
		return MathX.Factorial((int)d);
	}
}
