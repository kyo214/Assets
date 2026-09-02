using System.Text;

namespace NPOI.SS.Formula.PTG;

public class GreaterThanPtg : ValueOperatorPtg
{
	public const byte sid = 13;

	private const string GREATERTHAN = ">";

	public static readonly ValueOperatorPtg instance = new GreaterThanPtg();

	protected override byte Sid => 13;

	public override int NumberOfOperands => 2;

	private GreaterThanPtg()
	{
	}

	public override string ToFormulaString(string[] operands)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(operands[0]);
		stringBuilder.Append(">");
		stringBuilder.Append(operands[1]);
		return stringBuilder.ToString();
	}
}
