namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/Trim")]
public class BGCalcUnitStringTrim : BGCalcUnitStringAString
{
	public const int Code = 58;

	public override ushort TypeCode => 58;

	protected override string OutputLabel => "toUpper(A)";

	protected override string Operation(string a)
	{
		return a.Trim();
	}
}
