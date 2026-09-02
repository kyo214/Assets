using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcUnitGetCurrentGameObject : BGCalcUnit
{
	public const int Code = 125;

	public override ushort TypeCode => 125;

	public override string Title => "Get current GameObject";

	public override void Definition()
	{
		ValueOutput(BGCalcTypeCodeRegistry.GameObject, "gameObject", "e", GetGameObject);
	}

	private GameObject GetGameObject(BGCalcFlowI flow)
	{
		return flow.Context.CurrentGameObject;
	}
}
