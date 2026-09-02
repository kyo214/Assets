using System;
using System.Reflection;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Reflection/Get field(property)")]
public class BGCalcUnitReflectionGetFieldOrProperty : BGCalcUnit
{
	public const int Code = 129;

	private BGCalcValueInput objectInput;

	private BGCalcValueInput nameInput;

	private BGCalcValueInput isPropertyInput;

	public override ushort TypeCode => 129;

	public override void Definition()
	{
		objectInput = ValueInput(BGCalcTypeCodeRegistry.Object, "object", "a");
		nameInput = ValueInput(BGCalcTypeCodeRegistry.String, "fieldName", "b");
		isPropertyInput = ValueInput(BGCalcTypeCodeRegistry.Bool, "isProperty", "c");
		ValueOutput(BGCalcTypeCodeRegistry.Object, "value", "d", GetValue);
	}

	private object GetValue(BGCalcFlowI flow)
	{
		object value = flow.GetValue<object>(objectInput);
		if (value == null)
		{
			throw new Exception("object is not set!");
		}
		string value2 = flow.GetValue<string>(nameInput);
		if (string.IsNullOrEmpty(value2))
		{
			throw new Exception("field/property name is not set!");
		}
		if (flow.GetValue<bool>(isPropertyInput))
		{
			PropertyInfo property = value.GetType().GetProperty(value2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				throw new Exception("property " + value2 + " can not be found!");
			}
			return property.GetValue(value);
		}
		FieldInfo field = value.GetType().GetField(value2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field == null)
		{
			throw new Exception("field " + value2 + " can not be found!");
		}
		return field.GetValue(value);
	}
}
