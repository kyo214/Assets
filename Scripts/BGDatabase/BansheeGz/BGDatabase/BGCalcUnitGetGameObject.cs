using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Unity/Component GetGameObject")]
public class BGCalcUnitGetGameObject : BGCalcUnit
{
	public const int Code = 128;

	private BGCalcValueInput componentInput;

	public override ushort TypeCode => 128;

	public override void Definition()
	{
		componentInput = ValueInput(BGCalcTypeCodeRegistry.Component, "component", "a");
		ValueOutput(BGCalcTypeCodeRegistry.GameObject, "component", "c", GetGameObject);
	}

	private GameObject GetGameObject(BGCalcFlowI flow)
	{
		Component value = flow.GetValue<Component>(componentInput);
		if (value == null)
		{
			throw new Exception("Component is not set!");
		}
		return value.gameObject;
	}
}
