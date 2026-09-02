using System.Collections.Generic;
using System.Linq;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using _Modules.Steam.Scripts;

public class SteamFriendView : MonoBehaviour
{
	[Header("Game Object Reference")]
	[SerializeField]
	private SteamFriendContentUI _steamFriendContentUIPrefab;

	[Header("Child Reference")]
	[SerializeField]
	private Transform _groupFriend;

	[SerializeField]
	private Transform _groupList;

	[Header("Data Setup")]
	[SerializeField]
	private int showItems;

	[SerializeField]
	private RectTransform _scrollViewContent;

	[SerializeField]
	private SteamLobby _steamLobby;

	private List<Friend> _friendListOnline;

	public bool IsShow;

	private void Start()
	{
		if ((object)_scrollViewContent == null)
		{
			_scrollViewContent = _groupList.GetComponent<RectTransform>();
		}
		if ((object)_steamLobby == null)
		{
			_steamLobby = base.transform.parent.GetComponent<SteamLobby>();
		}
		Hide();
	}

	public void Show()
	{
		if (SteamManager.Initialized)
		{
			Debug.Log("<color=#acd550>[Steam Lobby]</color> Checking active lobby");
			if (!SteamManager.JoinedLobby)
			{
				Debug.Log("<color=#acd550>[Steam Lobby]</color> Parsing lobby data " + GameManagerPhoton.Instance.LobbyId);
				SteamId id = new SteamId
				{
					Value = GameManagerPhoton.Instance.LobbyId
				};
				SteamManager.ActiveLobby = new Lobby(id);
				if (SteamManager.JoinedLobby)
				{
					Debug.Log("<color=#acd550>[Steam Lobby]</color> Active Lobby forwarding success " + SteamManager.ActiveLobby.Id.ToString());
				}
			}
			else
			{
				Debug.Log("<color=#acd550>[Steam Lobby]</color> " + SteamManager.ActiveLobby.Id.ToString());
			}
		}
		_groupFriend.gameObject.SetActive(value: true);
		RefreshFriendList();
		IsShow = true;
	}

	public void Hide()
	{
		_groupFriend.gameObject.SetActive(value: false);
		IsShow = false;
	}

	public void RefreshFriendList()
	{
		ClearListView();
		_friendListOnline = new List<Friend>();
		_friendListOnline = _steamLobby.FetchOnlineFriends();
		int num = _friendListOnline.Count();
		if (num > 0)
		{
			InstantiateFriendView(_friendListOnline, showItems, 0);
		}
		else
		{
			Debug.Log("List Empty");
		}
		_scrollViewContent.sizeDelta = new Vector2(_scrollViewContent.sizeDelta.x, 50 * num);
	}

	private void InstantiateFriendView(List<Friend> friendList, int show, int page)
	{
		int num = 0;
		int num2 = friendList.Count();
		if (show > 0)
		{
			if (page > 0)
			{
				ClearListView();
			}
			if (num2 > show)
			{
				num2 = show;
			}
			num = page * num2;
			if (num >= show)
			{
				num = 0;
			}
		}
		for (int i = num; i < num + num2; i++)
		{
			SteamFriendContentUI steamFriendContentUI = Object.Instantiate(_steamFriendContentUIPrefab, _groupList.transform);
			Friend friend = friendList[i];
			steamFriendContentUI.Init(friend, () =>
			{
				TriggerInvite(friend.Id);
			});
		}
	}

	private void ClearListView()
	{
		if (_groupList.childCount <= 0)
		{
			return;
		}
		foreach (Transform group in _groupList)
		{
			Object.Destroy(group.gameObject);
		}
	}

	public void TriggerInvite(SteamId friendId)
	{
		Debug.Log("Begin Invite Friend ID " + friendId.ToString());
		_steamLobby.InviteFriend(friendId);
	}

	public void ShowFriendList(bool isShow)
	{
		_groupFriend.gameObject.SetActive(isShow);
		if (isShow)
		{
			RefreshFriendList();
		}
	}
}
