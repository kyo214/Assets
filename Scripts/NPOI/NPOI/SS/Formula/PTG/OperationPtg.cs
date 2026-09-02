using System;

namespace NPOI.SS.Formula.PTG;

[Serializable]
public abstract class OperationPtg : Ptg
{
	public const int TYPE_UNARY = 0;

	public const int TYPE_BINARY = 1;

	public const int TYPE_FUNCTION = 2;

	public abstract int NumberOfOperands { get; }

	public override byte DefaultOperandClass => 32;

	public abstract string ToFormulaString(string[] operands);
}
