using UnityEngine;

namespace _Modules.Cutscene.Scripts;

[CreateAssetMenu(fileName = "HideUICutsceneAction", menuName = "WMO/Cutscene/Cutscene Action/HideUICutsceneAction", order = 0)]
public class HideUICutsceneAction : CustomCutsceneAction
{
	public override void Invoke()
	{
		if (!NetworkGameManager.Instance)
		{
			return;
		}
		PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
		if ((bool)ownPlayer)
		{
			UIGameManager instance = UIGameManager.Instance;
			if ((bool)instance && !instance.UIMenuPuzzle.isHidden)
			{
				ownPlayer.ClosePuzzle(forceShow: true);
			}
		}
	}
}
