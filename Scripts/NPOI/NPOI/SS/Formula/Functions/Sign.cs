namespace NPOI.SS.Formula.Functions;

public class Sign : OneArg
{
	public override double Evaluate(double d)
	{
		return MathX.Sign(d);
	}
}
