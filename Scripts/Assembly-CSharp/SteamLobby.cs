using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{
	public bool InitComplete;

	public SteamId AwaitingInviteId;

	public string HostRoomCode;

	[Header("Create Lobby")]
	[SerializeField]
	private int _maxMembers;

	private List<Friend> _friendList;

	private const string KEY_ROOM_CODE = "roomCode";

	public event Action<string> ActInviteNoticeShow;

	public event Action ActInviteNoticeHide;

	private void Start()
	{
		InitStartSequence();
	}

	private void InitStartSequence()
	{
		if (SceneManager.GetActiveScene().name == "Lobby")
		{
			StartCoroutine(WaitForServer());
			StartCoroutine(WaitServerTimeOut());
		}
		else if (SceneManager.GetActiveScene().name == "MainMenu")
		{
			LeaveLobby();
			Debug.Log("<color=#acd550>[SteamWorks]</color> Leaving Lobby ...");
		}
	}

	private void OnEnable()
	{
		SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
		SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
		SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
		SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
		SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
		SteamInviteNotice.ActInviteResponse += SteamInviteNotice_ActInviteResponse;
		InitComplete = true;
		Debug.Log("Steam Lobby Initialized!");
	}

	private void OnDisable()
	{
		SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
		SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
		SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
		SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
		SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
		SteamInviteNotice.ActInviteResponse -= SteamInviteNotice_ActInviteResponse;
	}

	private IEnumerator WaitForServer()
	{
		Debug.Log("Wait For Server");
		yield return new WaitUntil(() => NetworkGameManager.Instance.isServer);
		if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
		{
			CreateLobby(NetworkGameManager.Instance.sessionName);
		}
	}

	private IEnumerator WaitServerTimeOut()
	{
		yield return new WaitForSeconds(30f);
		StopCoroutine(WaitForServer());
	}

	private void SteamInviteNotice_ActInviteResponse(bool isAccept)
	{
		Debug.Log("Invite Feedback Received");
		if (isAccept)
		{
			JoinLobby();
		}
		else
		{
			RejectAwaitingInvite();
		}
		ActInviteNoticeHide();
	}

	public void CreateLobby(string roomCode)
	{
		HostRoomCode = roomCode;
		Debug.Log("<color=#acd550>[SteamWorks]</color> Creating Lobby with Room Code : " + HostRoomCode);
		SteamMatchmaking.CreateLobbyAsync(_maxMembers);
	}

	public void InviteFriend(SteamId friendId)
	{
		Debug.Log(SteamManager.ActiveLobby.Id.ToString());
		SteamManager.ActiveLobby.InviteFriend(friendId);
		Debug.Log("<color=#acd550>[SteamWorks]</color> Friend Invitation Sent from : " + SteamManager.ActiveLobby.Id.ToString());
	}

	public List<Friend> FetchOnlineFriends()
	{
		IEnumerable<Friend> friends = SteamFriends.GetFriends();
		_friendList = new List<Friend>();
		foreach (Friend item in friends)
		{
			if (item.IsOnline && item.IsPlayingThisGame)
			{
				_friendList.Add(item);
			}
		}
		foreach (Friend item2 in friends)
		{
			if (item2.IsOnline && !item2.IsPlayingThisGame)
			{
				_friendList.Add(item2);
			}
		}
		return _friendList;
	}

	public void JoinLobby()
	{
		if ((ulong)AwaitingInviteId != 0L)
		{
			SteamId awaitingInviteId = AwaitingInviteId;
			Debug.Log("<color=#acd550>[SteamWorks]</color> Awaiting Invite ID : " + awaitingInviteId.ToString());
			SteamMatchmaking.JoinLobbyAsync(AwaitingInviteId);
		}
		else
		{
			Debug.Log("<color=#acd550>[SteamWorks]</color> No Invite ID Provided");
		}
	}

	public void RejectAwaitingInvite()
	{
		AwaitingInviteId = 0uL;
	}

	public void LeaveLobby()
	{
		if (SteamManager.Initialized)
		{
			Debug.Log("<color=#acd550>[SteamWorks]</color> Leaving Lobby");
			SteamManager.ActiveLobby.Leave();
			SteamManager.JoinedLobby = false;
		}
		AwaitingInviteId = 0uL;
	}

	private void SteamMatchmaking_OnLobbyCreated(Result arg1, Lobby lobbyData)
	{
		lobbyData.SetData("roomCode", HostRoomCode);
		Debug.Log(HostRoomCode);
		Debug.Log("<color=#acd550>[SteamWorks]</color> Lobby " + lobbyData.Id.ToString() + " created!");
	}

	private void SteamMatchmaking_OnLobbyInvite(Friend friendData, Lobby lobbyData)
	{
		Debug.Log("<color=#acd550>[SteamWorks]</color> You are invited by " + friendData.Name + " to lobby id : " + lobbyData.Id.ToString());
		ActInviteNoticeShow(friendData.Name);
		AwaitingInviteId = lobbyData.Id;
		SteamId awaitingInviteId = AwaitingInviteId;
		Debug.Log("Awaiting Invite ID : " + awaitingInviteId.ToString());
	}

	private void SteamMatchmaking_OnLobbyMemberLeave(Lobby lobbyData, Friend friendData)
	{
		Debug.Log("<color=#acd550>[SteamWorks]</color> " + friendData.Name + " left lobby.");
	}

	private void SteamMatchmaking_OnLobbyMemberJoined(Lobby lobbyData, Friend friendData)
	{
		Debug.Log("<color=#acd550>[SteamWorks]</color> " + friendData.Name + " joined lobby.");
		AwaitingInviteId = 0uL;
	}

	private void SteamMatchmaking_OnLobbyEntered(Lobby lobbyData)
	{
		if (!SteamManager.JoinedLobby)
		{
			SteamManager.ActiveLobby = lobbyData;
			SteamManager.JoinedLobby = true;
			HostRoomCode = lobbyData.GetData("roomCode");
			if (lobbyData.IsOwnedBy(SteamClient.SteamId))
			{
				GameManagerPhoton.Instance.LobbyId = lobbyData.Id.Value;
			}
			else
			{
				UITitleMenuManager.Instance.ClickJoinRoom(HostRoomCode);
			}
		}
	}
}
