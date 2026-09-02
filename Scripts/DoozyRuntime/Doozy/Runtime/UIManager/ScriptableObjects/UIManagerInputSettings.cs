using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.ScriptableObjects;
using Doozy.Runtime.UIManager.Input;
using UnityEngine;

namespace Doozy.Runtime.UIManager.ScriptableObjects;

public class UIManagerInputSettings : SingletonRuntimeScriptableObject<UIManagerInputSettings>
{
	public const InputHandling k_InputHandling = InputHandling.InputSystemPackage;

	public const int k_LifeTheUniverseAndEverything = 42;

	public const float k_BackButtonCooldown = 0.1f;

	[SerializeField]
	private int DefaultPlayerIndex = -42;

	[SerializeField]
	private bool MultiplayerMode;

	[SerializeField]
	private float BackButtonCooldown = 0.1f;

	[SerializeField]
	private bool SubmitTriggersPointerClick = true;

	[SerializeField]
	private string BackButtonVirtualButtonName = "Cancel";

	public int defaultPlayerIndex => DefaultPlayerIndex;

	public bool multiplayerMode
	{
		get
		{
			return MultiplayerMode;
		}
		set
		{
			MultiplayerMode = value;
		}
	}

	public float backButtonCooldown
	{
		get
		{
			return BackButtonCooldown;
		}
		set
		{
			BackButtonCooldown = value;
		}
	}

	public bool submitTriggersPointerClick
	{
		get
		{
			return SubmitTriggersPointerClick;
		}
		set
		{
			SubmitTriggersPointerClick = value;
		}
	}

	public string backButtonVirtualButtonName
	{
		get
		{
			return BackButtonVirtualButtonName;
		}
		set
		{
			BackButtonVirtualButtonName = value;
		}
	}

	[RestoreData("UIManagerInputSettings")]
	public static UIManagerInputSettings Get()
	{
		return SingletonRuntimeScriptableObject<UIManagerInputSettings>.instance;
	}
}
