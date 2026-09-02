namespace Fusion;

public class Pow2SliderAttribute : PropertyAttribute
{
	public Units Unit = Units.None;

	public int MinPower = 0;

	public int MaxPower = 7;

	public bool AllowZero = true;

	public Pow2SliderAttribute(int min, int max)
	{
		MinPower = min;
		MaxPower = max;
	}
}
