namespace BansheeGz.BGDatabase;

public class BGEventArgsField : BGEventArgsA
{
	private static readonly BGObjectPoolNTS<BGEventArgsField> pool = new BGObjectPoolNTS<BGEventArgsField>(() => new BGEventArgsField());

	protected override BGObjectPool Pool => pool;

	public BGId FieldId { get; protected set; }

	public BGEntity Entity { get; protected set; }

	protected BGEventArgsField()
	{
	}

	public static BGEventArgsField GetInstance(BGEntity entity, BGId fieldId)
	{
		BGEventArgsField bGEventArgsField = pool.Get();
		bGEventArgsField.FieldId = fieldId;
		bGEventArgsField.Entity = entity;
		return bGEventArgsField;
	}

	public override void Clear()
	{
		Entity = null;
	}

	public override string ToString()
	{
		return $"BGEventArgsField: fieldId [{FieldId}, entity [{Entity}]]";
	}
}
