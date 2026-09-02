namespace Fusion;

public class MultiPropertyDrawersFixAttribute : PropertyAttribute
{
	public MultiPropertyDrawersFixAttribute()
	{
		base.order = int.MaxValue;
	}
}
