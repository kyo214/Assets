using System;

namespace BansheeGz.BGDatabase;

public static class BGCalcUnitLocalizationDelegateProvider
{
	private const string DelegateClass = "BansheeGz.BGDatabase.BGCalcUnitLocalizationDelegate";

	private static BGCalcUnitLocalizationI delegateObject;

	public static BGCalcUnitLocalizationI Delegate
	{
		get
		{
			if (delegateObject != null)
			{
				return delegateObject;
			}
			Type type = BGUtil.GetType("BansheeGz.BGDatabase.BGCalcUnitLocalizationDelegate");
			if (type == null)
			{
				throw new Exception("Can not find localization delegate class BansheeGz.BGDatabase.BGCalcUnitLocalizationDelegate");
			}
			delegateObject = Activator.CreateInstance(type) as BGCalcUnitLocalizationI;
			if (delegateObject == null)
			{
				throw new Exception("Can not create localization delegate - object is null ");
			}
			return delegateObject;
		}
	}
}
