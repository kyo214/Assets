using System;
using Toked.Skill;
using UnityEngine;
using _Modules.Skill.Scripts.PerksNew;

namespace _Modules.DialogueSystem;

[CreateAssetMenu(fileName = "ShowPerkLearnPopUpUI_DialogueCustomAction", menuName = "WMO/ScriptableObjects/Dialogue/CustomAction/ShowPerkLearnPopUpUI", order = 0)]
public class ShowPerkLearnPopUpUI_DialogueCustomAction : DialogueCustomActionBase
{
	private enum PerkUiType
	{
		PerkV1 = 0,
		PerkV2 = 1
	}

	[SerializeField]
	private PerkUiType _perkUiType;

	public override void Invoke(Action onCompleted)
	{
		switch (_perkUiType)
		{
		case PerkUiType.PerkV1:
			GenericSingleton<PerkLearnPopupUI>.Instance.Init(onCompleted);
			break;
		case PerkUiType.PerkV2:
			CameraGame.Instance.RemoveAllMember();
			CameraGame.Instance.CinemachineTarget.AddMember(NetworkGameManager.Instance.ownPlayer.transform, 1f, 3f);
			GenericSingleton<PerkLearnPopupNewUI>.Instance.Init(onCompleted);
			break;
		}
	}
}
