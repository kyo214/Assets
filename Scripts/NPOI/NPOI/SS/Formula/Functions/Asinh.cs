namespace NPOI.SS.Formula.Functions;

public class Asinh : OneArg
{
	public override double Evaluate(double d)
	{
		return MathX.Asinh(d);
	}
}
