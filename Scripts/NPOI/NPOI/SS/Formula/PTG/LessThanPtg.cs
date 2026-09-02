using System.Text;

namespace NPOI.SS.Formula.PTG;

public class LessThanPtg : ValueOperatorPtg
{
	public const byte sid = 9;

	private const string LESSTHAN = "<";

	public static readonly ValueOperatorPtg instance = new LessThanPtg();

	protected override byte Sid => 9;

	public override int NumberOfOperands => 2;

	private LessThanPtg()
	{
	}

	public override string ToFormulaString(string[] operands)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(operands[0]);
		stringBuilder.Append("<");
		stringBuilder.Append(operands[1]);
		return stringBuilder.ToString();
	}
}
