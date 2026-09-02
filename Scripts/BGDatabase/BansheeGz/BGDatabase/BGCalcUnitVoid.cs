namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("Special/void", true)]
public class BGCalcUnitVoid : BGCalcUnit
{
	public const int Code = 123;

	public override ushort TypeCode => 123;

	public override string Title => "VOID (I do not exist)";

	public override void Definition()
	{
	}
}
