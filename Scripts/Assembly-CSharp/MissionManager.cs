using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using _Modules.UIGlobal;

public class MissionManager : MonoBehaviour
{
	public bool IsTimerCountdownMode;

	public bool IsCountAgentInCircle;

	public int TimerCountdown;

	public int KeyItemToActivateCar;

	public int KeyItemForDecreasedCountdown;

	public static MissionManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public void CheckTimeCountdown()
	{
		if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission && GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsCountdownEndlessHordeEnable)
		{
			UIGameManager.Instance.txtTime.transform.parent.gameObject.SetActive(value: true);
			IsTimerCountdownMode = true;
			TimerCountdown = GameManagerPhoton.Instance.CurrentMission.MissionObjective.GetCountdownTimerEndlessHorde(NetworkGameManager.Instance.arrPlayerController.Count);
		}
		else
		{
			IsTimerCountdownMode = false;
			TimerCountdown = 0;
		}
	}

	private IEnumerator Start()
	{
		if ((bool)LobbyManager.Instance)
		{
			yield break;
		}
		IsTimerCountdownMode = false;
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		if (string.IsNullOrWhiteSpace(NetworkGameManager.Instance.ownPlayer.data.SkillData.PerkId))
		{
			NetworkGameManager.Instance.ownPlayer.audioListener.enabled = false;
			GenericSingleton<PopupUIManager>.Instance.Show(PopupUIManager.Type.OK, "Menu/ErrorJoinRoom", () =>
			{
				UIGameManager.Instance.QuitGame();
				UIGameManager.Instance.fadeBlack.DOKill();
				UIGameManager.Instance.fadeBlack.color = new Color(0f, 0f, 0f, 1f);
			});
		}
		while (GameManagerPhoton.Instance.CurrentMission == null)
		{
			yield return null;
		}
		if (!NetworkGameManager.Instance.isServer)
		{
			GameManagerPhoton.Instance.CurrentMission.MissionObjective = GameManagerPhoton.Instance.CurrentMission.MissionObjective;
		}
		CheckTimeCountdown();
		if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission && GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning)
		{
			UIGameManager.Instance.txtInfoUITerm.SetTerm("Menu/Wave");
			if (GameManagerPhoton.Instance.Wave > 0)
			{
				UIGameManager.Instance.txtTime.text = GameManagerPhoton.Instance.Wave.ToString();
			}
			else
			{
				UIGameManager.Instance.txtTime.text = "1";
			}
			if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.MaxWave > 0)
			{
				TextMeshProUGUI txtTime = UIGameManager.Instance.txtTime;
				txtTime.text = txtTime.text + " / " + GameManagerPhoton.Instance.CurrentMission.MissionObjective.MaxWave;
			}
		}
		else if (!IsTimerCountdownMode && (bool)UIGameManager.Instance)
		{
			UIGameManager.Instance.txtTime.transform.parent.gameObject.SetActive(value: false);
			RectTransform component = UIGameManager.Instance.uiHordeIncoming.GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(component.anchoredPosition.x, -285f);
		}
		if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission && GameManagerPhoton.Instance.CurrentMission.MissionObjective.MissionKeyItem != 0)
		{
			KeyItemToActivateCar = GameManagerPhoton.Instance.CurrentMission.MissionObjective.MissionKeyItem;
		}
		yield return new WaitForSeconds(1f);
		if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.MinTargetDestroy > 0)
		{
			if (GameManagerPhoton.Instance.TargetDestroyed < GameManagerPhoton.Instance.CurrentMission.MissionObjective.MinTargetDestroy)
			{
				foreach (ItemInteractable item in GameManager.Instance.ListBrimCarInteractable)
				{
					item?.DisableCollider();
				}
			}
			else
			{
				foreach (ItemInteractable item2 in GameManager.Instance.ListBrimCarInteractable)
				{
					item2?.EnableCollider();
				}
			}
		}
		GameManager.Instance.waveManager.InitHorde(isInit: true);
	}
}
