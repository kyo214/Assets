using System;

namespace BansheeGz.BGDatabase;

public interface BGCalcValueInputI : BGCalcPortI
{
	BGCalcValueOutputI ConnectedPort { get; }

	bool SupportDefaultValue { get; }

	object DefaultValue { get; set; }

	bool HasDefaultValue { get; }

	event Action OnChange;
}
