using System.Text;

namespace NPOI.SS.Formula.PTG;

public class UnaryPlusPtg : ValueOperatorPtg
{
	public const byte sid = 18;

	private static string Add = "+";

	public static ValueOperatorPtg instance = new UnaryPlusPtg();

	protected override byte Sid => 18;

	public override int NumberOfOperands => 1;

	private UnaryPlusPtg()
	{
	}

	public override string ToFormulaString(string[] operands)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Add);
		stringBuilder.Append(operands[0]);
		return stringBuilder.ToString();
	}
}
