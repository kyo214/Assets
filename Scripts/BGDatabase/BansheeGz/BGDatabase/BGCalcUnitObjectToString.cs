namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/object/Object to string")]
public class BGCalcUnitObjectToString : BGCalcUnit
{
	private BGCalcValueInput a;

	public const int Code = 81;

	public override ushort TypeCode => 81;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Object, "A", "a");
		ValueOutput(BGCalcTypeCodeRegistry.String, "A.ToString()", "r", GetValue);
	}

	private string GetValue(BGCalcFlowI flow)
	{
		return flow.GetValue<object>(a)?.ToString();
	}
}
