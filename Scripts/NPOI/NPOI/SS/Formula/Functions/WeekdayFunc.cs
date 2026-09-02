using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Functions;

public class WeekdayFunc : Function
{
	public static Function instance = new WeekdayFunc();

	private WeekdayFunc()
	{
	}

	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		try
		{
			if (args.Length < 1 || args.Length > 2)
			{
				return ErrorEval.VALUE_INVALID;
			}
			double num = OperandResolver.CoerceValueToDouble(OperandResolver.GetSingleValue(args[0], srcRowIndex, srcColumnIndex));
			if (!DateUtil.IsValidExcelDate(num))
			{
				return ErrorEval.NUM_ERROR;
			}
			int num2 = (int)(DateUtil.GetJavaCalendar(num, use1904windowing: false).DayOfWeek + 1);
			int num3 = 1;
			if (args.Length == 2)
			{
				ValueEval singleValue = OperandResolver.GetSingleValue(args[1], srcRowIndex, srcColumnIndex);
				if (singleValue == MissingArgEval.instance || singleValue == BlankEval.instance)
				{
					return ErrorEval.NUM_ERROR;
				}
				num3 = OperandResolver.CoerceValueToInt(singleValue);
				if (num3 == 2)
				{
					num3 = 11;
				}
			}
			double value;
			switch (num3)
			{
			case 1:
				value = num2;
				break;
			case 3:
				value = (num2 + 6 - 1) % 7;
				break;
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
				value = (num2 + 6 - (num3 - 10)) % 7 + 1;
				break;
			default:
				return ErrorEval.NUM_ERROR;
			}
			return new NumberEval(value);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}
}
