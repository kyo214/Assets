using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Extensions;

public static class RectTransformExtensions
{
	public static Vector3Reaction AnchorPosition3DReaction(this RectTransform target, Vector3 targetValue, float duration, bool relative)
	{
		return Reactor.To(() => target.anchoredPosition3D, (Vector3 value) =>
		{
			target.anchoredPosition3D = value;
		}, targetValue, duration, relative);
	}

	public static Vector3Reaction RotationReaction(this RectTransform target, Vector3 targetValue, float duration, bool relative)
	{
		return Reactor.To(() => target.localEulerAngles, (Vector3 value) =>
		{
			target.localRotation = Quaternion.Euler(value);
		}, targetValue, duration, relative);
	}
}
