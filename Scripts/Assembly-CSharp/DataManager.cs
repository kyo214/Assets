using BansheeGz.BGDatabase;
using Database.Tools;
using Toked.Crafting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
	public DatabaseUpdaterConfig databaseLibrary;

	public static DataManager Instance { get; private set; }

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
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public string GetValueDatabase(string tableName, int entryID, string fieldName)
	{
		BGEntity bGEntity = BGRepo.I[tableName].FindEntity((BGEntity entity) => entity.Name == entryID.ToString());
		string result = "";
		if (bGEntity != null && bGEntity.GetType().GetProperty(fieldName) != null)
		{
			result = (string)bGEntity.GetType().GetProperty(fieldName).GetValue(bGEntity);
		}
		return result;
	}

	public int GetValueDatabase(string tableName, string entryID, string fieldName)
	{
		BGEntity bGEntity = BGRepo.I[tableName].FindEntity((BGEntity entity) => entity.Name == entryID);
		int result = -1;
		if (bGEntity != null && bGEntity.GetType().GetProperty(fieldName) != null)
		{
			result = (int)bGEntity.GetType().GetProperty(fieldName).GetValue(bGEntity);
		}
		return result;
	}

	public int GetEntities(string tableName, string entryID, string fieldName)
	{
		BGEntity bGEntity = BGRepo.I[tableName].FindEntity((BGEntity entity) => entity.Name == entryID);
		return (int)bGEntity.GetType().GetProperty(fieldName).GetValue(bGEntity);
	}

	public T GetValueDatabase<T>(string tableName, string entryID, string fieldName)
	{
		return BGRepo.I[tableName].FindEntity((BGEntity entity) => entity.Name == entryID).Get<T>(fieldName);
	}

	public T Get<T>() where T : IScriptableObjectLibrary
	{
		return databaseLibrary.GetData<T>();
	}

	public ItemScriptableObject GetItemData(string id)
	{
		return databaseLibrary.GetData<ItemLibraryScriptableObject>().GetData(id);
	}

	public Sprite GetItemSprite(string id)
	{
		return databaseLibrary.GetData<ItemLibraryScriptableObject>().GetData(id)?.ItemSprite;
	}

	public string GetItemType(int itemID)
	{
		string result = "";
		if (itemID >= 400)
		{
			result = "Material";
		}
		else if (itemID >= 300)
		{
			result = "Item";
		}
		else if (itemID >= 200)
		{
			result = "HealingItem";
		}
		else if (itemID >= 100)
		{
			result = "Ammunition";
		}
		else if (itemID >= 0)
		{
			result = "Weapon";
		}
		return result;
	}

	public int GetBaseWeapon(int idWeapon)
	{
		return BGDatabase_Weapon.GetEntityByKeyid(idWeapon)?.BaseWeaponID ?? 0;
	}
}
