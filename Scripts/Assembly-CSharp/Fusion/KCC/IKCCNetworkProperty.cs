namespace Fusion.KCC;

public interface IKCCNetworkProperty
{
	int WordCount { get; }

	unsafe void Read(int* ptr);

	unsafe void Write(int* ptr);

	void Interpolate(InterpolationData interpolationData);
}
