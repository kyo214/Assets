using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Components;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Components/UIScrollbar")]
[SelectionBase]
public class UIScrollbar : UISelectable, IBeginDragHandler, IEventSystemHandler, IDragHandler, IInitializePotentialDragHandler
{
	public const float k_MINValue = 0f;

	public const float k_MAXValue = 1f;

	public const int k_MINNumberOfSteps = 0;

	public const int k_MAXNumberOfSteps = 20;

	[ClearOnReload]
	private static SignalStream s_stream;

	public FloatEvent OnValueChangedCallback;

	[SerializeField]
	private RectTransform HandleRect;

	[SerializeField]
	private SlideDirection Direction;

	[SerializeField]
	protected float Value;

	[SerializeField]
	private float Size = 0.2f;

	[Range(0f, 11f)]
	[SerializeField]
	private int NumberOfSteps;

	private RectTransform m_ContainerRect;

	private Vector2 m_Offset = Vector2.zero;

	private DrivenRectTransformTracker m_Tracker;

	private Coroutine m_PointerDownRepeat;

	private bool m_IsPointerDownAndNotDragging;

	private bool m_DelayedUpdateVisuals;

	public static HashSet<UIScrollbar> database { get; private set; } = new HashSet<UIScrollbar>();

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream("UISelectable", "UIScrollbar"));

	public static IEnumerable<UIScrollbar> availableScrollbars => database.Where((UIScrollbar item) => item.isActiveAndEnabled);

	public bool isSelected => EventSystem.current.currentSelectedGameObject == base.gameObject;

	public override SelectableType selectableType => SelectableType.Button;

	public RectTransform handleRect
	{
		get
		{
			return HandleRect;
		}
		set
		{
			if (!(value == HandleRect))
			{
				HandleRect = value;
				UpdateCachedReferences();
				UpdateVisuals();
			}
		}
	}

	public SlideDirection direction
	{
		get
		{
			return Direction;
		}
		set
		{
			Direction = value;
			UpdateVisuals();
		}
	}

	public virtual float value
	{
		get
		{
			if (NumberOfSteps <= 1)
			{
				return Value;
			}
			return Mathf.Round(Value * (float)(NumberOfSteps - 1)) / (float)(NumberOfSteps - 1);
		}
		set
		{
			Set(value);
		}
	}

	public float size
	{
		get
		{
			return Size;
		}
		set
		{
			Size = value.Clamp01();
			UpdateVisuals();
		}
	}

	public int numberOfSteps
	{
		get
		{
			return NumberOfSteps;
		}
		set
		{
			NumberOfSteps = value.Clamp(0, 11);
			UpdateVisuals();
		}
	}

	private Axis axis => Direction switch
	{
		SlideDirection.LeftToRight => Axis.Horizontal, 
		SlideDirection.RightToLeft => Axis.Horizontal, 
		SlideDirection.BottomToTop => Axis.Vertical, 
		SlideDirection.TopToBottom => Axis.Vertical, 
		_ => throw new ArgumentOutOfRangeException(), 
	};

	private bool reverseValue
	{
		get
		{
			if (Direction != SlideDirection.RightToLeft)
			{
				return Direction == SlideDirection.TopToBottom;
			}
			return true;
		}
	}

	private float stepSize
	{
		get
		{
			if (NumberOfSteps <= 1)
			{
				return 0.1f;
			}
			return 1f / (float)(NumberOfSteps - 1);
		}
	}

	[ExecuteOnReload]
	private static void OnReload()
	{
		database = new HashSet<UIScrollbar>();
	}

	private UIScrollbar()
	{
	}

	public override void Rebuild(CanvasUpdate executing)
	{
		base.Rebuild(executing);
	}

	protected override void Awake()
	{
		database.Add(this);
		base.Awake();
	}

	protected override void OnEnable()
	{
		database.Remove(null);
		base.OnEnable();
		UpdateCachedReferences();
		Set(Value, sendCallback: false);
		UpdateVisuals();
	}

	protected override void OnDisable()
	{
		database.Remove(null);
		m_Tracker.Clear();
		base.OnDisable();
	}

	protected override void OnDestroy()
	{
		database.Remove(null);
		database.Remove(this);
		base.OnDestroy();
	}

	private void Update()
	{
		if (m_DelayedUpdateVisuals)
		{
			m_DelayedUpdateVisuals = false;
			Set(Value, sendCallback: false);
			UpdateVisuals();
		}
	}

	private void UpdateCachedReferences()
	{
		Transform transform = ((HandleRect != null) ? HandleRect.parent : null);
		m_ContainerRect = ((transform != null) ? transform.GetComponent<RectTransform>() : null);
	}

	public virtual void SetValueWithoutNotify(float input)
	{
		Set(input, sendCallback: false);
	}

	private void Set(float input, bool sendCallback = true)
	{
		if (!input.Approximately(value))
		{
			Value = input;
			UpdateVisuals();
			if (sendCallback)
			{
				UISystemProfilerApi.AddMarker("UIScrollbar.value", this);
				OnValueChangedCallback.Invoke(value);
				stream.SendSignal(input);
			}
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		if (IsActive())
		{
			UpdateVisuals();
		}
	}

	public void UpdateVisuals()
	{
		m_Tracker.Clear();
		if (!(m_ContainerRect == null))
		{
			m_Tracker.Add(this, HandleRect, DrivenTransformProperties.Anchors);
			Vector2 zero = Vector2.zero;
			Vector2 one = Vector2.one;
			float num = value.Clamp01() * (1f - size);
			if (reverseValue)
			{
				zero[(int)axis] = 1f - num - size;
				one[(int)axis] = 1f - num;
			}
			else
			{
				zero[(int)axis] = num;
				one[(int)axis] = num + size;
			}
			HandleRect.anchorMin = zero;
			HandleRect.anchorMax = one;
		}
	}

	private void UpdateDrag(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left || m_ContainerRect == null)
		{
			return;
		}
		Vector2 position = Vector2.zero;
		if (MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position) && RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ContainerRect, position, eventData.pressEventCamera, out var localPoint))
		{
			Rect rect = m_ContainerRect.rect;
			Vector2 handleCorner = localPoint - m_Offset - rect.position - (HandleRect.rect.size - HandleRect.sizeDelta) * 0.5f;
			float num = ((axis == Axis.Horizontal) ? rect.width : rect.height) * (1f - size);
			if (!(num <= 0f))
			{
				DoUpdateDrag(handleCorner, num);
			}
		}
	}

	private void DoUpdateDrag(Vector2 handleCorner, float remainingSize)
	{
		switch (Direction)
		{
		case SlideDirection.LeftToRight:
			Set(Mathf.Clamp01(handleCorner.x / remainingSize));
			break;
		case SlideDirection.RightToLeft:
			Set(Mathf.Clamp01(1f - handleCorner.x / remainingSize));
			break;
		case SlideDirection.BottomToTop:
			Set(Mathf.Clamp01(handleCorner.y / remainingSize));
			break;
		case SlideDirection.TopToBottom:
			Set(Mathf.Clamp01(1f - handleCorner.y / remainingSize));
			break;
		}
	}

	private bool MayDrag(PointerEventData eventData)
	{
		if (IsActive() && IsInteractable())
		{
			return eventData.button == PointerEventData.InputButton.Left;
		}
		return false;
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		m_IsPointerDownAndNotDragging = false;
		if (MayDrag(eventData) && !(m_ContainerRect == null))
		{
			m_Offset = Vector2.zero;
			if (RectTransformUtility.RectangleContainsScreenPoint(HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera) && RectTransformUtility.ScreenPointToLocalPointInRectangle(HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out var localPoint))
			{
				m_Offset = localPoint - HandleRect.rect.center;
			}
		}
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		if (MayDrag(eventData) && m_ContainerRect != null)
		{
			UpdateDrag(eventData);
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (MayDrag(eventData))
		{
			base.OnPointerDown(eventData);
			m_IsPointerDownAndNotDragging = true;
			m_PointerDownRepeat = StartCoroutine(ClickRepeat(eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera));
		}
	}

	protected IEnumerator ClickRepeat(PointerEventData eventData)
	{
		return ClickRepeat(eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera);
	}

	protected IEnumerator ClickRepeat(Vector2 screenPosition, Camera sourceCamera)
	{
		while (m_IsPointerDownAndNotDragging)
		{
			if (!RectTransformUtility.RectangleContainsScreenPoint(HandleRect, screenPosition, sourceCamera) && RectTransformUtility.ScreenPointToLocalPointInRectangle(HandleRect, screenPosition, sourceCamera, out var localPoint))
			{
				float num = ((((axis == Axis.Horizontal) ? localPoint.x : localPoint.y) < 0f) ? size : (0f - size));
				value += (reverseValue ? num : (0f - num));
			}
			yield return new WaitForEndOfFrame();
		}
		StopCoroutine(m_PointerDownRepeat);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		m_IsPointerDownAndNotDragging = false;
	}

	public override void OnMove(AxisEventData eventData)
	{
		if (!IsActive() || !IsInteractable())
		{
			base.OnMove(eventData);
			return;
		}
		switch (eventData.moveDir)
		{
		case MoveDirection.Left:
			if (axis == Axis.Horizontal && FindSelectableOnLeft() == null)
			{
				Set(Mathf.Clamp01(reverseValue ? (value + stepSize) : (value - stepSize)));
			}
			else
			{
				base.OnMove(eventData);
			}
			break;
		case MoveDirection.Right:
			if (axis == Axis.Horizontal && FindSelectableOnRight() == null)
			{
				Set(Mathf.Clamp01(reverseValue ? (value - stepSize) : (value + stepSize)));
			}
			else
			{
				base.OnMove(eventData);
			}
			break;
		case MoveDirection.Up:
			if (axis == Axis.Vertical && FindSelectableOnUp() == null)
			{
				Set(Mathf.Clamp01(reverseValue ? (value - stepSize) : (value + stepSize)));
			}
			else
			{
				base.OnMove(eventData);
			}
			break;
		case MoveDirection.Down:
			if (axis == Axis.Vertical && FindSelectableOnDown() == null)
			{
				Set(Mathf.Clamp01(reverseValue ? (value + stepSize) : (value - stepSize)));
			}
			else
			{
				base.OnMove(eventData);
			}
			break;
		}
	}

	public override Selectable FindSelectableOnLeft()
	{
		if (base.navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
		{
			return null;
		}
		return base.FindSelectableOnLeft();
	}

	public override Selectable FindSelectableOnRight()
	{
		if (base.navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
		{
			return null;
		}
		return base.FindSelectableOnRight();
	}

	public override Selectable FindSelectableOnUp()
	{
		if (base.navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
		{
			return null;
		}
		return base.FindSelectableOnUp();
	}

	public override Selectable FindSelectableOnDown()
	{
		if (base.navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
		{
			return null;
		}
		return base.FindSelectableOnDown();
	}

	public virtual void OnInitializePotentialDrag(PointerEventData eventData)
	{
		eventData.useDragThreshold = false;
	}

	public void SetDirection(SlideDirection slideDirection, bool includeRectLayouts)
	{
		Axis axis = this.axis;
		bool flag = reverseValue;
		direction = slideDirection;
		if (includeRectLayouts)
		{
			if (this.axis != axis)
			{
				RectTransformUtility.FlipLayoutAxes(base.transform as RectTransform, keepPositioning: true, recursive: true);
			}
			if (reverseValue != flag)
			{
				RectTransformUtility.FlipLayoutOnAxis(base.transform as RectTransform, (int)this.axis, keepPositioning: true, recursive: true);
			}
		}
	}
}
