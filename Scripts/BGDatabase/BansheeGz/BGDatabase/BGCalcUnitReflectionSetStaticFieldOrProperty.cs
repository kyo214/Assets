using System;
using System.Reflection;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Reflection/Set static field(property)")]
public class BGCalcUnitReflectionSetStaticFieldOrProperty : BGCalcUnit2ControlsA
{
	public const int Code = 132;

	private BGCalcValueInput typeNameInput;

	private BGCalcValueInput nameInput;

	private BGCalcValueInput isPropertyInput;

	private BGCalcValueInput valueInput;

	public override ushort TypeCode => 132;

	public override void Definition()
	{
		base.Definition();
		typeNameInput = ValueInput(BGCalcTypeCodeRegistry.String, "typeName", "a");
		nameInput = ValueInput(BGCalcTypeCodeRegistry.String, "fieldName", "b");
		isPropertyInput = ValueInput(BGCalcTypeCodeRegistry.Bool, "isProperty", "c");
		valueInput = ValueInput(BGCalcTypeCodeRegistry.Object, "value", "d");
	}

	protected override void Run(BGCalcFlowI flow)
	{
		string value = flow.GetValue<string>(typeNameInput);
		if (string.IsNullOrEmpty(value))
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
		object value3 = flow.GetValue<object>(valueInput);
		if (flow.GetValue<bool>(isPropertyInput))
		{
			PropertyInfo property = type.GetProperty(value2, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (property == null)
			{
				throw new Exception("property " + value2 + " can not be found!");
			}
			property.SetValue(null, value3);
		}
		else
		{
			FieldInfo field = type.GetField(value2, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (field == null)
			{
				throw new Exception("field " + value2 + " can not be found!");
			}
			field.SetValue(null, value3);
		}
	}
}
