namespace BansheeGz.BGDatabase;

public abstract class BGDnaCreatableMeta : BGDnaMeta, BGDnaCreatable.CreatableI
{
	public bool Singleton;

	public bool UniqueName;

	public bool EmptyName;

	public BGDnaCreatableMeta(BGDna dna, string dnaName)
		: base(dna, dnaName)
	{
	}

	public virtual void Create(BGRepo repo, string addon)
	{
		base.Meta = New(repo, addon);
		base.Meta.Addon = addon;
		base.Meta.Singleton = Singleton;
		base.Meta.UniqueName = UniqueName;
		base.Meta.EmptyName = EmptyName;
	}

	protected abstract BGMetaEntity New(BGRepo repo, string addon);
}
