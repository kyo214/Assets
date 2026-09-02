namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/object/Cast")]
public class BGCalcUnitCast : BGCalcUnit
{
	private BGCalcValueInput a;

	public const int Code = 63;

	public override ushort TypeCode => 63;

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Object, "A", "a");
		ValueOutput(BGCalcTypeCodeRegistry.Bool, "as bool", "b", (BGCalcFlowI flow) => (bool)flow.GetValue<object>(a));
		ValueOutput(BGCalcTypeCodeRegistry.String, "as string", "c", (BGCalcFlowI flow) => (string)flow.GetValue<object>(a));
		ValueOutput(BGCalcTypeCodeRegistry.Int, "as int", "d", (BGCalcFlowI flow) => (int)flow.GetValue<object>(a));
		ValueOutput(BGCalcTypeCodeRegistry.Float, "as float", "e", (BGCalcFlowI flow) => (float)flow.GetValue<object>(a));
		ValueOutput(BGCalcTypeCodeRegistry.BGId, "as ID", "f", (BGCalcFlowI flow) => (BGId)flow.GetValue<object>(a));
		ValueOutput(BGCalcTypeCodeRegistry.Entity, "as entity", "g", (BGCalcFlowI flow) => (BGEntity)flow.GetValue<object>(a));
		ValueOutput(BGCalcTypeCodeRegistry.Field, "as field", "h", (BGCalcFlowI flow) => (BGField)flow.GetValue<object>(a));
		ValueOutput(BGCalcTypeCodeRegistry.Meta, "as meta", "i", (BGCalcFlowI flow) => (BGMetaEntity)flow.GetValue<object>(a));
	}
}
