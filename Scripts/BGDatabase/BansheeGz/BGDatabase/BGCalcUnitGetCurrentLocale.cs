namespace BansheeGz.BGDatabase;

public class BGCalcUnitGetCurrentLocale : BGCalcUnit
{
	public const int Code = 112;

	public override ushort TypeCode => 112;

	public override string Title => "Get locale";

	public override void Definition()
	{
		ValueOutput(BGCalcTypeCodeRegistry.String, "locale", "l", GetLocale);
	}

	private string GetLocale(BGCalcFlowI flow)
	{
		return BGCalcUnitLocalizationDelegateProvider.Delegate.CurrentLocale;
	}
}
