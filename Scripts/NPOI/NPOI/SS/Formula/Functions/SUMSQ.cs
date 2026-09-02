namespace NPOI.SS.Formula.Functions;

public class SUMSQ : AggregateFunction
{
	protected internal override double Evaluate(double[] values)
	{
		return MathX.Sumsq(values);
	}
}
