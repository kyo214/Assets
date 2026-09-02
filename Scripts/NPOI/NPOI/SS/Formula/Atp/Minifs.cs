using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.Atp;

public class Minifs : FreeRefFunction
{
	public static FreeRefFunction instance = new Minifs();

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length < 3 || args.Length % 2 == 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		try
		{
			AreaEval areaEval = Sumifs.ConvertRangeArg(args[0]);
			AreaEval[] array = new AreaEval[(args.Length - 1) / 2];
			IMatchPredicate[] array2 = new IMatchPredicate[array.Length];
			int num = 1;
			int num2 = 0;
			while (num < args.Length)
			{
				array[num2] = Sumifs.ConvertRangeArg(args[num]);
				array2[num2] = Countif.CreateCriteriaPredicate(args[num + 1], ec.RowIndex, ec.ColumnIndex);
				num += 2;
				num2++;
			}
			Sumifs.ValidateCriteriaRanges(array, areaEval);
			Sumifs.ValidateCriteria(array2);
			return new NumberEval(Sumifs.CalcMatchingCells(array, array2, areaEval, double.NaN, (double init, double? current) => current.HasValue ? ((!double.IsNaN(init)) ? ((!(current.Value < init)) ? init : current.Value) : current.Value) : init));
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}
}
