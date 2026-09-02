namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Flow/Break")]
public class BGCalcUnitBreak : BGCalcUnit
{
	private BGCalcControlInput input;

	public const int Code = 118;

	public override ushort TypeCode => 118;

	public override void Definition()
	{
		input = ControlInput("enter", "a", Run);
	}

	private BGCalcControlOutputI Run(BGCalcFlowI flow)
	{
		flow.BreakIsRequested = true;
		return null;
	}
}
