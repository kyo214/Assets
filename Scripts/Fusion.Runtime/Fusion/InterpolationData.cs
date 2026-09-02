namespace Fusion;

public struct InterpolationData
{
	public unsafe int* From;

	public Tick FromTick;

	public unsafe int* To;

	public Tick ToTick;

	public float Alpha;
}
