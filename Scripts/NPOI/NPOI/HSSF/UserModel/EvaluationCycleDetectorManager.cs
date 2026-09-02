using System;

namespace NPOI.HSSF.UserModel;

internal class EvaluationCycleDetectorManager
{
	[ThreadStatic]
	private static EvaluationCycleDetector ecd = new EvaluationCycleDetector();

	public static EvaluationCycleDetector GetTracker()
	{
		return ecd;
	}

	private EvaluationCycleDetectorManager()
	{
	}
}
