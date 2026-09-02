using System;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

[Serializable]
public abstract class OperandPtg : Ptg
{
	public override bool IsBaseToken => false;

	public OperandPtg Copy()
	{
		try
		{
			return (OperandPtg)Clone();
		}
		catch (NotSupportedException e)
		{
			throw new RuntimeException(e);
		}
	}
}
