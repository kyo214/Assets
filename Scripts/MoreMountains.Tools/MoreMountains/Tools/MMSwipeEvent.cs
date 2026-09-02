using UnityEngine;

namespace MoreMountains.Tools;

public struct MMSwipeEvent(MMPossibleSwipeDirections direction, float angle, float length, Vector2 origin, Vector2 destination, float swipeDuration)
{
	public MMPossibleSwipeDirections SwipeDirection = direction;

	public float SwipeAngle = angle;

	public float SwipeLength = length;

	public Vector2 SwipeOrigin = origin;

	public Vector2 SwipeDestination = destination;

	public float SwipeDuration = swipeDuration;

	private static MMSwipeEvent e;

	public static void Trigger(MMPossibleSwipeDirections direction, float angle, float length, Vector2 origin, Vector2 destination, float swipeDuration)
	{
		e.SwipeDirection = direction;
		e.SwipeAngle = angle;
		e.SwipeLength = length;
		e.SwipeOrigin = origin;
		e.SwipeDestination = destination;
		e.SwipeDuration = swipeDuration;
		MMEventManager.TriggerEvent(e);
	}
}
