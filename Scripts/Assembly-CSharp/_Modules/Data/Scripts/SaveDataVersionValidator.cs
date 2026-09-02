using System.Collections;
using UnityEngine;

namespace _Modules.Data.Scripts;

public class SaveDataVersionValidator : MonoBehaviour
{
	private bool _showPopup;

	private void Start()
	{
		for (int i = 0; i < 3; i++)
		{
			CheckSoloSaveData(i);
			CheckMultiplayerSaveData(i);
		}
		Debug.Log("cek-- " + _showPopup);
		if (_showPopup)
		{
			_showPopup = false;
		}
	}

	private void CheckSoloSaveData(int index)
	{
		if (GlobalSaveData.instance.CheckInGameDataExists(index))
		{
			GameData gameData = GlobalSaveData.instance.LoadSoloGameData(index);
			if (gameData != null && !gameData.CheckVersionCompability())
			{
				_showPopup = true;
			}
		}
	}

	private void CheckMultiplayerSaveData(int index)
	{
		if (GlobalSaveData.instance.CheckMultiplayerInGameDataExists(index))
		{
			GameData gameData = GlobalSaveData.instance.LoadMultiplayerGameData(index);
			if (gameData != null && !gameData.CheckVersionCompability())
			{
				_showPopup = true;
			}
		}
	}

	private IEnumerator ShowPopup()
	{
		while (true)
		{
			if ((bool)UITitleMenuManager.Instance.flowControlGraph.flow.activeNode)
			{
				string nodeName = UITitleMenuManager.Instance.flowControlGraph.flow.activeNode.nodeName;
				if (nodeName == "Main Menu" || nodeName == "New Game")
				{
					break;
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
		if (!GlobalSaveData.instance.IsPatchNoteShown)
		{
			UITitleMenuManager.Instance.ShowUIPatchNote();
		}
	}

	public static bool CheckVersionCompability(string currentVersion)
	{
		int.TryParse(currentVersion, out var result);
		return result >= 450;
	}
}
