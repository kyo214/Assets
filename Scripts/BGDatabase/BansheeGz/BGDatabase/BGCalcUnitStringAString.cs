namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnitStringAString : BGCalcUnitStringA<string>
{
	protected override BGCalcTypeCode<string> OutputCode => BGCalcTypeCodeRegistry.String;
}
