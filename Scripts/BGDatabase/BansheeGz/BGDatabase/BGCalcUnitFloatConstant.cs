namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitFloatConstant : BGCalcUnit
{
	protected abstract string OutputLabel { get; }

	protected abstract float Operation();

	public override void Definition()
	{
		ValueOutput(BGCalcTypeCodeRegistry.Float, OutputLabel, "r", (BGCalcFlowI flow) => Operation());
	}
}
