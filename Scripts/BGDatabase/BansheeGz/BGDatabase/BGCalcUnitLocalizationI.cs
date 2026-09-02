using System;

namespace BansheeGz.BGDatabase;

public interface BGCalcUnitLocalizationI
{
	string CurrentLocale { get; set; }

	Type GetValueType(BGMetaEntity meta);

	object GetValue(BGMetaEntity meta, BGEntity entity);
}
