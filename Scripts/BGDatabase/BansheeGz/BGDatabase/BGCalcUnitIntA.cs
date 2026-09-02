namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitIntA<T> : BGCalcUnit
{
	private BGCalcValueInput a;

	protected abstract BGCalcTypeCode<T> OutputCode { get; }

	protected abstract string OutputLabel { get; }

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.Int, "A", "a");
		ValueOutput(OutputCode, OutputLabel, "r", (BGCalcFlowI flow) => Operation(flow.GetValue<int>(a)));
	}

	protected abstract T Operation(int a);
}
