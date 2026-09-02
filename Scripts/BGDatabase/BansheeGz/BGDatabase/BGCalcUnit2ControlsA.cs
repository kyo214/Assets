namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnit2ControlsA : BGCalcUnit
{
	public const string EnterPortName = "y";

	public const string ExitPortName = "z";

	protected BGCalcControlInput enterPort;

	protected BGCalcControlOutput exitPort;

	public override void Definition()
	{
		enterPort = ControlInput("enter", "y", RunMe);
		exitPort = ControlOutput("exit", "z");
	}

	private BGCalcControlOutputI RunMe(BGCalcFlowI flow)
	{
		Run(flow);
		return exitPort;
	}

	protected abstract void Run(BGCalcFlowI flow);
}
