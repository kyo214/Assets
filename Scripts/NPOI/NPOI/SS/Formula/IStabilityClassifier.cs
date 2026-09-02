namespace NPOI.SS.Formula;

public abstract class IStabilityClassifier
{
	private static IStabilityClassifier TOTALLY_IMMUTABLE = new TotallyImmutable();

	public abstract bool IsCellFinal(int sheetIndex, int rowIndex, int columnIndex);
}
