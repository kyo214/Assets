using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Unity/GameObject GetComponents")]
public class BGCalcUnitGetComponents : BGCalcUnit
{
	public const int Code = 127;

	private BGCalcValueInput gameObjectInput;

	private BGCalcValueInput typeInput;

	private BGCalcValueOutput componentsOutput;

	public override ushort TypeCode => 127;

	public override void Definition()
	{
		gameObjectInput = ValueInput(BGCalcTypeCodeRegistry.GameObject, "gameObject", "a");
		typeInput = ValueInput(BGCalcTypeCodeRegistry.String, "type", "b");
		componentsOutput = ValueOutput(BGCalcTypeCodeRegistry.List, "components", "c", GetComponents);
	}

	private Array GetComponents(BGCalcFlowI flow)
	{
		GameObject value = flow.GetValue<GameObject>(gameObjectInput);
		if (value == null)
		{
			throw new Exception("Game Object is not set!");
		}
		string value2 = flow.GetValue<string>(typeInput);
		if (string.IsNullOrEmpty(value2))
		{
			throw new Exception("Component type is not set!");
		}
		Type type = BGUtil.GetType(value2);
		if (type == null)
		{
			throw new Exception("Type " + value2 + " is not found");
		}
		return value.GetComponents(type);
	}
}
