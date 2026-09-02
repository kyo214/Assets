using System;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class Ref3DPtg : RefPtgBase, WorkbookDependentFormula, IExternSheetReferenceToken
{
	public const byte sid = 58;

	private const int SIZE = 7;

	private int field_1_index_extern_sheet;

	private BitField rowRelative = BitFieldFactory.GetInstance(32768);

	private BitField colRelative = BitFieldFactory.GetInstance(16384);

	public override int Size => 7;

	public int ExternSheetIndex
	{
		get
		{
			return field_1_index_extern_sheet;
		}
		set
		{
			field_1_index_extern_sheet = value;
		}
	}

	public override byte DefaultOperandClass => 0;

	public Ref3DPtg()
	{
	}

	public Ref3DPtg(ILittleEndianInput in1)
	{
		field_1_index_extern_sheet = in1.ReadShort();
		ReadCoordinates(in1);
	}

	public Ref3DPtg(string cellref, int externIdx)
	{
		CellReference cellReference = new CellReference(cellref);
		base.Row = cellReference.Row;
		base.Column = cellReference.Col;
		base.IsColRelative = !cellReference.IsColAbsolute;
		base.IsRowRelative = !cellReference.IsRowAbsolute;
		ExternSheetIndex = externIdx;
	}

	public Ref3DPtg(CellReference cr, int externIdx)
		: base(cr)
	{
		ExternSheetIndex = externIdx;
	}

	public override string ToString()
	{
		CellReference cellReference = new CellReference(base.Row, base.Column, !base.IsRowRelative, !base.IsColRelative);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetType().Name);
		stringBuilder.Append(" [");
		stringBuilder.Append("sheetIx=").Append(ExternSheetIndex);
		stringBuilder.Append(" ! ");
		stringBuilder.Append(cellReference.FormatAsString());
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public override void Write(ILittleEndianOutput out1)
	{
		out1.WriteByte(58 + base.PtgClass);
		out1.WriteShort(ExternSheetIndex);
		WriteCoordinates(out1);
	}

	public string ToFormulaString(IFormulaRenderingWorkbook book)
	{
		return ExternSheetNameResolver.PrependSheetName(book, field_1_index_extern_sheet, FormatReferenceAsString());
	}

	public override string ToFormulaString()
	{
		throw new NotImplementedException("3D references need a workbook to determine formula text");
	}
}
