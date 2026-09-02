using System;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

[Serializable]
public class NamePtg : OperandPtg, WorkbookDependentFormula
{
	public const short sid = 35;

	private const int SIZE = 5;

	private int field_1_label_index;

	private short field_2_zero;

	public int Index => field_1_label_index - 1;

	public override int Size => 5;

	public override byte DefaultOperandClass => 0;

	public NamePtg(int nameIndex)
	{
		field_1_label_index = 1 + nameIndex;
	}

	public NamePtg(ILittleEndianInput in1)
	{
		field_1_label_index = in1.ReadShort();
		field_2_zero = in1.ReadShort();
	}

	public override void Write(ILittleEndianOutput out1)
	{
		out1.WriteByte(35 + base.PtgClass);
		out1.WriteShort(field_1_label_index);
		out1.WriteShort(field_2_zero);
	}

	public string ToFormulaString(IFormulaRenderingWorkbook book)
	{
		return book.GetNameText(this);
	}

	public override string ToFormulaString()
	{
		throw new NotImplementedException("3D references need a workbook to determine formula text");
	}
}
