namespace Fusion.KCC;

public sealed class KCCStatistics
{
	public int OverlapQueries;

	public int RaycastQueries;

	public int ShapecastQueries;

	public void Reset()
	{
		OverlapQueries = 0;
		RaycastQueries = 0;
		ShapecastQueries = 0;
	}
}
