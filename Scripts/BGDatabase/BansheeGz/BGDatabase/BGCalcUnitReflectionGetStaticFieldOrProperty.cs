using System;
using System.Reflection;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Reflection/Get static field(property)")]
public class BGCalcUnitReflectionGetStaticFieldOrProperty : BGCalcUnit
{
	public const int Code = 130;

	private BGCalcValueInput typeNameInput;

	private BGCalcValueInput nameInput;

	private BGCalcValueInput isPropertyInput;

	public override ushort TypeCode => 130;

	public override void Definition()
	{
		typeNameInput = ValueInput(BGCalcTypeCodeRegistry.String, "typeName", "a");
		nameInput = ValueInput(BGCalcTypeCodeRegistry.String, "fieldName", "b");
		isPropertyInput = ValueInput(BGCalcTypeCodeRegistry.Bool, "isProperty", "c");
		ValueOutput(BGCalcTypeCodeRegistry.Object, "value", "d", GetValue);
	}

	private object GetValue(BGCalcFlowI flow)
	{
		string value = flow.GetValue<string>(typeNameInput);
		if (value == null)
		{
			throw new Exception("typeName is not set!");
		}
		Type type = BGUtil.GetType(value);
		if (type == null)
		{
			throw new Exception("type with name " + value + " can not be found!");
		}
		string value2 = flow.GetValue<string>(nameInput);
		if (string.IsNullOrEmpty(value2))
		{
			throw new Exception("field/property name is not set!");
		}
		if (flow.GetValue<bool>(isPropertyInput))
		{
			PropertyInfo property = type.GetProperty(value2, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (property == null)
			{
				throw new Exception("property " + value2 + " can not be found!");
			}
			return property.GetValue(null);
		}
		FieldInfo field = type.GetField(value2, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (field == null)
		{
			throw new Exception("field " + value2 + " can not be found!");
		}
		return field.GetValue(null);
	}
}
