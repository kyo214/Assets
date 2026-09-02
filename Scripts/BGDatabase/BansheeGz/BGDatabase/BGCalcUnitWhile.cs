using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Flow/While")]
public class BGCalcUnitWhile : BGCalcUnit2ControlsA
{
	public const int Code = 107;

	private BGCalcControlOutput bodyPort;

	private BGCalcValueInput condition;

	private BGCalcValueOutput indexOutput;

	public override ushort TypeCode => 107;

	public override void Definition()
	{
		base.Definition();
		bodyPort = ControlOutput("body", "a");
		condition = ValueInput(BGCalcTypeCodeRegistry.Bool, "condition", "b");
		indexOutput = ValueOutput(BGCalcTypeCodeRegistry.Int, "index", "c", (BGCalcFlowI flow) =>
		{
			object localVar = flow.GetLocalVar(indexOutput);
			return (localVar != null) ? ((int)localVar) : 0;
		});
	}

	private bool GetCondition(BGCalcFlowI flow)
	{
		return flow.GetValue<bool>(condition);
	}

	protected override void Run(BGCalcFlowI flow)
	{
		if (!bodyPort.IsConnected)
		{
			return;
		}
		int num = 0;
		while (GetCondition(flow))
		{
			flow.SetValue(indexOutput, num);
			flow.RunNested(bodyPort.ConnectedPort);
			if (num++ > 10000)
			{
				throw new Exception($"Maximum number of iterations={10000} is exceeded!");
			}
			if (flow.BreakIsRequested)
			{
				break;
			}
		}
	}
}
