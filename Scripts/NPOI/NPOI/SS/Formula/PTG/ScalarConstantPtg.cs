namespace NPOI.SS.Formula.PTG;

public abstract class ScalarConstantPtg : Ptg
{
	public override bool IsBaseToken => true;

	public override byte DefaultOperandClass => 32;
}
