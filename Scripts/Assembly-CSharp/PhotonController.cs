using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using I2.Loc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonController : MonoBehaviour, INetworkRunnerCallbacks
{
	[SerializeField]
	private NetworkPrefabRef _playerPrefab;

	[SerializeField]
	private bool _isTimerDisconnectedRunning;

	[SerializeField]
	private float timeDisconnect;

	[SerializeField]
	private bool _isDisconnectedFromServer;

	private void Start()
	{
		_isDisconnectedFromServer = false;
		NetworkRunner.CloudConnectionLost = (NetworkRunner.CloudConnectionLostHandler)Delegate.Combine(NetworkRunner.CloudConnectionLost, new NetworkRunner.CloudConnectionLostHandler(OnCloudConnectionLost));
	}

	private void OnDisable()
	{
		NetworkRunner.CloudConnectionLost = (NetworkRunner.CloudConnectionLostHandler)Delegate.Remove(NetworkRunner.CloudConnectionLost, new NetworkRunner.CloudConnectionLostHandler(OnCloudConnectionLost));
	}

	private void OnCloudConnectionLost(NetworkRunner runner, ShutdownReason reason, bool reconnecting)
	{
		if (reason == ShutdownReason.GameNotFound && NetworkGameManager.Instance.isServer)
		{
			runner.Shutdown(destroyGameObject: true, ShutdownReason.Error);
			return;
		}
		Debug.Log($"Cloud Connection Lost: {reason} (Reconnecting: {reconnecting})");
		if (!reconnecting)
		{
			runner.Shutdown(destroyGameObject: true, ShutdownReason.Error);
		}
		else if (!_isTimerDisconnectedRunning)
		{
			_isTimerDisconnectedRunning = true;
			StartCoroutine(WaitForReconnection(runner));
		}
	}

	private IEnumerator WaitForReconnection(NetworkRunner runner)
	{
		timeDisconnect = Time.time;
		float timeLimit = 20f;
		while (Time.time - timeDisconnect < timeLimit)
		{
			Debug.Log("Disconnected " + (Time.time - timeDisconnect));
			if (runner.IsCloudReady)
			{
				_isTimerDisconnectedRunning = false;
				Debug.Log("Reconnect Success");
				yield break;
			}
			yield return new WaitForSeconds(1f);
		}
		if (!runner.IsCloudReady)
		{
			Debug.Log("Cannot reconnect to the server after 20 seconds, shutting down the runner.");
			if (!NetworkGameManager.Instance.isServer)
			{
				NetworkGameManager.Instance.ShowButtonReconnect = true;
			}
			runner.Shutdown(destroyGameObject: true, ShutdownReason.Error);
		}
	}

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		PlayerJoined(runner, player);
	}

	public void PlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		string text = runner.CurrentScene.ToString();
		text = text.Replace("[Scene:", "");
		text = text.Replace("]", "");
		bool flag = false;
		if (runner.SessionInfo.PlayerCount > PhotonMultiplayerManager.MAX_PLAYERS || NetworkGameManager.Instance.arrPlayerController.Count >= PhotonMultiplayerManager.MAX_PLAYERS)
		{
			if (runner.Simulation.LocalPlayer == player)
			{
				SetActiveUiLoading(setActive: true);
				UIGameManager.Instance.ShowFailedConnect("ErrorRoomFul");
			}
			return;
		}
		if (text == "None")
		{
			if (runner.Simulation.IsServer && !NetworkGameManager.Instance.SpawnedCharacters.Contains(player))
			{
				NetworkGameManager.Instance.SpawnedCharacters.Add(player);
				Vector3 spawnPosition = GameManager.Instance.MapManager.GetSpawnPosition(0, player.RawEncoded % runner.Config.Simulation.DefaultPlayers);
				NetworkObject networkObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
				runner.SetPlayerObject(player, networkObject);
			}
			return;
		}
		if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo && GlobalSaveData.instance.buildVer != (int)runner.SessionInfo.Properties["buildVer"].PropertyValue)
		{
			if (runner.Simulation.LocalPlayer == player)
			{
				SetActiveUiLoading(setActive: true);
				UIGameManager.Instance.ShowFailedConnect("ErrorVersion");
			}
			return;
		}
		if (runner.Simulation.LocalPlayer == player)
		{
			Debug.Log("Connect - Current Scene = " + SceneManager.GetSceneByBuildIndex(int.Parse(text)).name);
		}
		bool flag2 = false;
		NetworkObject networkObject2 = runner.GetPlayerObject(player);
		PlayerController playerJoin = null;
		if ((bool)networkObject2)
		{
			playerJoin = networkObject2.GetComponent<PlayerController>();
		}
		string text2 = "";
		if (runner.GetPlayerConnectionToken(player) != null)
		{
			text2 = Encoding.UTF8.GetString(runner.GetPlayerConnectionToken(player));
		}
		if (runner.IsServer)
		{
			for (int i = 0; i < NetworkGameManager.Instance.arrPlayerIDDisconnected.Count; i++)
			{
				if (NetworkGameManager.Instance.arrPlayerIDDisconnected[i] == text2)
				{
					playerJoin = NetworkGameManager.Instance.arrPlayerDisconnected[i];
					if (playerJoin != null)
					{
						networkObject2 = playerJoin.GetComponent<NetworkObject>();
						flag2 = true;
						break;
					}
				}
			}
		}
		if ((text2 != "" && runner.Simulation.LocalPlayer != player && playerJoin != null) & flag2)
		{
			if (runner.Simulation.IsServer)
			{
				networkObject2.AssignInputAuthority(player);
				playerJoin.network.playerPhoton.IsDisconnected = false;
			}
			Debug.Log("-----Reconnect");
			playerJoin.network.GetIDX();
			playerJoin.DespawnTimer.StopDuration();
			NetworkGameManager.Instance.arrPlayerIDDisconnected.Remove(text2);
			NetworkGameManager.Instance.arrPlayerDisconnected.Remove(playerJoin);
			if (runner.IsServer)
			{
				PhotonMultiplayerManager.Instance.UpdateSessionDisconnectedPlayer();
			}
			playerJoin.Reconnected();
			if (!NetworkGameManager.Instance.arrPlayerController.Exists((PlayerController p) => p == playerJoin))
			{
				NetworkGameManager.Instance.arrPlayerController.Add(playerJoin);
			}
			flag = true;
			if (runner.Simulation.IsServer)
			{
				if ((bool)LobbyManager.Instance)
				{
					playerJoin.network.SetEnableControl(value: true);
				}
				playerJoin.network.playerPhoton.disconnected = false;
			}
			runner.SetPlayerObject(player, networkObject2);
			playerJoin.DisconnectedTimer.StopDuration();
		}
		if (playerJoin != null && !flag && !runner.IsServer && !NetworkGameManager.Instance.arrPlayerController.Exists((PlayerController p) => p == playerJoin))
		{
			NetworkGameManager.Instance.arrPlayerController.Add(playerJoin);
		}
		if (runner.Simulation.IsServer && !flag2)
		{
			NetworkObject playerObject = runner.GetPlayerObject(player);
			if (playerObject != null)
			{
				PlayerController component = playerObject.GetComponent<PlayerController>();
				NetworkGameManager.Instance.arrPlayerController.Remove(component);
				for (int num = 0; num < NetworkGameManager.Instance.arrPlayerNetworkController.Count; num++)
				{
					if (NetworkGameManager.Instance.arrPlayerNetworkController[num] == component)
					{
						NetworkGameManager.Instance.arrPlayerNetworkController[num] = null;
					}
				}
				Debug.Log("Despawn Duplicate Player");
				runner.Despawn(playerObject);
				UIGameManager.Instance?.RefreshPlayerCountText();
				SurvivorLobbyManager.Instance?.ShowBoard();
			}
			Vector3 spawnPosition2 = GameManager.Instance.MapManager.GetSpawnPosition(0, player.RawEncoded % runner.Config.Simulation.DefaultPlayers);
			NetworkObject networkObject3 = runner.Spawn(_playerPrefab, spawnPosition2, Quaternion.identity, player);
			runner.SetPlayerObject(player, networkObject3);
			NetworkGameManager.Instance.SpawnedCharacters.Add(player);
		}
		if (!flag2 && runner.Simulation.LocalPlayer == player)
		{
			if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
			{
				NetworkGameManager.Instance.sessionName = runner.SessionInfo.Name;
				if (!runner.Simulation.IsServer)
				{
					GlobalSaveData.instance.optionData.lastRoomCode = NetworkGameManager.Instance.sessionName;
					GlobalSaveData.instance.optionData.lastRegion = PhotonAppSettings.Instance.AppSettings.FixedRegion;
					GlobalSaveData.instance.SaveOptionData();
				}
				if (UIGameManager.Instance.sessionName != null)
				{
					if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
					{
						UIGameManager.Instance.sessionName.transform.parent.gameObject.SetActive(value: false);
						UIGameManager.Instance.sessionName.text = "";
					}
					else
					{
						UIGameManager.Instance.sessionName.text = "******";
					}
				}
			}
			GlobalDebugManager.Instance.ShowLog("Player Create room, ID = " + player.RawEncoded % runner.Config.Simulation.DefaultPlayers);
		}
		else
		{
			GlobalDebugManager.Instance.ShowLog("Other Player Joined, ID = " + player.RawEncoded % runner.Config.Simulation.DefaultPlayers);
		}
		UIGameManager.Instance.RefreshPlayerCountText();
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		NetworkObject networkObject = runner.GetPlayerObject(player);
		if (!(networkObject != null))
		{
			return;
		}
		PlayerController playerLeft = networkObject.GetComponent<PlayerController>();
		if (!(NetworkGameManager.Instance != null))
		{
			return;
		}
		playerLeft.Disconnected();
		string translation = LocalizationManager.GetTranslation("Menu/PlayerDisconnected");
		translation = translation.Replace("[n]", playerLeft.network.GetPlayerName());
		if (UIGameManager.Instance != null)
		{
			UIGameManager.Instance.ShowPlayerInfo(translation);
		}
		if ((bool)LobbyManager.Instance && LobbyManager.Instance.allReady)
		{
			GameManager.Instance.gameManagerPhoton.arrPlayerReady.Set(0, value: false);
			LobbyManager.Instance.timerCountDown.StopDuration();
			LobbyManager.Instance.allReady = false;
			UIGameManager.Instance.txtCountDown.gameObject.SetActive(value: false);
		}
		if (runner.Simulation.IsServer)
		{
			networkObject.AssignInputAuthority(PlayerRef.None);
			GameManager.Instance.gameManagerPhoton.arrPlayerReady.Set(playerLeft.network.GetIDX(), value: false);
			playerLeft.network.SetEnableControl(value: true);
			playerLeft.network.playerPhoton.disconnected = true;
			if (LobbyManager.Instance != null)
			{
				foreach (InventoryObject item in playerLeft.data.arrInventory)
				{
					if (item.Name != "Null" && item.ID == 55 && item.IdxInventory != 0)
					{
						playerLeft.inventoryManager.FunctionItemDrop(item.IdxInventory, isSwapWeapon: false);
					}
				}
			}
			else
			{
				for (int i = 0; i < playerLeft.data.arrInventory.Count; i++)
				{
					if (i != 0 && playerLeft.data.arrInventory[i] != null && playerLeft.data.arrInventory[i].Name != "Null" && UIResultManager.Instance == null)
					{
						playerLeft.inventoryManager.FunctionItemDrop(i, isSwapWeapon: false);
					}
				}
			}
		}
		Debug.Log("--cek ArrPlayerContoller Remove Player Left");
		NetworkGameManager.Instance.arrPlayerController.Remove(playerLeft);
		if (NetworkGameManager.Instance.IsAllPlayerDead() && !GameManager.Instance.IsCutscenePlaying)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				GameManagerPhoton.Instance.IsWin = false;
			}
			NetworkGameManager.Instance.ownPlayer.network.StartCoroutine(NetworkGameManager.Instance.ownPlayer.network.ShowResultScene());
		}
		UIGameManager.Instance.RefreshPlayerCountText();
		if (!string.IsNullOrWhiteSpace(playerLeft.data.SkillData.PerkId))
		{
			if (!playerLeft.network.playerPhoton.isQuitGame && runner.IsServer)
			{
				NetworkGameManager.Instance.arrPlayerIDDisconnected.Add(playerLeft.network.playerPhoton.PlayerDeviceID);
				NetworkGameManager.Instance.arrPlayerDisconnected.Add(playerLeft);
				PhotonMultiplayerManager.Instance.UpdateSessionDisconnectedPlayer();
			}
			if (runner.Simulation.IsServer)
			{
				playerLeft.network.playerPhoton.IsDisconnected = true;
				if (((bool)LobbyManager.Instance && !LobbyManager.Instance.allReady) || !GameManagerPhoton.Instance.HostLoadingGameComplete)
				{
					playerLeft.network.playerPhoton.IsDisconnectedOnLobby = true;
				}
				else
				{
					playerLeft.network.playerPhoton.IsDisconnectedOnLobby = false;
				}
			}
			UniTaskUtil.DelayedCall(playerLeft, 1f, () =>
			{
				if ((bool)playerLeft.network.playerPhoton.isQuitGame)
				{
					if (NetworkGameManager.Instance.isServer)
					{
						runner.Despawn(networkObject);
						NetworkGameManager.Instance.SpawnedCharacters.Remove(player);
					}
				}
				else if ((bool)GameManager.Instance)
				{
					int num = 90;
					if (LobbyManager.Instance == null || LobbyManager.Instance.allReady)
					{
						num = 7200;
					}
					playerLeft.DisconnectedTimer.StartDuration(num + 1);
					playerLeft.DespawnTimer.StartDuration(num);
				}
			}).Forget();
		}
		else
		{
			playerLeft.network.playerPhoton.isQuitGame = true;
			UniTaskUtil.DelayedCall(this, 1f, () =>
			{
				if (NetworkGameManager.Instance.isServer)
				{
					runner.Despawn(networkObject);
					NetworkGameManager.Instance.SpawnedCharacters.Remove(player);
				}
			}).Forget();
		}
		foreach (EnemyController item2 in GameManager.Instance.arrEnemyController)
		{
			if (item2.aiTarget.target == playerLeft.targetedPoint && item2.network.GetHealth() > 0f)
			{
				item2.movement.SetStateAfterPlayerDead();
			}
		}
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		if (NetworkGameManager.Instance.ownPlayer != null)
		{
			NetworkInputData value = NetworkGameManager.Instance.ownPlayer.InputNetworkPlayer();
			input.Set(value);
		}
	}

	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		bool num = shutdownReason == ShutdownReason.Error || shutdownReason == ShutdownReason.PhotonCloudTimeout;
		if (shutdownReason == ShutdownReason.Ok && (bool)LobbyManager.Instance && !GameManager.Instance.quitGame)
		{
			SavePlayerData();
		}
		if ((bool)VoiceChatGlobalController.Instance && VoiceChatGlobalController.Instance.gameObject != null)
		{
			UnityEngine.Object.Destroy(VoiceChatGlobalController.Instance.gameObject);
		}
		if ((bool)VoiceBroadcastController.Instance)
		{
			UnityEngine.Object.Destroy(VoiceBroadcastController.Instance.gameObject);
		}
		Debug.Log("PHOTON - SHUTDOWN =  " + shutdownReason);
		if (CameraGame.Instance != null)
		{
			CameraGame.Instance.mainCam.GetComponent<AudioListener>().enabled = true;
		}
		if (UITitleMenuManager.Instance == null && NetworkGameManager.Instance.photonNetworking != null)
		{
			UnityEngine.Object.Destroy(NetworkGameManager.Instance.photonNetworking.gameObject);
		}
		if (num)
		{
			SetActiveUiLoading(setActive: true);
			if ((bool)UIGameManager.Instance)
			{
				UIGameManager.Instance.ShowFailedConnect("ErrorConnection");
				if (!NetworkGameManager.Instance.isServer)
				{
					NetworkGameManager.Instance.ShowButtonReconnect = true;
				}
			}
		}
		else if (UITitleMenuManager.Instance == null && (LobbyManager.Instance == null || shutdownReason == ShutdownReason.DisconnectedByPluginLogic || shutdownReason == ShutdownReason.Ok) && ((GameManager.Instance != null && !GameManager.Instance.quitGame) || GameManager.Instance == null) && SceneManager.GetActiveScene().name.IndexOf("SocialMedia") < 0)
		{
			if (_isDisconnectedFromServer)
			{
				if (!UIFinalResultManager.Instance)
				{
					GlobalSaveData.instance.optionData.lastRoomCode = "";
					if ((bool)GameManagerPhoton.Instance && GameManagerPhoton.Instance.Life > 0)
					{
						UIGameManager.Instance.ShowFailedConnect("ErrorDisconnectedFromServer");
					}
					_isDisconnectedFromServer = false;
					NetworkGameManager.Instance.ShowButtonReconnect = false;
				}
			}
			else if (!GameManager.Instance.isKicked)
			{
				NetworkGameManager.Instance.ShowButtonReconnect = false;
				if (!UIFinalResultManager.Instance)
				{
					GlobalUIManager.Instance.ClickGoToScene("MainMenu");
				}
			}
		}
		if (GameManager.Instance != null && !GameManager.Instance.quitGame)
		{
			GameManager.Instance.quitGame = true;
		}
		SetActiveUiLoading(setActive: true);
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
	}

	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		Debug.Log("Disconnected From Server");
		if (!NetworkGameManager.Instance.isServer && runner.IsCloudReady)
		{
			_isDisconnectedFromServer = true;
			NetworkGameManager.Instance.Shutdown();
		}
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		Debug.Log("Connect Failed: " + reason);
	}

	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
		NetworkGameManager.Instance.sessionList.Clear();
		Debug.Log("Session Updated Total Session = " + sessionList.Count);
		foreach (SessionInfo session in sessionList)
		{
			NetworkGameManager.Instance.sessionList.Add(session);
		}
		if ((bool)UITitleMenuManager.Instance)
		{
			foreach (SessionInfo session2 in NetworkGameManager.Instance.sessionList)
			{
				if (GlobalSaveData.instance.optionData.lastRoomCode == session2.Name)
				{
					if (session2.Properties["PlayersDisconnect"].ToString().IndexOf(SystemInfo.deviceUniqueIdentifier, StringComparison.Ordinal) < 0)
					{
						UITitleMenuManager.Instance.btnReconnect.gameObject.SetActive(value: false);
						Navigation navigation = UITitleMenuManager.Instance.btnSolo.navigation;
						navigation.selectOnUp = UITitleMenuManager.Instance.btnPatchNote;
						UITitleMenuManager.Instance.btnSolo.navigation = navigation;
						Navigation navigation2 = UITitleMenuManager.Instance.btnPatchNote.navigation;
						navigation2.selectOnDown = UITitleMenuManager.Instance.btnSolo;
						UITitleMenuManager.Instance.btnPatchNote.navigation = navigation2;
					}
					else
					{
						UITitleMenuManager.Instance.btnReconnect.gameObject.SetActive(value: true);
						Navigation navigation3 = UITitleMenuManager.Instance.btnSolo.navigation;
						navigation3.selectOnUp = UITitleMenuManager.Instance.btnReconnect;
						UITitleMenuManager.Instance.btnSolo.navigation = navigation3;
						Navigation navigation4 = UITitleMenuManager.Instance.btnPatchNote.navigation;
						navigation4.selectOnDown = UITitleMenuManager.Instance.btnReconnect;
						UITitleMenuManager.Instance.btnPatchNote.navigation = navigation4;
					}
				}
			}
			if (!UITitleMenuManager.Instance.btnReconnect.gameObject.activeSelf)
			{
				UITitleMenuManager.Instance.btnReconnect.gameObject.SetActive(value: false);
				Navigation navigation5 = UITitleMenuManager.Instance.btnSolo.navigation;
				navigation5.selectOnUp = UITitleMenuManager.Instance.btnPatchNote;
				UITitleMenuManager.Instance.btnSolo.navigation = navigation5;
				Navigation navigation6 = UITitleMenuManager.Instance.btnPatchNote.navigation;
				navigation6.selectOnDown = UITitleMenuManager.Instance.btnSolo;
				UITitleMenuManager.Instance.btnPatchNote.navigation = navigation6;
			}
		}
		PhotonMultiplayerManager.Instance._ListRoomUpdated = true;
	}

	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
	}

	public void OnSceneLoadDone(NetworkRunner runner)
	{
		if (runner.Simulation.IsServer)
		{
			GameManager.Instance.SpawnPhotonGameManager();
		}
		PhotonMultiplayerManager.Instance.sceneLoaded = true;
	}

	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		throw new NotImplementedException();
	}

	private void SetActiveUiLoading(bool setActive)
	{
		if (UIGameManager.Instance != null)
		{
			UIGameManager.Instance.loading?.loadingUI?.SetActive(setActive);
		}
	}

	private void SavePlayerData()
	{
		GameManagerPhoton instance = GameManagerPhoton.Instance;
		if ((bool)instance)
		{
			try
			{
				instance.Save();
			}
			catch (Exception value)
			{
				Save();
				Console.WriteLine(value);
				throw;
			}
			Debug.Log("Save Item Lobby");
		}
		else
		{
			Save();
			Debug.Log("Save biasa Lobby");
		}
		static void Save()
		{
			GlobalSaveData.instance.SavePlayerDataGameData(NetworkGameManager.Instance.ownPlayer);
		}
	}
}
