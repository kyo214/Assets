using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using _Modules.GameSystem.BaseScripts.Difficulty;

public class GameModes : MonoBehaviour
{
	public bool friendlyFire;

	public bool isBriefingShowed;

	public bool weaponInBackpack;

	[SerializeField]
	private bool disableMetaProgression = true;

	[SerializeField]
	private bool disableSave;

	public float friendlyFireDmgMultiply;

	public int chancePercentDropAmmo;

	public int chancePercentDropScraps;

	public int chancePercentDropGunPowder;

	public int chancePercentDropChemical;

	public string modeGame;

	public bool randomizeMeleeHero;

	public bool isGrenadeFriendlyFire;

	public bool isEvent;

	public bool isInitDemo;

	public bool isDemo;

	public bool isDebug;

	public bool isShowingDisclaimer;

	public bool isItemBoxGlobal;

	public bool IsRandomRotateCam;

	public bool HaveBattleRoyale;

	[SerializeField]
	private string _scenarioId;

	[FormerlySerializedAs("WaveMultiplier")]
	public float WaveMultiplierByPlayer;

	public const int MAX_INVENTORY_SIZE = 12;

	[SerializeField]
	private DifficultySetting _difficultySetting;

	public List<SO_GameModifier> ListGameModifierEffect = new List<SO_GameModifier>();

	public static GameModes Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		Instance = this;
		isInitDemo = isDemo;
	}

	private void Start()
	{
		InitBuildType();
		Init();
	}

	public void ApplyModifier()
	{
		foreach (SO_GameModifier item in ListGameModifierEffect)
		{
			item.Apply();
		}
	}

	private void InitBuildType()
	{
		isDemo = (isInitDemo = true);
		disableSave = true;
	}

	public void Init()
	{
		if (modeGame == "PVP")
		{
			friendlyFire = true;
		}
		if (friendlyFire)
		{
			isGrenadeFriendlyFire = true;
		}
		friendlyFire = BGDatabase_GameConfig.GetEntityByKeyid(Instance.modeGame).FriendlyFire;
		friendlyFireDmgMultiply = BGDatabase_GameConfig.GetEntityByKeyid(Instance.modeGame).FriendlyFireDmgMultiply;
		chancePercentDropAmmo = BGDatabase_GameConfig.GetEntityByKeyid(Instance.modeGame).ChancePercentDropAmmo;
		weaponInBackpack = BGDatabase_GameConfig.GetEntityByKeyid(Instance.modeGame).WeaponInBackpack;
	}

	public IEnumerator InitGameModeSettings()
	{
		while (GameManagerPhoton.Instance == null || NetworkGameManager.Instance == null)
		{
			yield return null;
		}
		if ((bool)GameManagerPhoton.Instance)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				SetDifficultyNetwork(_difficultySetting.GetDifficultyData().DifficultySetting);
				GameManagerPhoton.Instance.ScenarioId = _scenarioId;
			}
			else
			{
				SetDifficulty((DifficultySetting.Difficulty)GameManagerPhoton.Instance.Difficulty);
				SetScenarioId(GameManagerPhoton.Instance.ScenarioId);
			}
		}
	}

	public bool CheckDisableMetaProgression()
	{
		if (!isEvent)
		{
			return disableMetaProgression;
		}
		return true;
	}

	public bool CheckDisableSaveData()
	{
		if (!isEvent)
		{
			return disableSave;
		}
		return true;
	}

	public void SetScenarioId(string scenarioId)
	{
		_scenarioId = scenarioId;
	}

	public string GetScenarioId()
	{
		return _scenarioId;
	}

	public DifficultyData GetDifficultyData()
	{
		return _difficultySetting?.GetDifficultyData() ?? new DifficultyData();
	}

	public void SetDifficulty(DifficultySetting.Difficulty difficulty)
	{
		_difficultySetting?.SetDifficulty(difficulty);
	}

	public void SetDifficultyNetwork(DifficultySetting.Difficulty difficulty)
	{
		_difficultySetting?.SetDifficultyNetwork(difficulty);
	}

	public void SetGameModeSetting(GameData gameData)
	{
		SetDifficulty((DifficultySetting.Difficulty)gameData.Difficulty);
		SetScenarioId(gameData.ScenarioId);
	}
}
