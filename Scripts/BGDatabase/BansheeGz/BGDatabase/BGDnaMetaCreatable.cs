namespace BansheeGz.BGDatabase;

public abstract class BGDnaMetaCreatable<T> : BGDnaCreatableMeta where T : BGMetaEntity
{
	protected BGDnaMetaCreatable(BGDna dna, string dnaName)
		: base(dna, dnaName)
	{
	}

	protected override BGMetaEntity New(BGRepo repo, string addon)
	{
		return BGUtil.Create<BGMetaEntity>(typeof(T), includePrivateConstructors: true, new object[2] { repo, DnaName });
	}
}
