using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MoreMountains.Tools;

[RequireComponent(typeof(Rect))]
[RequireComponent(typeof(CanvasGroup))]
[AddComponentMenu("More Mountains/Tools/Controls/MMTouchButton")]
public class MMTouchButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler, ISubmitHandler
{
	public enum ButtonStates
	{
		Off = 0,
		ButtonDown = 1,
		ButtonPressed = 2,
		ButtonUp = 3,
		Disabled = 4
	}

	[Header("Interaction")]
	public bool Interactable = true;

	[Header("Binding")]
	public UnityEvent ButtonPressedFirstTime;

	public UnityEvent ButtonReleased;

	public UnityEvent ButtonPressed;

	[Header("Sprite Swap")]
	[MMInformation("Here you can define, for disabled and pressed states, if you want a different sprite, and a different color.", MMInformationAttribute.InformationType.Info, false)]
	public Sprite DisabledSprite;

	public bool DisabledChangeColor;

	public Color DisabledColor = Color.white;

	public Sprite PressedSprite;

	public bool PressedChangeColor;

	public Color PressedColor = Color.white;

	public Sprite HighlightedSprite;

	public bool HighlightedChangeColor;

	public Color HighlightedColor = Color.white;

	[Header("Opacity")]
	[MMInformation("Here you can set different opacities for the button when it's pressed, idle, or disabled. Useful for visual feedback.", MMInformationAttribute.InformationType.Info, false)]
	public float PressedOpacity = 1f;

	public float IdleOpacity = 1f;

	public float DisabledOpacity = 1f;

	[Header("Delays")]
	[MMInformation("Specify here the delays to apply when the button is pressed initially, and when it gets released. Usually you'll keep them at 0.", MMInformationAttribute.InformationType.Info, false)]
	public float PressedFirstTimeDelay;

	public float ReleasedDelay;

	[Header("Buffer")]
	public float BufferDuration;

	[Header("Animation")]
	[MMInformation("Here you can bind an animator, and specify animation parameter names for the various states.", MMInformationAttribute.InformationType.Info, false)]
	public Animator Animator;

	public string IdleAnimationParameterName = "Idle";

	public string DisabledAnimationParameterName = "Disabled";

	public string PressedAnimationParameterName = "Pressed";

	[Header("Mouse Mode")]
	[MMInformation("If you set this to true, you'll need to actually press the button for it to be triggered, otherwise a simple hover will trigger it (better to leave it unchecked if you're going for touch input).", MMInformationAttribute.InformationType.Info, false)]
	public bool MouseMode;

	protected bool _zonePressed;

	protected CanvasGroup _canvasGroup;

	protected float _initialOpacity;

	protected Animator _animator;

	protected Image _image;

	protected Sprite _initialSprite;

	protected Color _initialColor;

	protected float _lastClickTimestamp;

	protected Selectable _selectable;

	public bool ReturnToInitialSpriteAutomatically { get; set; }

	public ButtonStates CurrentState { get; protected set; }

	public event Action<PointerEventData.FramePressState, PointerEventData> ButtonStateChange;

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		ReturnToInitialSpriteAutomatically = true;
		_selectable = GetComponent<Selectable>();
		_image = GetComponent<Image>();
		if (_image != null)
		{
			_initialColor = _image.color;
			_initialSprite = _image.sprite;
		}
		_animator = GetComponent<Animator>();
		if (Animator != null)
		{
			_animator = Animator;
		}
		_canvasGroup = GetComponent<CanvasGroup>();
		if (_canvasGroup != null)
		{
			_initialOpacity = IdleOpacity;
			_canvasGroup.alpha = _initialOpacity;
			_initialOpacity = _canvasGroup.alpha;
		}
		ResetButton();
	}

	protected virtual void Update()
	{
		switch (CurrentState)
		{
		case ButtonStates.Off:
			SetOpacity(IdleOpacity);
			if (_image != null && ReturnToInitialSpriteAutomatically)
			{
				_image.color = _initialColor;
				_image.sprite = _initialSprite;
			}
			if (!(_selectable != null))
			{
				break;
			}
			_selectable.interactable = true;
			if (EventSystem.current.currentSelectedGameObject == base.gameObject)
			{
				if (_image != null && HighlightedChangeColor)
				{
					_image.color = HighlightedColor;
				}
				if (HighlightedSprite != null)
				{
					_image.sprite = HighlightedSprite;
				}
			}
			break;
		case ButtonStates.Disabled:
			SetOpacity(DisabledOpacity);
			if (_image != null)
			{
				if (DisabledSprite != null)
				{
					_image.sprite = DisabledSprite;
				}
				if (DisabledChangeColor)
				{
					_image.color = DisabledColor;
				}
			}
			if (_selectable != null)
			{
				_selectable.interactable = false;
			}
			break;
		case ButtonStates.ButtonPressed:
			SetOpacity(PressedOpacity);
			OnPointerPressed();
			if (_image != null)
			{
				if (PressedSprite != null)
				{
					_image.sprite = PressedSprite;
				}
				if (PressedChangeColor)
				{
					_image.color = PressedColor;
				}
			}
			break;
		}
		UpdateAnimatorStates();
	}

	protected virtual void LateUpdate()
	{
		if (CurrentState == ButtonStates.ButtonUp)
		{
			CurrentState = ButtonStates.Off;
		}
		if (CurrentState == ButtonStates.ButtonDown)
		{
			CurrentState = ButtonStates.ButtonPressed;
		}
	}

	public virtual void OnPointerDown(PointerEventData data)
	{
		if (Interactable && !(Time.time - _lastClickTimestamp < BufferDuration) && CurrentState == ButtonStates.Off)
		{
			CurrentState = ButtonStates.ButtonDown;
			_lastClickTimestamp = Time.time;
			ButtonStateChange?.Invoke(PointerEventData.FramePressState.Pressed, data);
			if (Time.timeScale != 0f && PressedFirstTimeDelay > 0f)
			{
				Invoke("InvokePressedFirstTime", PressedFirstTimeDelay);
			}
			else
			{
				ButtonPressedFirstTime.Invoke();
			}
		}
	}

	protected virtual void InvokePressedFirstTime()
	{
		if (ButtonPressedFirstTime != null)
		{
			ButtonPressedFirstTime.Invoke();
		}
	}

	public virtual void OnPointerUp(PointerEventData data)
	{
		if (Interactable && (CurrentState == ButtonStates.ButtonPressed || CurrentState == ButtonStates.ButtonDown))
		{
			CurrentState = ButtonStates.ButtonUp;
			ButtonStateChange?.Invoke(PointerEventData.FramePressState.Released, data);
			if (Time.timeScale != 0f && ReleasedDelay > 0f)
			{
				Invoke("InvokeReleased", ReleasedDelay);
			}
			else
			{
				ButtonReleased.Invoke();
			}
		}
	}

	protected virtual void InvokeReleased()
	{
		if (ButtonReleased != null)
		{
			ButtonReleased.Invoke();
		}
	}

	public virtual void OnPointerPressed()
	{
		if (Interactable)
		{
			CurrentState = ButtonStates.ButtonPressed;
			if (ButtonPressed != null)
			{
				ButtonPressed.Invoke();
			}
		}
	}

	protected virtual void ResetButton()
	{
		SetOpacity(_initialOpacity);
		CurrentState = ButtonStates.Off;
	}

	public virtual void OnPointerEnter(PointerEventData data)
	{
		if (Interactable && !MouseMode)
		{
			OnPointerDown(data);
		}
	}

	public virtual void OnPointerExit(PointerEventData data)
	{
		if (Interactable && !MouseMode)
		{
			OnPointerUp(data);
		}
	}

	protected virtual void OnEnable()
	{
		ResetButton();
	}

	private void OnDisable()
	{
		bool num = CurrentState != ButtonStates.Off && CurrentState != ButtonStates.Disabled;
		DisableButton();
		CurrentState = ButtonStates.Off;
		if (num)
		{
			ButtonStateChange?.Invoke(PointerEventData.FramePressState.Released, null);
			ButtonReleased?.Invoke();
		}
	}

	public virtual void DisableButton()
	{
		CurrentState = ButtonStates.Disabled;
	}

	public virtual void EnableButton()
	{
		if (CurrentState == ButtonStates.Disabled)
		{
			CurrentState = ButtonStates.Off;
		}
	}

	protected virtual void SetOpacity(float newOpacity)
	{
		if (_canvasGroup != null)
		{
			_canvasGroup.alpha = newOpacity;
		}
	}

	protected virtual void UpdateAnimatorStates()
	{
		if (!(_animator == null))
		{
			if (DisabledAnimationParameterName != null)
			{
				_animator.SetBool(DisabledAnimationParameterName, CurrentState == ButtonStates.Disabled);
			}
			if (PressedAnimationParameterName != null)
			{
				_animator.SetBool(PressedAnimationParameterName, CurrentState == ButtonStates.ButtonPressed);
			}
			if (IdleAnimationParameterName != null)
			{
				_animator.SetBool(IdleAnimationParameterName, CurrentState == ButtonStates.Off);
			}
		}
	}

	public virtual void OnSubmit(BaseEventData eventData)
	{
		if (ButtonPressedFirstTime != null)
		{
			ButtonPressedFirstTime.Invoke();
		}
		if (ButtonReleased != null)
		{
			ButtonReleased.Invoke();
		}
	}
}
