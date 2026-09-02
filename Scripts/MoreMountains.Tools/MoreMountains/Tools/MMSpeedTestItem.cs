using System.Diagnostics;

namespace MoreMountains.Tools;

public struct MMSpeedTestItem(string testID)
{
	public string TestID = testID;

	public Stopwatch Timer = Stopwatch.StartNew();
}
