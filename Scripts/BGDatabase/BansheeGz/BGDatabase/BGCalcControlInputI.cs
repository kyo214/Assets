using System;

namespace BansheeGz.BGDatabase;

public interface BGCalcControlInputI : BGCalcPortI
{
	Func<BGCalcFlowI, BGCalcControlOutputI> Action { get; }
}
