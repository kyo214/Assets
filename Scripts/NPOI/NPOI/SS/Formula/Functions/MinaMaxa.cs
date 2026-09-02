namespace NPOI.SS.Formula.Functions;

public abstract class MinaMaxa : MultiOperandNumericFunction
{
	public static readonly Function MAXA = new Maxa();

	public static readonly Function MINA = new Mina();

	protected MinaMaxa()
		: base(isReferenceBoolCounted: true, isBlankCounted: true)
	{
	}
}
