namespace BansheeGz.BGDatabase;

public class BGDnaMetaNested : BGDnaMetaCreatable<BGMetaNested>
{
	private readonly BGDnaMeta owner;

	public BGDnaMetaNested(string dnaName, BGDnaMeta owner)
		: base((BGDna)null, dnaName)
	{
		this.owner = owner;
	}

	protected override BGMetaEntity New(BGRepo repo, string addon)
	{
		BGMetaNested bGMetaNested = new BGMetaNested(repo, DnaName, repo.GetMeta(owner.DnaName))
		{
			Addon = addon
		};
		bGMetaNested.OwnerRelation.Addon = addon;
		return bGMetaNested;
	}
}
