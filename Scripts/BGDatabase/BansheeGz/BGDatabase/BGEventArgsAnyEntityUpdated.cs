namespace BansheeGz.BGDatabase;

public class BGEventArgsAnyEntityUpdated : BGEventArgsAnyEntity
{
	private static readonly BGObjectPoolNTS<BGEventArgsAnyEntityUpdated> pool = new BGObjectPoolNTS<BGEventArgsAnyEntityUpdated>(() => new BGEventArgsAnyEntityUpdated());

	protected override BGObjectPool Pool => pool;

	public BGId FieldId { get; protected set; }

	protected BGEventArgsAnyEntityUpdated()
	{
	}

	public static BGEventArgsAnyEntityUpdated GetInstance(BGEntity entity, BGId fieldId)
	{
		BGEventArgsAnyEntityUpdated bGEventArgsAnyEntityUpdated = pool.Get();
		bGEventArgsAnyEntityUpdated.Entity = entity;
		bGEventArgsAnyEntityUpdated.FieldId = fieldId;
		return bGEventArgsAnyEntityUpdated;
	}

	public override string ToString()
	{
		return $"BGEventArgsAnyEntityUpdated: fieldId [{FieldId}], entity [{base.Entity}]";
	}
}
