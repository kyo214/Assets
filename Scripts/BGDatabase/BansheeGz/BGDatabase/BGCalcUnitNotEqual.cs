namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/object/NotEqual")]
public class BGCalcUnitNotEqual : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 7;

	public override ushort TypeCode => 7;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Object, "A", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Object, "B", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "A!=B", "r", IsEqual);
	}

	private bool IsEqual(BGCalcFlowI flow)
	{
		return !object.Equals(flow.GetValue(a), flow.GetValue(b));
	}
}
