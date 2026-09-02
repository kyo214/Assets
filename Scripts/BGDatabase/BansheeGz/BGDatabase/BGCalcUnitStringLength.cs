namespace BansheeGz.BGDatabase;

[BGCalcUnitDefinition("By type/string/String length")]
public class BGCalcUnitStringLength : BGCalcUnitStringA<int>
{
	public const int Code = 95;

	public override ushort TypeCode => 95;

	protected override BGCalcTypeCode<int> OutputCode => BGCalcTypeCodeRegistry.Int;

	protected override string OutputLabel => "A.Length";

	protected override int Operation(string a)
	{
		if (!string.IsNullOrEmpty(a))
		{
			return a.Length;
		}
		return 0;
	}
}
