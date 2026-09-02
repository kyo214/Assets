namespace BansheeGz.BGDatabase;

public class BGCalcUnitGetCurrentEntity : BGCalcUnit
{
	public const int Code = 111;

	public override ushort TypeCode => 111;

	public override string Title => "Get current entity";

	public override void Definition()
	{
		ValueOutput(BGCalcTypeCodeRegistry.Entity, "entity", "e", GetEntity);
	}

	private BGEntity GetEntity(BGCalcFlowI flow)
	{
		return flow.Context.CurrentEntity;
	}
}
