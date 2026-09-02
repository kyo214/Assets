using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Database/Generic/entity/New entity", true)]
public class BGCalcUnitCreateEntity : BGCalcUnit2ControlsA
{
	private BGCalcValueInput metaInput;

	private BGCalcValueOutput newEntityOutput;

	public const int Code = 79;

	public override ushort TypeCode => 79;

	public override void Definition()
	{
		base.Definition();
		metaInput = ValueInput(BGCalcTypeCodeRegistry.Meta, "meta", "q");
		newEntityOutput = ValueOutput(BGCalcTypeCodeRegistry.Entity, "entity", "e", null);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		BGEntity value = GetMeta(flow).NewEntity();
		flow.SetValue(newEntityOutput, value);
	}

	private BGMetaEntity GetMeta(BGCalcFlowI flow)
	{
		BGMetaEntity value = flow.GetValue<BGMetaEntity>(metaInput);
		if (value == null)
		{
			throw new Exception("Can not create an entity cause meta is not set!");
		}
		return value;
	}
}
