using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Components;

[AddComponentMenu("UI/Components/UI Stepper")]
public class UIStepper : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
{
	public enum Direction
	{
		Horizontal = 0,
		Vertical = 1
	}

	private const float TOLERANCE = 0.0001f;

	private const float DRAG_DISTANCE = 20f;

	private const float WAIT_BEFORE_STARTING = 0.6f;

	private const float WAIT_TIME = 0.4f;

	private const float WAIT_TIME_MIN = 0.04f;

	private const float WAIT_TIME_REDUCTION = 0.4f;

	[ClearOnReload]
	private static SignalStream s_stream;

	public UIStepperId Id;

	public float AutoRepeatWaitTime = 0.4f;

	public float AutoRepeatWaitTimeReduction = 0.4f;

	public float AutoRepeatMinWaitTime = 0.04f;

	[SerializeField]
	private UIButton MinusButton;

	[SerializeField]
	private UIButton PlusButton;

	[SerializeField]
	private UIButton ResetButton;

	[SerializeField]
	private TMP_Text TargetLabel;

	[SerializeField]
	private float MinValue;

	[SerializeField]
	private float MaxValue = 1f;

	[SerializeField]
	private float Value;

	[SerializeField]
	private float DefaultValue;

	[SerializeField]
	private float Step = 0.1f;

	[SerializeField]
	private UISlider TargetSlider;

	[SerializeField]
	private Progressor TargetProgressor;

	public bool InstantProgressorUpdate = true;

	public bool ResetValueOnEnable = true;

	public int ValuePrecision = 2;

	public bool DragEnabled;

	[SerializeField]
	private RectTransform DragHandle;

	[SerializeField]
	private Direction DragDirection;

	public float MaxDragDistance = 20f;

	public FloatEvent OnValueChanged = new FloatEvent();

	public FloatEvent OnValueIncremented = new FloatEvent();

	public FloatEvent OnValueDecremented = new FloatEvent();

	public ModyEvent OnValueReset = new ModyEvent();

	public ModyEvent OnValueReachedMin = new ModyEvent();

	public ModyEvent OnValueReachedMax = new ModyEvent();

	private Vector2 m_DragHandleInitialPosition;

	public static HashSet<UIStepper> database { get; private set; } = new HashSet<UIStepper>();

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream("UISelectable", "UIStepper"));

	public static IEnumerable<UIStepper> availableSteppers => database.Where((UIStepper item) => item.isActiveAndEnabled);

	public UIButton minusButton
	{
		get
		{
			return MinusButton;
		}
		private set
		{
			if (MinusButton != null)
			{
				MinusButton.pressedState.stateEvent.Event.RemoveListener(OnMinusButtonClicked);
				MinusButton.onPointerDownEvent.RemoveListener(OnMinusButtonDown);
				MinusButton.onPointerUpEvent.RemoveListener(OnMinusButtonUp);
			}
			if (value != null)
			{
				value.pressedState.stateEvent.Event.AddListener(OnMinusButtonClicked);
				value.onPointerDownEvent.AddListener(OnMinusButtonDown);
				value.onPointerUpEvent.AddListener(OnMinusButtonUp);
			}
			MinusButton = value;
		}
	}

	public UIButton plusButton
	{
		get
		{
			return PlusButton;
		}
		private set
		{
			if (PlusButton != null)
			{
				PlusButton.pressedState.stateEvent.Event.RemoveListener(OnPlusButtonClicked);
				PlusButton.onPointerDownEvent.RemoveListener(OnPlusButtonDown);
				PlusButton.onPointerUpEvent.RemoveListener(OnPlusButtonUp);
			}
			if (value != null)
			{
				value.pressedState.stateEvent.Event.AddListener(OnPlusButtonClicked);
				value.onPointerDownEvent.AddListener(OnPlusButtonDown);
				value.onPointerUpEvent.AddListener(OnPlusButtonUp);
			}
			PlusButton = value;
		}
	}

	public UIButton resetButton
	{
		get
		{
			return ResetButton;
		}
		private set
		{
			if (ResetButton != null)
			{
				ResetButton.pressedState.stateEvent.Event.RemoveListener(OnResetButtonClicked);
			}
			if (value != null)
			{
				value.pressedState.stateEvent.Event.AddListener(OnResetButtonClicked);
			}
			ResetButton = value;
		}
	}

	public TMP_Text targetLabel
	{
		get
		{
			return TargetLabel;
		}
		private set
		{
			TargetLabel = value;
			UpdateValueLabel();
		}
	}

	public float minValue
	{
		get
		{
			return MinValue;
		}
		private set
		{
			MinValue = value.Round(ValuePrecision);
			MaxValue = ((MinValue > MaxValue) ? MinValue : MaxValue);
			if (Value < MinValue)
			{
				SetValue(MinValue);
			}
			UpdateTargetProgressorMinMax();
			UpdateTargetProgressorValue();
			UpdateTargetProgressorMinMax();
			UpdateTargetSliderValue(Value);
		}
	}

	public float maxValue
	{
		get
		{
			return MaxValue;
		}
		private set
		{
			MaxValue = value.Round(ValuePrecision);
			MinValue = ((MaxValue < MinValue) ? MaxValue : MinValue);
			if (Value > MaxValue)
			{
				SetValue(MaxValue);
			}
			UpdateTargetProgressorMinMax();
			UpdateTargetProgressorValue();
			UpdateTargetProgressorMinMax();
			UpdateTargetSliderValue(Value);
		}
	}

	public float value
	{
		get
		{
			return Value;
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

	public float step
	{
		get
		{
			return Step;
		}
		private set
		{
			Step = value;
			SetValue(NearestStep(value));
			UpdateTargetSliderValue(value);
			stepValueChanged = true;
		}
	}

	public UISlider targetSlider
	{
		get
		{
			return TargetSlider;
		}
		private set
		{
			if (TargetSlider != null)
			{
				TargetSlider.OnValueChanged.RemoveListener(OnTargetSliderValueChanged);
				OnValueChanged.RemoveListener(UpdateTargetSliderValue);
			}
			if (value != null)
			{
				value.OnValueChanged.AddListener(OnTargetSliderValueChanged);
				OnValueChanged.AddListener(UpdateTargetSliderValue);
			}
			TargetSlider = value;
			UpdateTargetSliderMinMax();
			UpdateTargetSliderValue(Value);
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

	public RectTransform dragHandle
	{
		get
		{
			return DragHandle;
		}
		set
		{
			DragHandle = value;
			if ((bool)DragHandle)
			{
				m_DragHandleInitialPosition = DragHandle.anchoredPosition;
			}
		}
	}

	public Direction dragDirection
	{
		get
		{
			return DragDirection;
		}
		private set
		{
			DragDirection = value;
		}
	}

	private Coroutine autoIncrementCoroutine { get; set; }

	private Coroutine autoDecrementCoroutine { get; set; }

	private bool autoIncrementing { get; set; }

	private bool autoDecrementing { get; set; }

	private float draggedDistance => DragDirection switch
	{
		Direction.Horizontal => dragHandle.anchoredPosition.x - m_DragHandleInitialPosition.x, 
		Direction.Vertical => dragHandle.anchoredPosition.y - m_DragHandleInitialPosition.y, 
		_ => throw new ArgumentOutOfRangeException(), 
	};

	private float dragWaitTime
	{
		get
		{
			float num = Mathf.Abs(draggedDistance);
			num = Mathf.Clamp(num, 0f, MaxDragDistance);
			return Mathf.Clamp((1f - num / MaxDragDistance) * AutoRepeatWaitTime, AutoRepeatMinWaitTime, AutoRepeatWaitTime);
		}
	}

	private Coroutine dragIncrementCoroutine { get; set; }

	private Coroutine dragDecrementCoroutine { get; set; }

	private bool dragIncrementing { get; set; }

	private bool dragDecrementing { get; set; }

	private bool canDrag { get; set; }

	private bool isDragging { get; set; }

	private bool stepValueChanged { get; set; }

	private bool cannotDrag
	{
		get
		{
			if (DragEnabled)
			{
				return !canDrag;
			}
			return true;
		}
	}

	[ExecuteOnReload]
	private static void OnReload()
	{
		if (database == null)
		{
			database = new HashSet<UIStepper>();
		}
	}

	protected UIStepper()
	{
		Id = new UIStepperId();
	}

	protected virtual void OnValidate()
	{
		SetValue(value);
	}

	protected virtual void Awake()
	{
		database.Add(this);
	}

	protected virtual void Start()
	{
		UpdateValueLabel();
	}

	protected virtual void OnEnable()
	{
		database.Remove(null);
		if (Application.isPlaying)
		{
			if (minusButton != null)
			{
				minusButton.pressedState.stateEvent.Event.AddListener(OnMinusButtonClicked);
				minusButton.onPointerUpEvent.AddListener(OnMinusButtonUp);
				minusButton.onPointerDownEvent.AddListener(OnMinusButtonDown);
			}
			if (plusButton != null)
			{
				plusButton.pressedState.stateEvent.Event.AddListener(OnPlusButtonClicked);
				plusButton.onPointerUpEvent.AddListener(OnPlusButtonUp);
				plusButton.onPointerDownEvent.AddListener(OnPlusButtonDown);
			}
			if (resetButton != null)
			{
				resetButton.pressedState.stateEvent.Event.AddListener(OnResetButtonClicked);
			}
			if ((bool)TargetSlider)
			{
				TargetSlider.OnValueChanged.AddListener(OnTargetSliderValueChanged);
				OnValueChanged.AddListener(UpdateTargetSliderValue);
			}
			UpdateTargetProgressorMinMax();
			UpdateTargetSliderMinMax();
			if (ResetValueOnEnable)
			{
				ResetValue();
			}
			else
			{
				UpdateTargetProgressorValue();
				UpdateTargetSliderValue(value);
			}
			if ((bool)dragHandle)
			{
				m_DragHandleInitialPosition = dragHandle.anchoredPosition;
			}
			canDrag = true;
			isDragging = false;
		}
	}

	protected virtual void OnDisable()
	{
		database.Remove(null);
		if (Application.isPlaying)
		{
			if (plusButton != null)
			{
				plusButton.pressedState.stateEvent.Event.RemoveListener(OnPlusButtonClicked);
				plusButton.onPointerUpEvent.RemoveListener(OnPlusButtonUp);
				plusButton.onPointerDownEvent.RemoveListener(OnPlusButtonDown);
			}
			if (minusButton != null)
			{
				minusButton.pressedState.stateEvent.Event.RemoveListener(OnMinusButtonClicked);
				minusButton.onPointerUpEvent.RemoveListener(OnMinusButtonUp);
				minusButton.onPointerDownEvent.RemoveListener(OnMinusButtonDown);
			}
			if (resetButton != null)
			{
				resetButton.pressedState.stateEvent.Event.RemoveListener(OnResetButtonClicked);
			}
			autoIncrementing = false;
			if (autoIncrementCoroutine != null)
			{
				StopCoroutine(autoIncrementCoroutine);
			}
			autoDecrementing = false;
			if (autoDecrementCoroutine != null)
			{
				StopCoroutine(autoDecrementCoroutine);
			}
			dragIncrementing = false;
			if (dragIncrementCoroutine != null)
			{
				StopCoroutine(dragIncrementCoroutine);
			}
			dragDecrementing = false;
			if (dragDecrementCoroutine != null)
			{
				StopCoroutine(dragDecrementCoroutine);
			}
			UpdateTargetProgressorValue();
			UpdateTargetSliderValue(value);
			if ((bool)TargetSlider)
			{
				TargetSlider.OnValueChanged.RemoveListener(OnTargetSliderValueChanged);
				OnValueChanged.RemoveListener(UpdateTargetSliderValue);
			}
		}
	}

	protected virtual void OnDestroy()
	{
		database.Remove(null);
		database.Remove(this);
	}

	protected virtual void OnMinusButtonClicked()
	{
		DecrementValue();
		canDrag = true;
	}

	protected virtual void OnPlusButtonClicked()
	{
		IncrementValue();
		canDrag = true;
	}

	protected virtual void OnPlusButtonDown()
	{
		StopAutoIncrement();
		if (!isDragging)
		{
			canDrag = false;
			StartAutoIncrement();
		}
	}

	protected virtual void OnPlusButtonUp()
	{
		StopAutoIncrement();
		canDrag = true;
	}

	protected virtual void OnMinusButtonDown()
	{
		StopAutoDecrement();
		if (!isDragging)
		{
			canDrag = false;
			StartAutoDecrement();
		}
	}

	protected virtual void OnMinusButtonUp()
	{
		StopAutoDecrement();
		canDrag = true;
	}

	protected virtual void OnResetButtonClicked()
	{
		ResetValue();
	}

	protected virtual void OnTargetSliderValueChanged(float sliderValue)
	{
		float valueWithoutNotify = NearestStep(sliderValue);
		TargetSlider.SetValueWithoutNotify(valueWithoutNotify);
		SetValue(valueWithoutNotify);
	}

	public void ResetValue()
	{
		SetValue(defaultValue);
		OnValueReset.Execute();
	}

	public void SetValue(float newValue)
	{
		bool num = Math.Abs(Value - newValue) > 0.0001f;
		Value = Mathf.Clamp(newValue, minValue, maxValue).Round(ValuePrecision);
		if (stepValueChanged)
		{
			Value = NearestStep(Value);
			stepValueChanged = false;
		}
		UpdateValueLabel();
		if (num)
		{
			OnValueChanged.Invoke(Value);
			if (InstantProgressorUpdate)
			{
				UpdateTargetProgressorValue();
			}
			else
			{
				PlayTargetProgressorValue();
			}
			UpdateTargetSliderValue(Value);
			stream.SendSignal(new UIStepperSignalData(Id.Category, Id.Name, StepperState.ValueChanged, this));
		}
		if (Value <= minValue)
		{
			OnValueReachedMin.Execute();
			stream.SendSignal(new UIStepperSignalData(Id.Category, Id.Name, StepperState.ReachedMinValue, this));
			if (minusButton != null && minusButton.interactable)
			{
				minusButton.interactable = false;
			}
		}
		else if (minusButton != null && !minusButton.interactable)
		{
			minusButton.interactable = true;
		}
		if (Value >= maxValue)
		{
			OnValueReachedMax.Execute();
			stream.SendSignal(new UIStepperSignalData(Id.Category, Id.Name, StepperState.ReachedMaxValue, this));
			if (plusButton != null && plusButton.interactable)
			{
				plusButton.interactable = false;
			}
		}
		else if (plusButton != null && !plusButton.interactable)
		{
			plusButton.interactable = true;
		}
	}

	public void IncrementValue()
	{
		IncrementValue(step);
	}

	public void IncrementValue(float increment)
	{
		if (!(Math.Abs(value - maxValue) < 0.0001f))
		{
			OnValueIncremented.Invoke(increment);
		}
		SetValue(value + increment);
	}

	public void DecrementValue()
	{
		DecrementValue(step);
	}

	public void DecrementValue(float decrement)
	{
		if (!(Math.Abs(value - minValue) < 0.0001f))
		{
			OnValueDecremented.Invoke(0f - decrement);
		}
		SetValue(value - decrement);
	}

	public void UpdateValueLabel()
	{
		if (!(targetLabel == null))
		{
			targetLabel.text = value.ToString(CultureInfo.InvariantCulture);
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

	private void UpdateTargetSliderMinMax()
	{
		if (!(targetSlider == null))
		{
			targetSlider.minValue = minValue;
			targetSlider.maxValue = maxValue;
		}
	}

	private void UpdateTargetSliderValue(float newValue)
	{
		if (!(targetSlider == null))
		{
			targetSlider.SetValueWithoutNotify(newValue);
		}
	}

	public T SetMinusButton<T>(UIButton button) where T : UIStepper
	{
		minusButton = button;
		return (T)this;
	}

	public T SetPlusButton<T>(UIButton button) where T : UIStepper
	{
		plusButton = button;
		return (T)this;
	}

	public T SetValueLabel<T>(TMP_Text label) where T : UIStepper
	{
		targetLabel = label;
		return (T)this;
	}

	public T SetResetButton<T>(UIButton button) where T : UIStepper
	{
		resetButton = button;
		return (T)this;
	}

	public T SetMinValue<T>(float newMinValue) where T : UIStepper
	{
		minValue = newMinValue;
		return (T)this;
	}

	public T SetMaxValue<T>(float newMaxValue) where T : UIStepper
	{
		maxValue = newMaxValue;
		return (T)this;
	}

	public T SetDefaultValue<T>(float newResetValue) where T : UIStepper
	{
		defaultValue = newResetValue;
		return (T)this;
	}

	public T SetTargetProgressor<T>(Progressor progressor) where T : UIStepper
	{
		targetProgressor = progressor;
		return (T)this;
	}

	public T SetStepperDirection<T>(Direction direction) where T : UIStepper
	{
		DragDirection = direction;
		return (T)this;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!cannotDrag)
		{
			isDragging = true;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!cannotDrag)
		{
			isDragging = false;
			dragHandle.anchoredPosition = m_DragHandleInitialPosition;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!cannotDrag && isDragging)
		{
			float x = m_DragHandleInitialPosition.x;
			float y = m_DragHandleInitialPosition.y;
			switch (DragDirection)
			{
			case Direction.Horizontal:
			{
				float num2 = dragHandle.anchoredPosition.x + eventData.delta.x;
				num2 = Mathf.Clamp(num2, x - MaxDragDistance, x + MaxDragDistance);
				dragHandle.anchoredPosition = new Vector2(num2, m_DragHandleInitialPosition.y);
				break;
			}
			case Direction.Vertical:
			{
				float num = dragHandle.anchoredPosition.y + eventData.delta.y;
				num = Mathf.Clamp(num, y - MaxDragDistance, y + MaxDragDistance);
				dragHandle.anchoredPosition = new Vector2(m_DragHandleInitialPosition.x, num);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void LateUpdate()
	{
		if (!isDragging)
		{
			if (dragIncrementing)
			{
				StopDragIncrement();
			}
			if (dragDecrementing)
			{
				StopDragDecrement();
			}
			return;
		}
		if (autoIncrementing)
		{
			StopAutoIncrement();
		}
		if (autoDecrementing)
		{
			StopAutoDecrement();
		}
		if (draggedDistance > 0f)
		{
			if (dragDecrementing)
			{
				StopDragDecrement();
			}
			if (!dragIncrementing)
			{
				StartDragIncrement();
			}
		}
		else
		{
			if (dragIncrementing)
			{
				StopDragIncrement();
			}
			if (!dragDecrementing)
			{
				StartDragDecrement();
			}
		}
	}

	private bool CanIncrementValue()
	{
		return value < maxValue;
	}

	private void StartAutoIncrement()
	{
		StopAutoIncrement();
		autoIncrementCoroutine = StartCoroutine(AutoIncrementValue());
	}

	private void StopAutoIncrement()
	{
		autoIncrementing = false;
		if (autoIncrementCoroutine != null)
		{
			StopCoroutine(autoIncrementCoroutine);
		}
	}

	private IEnumerator AutoIncrementValue()
	{
		autoIncrementing = true;
		yield return new WaitForSecondsRealtime(0.6f);
		float waitTime = AutoRepeatWaitTime;
		while (CanIncrementValue())
		{
			IncrementValue();
			yield return new WaitForSecondsRealtime(waitTime);
			waitTime = Mathf.Clamp(waitTime * AutoRepeatWaitTimeReduction, AutoRepeatMinWaitTime, AutoRepeatWaitTime);
		}
		autoIncrementing = false;
		autoIncrementCoroutine = null;
	}

	private void StartDragIncrement()
	{
		StopDragIncrement();
		dragIncrementCoroutine = StartCoroutine(DragIncrementValue());
	}

	private void StopDragIncrement()
	{
		dragIncrementing = false;
		if (dragIncrementCoroutine != null)
		{
			StopCoroutine(dragIncrementCoroutine);
		}
	}

	private IEnumerator DragIncrementValue()
	{
		dragIncrementing = true;
		while (CanIncrementValue())
		{
			IncrementValue();
			yield return new WaitForSecondsRealtime(dragWaitTime);
		}
		dragIncrementing = false;
		dragIncrementCoroutine = null;
	}

	private bool CanDecrementValue()
	{
		return value > minValue;
	}

	private void StartAutoDecrement()
	{
		StopAutoDecrement();
		autoDecrementCoroutine = StartCoroutine(AutoDecrementValue());
	}

	private void StopAutoDecrement()
	{
		autoDecrementing = false;
		if (autoDecrementCoroutine != null)
		{
			StopCoroutine(autoDecrementCoroutine);
		}
	}

	private IEnumerator AutoDecrementValue()
	{
		autoDecrementing = true;
		yield return new WaitForSecondsRealtime(0.6f);
		float waitTime = AutoRepeatWaitTime;
		while (CanDecrementValue())
		{
			DecrementValue();
			yield return new WaitForSecondsRealtime(waitTime);
			waitTime = Mathf.Clamp(waitTime * AutoRepeatWaitTimeReduction, AutoRepeatMinWaitTime, AutoRepeatWaitTime);
		}
		autoDecrementing = false;
		autoDecrementCoroutine = null;
	}

	private void StartDragDecrement()
	{
		StopDragDecrement();
		dragDecrementCoroutine = StartCoroutine(DragDecrementValue());
	}

	private void StopDragDecrement()
	{
		dragDecrementing = false;
		if (dragDecrementCoroutine != null)
		{
			StopCoroutine(dragDecrementCoroutine);
		}
	}

	private IEnumerator DragDecrementValue()
	{
		dragDecrementing = true;
		while (CanDecrementValue())
		{
			DecrementValue();
			yield return new WaitForSecondsRealtime(dragWaitTime);
		}
		dragDecrementing = false;
		dragDecrementCoroutine = null;
	}

	private float NearestStep(float uncorrectedValue)
	{
		if (Step == 0f)
		{
			return uncorrectedValue;
		}
		return (float)(int)Math.Round((double)uncorrectedValue / (double)step, MidpointRounding.AwayFromZero) * step;
	}
}
