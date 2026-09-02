using System.Globalization;

namespace BansheeGz.BGDatabase;

public class BGExcelIdResolverFieldIntRT : BGExcelIdResolverFieldART<int>
{
	public BGExcelIdResolverFieldIntRT(BGLogger logger, BGFieldInt field)
		: base(logger, (BGField<int>)field)
	{
	}

	protected override int Convert(string value)
	{
		return int.Parse(value, CultureInfo.InvariantCulture);
	}
}
