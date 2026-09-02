namespace NPOI.SS.Formula.Functions;

public class Sinh : OneArg
{
	public override double Evaluate(double d)
	{
		return MathX.Sinh(d);
	}
}
