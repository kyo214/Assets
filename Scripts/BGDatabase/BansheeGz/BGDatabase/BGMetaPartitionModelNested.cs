namespace BansheeGz.BGDatabase;

public class BGMetaPartitionModelNested : BGMetaPartitionModelA
{
	private readonly BGFieldRelationSingle[] chainToDelegate;

	private readonly BGMetaPartitionModelDefault modelDelegate;

	public override bool IsRoot => false;

	public BGMetaPartitionModelNested(BGFieldRelationSingle[] chainToDelegate, BGMetaPartitionModelDefault modelDelegate)
		: base(chainToDelegate[0].Meta)
	{
		this.chainToDelegate = chainToDelegate;
		this.modelDelegate = modelDelegate;
	}

	public override int? GetPartitionIndex(BGEntity entity)
	{
		BGEntity bGEntity = entity;
		BGFieldRelationSingle[] array = chainToDelegate;
		foreach (BGFieldRelationSingle bGFieldRelationSingle in array)
		{
			bGEntity = bGFieldRelationSingle[bGEntity.Index];
			if (bGEntity == null)
			{
				return null;
			}
		}
		if (bGEntity == null)
		{
			return null;
		}
		return modelDelegate.GetPartitionIndex(bGEntity);
	}
}
