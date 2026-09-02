#define DEBUG
namespace Fusion.Profiling;

public struct SampleTime
{
	public const int TIME_ACCURACY = 100000;

	public float Time;

	public int Tick;

	public int Stage;

	public SampleTime(double time, int tick, int stage = 0)
	{
		Time = (float)time;
		Tick = tick;
		Stage = stage;
		Assert.Check(Time >= 0f);
	}

	public static SampleTime FromTick(int tick, int stage = 0)
	{
		return new SampleTime(0.0, tick, stage);
	}

	public static SampleTime FromTime(double time, int stage = 0)
	{
		return new SampleTime(time, 0, stage);
	}

	public static SampleTime FromTime(float time, int stage = 0)
	{
		return new SampleTime(time, 0, stage);
	}
}
