namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/cell/Cell construct")]
public class BGCalcUnitConstructCell : BGCalcUnit
{
	private BGCalcValueInput a;

	private BGCalcValueInput b;

	public const int Code = 77;

	public override ushort TypeCode => 77;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Entity, "entity", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Field, "field", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Cell, "cell", "c", GetCell);
	}

	private BGCalcCell GetCell(BGCalcFlowI flow)
	{
		return new BGCalcCell(GetField(flow), GetEntity(flow));
	}

	private BGField GetField(BGCalcFlowI flow)
	{
		return flow.GetValue<BGField>(b);
	}

	private BGEntity GetEntity(BGCalcFlowI flow)
	{
		return flow.GetValue<BGEntity>(a);
	}
}
