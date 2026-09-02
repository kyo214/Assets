using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/object/Change type")]
public class BGCalcUnitChangeType : BGCalcUnit
{
	private BGCalcValueInput a;

	public const int Code = 96;

	public override ushort TypeCode => 96;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Object, "A", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "as bool", "b", (BGCalcFlowI flow) => ChangeType(flow, System.TypeCode.Boolean));
		ValueOutput(BGCalcTypeCodeRegistry.Byte, "as byte", "c", (BGCalcFlowI flow) => ChangeType(flow, System.TypeCode.Byte));
		ValueOutput(BGCalcTypeCodeRegistry.Float, "as float", "d", (BGCalcFlowI flow) => ChangeType(flow, System.TypeCode.Single));
		ValueOutput(BGCalcTypeCodeRegistry.Int, "as int", "e", (BGCalcFlowI flow) => ChangeType(flow, System.TypeCode.Int32));
		ValueOutput(BGCalcTypeCodeRegistry.Short, "as short", "f", (BGCalcFlowI flow) => ChangeType(flow, System.TypeCode.Int16));
		ValueOutput(BGCalcTypeCodeRegistry.SByte, "as sbyte", "g", (BGCalcFlowI flow) => ChangeType(flow, System.TypeCode.SByte));
		ValueOutput(BGCalcTypeCodeRegistry.UShort, "as ushort", "h", (BGCalcFlowI flow) => ChangeType(flow, System.TypeCode.UInt16));
	}

	private object ChangeType(BGCalcFlowI flow, TypeCode typeCode)
	{
		return Convert.ChangeType(flow.GetValue<object>(a), typeCode);
	}
}
