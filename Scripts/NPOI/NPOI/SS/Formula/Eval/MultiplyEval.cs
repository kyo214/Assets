namespace NPOI.SS.Formula.Eval;

public class MultiplyEval : TwoOperandNumericOperation
{
	public override double Evaluate(double d0, double d1)
	{
		return d0 * d1;
	}
}
