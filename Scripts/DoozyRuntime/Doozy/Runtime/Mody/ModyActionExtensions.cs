using UnityEngine;

namespace Doozy.Runtime.Mody;

public static class ModyActionExtensions
{
	public static T SetEnabled<T>(this T target, bool value) where T : ModyAction
	{
		target.enabled = value;
		return target;
	}

	public static T SetBehaviour<T>(this T target, MonoBehaviour behaviour) where T : ModyAction
	{
		target.actionBehaviourReference = behaviour;
		return target;
	}

	public static T SetStopAllActionsOnStart<T>(this T target, bool value) where T : ModyAction
	{
		target.onStartStopOtherActions = value;
		return target;
	}

	public static T SetStartDelay<T>(this T target, float value) where T : ModyAction
	{
		target.startDelay = value;
		return target;
	}

	public static T SetDuration<T>(this T target, float value) where T : ModyAction
	{
		target.duration = value;
		return target;
	}

	public static T SetCooldown<T>(this T target, float value) where T : ModyAction
	{
		target.cooldown = value;
		return target;
	}

	public static T SetTimescaleIndependent<T>(this T target, bool value) where T : ModyAction
	{
		target.isTimescaleIndependent = value;
		return target;
	}
}
