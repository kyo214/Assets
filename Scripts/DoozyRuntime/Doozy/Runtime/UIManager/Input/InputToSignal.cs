using System;
using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Doozy.Runtime.UIManager.Input;

[AddComponentMenu("Input/Input To Signal")]
public class InputToSignal : MonoBehaviour
{
	[SerializeField]
	private bool AutoConnect;

	[SerializeField]
	private InputSystemUIInputModule UIInputModule;

	[SerializeField]
	private PlayerInput PlayerInput;

	[SerializeField]
	private int PlayerIndex;

	[SerializeField]
	private UIInputActionName InputActionName;

	[SerializeField]
	private string CustomInputActionName;

	private InputAction m_Action;

	private bool m_IsConnected;

	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	[ClearOnReload(true)]
	public static HashSet<InputToSignal> database { get; } = new HashSet<InputToSignal>();

	public PlayerInput playerInput => PlayerInput;

	public InputSystemUIInputModule uiInputModule => UIInputModule;

	public InputAction action => m_Action;

	public int playerIndex
	{
		get
		{
			if (!hasPlayerInput)
			{
				return PlayerIndex;
			}
			return PlayerInput.playerIndex;
		}
	}

	public string inputActionName
	{
		get
		{
			if (!hasCustomActionName)
			{
				return InputActionName.ToString();
			}
			return CustomInputActionName;
		}
	}

	public bool autoConnect => AutoConnect;

	public bool isConnected => m_IsConnected;

	public bool hasPlayerInput => playerInput != null;

	public bool hasUIInputModule => uiInputModule != null;

	public bool hasCustomActionName => InputActionName == UIInputActionName.CustomActionName;

	public static void CleanDatabase()
	{
		database.Remove(null);
	}

	private void GetReferences()
	{
		if ((object)PlayerInput == null)
		{
			PlayerInput = GetComponent<PlayerInput>();
		}
		if (PlayerInput != null)
		{
			UIInputModule = PlayerInput.uiInputModule;
		}
		else if ((object)UIInputModule == null)
		{
			UIInputModule = GetComponent<InputSystemUIInputModule>();
		}
	}

	private void Reset()
	{
		AutoConnect = true;
		PlayerIndex = inputSettings.defaultPlayerIndex;
		InputActionName = UIInputActionName.Cancel;
		CustomInputActionName = string.Empty;
		GetReferences();
	}

	private void Awake()
	{
		database.Add(this);
		GetReferences();
		if (!(UIInputModule != null))
		{
			base.enabled = false;
		}
	}

	private void OnEnable()
	{
		CleanDatabase();
		if (autoConnect)
		{
			Connect();
		}
	}

	private void OnDisable()
	{
		CleanDatabase();
		Disconnect();
	}

	private void OnDestroy()
	{
		database.Remove(this);
	}

	public InputToSignal Connect()
	{
		if (isConnected)
		{
			return this;
		}
		if (action != null)
		{
			action.performed -= OnActionPerformed;
			m_Action = null;
		}
		var (flag, message) = IsValid();
		if (!flag)
		{
			Debug.Log(message);
			return this;
		}
		action.performed += OnActionPerformed;
		m_IsConnected = true;
		return this;
	}

	public InputToSignal Disconnect()
	{
		if (!isConnected)
		{
			return this;
		}
		if (action == null)
		{
			return this;
		}
		action.performed -= OnActionPerformed;
		m_IsConnected = false;
		return this;
	}

	public InputToSignal ConnectToAction(UIInputActionName actionName)
	{
		Disconnect();
		m_Action = null;
		InputActionName = actionName;
		CustomInputActionName = string.Empty;
		Connect();
		return this;
	}

	public InputToSignal ConnectToCustomAction(string actionName)
	{
		Disconnect();
		m_Action = null;
		InputActionName = UIInputActionName.CustomActionName;
		CustomInputActionName = actionName;
		Connect();
		return this;
	}

	public InputToSignal ConnectToCustomAction(InputAction inputAction)
	{
		if (inputAction == null)
		{
			return this;
		}
		Disconnect();
		InputActionName = UIInputActionName.CustomActionName;
		CustomInputActionName = inputAction.name;
		m_Action = inputAction;
		Connect();
		return this;
	}

	private void OnActionPerformed(InputAction.CallbackContext context)
	{
		InputStream.stream.SendSignal(new InputSignalData(InputActionName, context, playerIndex, playerInput));
	}

	private (bool, string) IsValid()
	{
		if (m_Action != null)
		{
			return (true, "Valid");
		}
		if (UIInputModule == null)
		{
			return (false, "Not Valid: UIInputModule is null");
		}
		m_Action = null;
		m_Action = InputActionName switch
		{
			UIInputActionName.Point => UIInputModule.point.action, 
			UIInputActionName.Click => UIInputModule.leftClick.action, 
			UIInputActionName.MiddleClick => UIInputModule.middleClick.action, 
			UIInputActionName.RightClick => UIInputModule.rightClick.action, 
			UIInputActionName.ScrollWheel => UIInputModule.scrollWheel.action, 
			UIInputActionName.Navigate => UIInputModule.move.action, 
			UIInputActionName.Submit => UIInputModule.submit.action, 
			UIInputActionName.Cancel => UIInputModule.cancel.action, 
			UIInputActionName.TrackedDevicePosition => UIInputModule.trackedDevicePosition.action, 
			UIInputActionName.TrackedDeviceOrientation => UIInputModule.trackedDeviceOrientation.action, 
			UIInputActionName.CustomActionName => UIInputModule.actionsAsset.FindAction(CustomInputActionName), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		if (m_Action != null)
		{
			return (true, "Valid");
		}
		return (false, "Not Valid: m_Action is null");
	}
}
