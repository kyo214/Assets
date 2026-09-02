using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.Cutscene.Scripts;

public class StartMissionLobby_CutsceneTimelineController : CutsceneTimelineController
{
	[SerializeField]
	private GameObject _carCircleGameObject;

	[SerializeField]
	private GameObject _carGameObject;

	[SerializeField]
	private GameObject _carIconGameObject;

	[SerializeField]
	private List<GameObject> _objectsToBeHidden = new List<GameObject>();

	[SerializeField]
	private Image _blackscreen;

	public override void OnStart()
	{
		SurvivorLobbyManager.Instance.BackBtnClicked();
		HideGameObject();
		base.OnStart();
	}

	public override void OnComplete()
	{
		_blackscreen.DOFade(1f, 0.3f).OnComplete(() =>
		{
			UIGameManager.Instance.loading.loadingUI.SetActive(value: true);
		});
		base.BlackBarController.HideBar(() =>
		{
			LobbyManager.Instance.LoadInGameScene();
			GameManager.Instance.ShowAllPlayer(setInput: false);
		});
	}

	private void HideGameObject()
	{
		GameManager.Instance.HideAllPlayer(_carGameObject.transform);
		_carCircleGameObject.SetActive(value: false);
		_carIconGameObject.SetActive(value: false);
		foreach (GameObject item in _objectsToBeHidden)
		{
			if ((bool)item)
			{
				item.SetActive(value: false);
			}
		}
	}
}
