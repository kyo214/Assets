namespace NPOI.SS.Formula.Eval;

public interface OperationEval : Eval
{
	int NumberOfOperands { get; }

	Eval Evaluate(Eval[] evals, int srcCellRow, short srcCellCol);
}
