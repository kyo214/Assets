using UnityEngine;
using UnityEngine.EventSystems;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Controls/MMTouchRepositionableJoystick")]
public class MMTouchRepositionableJoystick : MMTouchJoystick, IPointerDownHandler, IEventSystemHandler
{
	[Header("Dynamic Joystick")]
	public CanvasGroup KnobCanvasGroup;

	public CanvasGroup BackgroundCanvasGroup;

	public bool ConstrainToInitialRectangle = true;

	protected Vector3 _initialPosition;

	protected Vector3 _newPosition;

	protected CanvasGroup _knobCanvasGroup;

	protected RectTransform _rect;

	protected override void Start()
	{
		base.Start();
		_initialPosition = GetComponent<RectTransform>().localPosition;
		_rect = GetComponent<RectTransform>();
	}

	public override void Initialize()
	{
		base.Initialize();
		SetKnobTransform(KnobCanvasGroup.transform);
		_canvasGroup = KnobCanvasGroup;
		_initialOpacity = _canvasGroup.alpha;
	}

	public virtual void OnPointerDown(PointerEventData data)
	{
		if (base.ParentCanvasRenderMode == RenderMode.ScreenSpaceCamera)
		{
			_newPosition = TargetCamera.ScreenToWorldPoint(data.position);
		}
		else
		{
			_newPosition = data.position;
		}
		_newPosition.z = base.transform.position.z;
		if (WithinBounds())
		{
			BackgroundCanvasGroup.transform.position = _newPosition;
			SetNeutralPosition(_newPosition);
			_knobTransform.position = _newPosition;
		}
	}

	protected virtual bool WithinBounds()
	{
		if (!ConstrainToInitialRectangle)
		{
			return true;
		}
		return RectTransformUtility.RectangleContainsScreenPoint(_rect, _newPosition);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
	}
}
