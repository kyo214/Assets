namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitFloatAB<T> : BGCalcUnit
{
	private BGCalcValueInput b;

	private BGCalcValueInput a;

	protected abstract BGCalcTypeCode<T> OutputCode { get; }

	protected abstract string OutputLabel { get; }

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Float, "A", "a");
		b = ValueInput(BGCalcTypeCodeRegistry.Float, "B", "b");
		ValueOutput(OutputCode, OutputLabel, "r", (BGCalcFlowI flow) => Operation(flow.GetValue<float>(a), flow.GetValue<float>(b)));
	}

	protected abstract T Operation(float a, float b);
}
