namespace MoreMountains.Feedbacks;

public struct TimeScaleProperties
{
	public float TimeScale;

	public float Duration;

	public bool Lerp;

	public float LerpSpeed;

	public bool Infinite;

	public override string ToString()
	{
		return $"REQUESTED ts={TimeScale} time={Duration} lerp={Lerp} speed={LerpSpeed} keep={Infinite}";
	}
}
