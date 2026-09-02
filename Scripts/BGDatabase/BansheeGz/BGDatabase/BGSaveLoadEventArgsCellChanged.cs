namespace BansheeGz.BGDatabase;

public class BGSaveLoadEventArgsCellChanged : BGEventArgsA
{
	private static readonly BGObjectPool<BGSaveLoadEventArgsCellChanged> pool = new BGObjectPool<BGSaveLoadEventArgsCellChanged>(() => new BGSaveLoadEventArgsCellChanged());

	private BGMetaEntity meta;

	private BGEntity entity;

	private BGField field;

	private object oldValue;

	private object newValue;

	protected override BGObjectPool Pool => pool;

	public BGMetaEntity Meta => meta;

	public BGEntity Entity => entity;

	public BGField Field => this.field;

	public object OldValue => oldValue;

	public object NewValue => newValue;

	private BGSaveLoadEventArgsCellChanged()
	{
	}

	public override void Clear()
	{
		meta = null;
		entity = null;
		field = null;
		oldValue = null;
		newValue = null;
	}

	public override string ToString()
	{
		return "BGSaveLoadEventArgsCellChanged: " + ((entity == null) ? "[no entity]" : entity.FullName) + ", field: " + ((field == null) ? "[no field]" : field.Name) + " [" + oldValue?.ToString() + "->" + newValue?.ToString() + "]";
	}

	public static BGSaveLoadEventArgsCellChanged Get(BGMetaEntity meta, BGField field, BGEntity entity, object oldValue, object newValue)
	{
		BGSaveLoadEventArgsCellChanged bGSaveLoadEventArgsCellChanged = pool.Get();
		bGSaveLoadEventArgsCellChanged.Clear();
		bGSaveLoadEventArgsCellChanged.meta = meta;
		bGSaveLoadEventArgsCellChanged.field = field;
		bGSaveLoadEventArgsCellChanged.entity = entity;
		bGSaveLoadEventArgsCellChanged.oldValue = oldValue;
		bGSaveLoadEventArgsCellChanged.newValue = newValue;
		return bGSaveLoadEventArgsCellChanged;
	}
}
