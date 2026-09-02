namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/bool/Or Group")]
public class BGCalcUnitOrGroup : BGCalcUnitWithInPortsCount
{
	public const int Code = 134;

	public override ushort TypeCode => 134;

	protected override int Min => 2;

	protected override BGCalcTypeCode InPortType => BGCalcTypeCodeRegistry.Bool;

	protected override BGCalcValueOutput CreateOutputPort()
	{
		return ValueOutput(BGCalcTypeCodeRegistry.Bool, "1 | n", "r", Result);
	}

	private bool Result(BGCalcFlowI flow)
	{
		int count = base.Count;
		for (int i = 0; i < count; i++)
		{
			if (flow.GetValue<bool>(inputs[i]))
			{
				return true;
			}
		}
		return false;
	}
}
