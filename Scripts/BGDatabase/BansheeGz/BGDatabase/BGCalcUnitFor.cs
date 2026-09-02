using System;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Flow/For")]
public class BGCalcUnitFor : BGCalcUnit2ControlsA
{
	public const int Code = 102;

	private BGCalcControlOutput bodyPort;

	private BGCalcValueInput firstPort;

	private BGCalcValueInput lastPort;

	private BGCalcValueInput stepPort;

	private BGCalcValueOutput indexPort;

	public override ushort TypeCode => 102;

	public override void Definition()
	{
		base.Definition();
		bodyPort = ControlOutput("body", "a");
		firstPort = ValueInput(BGCalcTypeCodeRegistry.Int, "First", "b");
		lastPort = ValueInput(BGCalcTypeCodeRegistry.Int, "Last", "c");
		stepPort = ValueInput(BGCalcTypeCodeRegistry.Int, "Step", "d");
		indexPort = ValueOutput(BGCalcTypeCodeRegistry.Int, "Index", "e", (BGCalcFlowI flow) =>
		{
			object localVar = flow.GetLocalVar(indexPort);
			return (localVar != null) ? ((int)localVar) : 0;
		});
	}

	protected override void Run(BGCalcFlowI flow)
	{
		int value = flow.GetValue<int>(firstPort);
		int value2 = flow.GetValue<int>(lastPort);
		int value3 = flow.GetValue<int>(stepPort);
		if (value2 - value >= value2 - value + value3)
		{
			throw new Exception($"Loop can not be executed, cause with such parameters (First={value}, last={value2}, step={value3}), the loop will never end");
		}
		if (value + value3 * 10000 < value2)
		{
			throw new Exception($"Maximum number of iterations={10000} is exceeded!");
		}
		if (bodyPort.IsConnected)
		{
			int num = 0;
			for (int i = value; i < value2; i += value3)
			{
				flow.SetValue(indexPort, i);
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
		else
		{
			flow.SetValue(indexPort, value2 - value3);
		}
	}
}
