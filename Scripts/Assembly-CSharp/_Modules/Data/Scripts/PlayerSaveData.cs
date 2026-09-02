using System;
using System.Collections.Generic;
using Toked.Inventory;
using Toked.Skill;
using UnityEngine;
using _Modules.CharacterSkin;

namespace _Modules.Data.Scripts;

[Serializable]
public class PlayerSaveData
{
	[SerializeField]
	private string _charJob;

	[SerializeField]
	private float _maxHealth = 100f;

	[SerializeField]
	private float _maxStamina = 100f;

	[SerializeField]
	private int _maxInventory = 6;

	[SerializeField]
	private int _meleeWeapon;

	[SerializeField]
	private int _rangeWeapon;

	[SerializeField]
	private string _perkId;

	[SerializeField]
	private List<string> _additionalPerkSkillDataList = new List<string>();

	[SerializeField]
	private int _skillPoint;

	[SerializeField]
	private List<string> _skillLearnDataList = new List<string>();

	[SerializeField]
	private string _headSkinId;

	[SerializeField]
	private string _bodySkinId;

	[SerializeField]
	private int _genderSkinId;

	[SerializeField]
	private string _materialSkinId;

	[SerializeField]
	private string _skinColorId;

	[ES3Serializable]
	private Dictionary<string, MaterialInventoryData> _materialInventoryDic = new Dictionary<string, MaterialInventoryData>();

	[SerializeField]
	private List<InventoryObject> _inventory = new List<InventoryObject>();

	[SerializeField]
	private List<InventoryObject> _itemBoxInventory = new List<InventoryObject>();

	[SerializeField]
	private ScoreDataNetwork _scoreDataNetwork;

	public float MaxHealth
	{
		get
		{
			return _maxHealth;
		}
		set
		{
			_maxHealth = value;
		}
	}

	public float MaxStamina
	{
		get
		{
			return _maxStamina;
		}
		set
		{
			_maxStamina = value;
		}
	}

	public int MaxInventory
	{
		get
		{
			return _maxInventory;
		}
		set
		{
			_maxInventory = value;
		}
	}

	public List<string> AdditionalPerkSkillDataList
	{
		get
		{
			return _additionalPerkSkillDataList;
		}
		set
		{
			_additionalPerkSkillDataList = value;
		}
	}

	public int GenderSkinId
	{
		get
		{
			return _genderSkinId;
		}
		set
		{
			_genderSkinId = value;
		}
	}

	public int MeleeWeapon
	{
		get
		{
			return _meleeWeapon;
		}
		set
		{
			_meleeWeapon = value;
		}
	}

	public int RangeWeapon
	{
		get
		{
			return _rangeWeapon;
		}
		set
		{
			_rangeWeapon = value;
		}
	}

	public string PerkId
	{
		get
		{
			return _perkId;
		}
		set
		{
			_perkId = value;
		}
	}

	public int SkillPoint
	{
		get
		{
			return _skillPoint;
		}
		set
		{
			_skillPoint = value;
		}
	}

	public List<string> SkillLearnDataList
	{
		get
		{
			return _skillLearnDataList;
		}
		set
		{
			_skillLearnDataList = value;
		}
	}

	public string HeadSkinId
	{
		get
		{
			return _headSkinId;
		}
		set
		{
			_headSkinId = value;
		}
	}

	public string BodySkinId
	{
		get
		{
			return _bodySkinId;
		}
		set
		{
			_bodySkinId = value;
		}
	}

	public string MaterialSkinId
	{
		get
		{
			return _materialSkinId;
		}
		set
		{
			_materialSkinId = value;
		}
	}

	public string SkinColorId
	{
		get
		{
			return _skinColorId;
		}
		set
		{
			_skinColorId = value;
		}
	}

	public Dictionary<string, MaterialInventoryData> MaterialInventoryDic
	{
		get
		{
			return _materialInventoryDic;
		}
		set
		{
			_materialInventoryDic = value;
		}
	}

	public List<InventoryObject> ItemBoxInventory
	{
		get
		{
			return _itemBoxInventory;
		}
		set
		{
			_itemBoxInventory = value;
		}
	}

	public string CharJob
	{
		get
		{
			return _charJob;
		}
		set
		{
			_charJob = value;
		}
	}

	public List<InventoryObject> Inventory
	{
		get
		{
			return _inventory;
		}
		set
		{
			_inventory = value;
		}
	}

	public ScoreDataNetwork ScoreDataNetwork
	{
		get
		{
			return _scoreDataNetwork;
		}
		set
		{
			_scoreDataNetwork = value;
		}
	}

	public void SetPlayerSaveData(PlayerController playerController)
	{
		PlayerData data = playerController.data;
		_maxHealth = data.GetMaxHealth();
		_maxStamina = data.GetMaxStamina();
		_maxInventory = data.GetMaxInventory();
		_charJob = data.CharacterJob;
		_meleeWeapon = data.GetWeaponMelee();
		_rangeWeapon = data.GetWeaponRange();
		_perkId = data.SkillData.PerkId;
		_additionalPerkSkillDataList = new List<string>(data.SkillData.AdditionalPerkSkillDataList);
		_skillPoint = data.SkillData.SkillPoint;
		_headSkinId = data.PlayerSkinData.HeadSkinId;
		_bodySkinId = data.PlayerSkinData.BodySkinId;
		_genderSkinId = (int)data.PlayerSkinData.Gender;
		_materialSkinId = data.PlayerSkinData.MaterialSkinId;
		_skinColorId = data.PlayerSkinData.SkinColorId;
		_skillLearnDataList = new List<string>(data.SkillData.SkillLearnDataList);
		_materialInventoryDic = new Dictionary<string, MaterialInventoryData>(data.MainMaterialInventory.MaterialInventoryDic);
		_inventory = new List<InventoryObject>(data.GetNormalizeInventoryData());
		_itemBoxInventory = PlayerData.NormalizeInventoryData(playerController.ItemBoxController.arrItem);
		_scoreDataNetwork = playerController.ScorePlayerNetwork.ScoreDataTotal;
	}

	public void LoadPlayerSaveData(PlayerController playerController)
	{
		PlayerData data = playerController.data;
		data.SkillData.SetAdditionalPerkSkill(new List<string>(_additionalPerkSkillDataList), executeEvent: true);
		SetInventory(playerController);
		data.SetCharacterJob(_charJob, applyStats: true);
		SetSkillLearn(playerController);
		SetWeapon(data);
		data.SkillData.SetSkillPoint(_skillPoint);
		SetPerk(playerController);
		SetSkin(playerController);
		data.SetHealth(_maxHealth);
		data.SetStamina(_maxStamina);
		data.SetMaxInventoryLocal(_maxInventory);
		data.MainMaterialInventory.SetMaterial(new Dictionary<string, MaterialInventoryData>(_materialInventoryDic));
		playerController.ItemBoxController.arrItem = PlayerData.NormalizeInventoryData(_itemBoxInventory);
		foreach (InventoryObject item in data.arrInventory)
		{
			data.CheckItemInventory(item.ID, item.IdxInventory, item.ItemType, item, init: false, isEquipableWeapon: false);
		}
		if (!NetworkGameManager.Instance.isServer)
		{
			data.SyncMaxInventory((byte)_maxInventory);
		}
		playerController.network.ExecSyncInventoryLocalPlayerToAll();
		playerController.network.ExecSyncItemBox(playerController, (short)Mathf.FloorToInt(GameManager.Instance.timer.interval), isForLocalPlayer: false);
		playerController.data.ApplyInventoryStatusEffect();
		playerController.ScorePlayerNetwork.SetTotalScore(_scoreDataNetwork);
	}

	private void SetPerk(PlayerController playerController)
	{
		playerController.data.SkillData.SetPerk(_perkId);
		DataManager.Instance.Get<PerkLibraryScriptableObject>()?.GetData(_perkId)?.ExecuteEffectSkill(playerController);
	}

	private void SetSkillLearn(PlayerController playerController)
	{
		playerController.data.SkillData.SetSkillLearn(new List<string>(_skillLearnDataList));
		SkillLibraryScriptableObject skillLibraryScriptableObject = DataManager.Instance.Get<SkillLibraryScriptableObject>();
		for (int i = 0; i < _skillLearnDataList.Count; i++)
		{
			(skillLibraryScriptableObject?.GetData(_skillLearnDataList[i]))?.ExecuteEffectSkill(playerController);
		}
	}

	private void SetInventory(PlayerController playerController)
	{
		playerController.data.SetInventoryData(_inventory);
	}

	private void SetSkin(PlayerController playerController)
	{
		ChangeHeadSkin(playerController, _headSkinId);
		ChangeBodySkin(playerController, _bodySkinId);
		ChangeMaterialSkin(playerController, _materialSkinId);
		ChangeSkinColor(playerController, _skinColorId);
	}

	private void SetWeapon(PlayerData playerData)
	{
		int num = PlayerData.NormalizeWeaponData(_meleeWeapon, 1);
		int num2 = PlayerData.NormalizeWeaponData(_rangeWeapon);
		playerData.playerController.network.SetWeapon0(PlayerData.NormalizeWeaponData(num, 1));
		playerData.playerController.network.SetWeapon1(num2);
		playerData.weaponController.EquipWeaponID(num, 0);
		playerData.weaponController.EquipWeaponID(num2, 1);
	}

	private void ChangeHeadSkin(PlayerController playerController, string id)
	{
		CharacterSkinData heroSkinById = SkinManager.Instance.GetHeroSkinById(id);
		playerController.data.PlayerSkinData.SetHeadSkinData(heroSkinById);
	}

	private void ChangeBodySkin(PlayerController playerController, string id)
	{
		CharacterSkinData heroSkinBodyById = SkinManager.Instance.GetHeroSkinBodyById(id);
		if (heroSkinBodyById != null)
		{
			playerController.data.PlayerSkinData.SetBodySkinData(heroSkinBodyById);
		}
	}

	private void ChangeMaterialSkin(PlayerController playerController, string id)
	{
		SkinColorPaletteScriptableObject heroColorPaletteById = SkinManager.Instance.GetHeroColorPaletteById(id);
		if (heroColorPaletteById != null)
		{
			playerController.data.PlayerSkinData.SetMaterialSkinData(heroColorPaletteById);
		}
	}

	private void ChangeSkinColor(PlayerController playerController, string id)
	{
		SkinColorScriptableObject heroSkinColorSOById = SkinManager.Instance.GetHeroSkinColorSOById(id);
		if (heroSkinColorSOById != null)
		{
			playerController.data.PlayerSkinData.SetSkinColorData(heroSkinColorSOById);
		}
	}

	public InventoryObject GetInventoryData(int index)
	{
		if (index >= _inventory.Count)
		{
			return null;
		}
		return _inventory[index];
	}
}
