namespace BansheeGz.BGDatabase;

public class BGEventArgsAnyEntity : BGEventArgsA
{
	private static readonly BGObjectPoolNTS<BGEventArgsAnyEntity> pool = new BGObjectPoolNTS<BGEventArgsAnyEntity>(() => new BGEventArgsAnyEntity());

	protected override BGObjectPool Pool => pool;

	public BGEntity Entity { get; protected set; }

	protected BGEventArgsAnyEntity()
	{
	}

	public static BGEventArgsAnyEntity GetInstance(BGEntity entity)
	{
		BGEventArgsAnyEntity bGEventArgsAnyEntity = pool.Get();
		bGEventArgsAnyEntity.Entity = entity;
		return bGEventArgsAnyEntity;
	}

	public override void Clear()
	{
		Entity = null;
	}

	public override string ToString()
	{
		return $"BGEventArgsAnyEntity: entity [{Entity}]]";
	}
}
