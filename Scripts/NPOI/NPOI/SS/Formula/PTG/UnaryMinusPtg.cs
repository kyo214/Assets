using System.Text;

namespace NPOI.SS.Formula.PTG;

public class UnaryMinusPtg : ValueOperatorPtg
{
	public const byte sid = 19;

	private const string MINUS = "-";

	public static readonly ValueOperatorPtg instance = new UnaryMinusPtg();

	protected override byte Sid => 19;

	public override int NumberOfOperands => 1;

	private UnaryMinusPtg()
	{
	}

	public override string ToFormulaString(string[] operands)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("-");
		stringBuilder.Append(operands[0]);
		return stringBuilder.ToString();
	}
}
