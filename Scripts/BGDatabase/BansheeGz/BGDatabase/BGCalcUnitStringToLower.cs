using System.Globalization;

namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/ToLower")]
public class BGCalcUnitStringToLower : BGCalcUnitStringAString
{
	public const int Code = 52;

	public override ushort TypeCode => 52;

	protected override string OutputLabel => "toLower(A)";

	protected override string Operation(string a)
	{
		return a.ToLower(CultureInfo.InvariantCulture);
	}
}
