using System.Collections;
using I2.Loc;
using TMPro;
using UnityEngine;

public class UIRegionDifficulty : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _textRegion;

	[SerializeField]
	private TextMeshProUGUI _textDifficulty;

	public static UIRegionDifficulty Instance;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private IEnumerator Start()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return new WaitForSeconds(0.1f);
		}
		Translate();
	}

	public void Translate()
	{
		if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
		{
			if (PhotonMultiplayerManager.Instance._runner.SessionInfo.Properties.TryGetValue("RoomType", out var value))
			{
				_textRegion.text = LocalizationManager.GetTranslation("Menu/" + PhotonMultiplayerManager.Instance._runner.SessionInfo.Region);
				_textRegion.text = _textRegion.text + " (" + LocalizationManager.GetTranslation("Menu/" + value.PropertyValue) + ")";
			}
			if (PhotonMultiplayerManager.Instance._runner.SessionInfo.Properties.TryGetValue("Difficulty", out var value2))
			{
				_textDifficulty.text = LocalizationManager.GetTranslation("Scenario/Scenario0").ToUpper();
				_textDifficulty.text = _textDifficulty.text + " | <color=red>" + LocalizationManager.GetTranslation("Difficulty/Difficulty" + value2.PropertyValue) + "</color>";
			}
		}
		else
		{
			_textRegion.text = "";
			_textDifficulty.text = "";
		}
	}
}
