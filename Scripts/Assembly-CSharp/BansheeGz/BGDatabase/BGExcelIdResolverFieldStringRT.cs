namespace BansheeGz.BGDatabase;

public class BGExcelIdResolverFieldStringRT : BGExcelIdResolverFieldART<string>
{
	public BGExcelIdResolverFieldStringRT(BGLogger logger, BGFieldString field)
		: base(logger, (BGField<string>)field)
	{
	}

	protected override string Convert(string value)
	{
		return value;
	}
}
