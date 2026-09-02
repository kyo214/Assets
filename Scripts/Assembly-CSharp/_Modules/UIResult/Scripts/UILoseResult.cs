using System.Collections;
using DG.Tweening;
using Fusion;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.UI;
using _Modules.Achievement.Scripts;
using _Modules.UIInGame.Scripts;
using _Modules.UILobby.Scripts;

namespace _Modules.UIResult.Scripts;

public class UILoseResult : MonoBehaviour
{
	[SerializeField]
	private GameObject _container;

	[SerializeField]
	private TMP_Text _gameOverText;

	[SerializeField]
	private TMP_Text _isThisEnd;

	[SerializeField]
	private PentagramLamp[] _pentagramLamps;

	[SerializeField]
	private GameObject[] _explosionParticle;

	[SerializeField]
	private LoadingInGameUIController _loseResultUIController;

	[SerializeField]
	private PentagramLampController _pentagramLampController;

	[SerializeField]
	private Image _fadeBlack;

	private bool _isGameOver;

	private int _life = 2;

	public PentagramLampController PentagramLampController => _pentagramLampController;

	public void Init(int life)
	{
		_loseResultUIController.Init();
		_pentagramLampController?.Init(life);
		_gameOverText.gameObject.SetActive(value: false);
		_isThisEnd.gameObject.SetActive(value: false);
		SetLifeImage(life + 1);
		_life = life;
		_isGameOver = life <= 0;
	}

	public IEnumerator Show(bool isShowFinalResult = false)
	{
		InputManager.DisableInput();
		_container.SetActive(value: true);
		_loseResultUIController.TriggerAnimation(_life);
		yield return new WaitForSeconds(1f);
		if (!UIResultManager.Instance._resultMission.IsBoss && !isShowFinalResult)
		{
			yield return AnimateLifImage(_life);
		}
		int delay = 3;
		if (_isGameOver)
		{
			delay = 4;
			DOTween.To(() => CameraGame.Instance.colorA.saturation.value, (float x) =>
			{
				CameraGame.Instance.colorA.saturation.value = x;
			}, -100f, 1f).OnComplete(() =>
			{
				if (UIResultManager.Instance._resultMission.IsBoss)
				{
					_isThisEnd.gameObject.SetActive(value: true);
					GameStatistic.AddCompletedGame();
				}
				else
				{
					_gameOverText.gameObject.SetActive(value: true);
					GameStatistic.AddGameOver();
				}
			});
		}
		else
		{
			_gameOverText.gameObject.SetActive(_isGameOver);
		}
		if (isShowFinalResult)
		{
			delay = 1;
		}
		if (_isGameOver && NetworkGameManager.Instance.arrPlayerController.Count > 1)
		{
			_fadeBlack.enabled = true;
			_fadeBlack.DOFade(1f, 1f);
			yield return new WaitForSeconds(2f);
		}
		yield return new WaitForSeconds(delay);
		if (!_isGameOver && !isShowFinalResult)
		{
			_container.SetActive(value: false);
			InputManager.EnableInput();
		}
		else if (NetworkGameManager.Instance.arrPlayerController.Count > 1 && GameModes.Instance.HaveBattleRoyale)
		{
			InputManager.EnableInput();
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
			{
				if ((bool)item)
				{
					item.network.charControllerPhoton.enabled = false;
				}
			}
			if (NetworkGameManager.Instance.isServer)
			{
				foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
				{
					item2.network.SetHealth(NetworkGameManager.Instance.ownPlayer.data.GetMaxHealth());
				}
				DOVirtual.DelayedCall(2f, () =>
				{
					GameManagerPhoton.Instance.showResult = false;
				});
			}
			GameManagerPhoton.Instance.CurrentMission = GlobalMissionManager.Instance.BattleRoyaleMissionSO;
			PhotonMultiplayerManager.Instance._runner.SetActiveScene("InGame9-0");
			PhotonMultiplayerManager.Instance.activeIngameScene = "InGame9-0";
		}
		else
		{
			UIResultManager.Instance.ResetFadeBlack();
			LobbyManager.Instance.UIResult.SetActive(value: true);
			LobbyManager.Instance.UIResult.GetComponent<Canvas>().enabled = true;
			UIResultManager.Instance.StartCoroutine(UIResultManager.Instance.FinalResult());
		}
	}

	private void SetLifeImage(int life)
	{
		if ((bool)UIResultManager.Instance && (bool)UIResultManager.Instance._resultMission && UIResultManager.Instance._resultMission.IsBoss)
		{
			for (int num = _pentagramLamps.Length - 1; num >= 0; num--)
			{
				_pentagramLamps[num].SetActive(setActive: false);
			}
			return;
		}
		for (int num2 = _pentagramLamps.Length - 1; num2 >= 0; num2--)
		{
			if ((bool)_pentagramLamps[num2])
			{
				_pentagramLamps[num2].SetActive(num2 < life);
			}
		}
	}

	private IEnumerator AnimateLifImage(int index)
	{
		if (index < _pentagramLamps.Length)
		{
			if (index < _pentagramLamps.Length)
			{
				_pentagramLamps[index].SetActive(setActive: false);
				_explosionParticle[index].SetActive(value: true);
			}
			AudioManager.PlaySFX("grenade");
			AudioManager.PlaySFX("molotov");
			yield return new WaitForSeconds(1.2f);
		}
	}

	private void Debug_Init(int life)
	{
		Init(life);
	}

	private void Debug_Show()
	{
		StartCoroutine(Show());
	}
}
