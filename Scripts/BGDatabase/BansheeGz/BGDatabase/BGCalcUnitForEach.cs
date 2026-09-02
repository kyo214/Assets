using System.Collections;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Flow/ForEach")]
public class BGCalcUnitForEach : BGCalcUnit2ControlsA
{
	public const int Code = 108;

	private BGCalcControlOutput bodyPort;

	private BGCalcValueInput listPort;

	private BGCalcValueOutput objPort;

	private BGCalcValueOutput indexPort;

	public override ushort TypeCode => 108;

	public override void Definition()
	{
		base.Definition();
		bodyPort = ControlOutput("body", "a");
		listPort = ValueInput(BGCalcTypeCodeRegistry.List, "list", "b");
		objPort = ValueOutput(BGCalcTypeCodeRegistry.Object, "object", "c", (BGCalcFlowI flow) => flow.GetLocalVar(objPort));
		indexPort = ValueOutput(BGCalcTypeCodeRegistry.Int, "index", "d", (BGCalcFlowI flow) =>
		{
			object localVar = flow.GetLocalVar(indexPort);
			return (localVar != null) ? ((int)localVar) : 0;
		});
	}

	protected override void Run(BGCalcFlowI flow)
	{
		if (!bodyPort.IsConnected)
		{
			return;
		}
		IList value = flow.GetValue<IList>(listPort);
		if (value == null || value.Count == 0)
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			flow.SetValue(objPort, value[i]);
			flow.SetValue(indexPort, i);
			flow.RunNested(bodyPort.ConnectedPort);
			if (flow.BreakIsRequested)
			{
				break;
			}
		}
	}
}
