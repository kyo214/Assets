namespace BansheeGz.BGDatabase;

public class BGCalcUnitSetCurrentLocale : BGCalcUnit2ControlsA
{
	private BGCalcValueInput locale;

	public const int Code = 113;

	public override ushort TypeCode => 113;

	public override string Title => "Set locale";

	public override void Definition()
	{
		base.Definition();
		locale = ValueInput(BGCalcTypeCodeRegistry.String, "locale", "l");
	}

	protected override void Run(BGCalcFlowI flow)
	{
		BGCalcUnitLocalizationDelegateProvider.Delegate.CurrentLocale = flow.GetValue<string>(locale);
	}
}
