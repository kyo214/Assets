using System.Collections.Generic;
using System.Text;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.Atp;

public class TextJoinFunction : FreeRefFunction
{
	public static FreeRefFunction instance = new TextJoinFunction(ArgumentsEvaluator.instance);

	private ArgumentsEvaluator evaluator;

	private TextJoinFunction(ArgumentsEvaluator anEvaluator)
	{
		evaluator = anEvaluator;
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length < 3 || args.Length > 254)
		{
			return ErrorEval.VALUE_INVALID;
		}
		int rowIndex = ec.RowIndex;
		int columnIndex = ec.ColumnIndex;
		try
		{
			List<ValueEval> values = GetValues(args[0], rowIndex, columnIndex, lastRowOnly: true);
			bool value = OperandResolver.CoerceValueToBoolean(OperandResolver.GetSingleValue(args[1], rowIndex, columnIndex), stringsAreBlanks: false).Value;
			List<string> list = new List<string>();
			for (int i = 2; i < args.Length; i++)
			{
				foreach (ValueEval value2 in GetValues(args[i], rowIndex, columnIndex, lastRowOnly: false))
				{
					string text = OperandResolver.CoerceValueToString(value2);
					if (!value || (text != null && text.Length > 0))
					{
						list.Add(text);
					}
				}
			}
			if (values.Count == 0)
			{
				return new StringEval(string.Join("", list));
			}
			if (values.Count == 1)
			{
				return new StringEval(string.Join(LaxValueToString(values[0]), list));
			}
			List<string> list2 = new List<string>();
			foreach (ValueEval item in values)
			{
				list2.Add(LaxValueToString(item));
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < list.Count; j++)
			{
				if (j > 0)
				{
					int index = (j - 1) % list2.Count;
					stringBuilder.Append(list2[index]);
				}
				stringBuilder.Append(list[j]);
			}
			return new StringEval(stringBuilder.ToString());
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	private string LaxValueToString(ValueEval eval)
	{
		if (!(eval is MissingArgEval))
		{
			return OperandResolver.CoerceValueToString(eval);
		}
		return "";
	}

	private List<ValueEval> GetValues(ValueEval eval, int srcRowIndex, int srcColumnIndex, bool lastRowOnly)
	{
		if (eval is AreaEval)
		{
			AreaEval areaEval = (AreaEval)eval;
			List<ValueEval> list = new List<ValueEval>();
			for (int i = (lastRowOnly ? areaEval.LastRow : areaEval.FirstRow); i <= areaEval.LastRow; i++)
			{
				for (int j = areaEval.FirstColumn; j <= areaEval.LastColumn; j++)
				{
					list.Add(OperandResolver.GetSingleValue(areaEval.GetAbsoluteValue(i, j), i, j));
				}
			}
			return list;
		}
		return new List<ValueEval> { OperandResolver.GetSingleValue(eval, srcRowIndex, srcColumnIndex) };
	}
}
