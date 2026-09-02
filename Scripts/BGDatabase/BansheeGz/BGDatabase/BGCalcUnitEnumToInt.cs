using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/enum/Enum convert")]
public class BGCalcUnitEnumToInt : BGCalcUnit
{
	private BGCalcValueInput input;

	public const int Code = 94;

	public override ushort TypeCode => 94;

	public override void Definition()
	{
		input = ValueInput(typeof(Enum), "value", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Int, "ToInt()", "b", GetValueInt);
	}

	private int GetValueInt(BGCalcFlowI flow)
	{
		Enum value = flow.GetValue<Enum>(input);
		return (int)Convert.ChangeType(value, System.TypeCode.Int32);
	}
}
