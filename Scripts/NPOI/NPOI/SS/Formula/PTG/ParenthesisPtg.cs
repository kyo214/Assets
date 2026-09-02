using NPOI.Util;

namespace NPOI.SS.Formula.PTG;

public class ParenthesisPtg : ControlPtg
{
	private const int SIZE = 1;

	public const byte sid = 21;

	public static ControlPtg instance = new ParenthesisPtg();

	public override int Size => 1;

	private ParenthesisPtg()
	{
	}

	public override void Write(ILittleEndianOutput out1)
	{
		out1.WriteByte(21 + base.PtgClass);
	}

	public override string ToFormulaString()
	{
		return "()";
	}

	public string ToFormulaString(string[] operands)
	{
		return "(" + operands[0] + ")";
	}
}
