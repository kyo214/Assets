namespace BansheeGz.BGDatabase;

public class BGEventArgsAnyEntityBeforeAdded : BGEventArgsA
{
	private static readonly BGObjectPoolNTS<BGEventArgsAnyEntityBeforeAdded> pool = new BGObjectPoolNTS<BGEventArgsAnyEntityBeforeAdded>(() => new BGEventArgsAnyEntityBeforeAdded());

	protected override BGObjectPool Pool => pool;

	public BGMetaEntity Meta { get; protected set; }

	protected BGEventArgsAnyEntityBeforeAdded()
	{
	}

	public static BGEventArgsAnyEntityBeforeAdded GetInstance(BGMetaEntity meta)
	{
		BGEventArgsAnyEntityBeforeAdded bGEventArgsAnyEntityBeforeAdded = pool.Get();
		bGEventArgsAnyEntityBeforeAdded.Meta = meta;
		return bGEventArgsAnyEntityBeforeAdded;
	}

	public override void Clear()
	{
		Meta = null;
	}

	public override string ToString()
	{
		return $"BGEventArgsAnyEntityBeforeAdded: meta [{Meta}]]";
	}
}
