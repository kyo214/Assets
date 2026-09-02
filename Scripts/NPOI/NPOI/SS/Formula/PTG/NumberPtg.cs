using System.Globalization;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class NumberPtg : ScalarConstantPtg
{
	public const int SIZE = 9;

	public const byte sid = 31;

	private double field_1_value;

	public double Value => field_1_value;

	public override int Size => 9;

	public NumberPtg(ILittleEndianInput in1)
	{
		field_1_value = in1.ReadDouble();
	}

	public NumberPtg(string value)
		: this(double.Parse(value, CultureInfo.InvariantCulture))
	{
	}

	public NumberPtg(double value)
	{
		field_1_value = value;
	}

	public override void Write(ILittleEndianOutput out1)
	{
		out1.WriteByte(31 + base.PtgClass);
		out1.WriteDouble(Value);
	}

	public override string ToFormulaString()
	{
		return NumberToTextConverter.ToText(Value);
	}
}
