using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Toked.FunctionComponent;

public class LongClickButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	private bool _pointerDown;

	private float _pointerDownTimer;

	[SerializeField]
	private UIButton _button;

	[SerializeField]
	private bool _intractable = true;

	[SerializeField]
	private float _requiredHoldTime = 1f;

	public UnityEvent onLongClick;

	[SerializeField]
	private SlicedFilledImage _fillImage;

	private InputAction _playerInputAction;

	[SerializeField]
	private bool _usingCustomInputReference;

	[SerializeField]
	private InputActionReference _playerInputActionReference;

	[SerializeField]
	private Vector2 _vibrate = new Vector2(0.1f, 0.1f);

	private void Start()
	{
		if ((object)_button == null)
		{
			_button = GetComponent<UIButton>();
		}
		_playerInputAction = ((_usingCustomInputReference && (bool)_playerInputActionReference) ? ((InputAction)_playerInputActionReference) : InputManager.inputActions.UI.Submit);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_pointerDown = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Reset();
	}

	private void Update()
	{
		if (_intractable && _button.isSelected && _playerInputAction.WasPressedThisFrame() && GlobalOptionsManager.Instance.usingGamepad)
		{
			InputManager.StartRumble(_vibrate.x, _vibrate.y);
		}
		if (_intractable && _button.isSelected && _playerInputAction.IsPressed())
		{
			_pointerDown = true;
		}
		if ((_pointerDown && _button.isSelected && _playerInputAction.WasReleasedThisFrame()) || (_pointerDown && !_button.isSelected))
		{
			Reset();
		}
		if (_pointerDown && _intractable)
		{
			_pointerDownTimer += Time.deltaTime;
			if (_pointerDownTimer >= _requiredHoldTime)
			{
				onLongClick?.Invoke();
				Reset(resetPointerDown: false);
			}
			if (_requiredHoldTime > 0f)
			{
				SetFillImage(_pointerDownTimer / _requiredHoldTime);
			}
		}
	}

	private void OnDisable()
	{
		Reset();
	}

	private void Reset(bool resetPointerDown = true)
	{
		if (resetPointerDown)
		{
			_pointerDown = false;
		}
		_pointerDownTimer = 0f;
		SetFillImage(_pointerDownTimer / _requiredHoldTime);
		InputManager.StopRumble();
	}

	public void SetIntractable(bool isIntractable)
	{
		_intractable = isIntractable;
	}

	private void SetFillImage(float fillAmount)
	{
		if (_fillImage != null)
		{
			_fillImage.fillAmount = fillAmount;
		}
	}
}
