using System.Globalization;
using System.Text;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Proper : SingleArgTextFunc
{
	public override ValueEval Evaluate(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		char[] array = text.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			if (flag)
			{
				stringBuilder.Append(c.ToString().ToUpper(CultureInfo.CurrentCulture));
			}
			else
			{
				stringBuilder.Append(c.ToString().ToLower(CultureInfo.CurrentCulture));
			}
			flag = !char.IsLetter(c);
		}
		return new StringEval(stringBuilder.ToString());
	}
}
