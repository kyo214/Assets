using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGDnaFieldNested : BGDnaCreatableField<List<BGEntity>, BGFieldNested>
{
	public readonly BGDnaMetaNested NestedDnaMeta;

	public bool AutoCreated;

	public BGDnaFieldNested(BGDnaMeta metaDna, string dnaName)
		: base(metaDna, dnaName)
	{
		NestedDnaMeta = new BGDnaMetaNested(dnaName, metaDna);
	}

	protected override BGField New(BGMetaEntity meta, string addon)
	{
		BGFieldNested bGFieldNested = (AutoCreated ? ((BGFieldNested)meta.GetField(DnaName)) : new BGFieldNested(meta, DnaName)
		{
			Addon = addon
		});
		NestedDnaMeta.Meta = bGFieldNested.NestedMeta;
		NestedDnaMeta.Meta.Addon = addon;
		((BGMetaNested)NestedDnaMeta.Meta).OwnerRelation.Addon = addon;
		foreach (BGDnaField field in NestedDnaMeta.Fields)
		{
			((BGDnaCreatable.CreatableI)field).Create(null, addon);
		}
		return bGFieldNested;
	}

	public override void Bind(BGMetaEntity meta)
	{
		base.Bind(meta);
		NestedDnaMeta.Bind(meta.Repo);
	}
}
