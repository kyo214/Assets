using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using UnityEngine.UI;

public class SteamInvitePause : MonoBehaviour
{
	[SerializeField]
	private SteamFriendView _steamFriendView;

	[SerializeField]
	private Transform _btnInvite;

	[SerializeField]
	private UIButton _btnResume;

	[SerializeField]
	private UIButton _btnOptions;

	private void Start()
	{
		_btnInvite.gameObject.SetActive(value: false);
		if ((bool)UIGameManager.Instance)
		{
			Navigation navigation = _btnResume.navigation;
			navigation.selectOnDown = _btnOptions;
			_btnResume.navigation = navigation;
			Navigation navigation2 = _btnOptions.navigation;
			navigation2.selectOnUp = _btnResume;
			_btnOptions.navigation = navigation2;
		}
	}

	public void ShowFriendView()
	{
		_steamFriendView?.Show();
	}
}
