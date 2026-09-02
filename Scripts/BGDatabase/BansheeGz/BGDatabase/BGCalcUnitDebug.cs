using UnityEngine;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Special/Debug")]
public class BGCalcUnitDebug : BGCalcUnit2ControlsA
{
	public const int Code = 101;

	private BGCalcValueInput messagePort;

	public override ushort TypeCode => 101;

	public override void Definition()
	{
		base.Definition();
		messagePort = ValueInput<object>("message", "m");
	}

	protected override void Run(BGCalcFlowI flow)
	{
		Debug.Log(flow.GetValue(messagePort));
	}
}
