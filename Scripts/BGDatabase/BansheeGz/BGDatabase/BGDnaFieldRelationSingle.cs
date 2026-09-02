namespace BansheeGz.BGDatabase;

public class BGDnaFieldRelationSingle : BGDnaCreatableField<BGEntity, BGFieldRelationSingle>
{
	private readonly BGDnaMeta metaDnaTo;

	public BGDnaFieldRelationSingle(BGDnaMeta metaDna, string dnaName, BGDnaMeta metaDnaTo)
		: base(metaDna, dnaName)
	{
		if (metaDnaTo == null)
		{
			throw new BGException("Related metaDna can not be null");
		}
		this.metaDnaTo = metaDnaTo;
	}

	protected override BGField New(BGMetaEntity meta, string addon)
	{
		return new BGFieldRelationSingle(meta, DnaName, meta.Repo.GetMeta(metaDnaTo.DnaName));
	}
}
