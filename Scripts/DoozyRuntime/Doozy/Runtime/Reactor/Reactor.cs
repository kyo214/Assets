using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;

namespace Doozy.Runtime.Reactor;

public static class Reactor
{
	public static ColorReaction To(PropertyGetter<Color> getter, PropertySetter<Color> setter, Color targetValue, float duration, bool relative = false, bool startReaction = false)
	{
		ColorReaction colorReaction = Reaction.Get<ColorReaction>().SetRuntimeHeartbeat().SetDuration(duration)
			.SetGetter(getter)
			.SetSetter(setter);
		colorReaction.SetValue(getter());
		if (startReaction)
		{
			colorReaction.PlayToValue(targetValue, relative);
		}
		else
		{
			colorReaction.SetTo(targetValue, relative);
		}
		return colorReaction;
	}

	public static FloatReaction To(PropertyGetter<float> getter, PropertySetter<float> setter, float targetValue, float duration, bool relative = false, bool startReaction = false)
	{
		FloatReaction floatReaction = Reaction.Get<FloatReaction>().SetRuntimeHeartbeat().SetDuration(duration);
		floatReaction.getter = getter;
		floatReaction.setter = setter;
		floatReaction.SetValue(getter());
		if (startReaction)
		{
			floatReaction.PlayToValue(targetValue, relative);
		}
		else
		{
			floatReaction.SetTo(targetValue, relative);
		}
		return floatReaction;
	}

	public static IntReaction To(PropertyGetter<int> getter, PropertySetter<int> setter, int targetValue, float duration, bool relative = false, bool startReaction = false)
	{
		IntReaction intReaction = Reaction.Get<IntReaction>().SetRuntimeHeartbeat().SetDuration(duration);
		intReaction.getter = getter;
		intReaction.setter = setter;
		intReaction.SetValue(getter());
		if (startReaction)
		{
			intReaction.PlayToValue(targetValue, relative);
		}
		else
		{
			intReaction.SetTo(targetValue, relative);
		}
		return intReaction;
	}

	public static Vector2Reaction To(PropertyGetter<Vector2> getter, PropertySetter<Vector2> setter, Vector2 targetValue, float duration, bool relative = false, bool startReaction = false)
	{
		Vector2Reaction vector2Reaction = Reaction.Get<Vector2Reaction>().SetRuntimeHeartbeat().SetDuration(duration);
		vector2Reaction.getter = getter;
		vector2Reaction.setter = setter;
		vector2Reaction.SetValue(getter());
		if (startReaction)
		{
			vector2Reaction.PlayToValue(targetValue, relative);
		}
		else
		{
			vector2Reaction.SetTo(targetValue, relative);
		}
		return vector2Reaction;
	}

	public static Vector3Reaction To(PropertyGetter<Vector3> getter, PropertySetter<Vector3> setter, Vector3 targetValue, float duration, bool relative = false, bool startReaction = false)
	{
		Vector3Reaction vector3Reaction = Reaction.Get<Vector3Reaction>().SetRuntimeHeartbeat().SetDuration(duration);
		vector3Reaction.getter = getter;
		vector3Reaction.setter = setter;
		vector3Reaction.SetValue(getter());
		if (startReaction)
		{
			vector3Reaction.PlayToValue(targetValue, relative);
		}
		else
		{
			vector3Reaction.SetTo(targetValue, relative);
		}
		return vector3Reaction;
	}
}
