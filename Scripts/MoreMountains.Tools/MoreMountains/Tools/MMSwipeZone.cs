using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace MoreMountains.Tools;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("More Mountains/Tools/Controls/MMSwipeZone")]
public class MMSwipeZone : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
	public float MinimalSwipeLength = 50f;

	public float MaximumPressLength = 10f;

	public SwipeEvent ZoneSwiped;

	public UnityEvent ZonePressed;

	[Header("Mouse Mode")]
	[MMInformation("If you set this to true, you'll need to actually press the button for it to be triggered, otherwise a simple hover will trigger it (better for touch input).", MMInformationAttribute.InformationType.Info, false)]
	public bool MouseMode;

	protected Vector2 _firstTouchPosition;

	protected float _angle;

	protected float _length;

	protected Vector2 _destination;

	protected Vector2 _deltaSwipe;

	protected MMPossibleSwipeDirections _swipeDirection;

	protected float _lastPointerUpAt;

	protected float _swipeStartedAt;

	protected float _swipeEndedAt;

	protected virtual void Swipe()
	{
		float swipeDuration = _swipeEndedAt - _swipeStartedAt;
		MMSwipeEvent mMSwipeEvent = new MMSwipeEvent(_swipeDirection, _angle, _length, _firstTouchPosition, _destination, swipeDuration);
		MMEventManager.TriggerEvent(mMSwipeEvent);
		if (ZoneSwiped != null)
		{
			ZoneSwiped.Invoke(mMSwipeEvent);
		}
	}

	protected virtual void Press()
	{
		if (ZonePressed != null)
		{
			ZonePressed.Invoke();
		}
	}

	public virtual void OnPointerDown(PointerEventData data)
	{
		_firstTouchPosition = Mouse.current.position.ReadValue();
		_swipeStartedAt = Time.unscaledTime;
	}

	public virtual void OnPointerUp(PointerEventData data)
	{
		if ((float)Time.frameCount != _lastPointerUpAt)
		{
			_destination = Mouse.current.position.ReadValue();
			_deltaSwipe = _destination - _firstTouchPosition;
			_length = _deltaSwipe.magnitude;
			if (_length > MinimalSwipeLength)
			{
				_angle = MMMaths.AngleBetween(_deltaSwipe, Vector2.right);
				_swipeDirection = AngleToSwipeDirection(_angle);
				_swipeEndedAt = Time.unscaledTime;
				Swipe();
			}
			if (_deltaSwipe.magnitude < MaximumPressLength)
			{
				Press();
			}
			_lastPointerUpAt = Time.frameCount;
		}
	}

	public virtual void OnPointerEnter(PointerEventData data)
	{
		if (!MouseMode)
		{
			OnPointerDown(data);
		}
	}

	public virtual void OnPointerExit(PointerEventData data)
	{
		if (!MouseMode)
		{
			OnPointerUp(data);
		}
	}

	protected virtual MMPossibleSwipeDirections AngleToSwipeDirection(float angle)
	{
		if (angle < 45f || angle >= 315f)
		{
			return MMPossibleSwipeDirections.Right;
		}
		if (angle >= 45f && angle < 135f)
		{
			return MMPossibleSwipeDirections.Up;
		}
		if (angle >= 135f && angle < 225f)
		{
			return MMPossibleSwipeDirections.Left;
		}
		if (angle >= 225f && angle < 315f)
		{
			return MMPossibleSwipeDirections.Down;
		}
		return MMPossibleSwipeDirections.Right;
	}
}
