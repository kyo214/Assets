namespace BansheeGz.BGDatabase;

public class BGEventArgsEntityUpdated : BGEventArgsEntity
{
	private static readonly BGObjectPoolNTS<BGEventArgsEntityUpdated> pool = new BGObjectPoolNTS<BGEventArgsEntityUpdated>(() => new BGEventArgsEntityUpdated());

	protected override BGObjectPool Pool => pool;

	public BGId FieldId { get; protected set; }

	protected BGEventArgsEntityUpdated()
	{
	}

	public static BGEventArgsEntityUpdated GetInstance(BGEntity entity, BGId fieldId)
	{
		BGEventArgsEntityUpdated bGEventArgsEntityUpdated = pool.Get();
		bGEventArgsEntityUpdated.Fill(entity);
		bGEventArgsEntityUpdated.FieldId = fieldId;
		return bGEventArgsEntityUpdated;
	}

	public override string ToString()
	{
		return $"BGEventArgsEntityUpdated: filedId [{FieldId}], entity [{base.Entity}]";
	}
}
