using I2.Loc;
using TMPro;
using Toked.FunctionComponent;
using Toked.Skill;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using _Modules.CharacterSkin;
using _Modules.CharacterSkin.Scripts;
using _Modules.Data.Scripts;
using _Modules.GameSystem.BaseScripts.Difficulty;
using _Modules.GameSystem.BaseScripts.Scenario;

namespace _Modules.UITitle.Scripts;

public class LoadGameDescriptionPanelUI : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _scenarioDifficultyText;

	[SerializeField]
	private Image _characterHeadImage;

	[SerializeField]
	private Image _characterBodyImage;

	[SerializeField]
	private Image _perkImage;

	[SerializeField]
	private TMP_Text _perkText;

	[SerializeField]
	private Localize _perkLocalize;

	[SerializeField]
	private TMP_Text _healthText;

	[SerializeField]
	private TMP_Text _staminaText;

	[SerializeField]
	private TMP_Text _mapClearedText;

	[SerializeField]
	private Image[] _inventoryImage;

	[SerializeField]
	private Image _meleeWeaponImage;

	[SerializeField]
	private Image _rangeWeaponImage;

	[SerializeField]
	private LongClickButton _longClickButton;

	[SerializeField]
	private Image _gameVersionCompatibilityImage;

	[SerializeField]
	private TMP_Text _gameVersionText;

	[SerializeField]
	private TMP_Text _gameClearedText;

	[SerializeField]
	private TMP_Text _gameOverText;

	[SerializeField]
	private Sprite _defaultNonePerkSprite;

	[SerializeField]
	private Sprite _defaultInventoryCloseSlot;

	public bool Hovered { get; private set; }

	public void SetActive(bool active)
	{
		SetHovered(hovered: false);
		base.gameObject.SetActive(active);
	}

	public void InitDeleteButton(UnityAction action)
	{
		_longClickButton.onLongClick.AddListener(action);
	}

	public void Init(GameData gameData)
	{
		if (gameData != null)
		{
			SetGameStatusText(gameData);
			SetCharacterProfile(gameData);
			SetPlayerStats(gameData);
			SetPerk(gameData);
			SetMapClear(gameData);
			SetInventory(gameData);
			SetScenarioDifficultyText(gameData);
		}
	}

	private void SetGameStatusText(GameData gameData)
	{
		if (gameData.ResetData)
		{
			_gameOverText.gameObject.SetActive(value: true);
			_gameClearedText.gameObject.SetActive(value: false);
		}
		else if (gameData.IsCompleted)
		{
			_gameOverText.gameObject.SetActive(value: false);
			_gameClearedText.gameObject.SetActive(value: true);
		}
		else
		{
			_gameOverText.gameObject.SetActive(value: false);
			_gameClearedText.gameObject.SetActive(value: false);
		}
	}

	private void SetCharacterProfile(GameData gameData)
	{
		_characterHeadImage.sprite = PlayerSkinData.GetHeadSkinAvatar(gameData.PlayerSaveData.HeadSkinId, gameData.PlayerSaveData.MaterialSkinId, 0);
		_characterBodyImage.sprite = PlayerSkinData.GetBodySkinAvatarSo((CharacterSkinData.Gender)gameData.PlayerSaveData.GenderSkinId, gameData.PlayerSaveData.BodySkinId, gameData.PlayerSaveData.MaterialSkinId, 0)?.AvatarSprite;
	}

	private void SetPlayerStats(GameData gameData)
	{
		_healthText.text = $"{gameData.PlayerSaveData.MaxHealth}/{gameData.PlayerSaveData.MaxHealth}";
		_staminaText.text = $"{gameData.PlayerSaveData.MaxStamina}/{gameData.PlayerSaveData.MaxStamina}";
	}

	private void SetPerk(GameData gameData)
	{
		string perkId = gameData.PlayerSaveData.PerkId;
		if (string.IsNullOrWhiteSpace(perkId))
		{
			_perkLocalize.SetTerm("");
			_perkText.text = "-----";
			_perkImage.sprite = _defaultNonePerkSprite;
			_perkText.gameObject.SetActive(value: true);
			_perkImage.gameObject.SetActive(_perkImage.sprite != null);
		}
		else
		{
			SkillScriptableObject data = DataManager.Instance.Get<PerkLibraryScriptableObject>().GetData(perkId);
			_perkText.text = "";
			_perkLocalize.SetTerm(data.SkillNameLocalizeId);
			_perkImage.sprite = data.SkillSprite;
			_perkText.gameObject.SetActive(value: true);
			_perkImage.gameObject.SetActive(value: true);
		}
	}

	private void SetMapClear(GameData gameData)
	{
		_mapClearedText.text = $"{gameData.GetTotalMissionsCleared()}/{gameData.MaxMission}";
	}

	private void SetInventory(GameData gameData)
	{
		PlayerSaveData playerSaveData = gameData.PlayerSaveData;
		Sprite itemSprite = DataManager.Instance.GetItemSprite(playerSaveData.MeleeWeapon.ToString());
		_meleeWeaponImage.sprite = itemSprite;
		_meleeWeaponImage.enabled = itemSprite;
		Sprite itemSprite2 = DataManager.Instance.GetItemSprite(playerSaveData.RangeWeapon.ToString());
		_rangeWeaponImage.sprite = itemSprite2;
		_rangeWeaponImage.enabled = itemSprite2;
		int num = playerSaveData.MaxInventory - 2;
		for (int i = 0; i < _inventoryImage.Length; i++)
		{
			Image image = _inventoryImage[i];
			InventoryObject inventoryData = playerSaveData.GetInventoryData(i + 2);
			if (inventoryData == null)
			{
				image.enabled = false;
			}
			else if (i < num)
			{
				Sprite sprite = (image.sprite = DataManager.Instance.GetItemSprite(inventoryData.ID.ToString()));
				image.enabled = sprite;
			}
			else
			{
				image.sprite = _defaultInventoryCloseSlot;
				image.enabled = true;
			}
		}
	}

	private void SetScenarioDifficultyText(GameData gameData)
	{
		ScenarioScriptableObject obj = (string.IsNullOrWhiteSpace(gameData.ScenarioId) ? DataManager.Instance.Get<ScenarioScriptableObjectLibrary>()?.GetDataByIndex(0) : DataManager.Instance.Get<ScenarioScriptableObjectLibrary>()?.GetData(gameData.ScenarioId));
		DifficultyScriptableObject difficultyScriptableObject = DataManager.Instance.Get<DifficultyScriptableObjectLibrary>()?.GetData((DifficultySetting.Difficulty)gameData.Difficulty);
		string text = LocalizationManager.GetTranslation(obj?.ScenarioNameLocalization) ?? gameData.ScenarioId;
		string text2 = LocalizationManager.GetTranslation(difficultyScriptableObject?.DifficultyLocalization) ?? ((DifficultySetting.Difficulty)gameData.Difficulty/*cast due to constrained. prefix*/).ToString();
		_scenarioDifficultyText.text = text + " | <color=red>" + text2 + "</color>";
		if ((bool)_gameVersionText)
		{
			_gameVersionText.text = "Ver. " + gameData.GameVersion;
		}
		if ((bool)_gameVersionCompatibilityImage)
		{
			_gameVersionCompatibilityImage.gameObject.SetActive(!gameData.CheckVersionCompability());
		}
	}

	public void SetHovered(bool hovered)
	{
		Hovered = hovered;
	}
}
