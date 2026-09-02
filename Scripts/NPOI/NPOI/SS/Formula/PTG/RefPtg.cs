using System;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

[Serializable]
public class RefPtg : Ref2DPtgBase
{
	public const byte sid = 36;

	protected override byte Sid => 36;

	public RefPtg(string cellref)
		: base(new CellReference(cellref))
	{
	}

	public RefPtg(int row, int column, bool isRowRelative, bool isColumnRelative)
		: base(row, column, isRowRelative, isColumnRelative)
	{
		base.Row = row;
		base.Column = column;
		base.IsRowRelative = isRowRelative;
		base.IsColRelative = isColumnRelative;
	}

	public RefPtg(ILittleEndianInput in1)
		: base(in1)
	{
	}

	public RefPtg(CellReference cr)
		: base(cr)
	{
	}
}
