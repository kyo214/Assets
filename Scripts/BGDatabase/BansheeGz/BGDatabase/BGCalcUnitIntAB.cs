namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitIntAB<T> : BGCalcUnit
{
	private BGCalcValueInput b;

	private BGCalcValueInput a;

	protected abstract BGCalcTypeCode<T> OutputCode { get; }

	protected abstract string OutputLabel { get; }

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Int, "A", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Int, "B", "b");
		ValueOutput(OutputCode, OutputLabel, "r", (BGCalcFlowI flow) => Operation(flow.GetValue<int>(a), flow.GetValue<int>(b)));
	}

	protected abstract T Operation(int a, int b);
}
