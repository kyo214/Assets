using System;

namespace BansheeGz.BGDatabase;

public class BGCalcUnitGetLocalizedValue : BGCalcUnitDbRowBasedA
{
	public const int Code = 114;

	public override ushort TypeCode => 114;

	protected override string Operation => "get localized";

	public override void Definition()
	{
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			throw new Exception("Meta is not found! id=" + base.MetaId.ToString());
		}
		base.Definition();
		Type valueType = BGCalcUnitLocalizationDelegateProvider.Delegate.GetValueType(meta);
		ValueOutput(valueType, "value", "r", GetValue);
	}

	private object GetValue(BGCalcFlowI flow)
	{
		return BGCalcUnitLocalizationDelegateProvider.Delegate.GetValue(base.MetaCached, GetEntity(flow));
	}
}
