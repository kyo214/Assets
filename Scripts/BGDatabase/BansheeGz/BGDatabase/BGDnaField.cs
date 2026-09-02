namespace BansheeGz.BGDatabase;

public abstract class BGDnaField : BGDnaDescriptor
{
	private BGDnaMeta metaDna;

	public BGField Field { get; set; }

	public BGDnaMeta MetaDna
	{
		get
		{
			return metaDna;
		}
		set
		{
			if (metaDna != value)
			{
				metaDna?.Fields.Remove(this);
				metaDna = value;
				metaDna?.Fields.Add(this);
			}
		}
	}

	protected BGDnaField(BGDnaMeta metaDna, string dnaName)
		: base(dnaName)
	{
		MetaDna = metaDna;
	}

	public virtual void Bind(BGMetaEntity meta)
	{
		Field = meta.GetField(DnaName, errorIfNotFound: false);
		if (Field == null)
		{
			throw new BGException("Error while dna binding: Can not find field with name ($) at meta with name ($)", DnaName, meta.Name);
		}
	}
}
public class BGDnaField<T> : BGDnaField
{
	public BGDnaField(BGDnaMeta metaDna, string dnaName)
		: base(metaDna, dnaName)
	{
	}

	public T Get(BGEntity entity)
	{
		return entity.Get<T>(base.Field);
	}

	public void Set(BGEntity entity, T value)
	{
		entity.Set(base.Field, value);
	}
}
