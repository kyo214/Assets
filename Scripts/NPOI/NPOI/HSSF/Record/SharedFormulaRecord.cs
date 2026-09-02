using System;
using System.Text;
using NPOI.SS;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class SharedFormulaRecord : SharedValueRecordBase
{
	public const short sid = 1212;

	private int field_5_reserved;

	private Formula field_7_parsed_expr;

	protected override int ExtraDataSize => 2 + field_7_parsed_expr.EncodedSize;

	public override short Sid => 1212;

	public SharedFormulaRecord()
		: this(new CellRangeAddress8Bit(0, 0, 0, 0))
	{
	}

	private SharedFormulaRecord(CellRangeAddress8Bit range)
		: base(range)
	{
		field_7_parsed_expr = Formula.Create(Ptg.EMPTY_PTG_ARRAY);
	}

	public SharedFormulaRecord(RecordInputStream in1)
		: base(in1)
	{
		field_5_reserved = in1.ReadShort();
		int encodedTokenLen = in1.ReadShort();
		int totalEncodedLen = in1.Available();
		field_7_parsed_expr = Formula.Read(encodedTokenLen, in1, totalEncodedLen);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SHARED FORMULA (").Append(HexDump.IntToHex(1212)).Append("]\n");
		stringBuilder.Append("    .range      = ").Append(base.Range.ToString()).Append("\n");
		stringBuilder.Append("    .reserved    = ").Append(HexDump.ShortToHex(field_5_reserved)).Append("\n");
		Ptg[] tokens = field_7_parsed_expr.Tokens;
		for (int i = 0; i < tokens.Length; i++)
		{
			stringBuilder.Append("Formula[").Append(i).Append("]");
			Ptg ptg = tokens[i];
			stringBuilder.Append(ptg.ToString()).Append(ptg.RVAType).Append("\n");
		}
		stringBuilder.Append("[/SHARED FORMULA]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return new SharedFormulaRecord(base.Range)
		{
			field_5_reserved = field_5_reserved,
			field_7_parsed_expr = field_7_parsed_expr.Copy()
		};
	}

	protected override void SerializeExtraData(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_5_reserved);
		field_7_parsed_expr.Serialize(out1);
	}

	public Ptg[] GetFormulaTokens(FormulaRecord formula)
	{
		int row = formula.Row;
		int column = formula.Column;
		if (!IsInRange(row, column))
		{
			throw new Exception("Shared Formula Conversion: Coding Error");
		}
		return new SharedFormula(SpreadsheetVersion.EXCEL97).ConvertSharedFormulas(field_7_parsed_expr.Tokens, row, column);
	}

	public bool IsFormulaSame(SharedFormulaRecord other)
	{
		return field_7_parsed_expr.IsSame(other.field_7_parsed_expr);
	}
}
