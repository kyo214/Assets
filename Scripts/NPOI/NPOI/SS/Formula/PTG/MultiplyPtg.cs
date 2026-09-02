using System.Text;

namespace NPOI.SS.Formula.PTG;

public class MultiplyPtg : ValueOperatorPtg
{
	public const byte sid = 5;

	public static ValueOperatorPtg instance = new MultiplyPtg();

	protected override byte Sid => 5;

	public override int NumberOfOperands => 2;

	private MultiplyPtg()
	{
	}

	public override string ToFormulaString(string[] operands)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(operands[0]);
		stringBuilder.Append("*");
		stringBuilder.Append(operands[1]);
		return stringBuilder.ToString();
	}
}
