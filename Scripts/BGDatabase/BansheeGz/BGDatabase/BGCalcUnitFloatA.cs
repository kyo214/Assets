namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitFloatA<T> : BGCalcUnit
{
	private BGCalcValueInput a;

	protected abstract BGCalcTypeCode<T> OutputCode { get; }

	protected abstract string OutputLabel { get; }

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Float, "A", "a");
		ValueOutput(OutputCode, OutputLabel, "r", (BGCalcFlowI flow) => Operation(flow.GetValue<float>(a)));
	}

	protected abstract T Operation(float a);
}
