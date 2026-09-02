namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/object/Equal")]
public class BGCalcUnitEqual : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 6;

	public override ushort TypeCode => 6;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Object, "A", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Object, "B", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "A==B", "r", IsEqual);
	}

	private bool IsEqual(BGCalcFlowI flow)
	{
		return object.Equals(flow.GetValue(a), flow.GetValue(b));
	}
}
