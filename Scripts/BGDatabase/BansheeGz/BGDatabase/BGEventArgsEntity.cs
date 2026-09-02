namespace BansheeGz.BGDatabase;

public class BGEventArgsEntity : BGEventArgsA
{
	private static readonly BGObjectPoolNTS<BGEventArgsEntity> pool = new BGObjectPoolNTS<BGEventArgsEntity>(() => new BGEventArgsEntity());

	protected override BGObjectPool Pool => pool;

	public BGEntity Entity { get; protected set; }

	protected BGEventArgsEntity()
	{
	}

	public static BGEventArgsEntity GetInstance(BGEntity entity)
	{
		BGEventArgsEntity bGEventArgsEntity = pool.Get();
		bGEventArgsEntity.Fill(entity);
		return bGEventArgsEntity;
	}

	protected void Fill(BGEntity entity)
	{
		Entity = entity;
	}

	public override void Clear()
	{
		Entity = null;
	}

	public override string ToString()
	{
		return $"BGEventArgsEntity: entity [{Entity}]";
	}
}
