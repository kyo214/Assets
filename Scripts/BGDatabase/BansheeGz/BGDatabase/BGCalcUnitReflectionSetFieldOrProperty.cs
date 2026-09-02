using System;
using System.Reflection;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Reflection/Set field(property)")]
public class BGCalcUnitReflectionSetFieldOrProperty : BGCalcUnit2ControlsA
{
	public const int Code = 131;

	private BGCalcValueInput objectInput;

	private BGCalcValueInput nameInput;

	private BGCalcValueInput isPropertyInput;

	private BGCalcValueInput valueInput;

	public override ushort TypeCode => 131;

	public override void Definition()
	{
		base.Definition();
		objectInput = ValueInput(BGCalcTypeCodeRegistry.Object, "object", "a");
		nameInput = ValueInput(BGCalcTypeCodeRegistry.String, "fieldName", "b");
		isPropertyInput = ValueInput(BGCalcTypeCodeRegistry.Bool, "isProperty", "c");
		valueInput = ValueInput(BGCalcTypeCodeRegistry.Object, "value", "d");
	}

	protected override void Run(BGCalcFlowI flow)
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
		object value3 = flow.GetValue<object>(valueInput);
		if (flow.GetValue<bool>(isPropertyInput))
		{
			PropertyInfo property = value.GetType().GetProperty(value2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				throw new Exception("property " + value2 + " can not be found!");
			}
			property.SetValue(value, value3);
		}
		else
		{
			FieldInfo field = value.GetType().GetField(value2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new Exception("field " + value2 + " can not be found!");
			}
			field.SetValue(value, value3);
		}
	}
}
