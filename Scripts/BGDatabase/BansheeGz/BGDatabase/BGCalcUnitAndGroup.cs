namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/bool/And Group")]
public class BGCalcUnitAndGroup : BGCalcUnitWithInPortsCount
{
	public const int Code = 133;

	public override ushort TypeCode => 133;

	protected override BGCalcTypeCode InPortType => BGCalcTypeCodeRegistry.Bool;

	protected override int Min => 2;

	protected override BGCalcValueOutput CreateOutputPort()
	{
		return ValueOutput(BGCalcTypeCodeRegistry.Bool, "1 & n", "r", Result);
	}

	private bool Result(BGCalcFlowI flow)
	{
		int count = base.Count;
		for (int i = 0; i < count; i++)
		{
			if (!flow.GetValue<bool>(inputs[i]))
			{
				return false;
			}
		}
		return true;
	}
}
