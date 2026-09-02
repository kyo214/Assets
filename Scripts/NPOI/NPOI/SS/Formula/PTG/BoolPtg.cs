using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class BoolPtg : ScalarConstantPtg
{
	public const int SIZE = 2;

	public const byte sid = 29;

	private bool field_1_value;

	public bool Value => field_1_value;

	public override int Size => 2;

	public BoolPtg(ILittleEndianInput in1)
	{
		field_1_value = in1.ReadByte() == 1;
	}

	public BoolPtg(string formulaToken)
	{
		field_1_value = formulaToken.Equals("TRUE");
	}

	public override void Write(ILittleEndianOutput out1)
	{
		out1.WriteByte(29 + base.PtgClass);
		out1.WriteByte(field_1_value ? 1 : 0);
	}

	public override string ToFormulaString()
	{
		if (!field_1_value)
		{
			return "FALSE";
		}
		return "TRUE";
	}
}
