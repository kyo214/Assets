using System;
using System.Text;
using NPOI.Util;

namespace NPOI.SS.Format;

public class CellGeneralFormatter : CellFormatter
{
	public CellGeneralFormatter()
		: base("General")
	{
	}

	public override void FormatValue(StringBuilder toAppendTo, object value)
	{
		if (Number.IsNumber(value))
		{
			double.TryParse(value.ToString(), out var result);
			if (result == 0.0)
			{
				toAppendTo.Append('0');
				return;
			}
			double num = Math.Log10(Math.Abs(result));
			bool flag = true;
			string text;
			if (num > 10.0 || num < -9.0)
			{
				text = "E5";
			}
			else if ((double)(long)result != result)
			{
				text = "F9";
			}
			else
			{
				text = "F0";
				flag = false;
			}
			toAppendTo.Append(result.ToString(text));
			if (!flag)
			{
				return;
			}
			int index = ((!text.StartsWith("E")) ? (toAppendTo.Length - 1) : (toAppendTo.ToString().LastIndexOf("E") - 1));
			while (toAppendTo[index] == '0')
			{
				toAppendTo.Remove(index--, 1);
			}
			if (toAppendTo[index] == '.')
			{
				toAppendTo.Remove(index--, 1);
			}
			string text2 = toAppendTo.ToString();
			index = toAppendTo.ToString().LastIndexOf("E");
			if (index > 0)
			{
				index++;
				if (text2[index] == '+' || text2[index] == '-')
				{
					index++;
				}
				int i;
				for (i = 0; index + i < text2.Length && text2[index + i] == '0'; i++)
				{
				}
				toAppendTo.Remove(index, i);
			}
		}
		else if (value is bool)
		{
			toAppendTo.Append(value.ToString().ToUpper());
		}
		else
		{
			toAppendTo.Append(value.ToString());
		}
	}

	public override void SimpleValue(StringBuilder toAppendTo, object value)
	{
		FormatValue(toAppendTo, value);
	}
}
