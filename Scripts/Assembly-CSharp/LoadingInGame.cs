using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using Pathfinding;
using TMPro;
using UnityEngine;
using _Modules.UIInGame.Scripts;

public class LoadingInGame : MonoBehaviour
{
	[SerializeField]
	private LoadingInGameUIController _loadingInGameUIController;

	[SerializeField]
	public List<TextMeshProUGUI> playerNameList = new List<TextMeshProUGUI>();

	[SerializeField]
	private Camera camLoading;

	[SerializeField]
	private Localize textMap;

	[SerializeField]
	private Localize textObjective;

	public GameObject loadingUI;

	public GameObject loadingObject;

	public GameObject loadingText;

	public GameObject loadingScan;

	public GameObject pressAnyKey;

	public GameObject discaimer;

	[SerializeField]
	private GameObject _blackBG;

	public List<string> ListTitleTipsTerms = new List<string>();

	public TextMeshProUGUI textTitleTips;

	public TextMeshProUGUI textDescTips;

	public int TotalTips;

	private void Awake()
	{
		loadingUI.SetActive(value: true);
		if ((bool)loadingObject && (bool)NetworkGameManager.Instance && NetworkGameManager.Instance.arrPlayerController.Count == 0)
		{
			loadingObject.SetActive(value: false);
		}
		if ((bool)NetworkGameManager.Instance && NetworkGameManager.Instance.isServer && (bool)GameManagerPhoton.Instance)
		{
			GameManagerPhoton.Instance.HostLoadingGameComplete = false;
		}
	}

	private IEnumerator Start()
	{
		if (LobbyManager.Instance == null)
		{
			UIGameManager.Instance.fadeBlack.enabled = true;
			UIGameManager.Instance.fadeBlack.DOKill();
			UIGameManager.Instance.fadeBlack.DOFade(1f, 0f);
			int index = UnityEngine.Random.Range(0, TotalTips);
			textTitleTips.text = "- " + LocalizationManager.GetTranslation("Menu/" + ListTitleTipsTerms[index]).ToUpper() + " -";
			textDescTips.text = LocalizationManager.GetTranslation("Menu/Tips" + index);
			while (GameManagerPhoton.Instance == null || GameManagerPhoton.Instance.CurrentMission == null)
			{
				yield return null;
			}
			UIGameManager.Instance.SetMissionLocation(textMap, textObjective);
			UIGameManager.Instance.fadeBlack.DOFade(0f, 0.75f).SetDelay(1.5f);
			if (_loadingInGameUIController == null)
			{
				_loadingInGameUIController = loadingObject.GetComponent<LoadingInGameUIController>();
			}
			_loadingInGameUIController.Init();
			loadingUI.SetActive(value: true);
			yield return new WaitForSeconds(0.1f);
			if ((bool)NetworkGameManager.Instance.ownPlayer)
			{
				if (NetworkGameManager.Instance.isServer)
				{
					NetworkGameManager.Instance.ownPlayer.network.playerPhoton.SyncCurrentPosition = NetworkGameManager.Instance.ownPlayer.transform.position;
				}
				else
				{
					NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcSetSyncPosition(NetworkGameManager.Instance.ownPlayer.transform.position);
				}
			}
			yield return new WaitForSeconds(2f);
			GameManager.Instance.RandomizeItem();
			yield return new WaitForSeconds(2f);
			foreach (Progress item in GameManager.Instance.AStarPath.ScanAsync())
			{
				_ = item;
			}
			yield return new WaitForSeconds(1f);
			UIGameManager.Instance.fadeBlack.DOFade(1f, 0.3f).OnComplete(() =>
			{
				NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
				loadingUI.SetActive(value: false);
				loadingObject.SetActive(value: false);
				camLoading.gameObject.SetActive(value: false);
			});
			UIGameManager.Instance.fadeBlack.DOFade(0f, 0.5f).SetDelay(0.5f);
			if (!NetworkGameManager.Instance.isServer)
			{
				foreach (DestructibleObject item2 in GameManager.Instance.arrDestructibleObject)
				{
					if ((bool)item2.Object && item2.isDestroyed)
					{
						item2.colliderObject.enabled = false;
						GameManager.Instance.AStarPath.UpdateGraphs(item2.colliderObject.bounds);
						if ((bool)item2.ObjectCollision.destructibleComp)
						{
							UnityEngine.Object.Destroy(item2.ObjectCollision.destructibleComp.gameObject);
						}
					}
				}
				if ((bool)GameManagerPhoton.Instance.Object && GameManagerPhoton.Instance.objectiveComplete)
				{
					UIMissionObjective.Instance.SetCheckboxRetrieveKeyItem();
					GameManager.Instance.waveManager.ExecuteHorde();
				}
			}
			GameManager.Instance.timer.StartTimer();
			if (NetworkGameManager.Instance.isServer)
			{
				GameManagerPhoton.Instance.HostLoadingGameComplete = true;
			}
			else if ((bool)GameManagerPhoton.Instance.HostLoadingGameComplete)
			{
				GameManagerPhoton.Instance.RpcSyncTimer();
			}
			if (NetworkGameManager.Instance.isServer)
			{
				NetworkGameManager.Instance.ownPlayer.network.playerPhoton.SyncCurrentPosition = NetworkGameManager.Instance.ownPlayer.transform.position;
			}
			else
			{
				NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcSetSyncPosition(NetworkGameManager.Instance.ownPlayer.transform.position);
			}
			UIMissionObjective.Instance.SetUIMission();
			if (GameModes.Instance.IsRandomRotateCam)
			{
				UnityEngine.Random.InitState(GlobalOptionsManager.Instance.seed);
				CameraGame.Instance.RotateCamera(UnityEngine.Random.Range(0, 4) * CameraGame.Instance.CamRotationPerClick);
				UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
			}
			GameManager.Instance.SpawnInitEnemy = true;
			yield return new WaitForSeconds(4f);
			foreach (DestructibleObject item3 in GameManager.Instance.arrDestructibleObject)
			{
				if ((bool)item3 && (bool)item3.colliderObject && item3.colliderObject.gameObject.layer == 22)
				{
					item3.colliderObject.gameObject.layer = 9;
					GameManager.Instance.AStarPath.UpdateGraphs(item3.colliderObject.bounds);
				}
			}
			GameManager.Instance.AStarPath.FlushGraphUpdates();
			_blackBG.SetActive(value: true);
		}
		yield return new WaitForSeconds(0f);
	}
}
