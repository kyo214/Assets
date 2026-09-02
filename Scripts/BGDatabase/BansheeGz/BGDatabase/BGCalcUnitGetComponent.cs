using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Unity/GameObject GetComponent")]
public class BGCalcUnitGetComponent : BGCalcUnit
{
	public const int Code = 126;

	private BGCalcValueInput gameObjectInput;

	private BGCalcValueInput typeInput;

	public override ushort TypeCode => 126;

	public override void Definition()
	{
		gameObjectInput = ValueInput(BGCalcTypeCodeRegistry.GameObject, "gameObject", "a");
		typeInput = ValueInput(BGCalcTypeCodeRegistry.String, "type", "b");
		ValueOutput(BGCalcTypeCodeRegistry.Component, "component", "c", GetComponent);
	}

	private Component GetComponent(BGCalcFlowI flow)
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
		return value.GetComponent(value2);
	}
}
