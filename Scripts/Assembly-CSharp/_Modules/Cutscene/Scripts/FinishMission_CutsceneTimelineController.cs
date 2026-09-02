using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Modules.Cutscene.Scripts;

public class FinishMission_CutsceneTimelineController : CutsceneTimelineController
{
	[SerializeField]
	private GameObject _carCircleGameObject;

	[SerializeField]
	private GameObject _carGameObject;

	public override void OnStart()
	{
		UIGameManager.Instance.SetUIVisibility(setActiveUI: false);
		HideGameObject();
		base.OnStart();
	}

	public override void OnComplete()
	{
		GameManager.Instance.ShowAllPlayer(setInput: false);
		base.BlackBarController.FadeBlack();
		UniTaskUtil.DelayedCall(this, 0.75f, () =>
		{
			base.BlackBarController.HideBar(OnCompleteHideBar);
		}).Forget();
	}

	private void HideGameObject()
	{
		if ((bool)_carGameObject)
		{
			GameManager.Instance.HideAllPlayer(_carGameObject.transform);
		}
		else
		{
			GameManager.Instance.HideAllPlayer();
		}
		if ((bool)_carCircleGameObject)
		{
			_carCircleGameObject.SetActive(value: false);
		}
	}

	private void OnCompleteHideBar()
	{
		NetworkGameManager instance = NetworkGameManager.Instance;
		UIGameManager.Instance.loading.loadingUI.SetActive(value: true);
		instance.ownPlayer.characterRenderController.ShowCharacter();
		instance.StartCoroutine(NetworkGameManager.Instance.WinLevel());
	}
}
