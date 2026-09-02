using System.Collections;
using System.Collections.Generic;
using Fusion;
using Toked;
using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
	public enum MultiplayerMode
	{
		Solo = 0,
		Server = 1,
		Client = 2,
		Auto = 3
	}

	public PhotonMultiplayerManager photonNetworking;

	public GameObject PhotonNetworkingPrefab;

	public MultiplayerMode mode;

	public bool isReconnecting;

	public bool isSyncingMissionMap;

	public bool isPrivateRoom;

	public bool networkInitialized;

	public List<PlayerTempInventory> ListPlayerTempInventory = new List<PlayerTempInventory>();

	public List<PlayerController> arrPlayerController = new List<PlayerController>();

	public List<PlayerController> arrPlayerNetworkController = new List<PlayerController>();

	public List<PlayerController> arrPlayerDisconnected = new List<PlayerController>();

	public List<string> arrPlayerIDDisconnected = new List<string>();

	public List<SessionInfo> sessionList = new List<SessionInfo>();

	public PlayerController ownPlayer;

	public bool isLoadGame;

	public string sessionName;

	public bool isServer;

	public int Mission;

	public int MissionDifficulty;

	public List<PlayerRef> SpawnedCharacters = new List<PlayerRef>();

	public NetworkObjectPoolDefault networkPool;

	public bool IsErrorConnection;

	public bool ShowButtonReconnect;

	public static NetworkGameManager Instance { get; private set; }

	public void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
			photonNetworking.gameObject.SetActive(value: true);
			networkInitialized = true;
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void StartGame(MultiplayerMode gameMode, string _roomCode)
	{
		Instance.isServer = false;
		photonNetworking.StartGame(gameMode, _roomCode);
	}

	public void Shutdown()
	{
		photonNetworking.Shutdown();
	}

	public PlayerController GetPlayer(int idx)
	{
		if (arrPlayerNetworkController[idx] == null)
		{
			for (int i = 0; i < Instance.arrPlayerController.Count; i++)
			{
				Instance.arrPlayerNetworkController[Instance.arrPlayerController[i].network.GetIDX()] = Instance.arrPlayerController[i];
			}
		}
		return arrPlayerNetworkController[idx];
	}

	public PlayerController GetPlayerNearest(bool isHaveHealth, Vector3 posObject)
	{
		int num = -1;
		float num2 = 999999f;
		PlayerController result = null;
		for (int i = 0; i < arrPlayerController.Count; i++)
		{
			float num3 = MathFunc.Distance(posObject, arrPlayerController[i].transform.position);
			if (arrPlayerController[i].network.GetHealth() > 0f && num3 < num2 && !arrPlayerController[i].IsGhost)
			{
				num2 = num3;
				num = i;
			}
		}
		if (num >= 0)
		{
			result = arrPlayerController[num];
		}
		return result;
	}

	public PlayerController GetRandomPlayer(bool isHaveHealth, Transform excludeTarget = null)
	{
		int num = Random.Range(0, arrPlayerController.Count);
		PlayerController result = null;
		if (isHaveHealth)
		{
			if (arrPlayerController[num].network.GetHealth() > 0f)
			{
				result = arrPlayerController[num];
			}
			else
			{
				for (int i = 0; i < arrPlayerController.Count - 1; i++)
				{
					num++;
					if (num >= arrPlayerController.Count)
					{
						num = 0;
					}
					if (arrPlayerController[num].network.GetHealth() > 0f && arrPlayerController[num].transform != excludeTarget)
					{
						result = arrPlayerController[num];
						break;
					}
				}
			}
		}
		else
		{
			result = arrPlayerController[num];
		}
		return result;
	}

	public bool IsAllPlayerDead(bool isOnlyCheckPlayerDown = false)
	{
		bool result = false;
		float num = 0f;
		for (int i = 0; i < arrPlayerController.Count; i++)
		{
			if (!arrPlayerController[i].reviveArea.enabled && !arrPlayerController[i].isPermadeath)
			{
				num += arrPlayerController[i].network.GetHealth();
			}
			if (arrPlayerController[i].network.GetLife() > 0 && !isOnlyCheckPlayerDown)
			{
				num++;
			}
		}
		if (num <= 0f)
		{
			result = true;
		}
		return result;
	}

	public bool IsOnePlayerSurvive()
	{
		bool result = false;
		int num = 0;
		for (int i = 0; i < arrPlayerController.Count; i++)
		{
			if (arrPlayerController[i].network.GetHealth() > 0f)
			{
				num++;
			}
		}
		if (num == 1)
		{
			result = true;
		}
		return result;
	}

	public IEnumerator WinLevel()
	{
		ownPlayer.audioListener.transform.localPosition = new Vector3(ownPlayer.audioListener.transform.localPosition.x, 200f, ownPlayer.audioListener.transform.localPosition.z);
		UIMissionObjective.Instance.SetAllObjectiveCleared();
		yield return new WaitForSeconds(1f);
		foreach (PlayerController item in arrPlayerController)
		{
			item.network.SetEnableControl(value: false);
			item.direction = Vector3.zero;
			item.fsmUpperBody.SetBool("isMoving", value: false);
			item.fsmLowerBody.SetBool("isMoving", value: false);
			item.animLowerChar.Play("LegIdle" + ownPlayer.angleRot, 1);
		}
		if (isServer)
		{
			ownPlayer.network.StartCoroutine(ownPlayer.network.ShowResultScene());
		}
	}
}
