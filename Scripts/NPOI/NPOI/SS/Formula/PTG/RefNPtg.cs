using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class RefNPtg : Ref2DPtgBase
{
	public const byte sid = 44;

	protected override byte Sid => 44;

	public RefNPtg(ILittleEndianInput in1)
		: base(in1)
	{
	}
}
