namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/IsNullOrEmpty")]
public class BGCalcUnitStringIsNullOrEmpty : BGCalcUnitStringA<bool>
{
	public const int Code = 84;

	public override ushort TypeCode => 84;

	protected override BGCalcTypeCode<bool> OutputCode => BGCalcTypeCodeRegistry.Bool;

	protected override string OutputLabel => "IsEmpty(A)";

	protected override bool Operation(string a)
	{
		return string.IsNullOrEmpty(a);
	}
}
