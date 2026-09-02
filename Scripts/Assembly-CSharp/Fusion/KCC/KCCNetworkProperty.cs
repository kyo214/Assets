namespace Fusion.KCC;

public abstract class KCCNetworkProperty<TContext> : IKCCNetworkProperty where TContext : class
{
	public readonly TContext Context;

	public readonly int WordCount;

	int IKCCNetworkProperty.WordCount => WordCount;

	public KCCNetworkProperty(TContext context, int wordCount)
	{
		Context = context;
		WordCount = wordCount;
	}

	public unsafe abstract void Read(int* ptr);

	public unsafe abstract void Write(int* ptr);

	public abstract void Interpolate(InterpolationData interpolationData);
}
