using System.Text;

namespace NPOI.SS.Formula.PTG;

public class AddPtg : ValueOperatorPtg
{
	public const byte sid = 3;

	private static string Add = "+";

	public static ValueOperatorPtg instance = new AddPtg();

	protected override byte Sid => 3;

	public override int NumberOfOperands => 2;

	private AddPtg()
	{
	}

	public override string ToFormulaString(string[] operands)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(operands[0]);
		stringBuilder.Append(Add);
		stringBuilder.Append(operands[1]);
		return stringBuilder.ToString();
	}
}
