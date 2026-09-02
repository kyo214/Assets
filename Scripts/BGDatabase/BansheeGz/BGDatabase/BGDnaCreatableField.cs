namespace BansheeGz.BGDatabase;

public class BGDnaCreatableField<T, TF> : BGDnaField<T>, BGDnaCreatable.CreatableI where TF : BGField<T>
{
	public BGDnaCreatableField(BGDnaMeta metaDna, string dnaName)
		: base(metaDna, dnaName)
	{
	}

	public virtual void Create(BGRepo repo, string addon)
	{
		base.Field = New(base.MetaDna.Meta, addon);
		base.Field.Addon = addon;
	}

	protected virtual BGField New(BGMetaEntity meta, string addon)
	{
		return BGUtil.Create<TF>(typeof(TF), includePrivateConstructors: false, new object[2] { meta, DnaName });
	}
}
