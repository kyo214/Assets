using System.Text;

namespace NPOI.SS.Formula.PTG;

public class DividePtg : ValueOperatorPtg
{
	public const byte sid = 6;

	public static ValueOperatorPtg instance = new DividePtg();

	protected override byte Sid => 6;

	public override int NumberOfOperands => 2;

	private DividePtg()
	{
	}

	public override string ToFormulaString(string[] operands)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(operands[0]);
		stringBuilder.Append("/");
		stringBuilder.Append(operands[1]);
		return stringBuilder.ToString();
	}
}
