namespace MoreMountains.Tools;

public class MMLoot<T>
{
	public T Loot;

	public float Weight = 1f;

	[MMReadOnly]
	public float ChancePercentage;

	public float RangeFrom { get; set; }

	public float RangeTo { get; set; }
}
