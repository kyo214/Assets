namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Hidden/Start", true)]
public class BGCalcUnitGraphStart : BGCalcUnit
{
	public const int Code = 1;

	private BGCalcControlOutput startPort;

	public BGCalcControlOutput StartPort => startPort;

	public override ushort TypeCode => 1;

	public override void Definition()
	{
		startPort = ControlOutput("start", "s");
	}
}
