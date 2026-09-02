using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Components;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UI/Components/UISlider")]
[SelectionBase]
public class UISlider : UISelectable, IDragHandler, IEventSystemHandler, IInitializePotentialDragHandler
{
	private const float TOLERANCE = 0.0001f;

	[ClearOnReload]
	private static SignalStream s_stream;

	public UISliderId Id;

	[Obsolete("Use OnValueChanged instead")]
	public FloatEvent OnValueChangedCallback;

	public FloatEvent OnValueChanged = new FloatEvent();

	public FloatEvent OnValueIncremented = new FloatEvent();

	public FloatEvent OnValueDecremented = new FloatEvent();

	public ModyEvent OnValueReset = new ModyEvent();

	public ModyEvent OnValueReachedMin = new ModyEvent();

	public ModyEvent OnValueReachedMax = new ModyEvent();

	[SerializeField]
	private RectTransform FillRect;

	[SerializeField]
	private RectTransform HandleRect;

	[SerializeField]
	private SlideDirection Direction;

	[SerializeField]
	private float MinValue;

	[SerializeField]
	private float MaxValue = 1f;

	[SerializeField]
	private bool WholeNumbers;

	[SerializeField]
	protected float Value;

	[SerializeField]
	private float DefaultValue;

	[SerializeField]
	private TMP_Text ValueLabel;

	[SerializeField]
	private TMP_Text MinValueLabel;

	[SerializeField]
	private TMP_Text MaxValueLabel;

	[SerializeField]
	private Progressor TargetProgressor;

	public bool InstantProgressorUpdate = true;

	public bool ResetValueOnEnable;

	private Image m_FillImage;

	private Transform m_FillTransform;

	private RectTransform m_FillContainerRect;

	private Transform m_HandleTransform;

	private RectTransform m_HandleContainerRect;

	private Vector2 m_Offset = Vector2.zero;

	private DrivenRectTransformTracker m_Tracker;

	private bool m_DelayedUpdateVisuals;

	public static HashSet<UISlider> database { get; private set; } = new HashSet<UISlider>();

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream("UISelectable", "UISlider"));

	public static IEnumerable<UISlider> availableSliders => database.Where((UISlider item) => item.isActiveAndEnabled);

	public bool isSelected => EventSystem.current.currentSelectedGameObject == base.gameObject;

	public override SelectableType selectableType => SelectableType.Button;

	public RectTransform fillRect
	{
		get
		{
			return FillRect;
		}
		set
		{
			if (!(value == FillRect))
			{
				FillRect = value;
				UpdateCachedReferences();
				UpdateVisuals();
			}
		}
	}

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

	public float minValue
	{
		get
		{
			return MinValue;
		}
		set
		{
			MinValue = value;
			Value.Clamp(MinValue, MaxValue);
			UpdateLabel(minValueLabel, MinValue);
			UpdateVisuals();
		}
	}

	public float maxValue
	{
		get
		{
			return MaxValue;
		}
		set
		{
			MaxValue = value;
			Value.Clamp(MinValue, MaxValue);
			UpdateLabel(maxValueLabel, MaxValue);
			UpdateVisuals();
		}
	}

	public bool wholeNumbers
	{
		get
		{
			return WholeNumbers;
		}
		set
		{
			WholeNumbers = value;
			if (value)
			{
				MinValue = Mathf.Round(MinValue);
				MaxValue = Mathf.Round(MaxValue);
				Value.Clamp(MinValue, MaxValue);
				UpdateVisuals();
				UpdateLabel(minValueLabel, MinValue);
				UpdateLabel(maxValueLabel, MaxValue);
				UpdateLabel(valueLabel, Value);
			}
		}
	}

	public virtual float value
	{
		get
		{
			if (!wholeNumbers)
			{
				return Value;
			}
			return Mathf.Round(Value);
		}
		set
		{
			SetValue(value);
		}
	}

	public float defaultValue
	{
		get
		{
			return DefaultValue;
		}
		set
		{
			DefaultValue = Mathf.Clamp(value, minValue, maxValue);
		}
	}

	public TMP_Text valueLabel
	{
		get
		{
			return ValueLabel;
		}
		private set
		{
			ValueLabel = value;
			UpdateLabel(ValueLabel, Value);
		}
	}

	public TMP_Text minValueLabel
	{
		get
		{
			return MinValueLabel;
		}
		private set
		{
			MinValueLabel = value;
			UpdateLabel(MinValueLabel, minValue);
		}
	}

	public TMP_Text maxValueLabel
	{
		get
		{
			return MaxValueLabel;
		}
		private set
		{
			MaxValueLabel = value;
			UpdateLabel(MaxValueLabel, maxValue);
		}
	}

	public Progressor targetProgressor
	{
		get
		{
			return TargetProgressor;
		}
		private set
		{
			TargetProgressor = value;
			if (!(value == null))
			{
				UpdateTargetProgressorMinMax();
				UpdateTargetProgressorValue();
			}
		}
	}

	public float normalizedValue
	{
		get
		{
			if (!Mathf.Approximately(minValue, maxValue))
			{
				return Mathf.InverseLerp(minValue, maxValue, value);
			}
			return 0f;
		}
		set
		{
			this.value = Mathf.Lerp(minValue, maxValue, value);
		}
	}

	private Axis axis => GetAxis(direction);

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
			if (!wholeNumbers)
			{
				return (maxValue - minValue) * 0.1f;
			}
			return 1f;
		}
	}

	[ExecuteOnReload]
	private static void OnReload()
	{
		database = new HashSet<UISlider>();
	}

	private static Axis GetAxis(SlideDirection slideDirection)
	{
		switch (slideDirection)
		{
		case SlideDirection.LeftToRight:
		case SlideDirection.RightToLeft:
			return Axis.Horizontal;
		case SlideDirection.BottomToTop:
		case SlideDirection.TopToBottom:
			return Axis.Vertical;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private UISlider()
	{
		Id = new UISliderId();
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
		if (Application.isPlaying)
		{
			UpdateCachedReferences();
			UpdateTargetProgressorMinMax();
			if (ResetValueOnEnable)
			{
				ResetValue();
			}
			else
			{
				UpdateTargetProgressorValue();
			}
			UpdateVisuals();
			UpdateLabel(minValueLabel, minValue);
			UpdateLabel(maxValueLabel, maxValue);
		}
	}

	protected override void OnDisable()
	{
		database.Remove(null);
		m_Tracker.Clear();
		UpdateTargetProgressorValue();
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
			SetValue(Value, sendCallback: false);
			UpdateVisuals();
		}
	}

	protected override void OnDidApplyAnimationProperties()
	{
		Value = ClampValue(Value);
		float a = normalizedValue;
		if (m_FillContainerRect != null)
		{
			a = ((!(m_FillImage != null) || m_FillImage.type != Image.Type.Filled) ? (reverseValue ? (1f - FillRect.anchorMin[(int)axis]) : FillRect.anchorMax[(int)axis]) : m_FillImage.fillAmount);
		}
		else if (m_HandleContainerRect != null)
		{
			a = (reverseValue ? (1f - HandleRect.anchorMin[(int)axis]) : HandleRect.anchorMin[(int)axis]);
		}
		UpdateVisuals();
		if (!Mathf.Approximately(a, normalizedValue))
		{
			UISystemProfilerApi.AddMarker("Slider.value", this);
			OnValueChangedCallback.Invoke(Value);
			OnValueChanged.Invoke(Value);
		}
	}

	private void UpdateCachedReferences()
	{
		if ((bool)FillRect && FillRect != (RectTransform)base.transform)
		{
			m_FillTransform = FillRect.transform;
			m_FillImage = FillRect.GetComponent<Image>();
			if (m_FillTransform.parent != null)
			{
				m_FillContainerRect = m_FillTransform.parent.GetComponent<RectTransform>();
			}
		}
		else
		{
			FillRect = null;
			m_FillContainerRect = null;
			m_FillImage = null;
		}
		if ((bool)HandleRect && HandleRect != (RectTransform)base.transform)
		{
			m_HandleTransform = HandleRect.transform;
			if (m_HandleTransform.parent != null)
			{
				m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
			}
		}
		else
		{
			HandleRect = null;
			m_HandleContainerRect = null;
		}
	}

	public void ResetValue()
	{
		SetValue(defaultValue);
		OnValueReset.Execute();
	}

	private float ClampValue(float input)
	{
		if (!wholeNumbers)
		{
			return input.Clamp(minValue, maxValue);
		}
		return input.Clamp(minValue, maxValue).Round(0);
	}

	public virtual void SetValueWithoutNotify(float input)
	{
		SetValue(input, sendCallback: false);
	}

	public void SetValue(float newValue, bool sendCallback = true)
	{
		bool num = Math.Abs(Value - newValue) > 0.0001f;
		float num2 = Value;
		Value = Mathf.Clamp(newValue, minValue, maxValue);
		if (wholeNumbers)
		{
			Value = Value.Round(0);
		}
		UpdateLabel(valueLabel, Value);
		UpdateVisuals();
		if (num)
		{
			if (sendCallback)
			{
				UISystemProfilerApi.AddMarker("UISlider.value", this);
				OnValueChangedCallback.Invoke(Value);
				OnValueChanged?.Invoke(Value);
				stream.SendSignal(Value);
				if (num2 < Value)
				{
					OnValueIncremented?.Invoke(Value - num2);
					stream.SendSignal(new UISliderSignalData(Id.Category, Id.Name, SliderState.ValueIncremented, this));
				}
				else if (num2 > Value)
				{
					OnValueDecremented?.Invoke(num2 - Value);
					stream.SendSignal(new UISliderSignalData(Id.Category, Id.Name, SliderState.ValueDecremented, this));
				}
			}
			if (InstantProgressorUpdate)
			{
				UpdateTargetProgressorValue();
			}
			else
			{
				PlayTargetProgressorValue();
			}
		}
		if (sendCallback)
		{
			if (Value <= minValue)
			{
				OnValueReachedMin.Execute();
			}
			if (Value >= maxValue)
			{
				OnValueReachedMax.Execute();
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
		if (m_FillContainerRect != null)
		{
			m_Tracker.Add(this, FillRect, DrivenTransformProperties.Anchors);
			Vector2 zero = Vector2.zero;
			Vector2 one = Vector2.one;
			if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
			{
				m_FillImage.fillAmount = normalizedValue;
			}
			else if (reverseValue)
			{
				zero[(int)axis] = 1f - normalizedValue;
			}
			else
			{
				one[(int)axis] = normalizedValue;
			}
			FillRect.anchorMin = zero;
			FillRect.anchorMax = one;
		}
		if (!(m_HandleContainerRect == null))
		{
			m_Tracker.Add(this, HandleRect, DrivenTransformProperties.Anchors);
			Vector2 zero2 = Vector2.zero;
			Vector2 one2 = Vector2.one;
			Axis index = axis;
			float num = (one2[(int)axis] = (reverseValue ? (1f - normalizedValue) : normalizedValue));
			zero2[(int)index] = num;
			HandleRect.anchorMin = zero2;
			HandleRect.anchorMax = one2;
		}
	}

	private void UpdateDrag(PointerEventData eventData, Camera cam)
	{
		RectTransform rectTransform = (m_HandleContainerRect ? m_HandleContainerRect : m_FillContainerRect);
		if (!(rectTransform == null) && rectTransform.rect.size[(int)axis] > 0f)
		{
			Vector2 position = Vector2.zero;
			if (MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position) && RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, position, cam, out var localPoint))
			{
				Rect rect = rectTransform.rect;
				localPoint -= rect.position;
				float num = Mathf.Clamp01((localPoint - m_Offset)[(int)axis] / rect.size[(int)axis]);
				normalizedValue = (reverseValue ? (1f - num) : num);
			}
		}
	}

	private bool AllowDrag(PointerEventData eventData)
	{
		if (IsActive() && IsInteractable())
		{
			return eventData.button == PointerEventData.InputButton.Left;
		}
		return false;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (!AllowDrag(eventData))
		{
			return;
		}
		base.OnPointerDown(eventData);
		m_Offset = Vector2.zero;
		if (m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
		{
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out var localPoint))
			{
				m_Offset = localPoint;
			}
		}
		else
		{
			UpdateDrag(eventData, eventData.pressEventCamera);
		}
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		if (AllowDrag(eventData))
		{
			UpdateDrag(eventData, eventData.pressEventCamera);
		}
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
		case UnityEngine.EventSystems.MoveDirection.Left:
			if (axis == Axis.Horizontal && FindSelectableOnLeft() == null)
			{
				SetValue(reverseValue ? (value + stepSize) : (value - stepSize));
			}
			else
			{
				base.OnMove(eventData);
			}
			break;
		case UnityEngine.EventSystems.MoveDirection.Right:
			if (axis == Axis.Horizontal && FindSelectableOnRight() == null)
			{
				SetValue(reverseValue ? (value - stepSize) : (value + stepSize));
			}
			else
			{
				base.OnMove(eventData);
			}
			break;
		case UnityEngine.EventSystems.MoveDirection.Up:
			if (axis == Axis.Vertical && FindSelectableOnUp() == null)
			{
				SetValue(reverseValue ? (value - stepSize) : (value + stepSize));
			}
			else
			{
				base.OnMove(eventData);
			}
			break;
		case UnityEngine.EventSystems.MoveDirection.Down:
			if (axis == Axis.Vertical && FindSelectableOnDown() == null)
			{
				SetValue(reverseValue ? (value + stepSize) : (value - stepSize));
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

	public void SetDirection(SlideDirection previousDirection, SlideDirection newDirection, bool includeRectLayouts)
	{
		bool flag = reverseValue;
		Axis axis = GetAxis(previousDirection);
		direction = newDirection;
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

	private void UpdateTargetProgressorMinMax()
	{
		if ((bool)targetProgressor)
		{
			targetProgressor.fromValue = minValue;
			targetProgressor.toValue = maxValue;
			targetProgressor.SetValueAt(value);
		}
	}

	private void UpdateTargetProgressorValue()
	{
		if ((bool)targetProgressor)
		{
			targetProgressor.SetValueAt(value);
		}
	}

	private void PlayTargetProgressorValue()
	{
		if ((bool)targetProgressor)
		{
			targetProgressor.PlayToValue(value);
		}
	}

	public T SetValueLabel<T>(TMP_Text label) where T : UISlider
	{
		valueLabel = label;
		return (T)this;
	}

	public T SetMinValueLabel<T>(TMP_Text label) where T : UISlider
	{
		minValueLabel = label;
		return (T)this;
	}

	public T SetMaxValueLabel<T>(TMP_Text label) where T : UISlider
	{
		maxValueLabel = label;
		return (T)this;
	}

	public T SetValue<T>(float newValue) where T : UISlider
	{
		value = newValue;
		return (T)this;
	}

	public T SetMinValue<T>(float newMinValue) where T : UISlider
	{
		minValue = newMinValue;
		return (T)this;
	}

	public T SetMaxValue<T>(float newMaxValue) where T : UISlider
	{
		maxValue = newMaxValue;
		return (T)this;
	}

	public T SetDefaultValue<T>(float newResetValue) where T : UISlider
	{
		defaultValue = newResetValue;
		return (T)this;
	}

	public T SetTargetProgressor<T>(Progressor progressor) where T : UISlider
	{
		targetProgressor = progressor;
		return (T)this;
	}

	public static IEnumerable<UISlider> GetSliders(string category, string name)
	{
		return from slider in database
			where slider.Id.Category.Equals(category)
			where slider.Id.Name.Equals(name)
			select slider;
	}

	public static IEnumerable<UISlider> GetAllSlidersInCategory(string category)
	{
		return database.Where((UISlider slider) => slider.Id.Category.Equals(category));
	}

	public static IEnumerable<UISlider> GetAvailableSliders()
	{
		return database.Where((UISlider slider) => slider.isActiveAndEnabled);
	}

	public static UISlider GetSelectedSlider()
	{
		return database.FirstOrDefault((UISlider slider) => slider.isSelected);
	}

	public static bool SelectSlider(string category, string name)
	{
		UISlider uISlider = availableSliders.FirstOrDefault((UISlider b) => b.Id.Category.Equals(category) & b.Id.Name.Equals(name));
		if (uISlider == null)
		{
			return false;
		}
		uISlider.Select();
		return true;
	}

	private static void UpdateLabel(TMP_Text targetLabel, float displayValue)
	{
		if (!(targetLabel == null))
		{
			targetLabel.text = displayValue.ToString(CultureInfo.InvariantCulture);
		}
	}
}
