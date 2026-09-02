using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class AFKSystem : MonoBehaviour
{
	[SerializeField]
	private int idleTimeSetting = 60;

	[SerializeField]
	private int idleExitTimeSetting = 10;

	[SerializeField]
	private float lastIdleTime;

	[SerializeField]
	private int timerCountdown;

	[SerializeField]
	private float lastNotifTime;

	[SerializeField]
	private int timerCountdownExit;

	[SerializeField]
	private bool AFK;

	[SerializeField]
	private GameObject UINotif;

	[SerializeField]
	private TextMeshProUGUI txtCountdown;

	[SerializeField]
	private bool _quitGame;

	private void Awake()
	{
		if (GameModes.Instance != null && !GameModes.Instance.isEvent)
		{
			Object.Destroy(base.gameObject);
		}
		lastIdleTime = Time.time;
		UINotif.SetActive(value: false);
	}

	private void FixedUpdate()
	{
		InputSystem.onAnyButtonPress.CallOnce((InputControl ctrl) =>
		{
			lastIdleTime = Time.time;
			AFK = false;
			if (UINotif != null)
			{
				UINotif.SetActive(value: false);
			}
		});
		IdleCheck();
	}

	public void IdleCheck()
	{
		if (!AFK)
		{
			timerCountdown = idleTimeSetting - Mathf.CeilToInt(Time.time - lastIdleTime);
			if (Time.time - lastIdleTime > (float)idleTimeSetting)
			{
				AFK = true;
				lastNotifTime = Time.time;
				UINotif.SetActive(value: true);
			}
			return;
		}
		timerCountdownExit = idleExitTimeSetting - Mathf.CeilToInt(Time.time - lastNotifTime);
		if (timerCountdownExit >= 0)
		{
			txtCountdown.text = timerCountdownExit.ToString();
		}
		if (!(Time.time - lastNotifTime > (float)idleExitTimeSetting) || _quitGame)
		{
			return;
		}
		if (GameManager.Instance != null && !GameManager.Instance.quitGame)
		{
			_quitGame = true;
			if (UINotif != null)
			{
				UINotif.SetActive(value: false);
			}
			GameManager.Instance.quitGame = true;
			NetworkGameManager.Instance.Shutdown();
			UIGameManager.Instance.StartCoroutine(UIGameManager.Instance.DelayQuitGame());
			UIGameManager.Instance.loading.loadingUI.SetActive(value: true);
		}
		else if (GameManager.Instance == null)
		{
			_quitGame = true;
			if (UINotif != null)
			{
				UINotif.SetActive(value: false);
			}
			NetworkGameManager.Instance.Shutdown();
			if (NetworkGameManager.Instance.photonNetworking != null)
			{
				Object.Destroy(NetworkGameManager.Instance.photonNetworking.gameObject);
			}
			GlobalSaveData.instance.optionData.lastSeed = 0;
			GlobalSaveData.instance.SaveOptionData();
			GlobalUIManager.Instance.ClickGoToScene("MainMenu");
		}
	}
}
