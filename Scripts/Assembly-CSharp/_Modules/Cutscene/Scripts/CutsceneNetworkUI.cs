using TMPro;
using UnityEngine;

namespace _Modules.Cutscene.Scripts;

public class CutsceneNetworkUI : MonoBehaviour
{
	[SerializeField]
	private GameObject _gameObjectUi;

	[SerializeField]
	private TMP_Text _skipCounterText;

	private void OnEnable()
	{
		CutsceneNetworkManager.OnPlayerSkipCutsceneEvent += OnPlayerSkipCutsceneAction;
		CutsceneNetworkManager.OnShowHideCutsceneEvent += OnShowHideCutsceneAction;
	}

	private void OnDisable()
	{
		CutsceneNetworkManager.OnPlayerSkipCutsceneEvent -= OnPlayerSkipCutsceneAction;
		CutsceneNetworkManager.OnShowHideCutsceneEvent -= OnShowHideCutsceneAction;
	}

	public void SetCounter(int skipCount, int totalPlayer)
	{
		_skipCounterText.text = $"{skipCount}/{totalPlayer}";
	}

	private void OnPlayerSkipCutsceneAction(CutsceneNetworkManager cutsceneNetworkManager)
	{
		if (!(cutsceneNetworkManager == null))
		{
			SetCounter(cutsceneNetworkManager.playerSkipCount, cutsceneNetworkManager.playerCount);
			if (cutsceneNetworkManager.allPlayerSkip)
			{
				_gameObjectUi.SetActive(value: false);
			}
		}
	}

	private void OnShowHideCutsceneAction(CutsceneNetworkManager cutsceneNetworkManager)
	{
		if (cutsceneNetworkManager == null)
		{
			return;
		}
		if (cutsceneNetworkManager.showCutscene && cutsceneNetworkManager.cutsceneManager.CutsceneSo != null)
		{
			bool skippable = cutsceneNetworkManager.cutsceneManager.CutsceneSo.Skippable;
			if (skippable)
			{
				SetCounter(cutsceneNetworkManager.playerSkipCount, cutsceneNetworkManager.playerCount);
			}
			else
			{
				_skipCounterText.text = "";
			}
			_gameObjectUi.SetActive(skippable);
		}
		else
		{
			_gameObjectUi.SetActive(value: false);
		}
	}
}
