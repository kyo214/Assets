namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Flow/If")]
public class BGCalcUnitIf : BGCalcUnit
{
	public const int Code = 106;

	private BGCalcControlOutput truePort;

	private BGCalcControlOutput falsePort;

	private BGCalcValueInput conditionPort;

	public override ushort TypeCode => 106;

	public override void Definition()
	{
		ControlInput("enter", "a", Eval);
		truePort = ControlOutput("true", "b");
		falsePort = ControlOutput("false", "c");
		conditionPort = ValueInput(BGCalcTypeCodeRegistry.Bool, "condition", "d");
	}

	private BGCalcControlOutput Eval(BGCalcFlowI flow)
	{
		if (!flow.GetValue<bool>(conditionPort))
		{
			return falsePort;
		}
		return truePort;
	}
}
