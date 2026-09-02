using TMPro;
using UnityEngine;

namespace _Modules.GameSystem.BaseScripts;

public class SaveDataDebugUI : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _text;

	private void OnEnable()
	{
		RefreshSaveDataInfo();
	}

	public void RefreshSaveDataInfo()
	{
		if (GlobalSaveData.instance?.gameData != null)
		{
			_text.text = JsonUtility.ToJson(GlobalSaveData.instance.gameData, prettyPrint: true);
		}
	}
}
