namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitStringA<T> : BGCalcUnit
{
	private BGCalcValueInput a;

	protected abstract BGCalcTypeCode<T> OutputCode { get; }

	protected abstract string OutputLabel { get; }

	public override void Definition()
	{
		a = ValueInput(BGCalcTypeCodeRegistry.String, "A", "a");
		ValueOutput(OutputCode, OutputLabel, "r", (BGCalcFlowI flow) => Operation(flow.GetValue<string>(a)));
	}

	protected abstract T Operation(string a);
}
