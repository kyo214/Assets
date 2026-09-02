using System.Linq;

namespace Doozy.Runtime.Mody;

public static class ModyModuleExtensions
{
	public static T SetName<T>(this T target, string value) where T : ModyModule
	{
		target.moduleName = value;
		return target;
	}

	public static T AddAction<T>(this T target, ModyAction action) where T : ModyModule
	{
		target.actions.Add(action);
		return target;
	}

	public static T RemoveAction<T>(this T target, ModyAction action) where T : ModyModule
	{
		target.actions.Remove(action);
		return target;
	}

	public static T RemoveAction<T>(this T target, string actionName) where T : ModyModule
	{
		foreach (ModyAction item in target.actions.ToList())
		{
			if (actionName.Equals(item.actionName))
			{
				target.actions.Remove(item);
			}
		}
		return target;
	}
}
