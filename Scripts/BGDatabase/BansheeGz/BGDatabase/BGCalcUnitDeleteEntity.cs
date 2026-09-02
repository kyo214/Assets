using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/entity/Delete entity", true)]
public class BGCalcUnitDeleteEntity : BGCalcUnit2ControlsA
{
	private BGCalcValueInput entityInput;

	public const int Code = 80;

	public override ushort TypeCode => 80;

	public override void Definition()
	{
		base.Definition();
		entityInput = ValueInput(BGCalcTypeCodeRegistry.Entity, "entity", "a");
	}

	protected override void Run(BGCalcFlowI flow)
	{
		BGEntity value = flow.GetValue<BGEntity>(entityInput);
		if (value == null)
		{
			throw new Exception("Can not delete an entity, cause entity is not set!");
		}
		value.Delete();
	}
}
