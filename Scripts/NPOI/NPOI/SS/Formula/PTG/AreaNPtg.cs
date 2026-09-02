using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class AreaNPtg : Area2DPtgBase
{
	public const short sid = 45;

	protected override byte Sid => 45;

	public AreaNPtg(ILittleEndianInput in1)
		: base(in1)
	{
	}
}
