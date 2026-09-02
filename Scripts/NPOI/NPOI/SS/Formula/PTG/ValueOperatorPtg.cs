using System;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public abstract class ValueOperatorPtg : OperationPtg
{
	public override bool IsBaseToken => true;

	public override byte DefaultOperandClass => 32;

	protected abstract byte Sid { get; }

	public override int Size => 1;

	public override void Write(ILittleEndianOutput out1)
	{
		out1.WriteByte(Sid + base.PtgClass);
	}

	public override string ToFormulaString()
	{
		throw new NotImplementedException("ToFormulaString(String[] operands) should be used for subclasses of OperationPtgs");
	}
}
