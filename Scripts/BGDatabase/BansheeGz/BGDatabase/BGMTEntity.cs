namespace BansheeGz.BGDatabase;

public struct BGMTEntity
{
	public readonly BGMTMeta Meta;

	private readonly int index;

	public BGId Id => Meta.GetEntityId(index);

	public bool IsDeleted => Meta.IsDeleted(index);

	public int Index => index;

	public string Name
	{
		get
		{
			return Get<string>("name");
		}
		set
		{
			Set("name", value);
		}
	}

	public BGMTEntity(BGMTMeta meta, int index)
	{
		this.index = index;
		Meta = meta;
	}

	public T Get<T>(string fieldName)
	{
		return Meta.GetField<T>(fieldName)[index];
	}

	public T Get<T>(int fieldIndex)
	{
		return Meta.GetField<T>(fieldIndex)[index];
	}

	public T Get<T>(BGId fieldId)
	{
		return Meta.GetField<T>(fieldId)[index];
	}

	public void Set<T>(string fieldName, T value)
	{
		Meta.Set(Meta.GetField(fieldName).Index, index, value);
	}

	public void Set<T>(int fieldIndex, T value)
	{
		Meta.Set(fieldIndex, index, value);
	}

	public void Set<T>(BGId fieldId, T value)
	{
		Meta.Set(Meta.GetField(fieldId).Index, index, value);
	}

	public void Delete()
	{
		Meta.Delete(index);
	}

	public override string ToString()
	{
		string name = Meta.Name;
		int num = index;
		return name + ".Entity #" + num;
	}
}
