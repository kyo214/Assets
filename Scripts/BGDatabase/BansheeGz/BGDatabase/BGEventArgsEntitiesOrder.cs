namespace BansheeGz.BGDatabase;

public class BGEventArgsEntitiesOrder : BGEventArgsA
{
	private static readonly BGObjectPoolNTS<BGEventArgsEntitiesOrder> pool = new BGObjectPoolNTS<BGEventArgsEntitiesOrder>(() => new BGEventArgsEntitiesOrder());

	protected override BGObjectPool Pool => pool;

	public BGMetaEntity Meta { get; private set; }

	private BGEventArgsEntitiesOrder()
	{
	}

	public static BGEventArgsEntitiesOrder GetInstance(BGMetaEntity meta)
	{
		BGEventArgsEntitiesOrder bGEventArgsEntitiesOrder = pool.Get();
		bGEventArgsEntitiesOrder.Meta = meta;
		return bGEventArgsEntitiesOrder;
	}

	public override void Clear()
	{
		Meta = null;
	}

	public override string ToString()
	{
		return $"BGEventArgsEntitiesOrder: meta [{Meta}]";
	}
}
