using System;
using System.Text;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.Record;

public class OldFormulaRecord : OldCellRecord
{
	public const short biff2_sid = 6;

	public const short biff3_sid = 518;

	public const short biff4_sid = 1030;

	public const short biff5_sid = 6;

	private SpecialCachedValue specialCachedValue;

	private double field_4_value;

	private short field_5_options;

	private Formula field_6_Parsed_expr;

	public double Value => field_4_value;

	public short Options => field_5_options;

	public Ptg[] ParsedExpression => field_6_Parsed_expr.Tokens;

	public Formula Formula => field_6_Parsed_expr;

	protected override string RecordName => "Old Formula";

	public OldFormulaRecord(RecordInputStream ris)
		: base(ris, ris.Sid == 6)
	{
		if (IsBiff2)
		{
			field_4_value = ris.ReadDouble();
		}
		else
		{
			long num = ris.ReadLong();
			specialCachedValue = SpecialCachedValue.Create(num);
			if (specialCachedValue == null)
			{
				field_4_value = BitConverter.Int64BitsToDouble(num);
			}
		}
		if (IsBiff2)
		{
			field_5_options = (short)ris.ReadUByte();
		}
		else
		{
			field_5_options = ris.ReadShort();
		}
		int encodedTokenLen = ris.ReadShort();
		int totalEncodedLen = ris.Available();
		field_6_Parsed_expr = Formula.Read(encodedTokenLen, ris, totalEncodedLen);
	}

	public CellType GetCachedResultType()
	{
		if (specialCachedValue == null)
		{
			return CellType.Numeric;
		}
		return specialCachedValue.GetValueType();
	}

	public bool GetCachedBooleanValue()
	{
		return specialCachedValue.GetBooleanValue();
	}

	public int GetCachedErrorValue()
	{
		return specialCachedValue.GetErrorValue();
	}

	protected override void AppendValueText(StringBuilder sb)
	{
		sb.Append("    .value       = ").Append(Value).Append("\n");
	}
}
