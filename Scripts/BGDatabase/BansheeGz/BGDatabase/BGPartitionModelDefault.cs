namespace BansheeGz.BGDatabase;

public class BGPartitionModelDefault : BGPartitionModelA
{
	private readonly BGEntity entity;

	public BGEntity Entity => entity;

	public BGPartitionModelDefault(BGEntity entity)
	{
		this.entity = entity;
	}
}
