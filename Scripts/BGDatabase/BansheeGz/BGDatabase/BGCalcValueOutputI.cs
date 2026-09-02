using System;

namespace BansheeGz.BGDatabase;

public interface BGCalcValueOutputI : BGCalcPortI
{
	Func<BGCalcFlowI, object> GetValue { get; }
}
