using System;
using UnityEngine.UI;

namespace MoreMountains.Tools;

[Serializable]
public class MMDebugMenuChoiceEntry
{
	public Button TargetButton;

	public Text ButtonText;

	public Image ButtonBg;

	public string ButtonEventName = "ButtonEvent";
}
