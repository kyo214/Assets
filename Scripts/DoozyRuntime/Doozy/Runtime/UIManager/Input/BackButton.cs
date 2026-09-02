using System.Collections;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace Doozy.Runtime.UIManager.Input;

[AddComponentMenu("Input/Back Button")]
[DisallowMultipleComponent]
public class BackButton : SingletonBehaviour<BackButton>
{
	public const string k_BackButtonVirtualButtonName = "Cancel";

	public const string k_StreamCategory = "Input";

	public const string k_StreamName = "BackButton";

	public const string k_StreamNameIgnoreDisabled = "BackButton.IgnoreDisabledState";

	public const string k_StreamNameOnEnabled = "BackButton.Enabled";

	public const string k_StreamNameOnDisabled = "BackButton.Disabled";

	public const string k_ButtonName = "Back";

	[ClearOnReload]
	private static SignalStream s_stream;

	[ClearOnReload]
	private static SignalStream s_streamIgnoreDisabled;

	[ClearOnReload]
	private static SignalStream s_streamOnEnabled;

	[ClearOnReload]
	private static SignalStream s_streamOnDisabled;

	private int m_BackButtonDisableLevel;

	private double m_LastTimeBackButtonWasExecuted;

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream("Input", "BackButton"));

	public static SignalStream streamIgnoreDisabled => s_streamIgnoreDisabled ?? (s_streamIgnoreDisabled = SignalsService.GetStream("Input", "BackButton.IgnoreDisabledState"));

	public static SignalStream streamOnEnabled => s_streamOnEnabled ?? (s_streamOnEnabled = SignalsService.GetStream("Input", "BackButton.Enabled"));

	public static SignalStream streamOnDisabled => s_streamOnDisabled ?? (s_streamOnDisabled = SignalsService.GetStream("Input", "BackButton.Disabled"));

	[ClearOnReload]
	private static SignalReceiver inputStreamReceiver { get; set; }

	[ClearOnReload]
	private static SignalReceiver buttonStreamReceiver { get; set; }

	public static UIManagerInputSettings inputSettings => SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;

	public static bool multiplayerMode => inputSettings.multiplayerMode;

	public static float cooldown => inputSettings.backButtonCooldown;

	public bool isDisabled
	{
		get
		{
			if (m_BackButtonDisableLevel < 0)
			{
				m_BackButtonDisableLevel = 0;
			}
			return m_BackButtonDisableLevel != 0;
		}
	}

	public bool isEnabled => !isDisabled;

	public bool inCooldown => (double)Time.realtimeSinceStartup - m_LastTimeBackButtonWasExecuted < (double)cooldown;

	public bool canFire
	{
		get
		{
			if (isEnabled)
			{
				return !inCooldown;
			}
			return false;
		}
	}

	public bool hasInput { get; private set; }

	private bool initialized { get; set; }

	private static void ConnectToInputStream()
	{
		InputStream.Start();
		InputStream.stream.ConnectReceiver(inputStreamReceiver);
	}

	private static void DisconnectFromInputStream()
	{
		InputStream.Stop();
		InputStream.stream.DisconnectReceiver(inputStreamReceiver);
	}

	private static void ConnectToButtonStream()
	{
		UIButton.stream.ConnectReceiver(buttonStreamReceiver);
	}

	private static void DisconnectFromButtonStream()
	{
		UIButton.stream.DisconnectReceiver(buttonStreamReceiver);
	}

	public static void Initialize()
	{
		if (!SingletonBehaviour<BackButton>.applicationIsQuitting)
		{
			_ = SingletonBehaviour<BackButton>.instance;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		initialized = false;
		m_LastTimeBackButtonWasExecuted = Time.realtimeSinceStartup;
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		CheckForInput();
	}

	private IEnumerator Start()
	{
		yield return null;
		CheckForInput();
		initialized = hasInput;
		inputStreamReceiver = new SignalReceiver().SetOnSignalCallback((Signal signal) =>
		{
			if (signal.hasValue && signal.valueAsObject is InputSignalData { inputActionName: UIInputActionName.Cancel } data)
			{
				Fire(data);
			}
		});
		ConnectToInputStream();
		buttonStreamReceiver = new SignalReceiver().SetOnSignalCallback((Signal signal) =>
		{
			if (signal.hasValue && signal.valueAsObject is UIButtonSignalData uIButtonSignalData && uIButtonSignalData.buttonName.Equals("Back"))
			{
				Fire(new InputSignalData(UIInputActionName.Cancel, uIButtonSignalData.playerIndex));
			}
		});
		ConnectToButtonStream();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		DisconnectFromInputStream();
		DisconnectFromButtonStream();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public void CheckForInput()
	{
		hasInput = false;
		if (EventSystem.current == null && !multiplayerMode)
		{
			Debug.LogWarning("EventSystem.current is null. Add it to the scene to fix this issue.");
			return;
		}
		if (EventSystem.current != null)
		{
			AddInputToSignalToGameObject(EventSystem.current.gameObject);
			hasInput = true;
		}
		if (!multiplayerMode)
		{
			return;
		}
		MultiplayerEventSystem[] array = Object.FindObjectsOfType<MultiplayerEventSystem>();
		if (!hasInput || array == null || array.Length == 0)
		{
			Debug.LogWarning("MultiplayerMode -> No MultiplayerEventSystem found. Add at least one to the scene to fix this issue.");
			return;
		}
		MultiplayerEventSystem[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			AddInputToSignalToGameObject(array2[i].gameObject);
		}
		hasInput = true;
	}

	public static void Fire(InputSignalData data)
	{
		if (!SingletonBehaviour<BackButton>.applicationIsQuitting && !SingletonBehaviour<BackButton>.instance.inCooldown)
		{
			SingletonBehaviour<BackButton>.instance.m_LastTimeBackButtonWasExecuted = Time.realtimeSinceStartup;
			streamIgnoreDisabled.SendSignal(data);
			if (SingletonBehaviour<BackButton>.instance.isEnabled)
			{
				stream.SendSignal(data);
			}
		}
	}

	public static void Fire()
	{
		if (!SingletonBehaviour<BackButton>.applicationIsQuitting && !SingletonBehaviour<BackButton>.instance.inCooldown)
		{
			streamIgnoreDisabled.SendSignal();
			SingletonBehaviour<BackButton>.instance.m_LastTimeBackButtonWasExecuted = Time.realtimeSinceStartup;
			if (SingletonBehaviour<BackButton>.instance.isEnabled)
			{
				stream.SendSignal();
			}
		}
	}

	public static bool IsEnabled()
	{
		if (!SingletonBehaviour<BackButton>.applicationIsQuitting)
		{
			return SingletonBehaviour<BackButton>.instance.isEnabled;
		}
		return false;
	}

	public static bool IsDisabled()
	{
		if (!SingletonBehaviour<BackButton>.applicationIsQuitting)
		{
			return SingletonBehaviour<BackButton>.instance.isDisabled;
		}
		return false;
	}

	public static void Disable()
	{
		if (!SingletonBehaviour<BackButton>.applicationIsQuitting)
		{
			if (SingletonBehaviour<BackButton>.instance.isEnabled)
			{
				streamOnDisabled.SendSignal("BackButton.Disable");
			}
			SingletonBehaviour<BackButton>.instance.m_BackButtonDisableLevel++;
		}
	}

	public static void Enable()
	{
		if (!SingletonBehaviour<BackButton>.applicationIsQuitting)
		{
			SingletonBehaviour<BackButton>.instance.m_BackButtonDisableLevel--;
			if (SingletonBehaviour<BackButton>.instance.m_BackButtonDisableLevel < 0)
			{
				SingletonBehaviour<BackButton>.instance.m_BackButtonDisableLevel = 0;
			}
			if (SingletonBehaviour<BackButton>.instance.isEnabled)
			{
				streamOnEnabled.SendSignal("BackButton.Enable");
			}
		}
	}

	public static void EnableByForce()
	{
		if (!SingletonBehaviour<BackButton>.applicationIsQuitting)
		{
			SingletonBehaviour<BackButton>.instance.m_BackButtonDisableLevel = 0;
			streamOnEnabled.SendSignal("BackButton.EnableByForce");
		}
	}

	private static void AddInputToSignalToGameObject(GameObject target)
	{
		InputToSignal[] components = target.GetComponents<InputToSignal>();
		if (components == null || components.Length == 0 || !components.Any((InputToSignal i) => i.SendsBackButtonSignal()))
		{
			target.AddComponent<InputToSignal>().ConnectToAction(UIInputActionName.Cancel);
		}
	}
}
