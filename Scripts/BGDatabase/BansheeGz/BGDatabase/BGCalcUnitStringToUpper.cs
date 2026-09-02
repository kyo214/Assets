using System.Globalization;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/ToUpper")]
public class BGCalcUnitStringToUpper : BGCalcUnitStringAString
{
	public const int Code = 53;

	public override ushort TypeCode => 53;

	protected override string OutputLabel => "toUpper(A)";

	protected override string Operation(string a)
	{
		return a.ToUpper(CultureInfo.InvariantCulture);
	}
}
