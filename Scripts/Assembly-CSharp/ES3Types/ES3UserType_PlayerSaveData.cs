using System.Collections.Generic;
using Toked.Inventory;
using UnityEngine.Scripting;
using _Modules.Data.Scripts;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[]
{
	"_charJob", "_maxHealth", "_maxStamina", "_maxInventory", "_meleeWeapon", "_rangeWeapon", "_perkId", "_additionalPerkSkillDataList", "_skillPoint", "_skillLearnDataList",
	"_headSkinId", "_bodySkinId", "_genderSkinId", "_materialSkinId", "_skinColorId", "_materialInventoryDic", "_inventory", "_itemBoxInventory", "_scoreDataNetwork"
})]
public class ES3UserType_PlayerSaveData : ES3ObjectType
{
	public static ES3Type Instance;

	public ES3UserType_PlayerSaveData()
		: base(typeof(PlayerSaveData))
	{
		Instance = this;
		priority = 1;
	}

	protected override void WriteObject(object obj, ES3Writer writer)
	{
		PlayerSaveData objectContainingField = (PlayerSaveData)obj;
		writer.WritePrivateField("_charJob", objectContainingField);
		writer.WritePrivateField("_maxHealth", objectContainingField);
		writer.WritePrivateField("_maxStamina", objectContainingField);
		writer.WritePrivateField("_maxInventory", objectContainingField);
		writer.WritePrivateField("_meleeWeapon", objectContainingField);
		writer.WritePrivateField("_rangeWeapon", objectContainingField);
		writer.WritePrivateField("_perkId", objectContainingField);
		writer.WritePrivateField("_additionalPerkSkillDataList", objectContainingField);
		writer.WritePrivateField("_skillPoint", objectContainingField);
		writer.WritePrivateField("_skillLearnDataList", objectContainingField);
		writer.WritePrivateField("_headSkinId", objectContainingField);
		writer.WritePrivateField("_bodySkinId", objectContainingField);
		writer.WritePrivateField("_genderSkinId", objectContainingField);
		writer.WritePrivateField("_materialSkinId", objectContainingField);
		writer.WritePrivateField("_skinColorId", objectContainingField);
		writer.WritePrivateField("_materialInventoryDic", objectContainingField);
		writer.WritePrivateField("_inventory", objectContainingField);
		writer.WritePrivateField("_itemBoxInventory", objectContainingField);
		writer.WritePrivateField("_scoreDataNetwork", objectContainingField);
	}

	protected override void ReadObject<T>(ES3Reader reader, object obj)
	{
		PlayerSaveData objectContainingField = (PlayerSaveData)obj;
		foreach (string property in reader.Properties)
		{
			switch (property)
			{
			case "_charJob":
				reader.SetPrivateField("_charJob", reader.Read<string>(), objectContainingField);
				break;
			case "_maxHealth":
				reader.SetPrivateField("_maxHealth", reader.Read<float>(), objectContainingField);
				break;
			case "_maxStamina":
				reader.SetPrivateField("_maxStamina", reader.Read<float>(), objectContainingField);
				break;
			case "_maxInventory":
				reader.SetPrivateField("_maxInventory", reader.Read<int>(), objectContainingField);
				break;
			case "_meleeWeapon":
				reader.SetPrivateField("_meleeWeapon", reader.Read<int>(), objectContainingField);
				break;
			case "_rangeWeapon":
				reader.SetPrivateField("_rangeWeapon", reader.Read<int>(), objectContainingField);
				break;
			case "_perkId":
				reader.SetPrivateField("_perkId", reader.Read<string>(), objectContainingField);
				break;
			case "_additionalPerkSkillDataList":
				reader.SetPrivateField("_additionalPerkSkillDataList", reader.Read<List<string>>(), objectContainingField);
				break;
			case "_skillPoint":
				reader.SetPrivateField("_skillPoint", reader.Read<int>(), objectContainingField);
				break;
			case "_skillLearnDataList":
				reader.SetPrivateField("_skillLearnDataList", reader.Read<List<string>>(), objectContainingField);
				break;
			case "_headSkinId":
				reader.SetPrivateField("_headSkinId", reader.Read<string>(), objectContainingField);
				break;
			case "_bodySkinId":
				reader.SetPrivateField("_bodySkinId", reader.Read<string>(), objectContainingField);
				break;
			case "_genderSkinId":
				reader.SetPrivateField("_genderSkinId", reader.Read<int>(), objectContainingField);
				break;
			case "_materialSkinId":
				reader.SetPrivateField("_materialSkinId", reader.Read<string>(), objectContainingField);
				break;
			case "_skinColorId":
				reader.SetPrivateField("_skinColorId", reader.Read<string>(), objectContainingField);
				break;
			case "_materialInventoryDic":
				reader.SetPrivateField("_materialInventoryDic", reader.Read<Dictionary<string, MaterialInventoryData>>(), objectContainingField);
				break;
			case "_inventory":
				reader.SetPrivateField("_inventory", reader.Read<List<InventoryObject>>(), objectContainingField);
				break;
			case "_itemBoxInventory":
				reader.SetPrivateField("_itemBoxInventory", reader.Read<List<InventoryObject>>(), objectContainingField);
				break;
			case "_scoreDataNetwork":
				reader.SetPrivateField("_scoreDataNetwork", reader.Read<ScoreDataNetwork>(), objectContainingField);
				break;
			default:
				reader.Skip();
				break;
			}
		}
	}

	protected override object ReadObject<T>(ES3Reader reader)
	{
		PlayerSaveData playerSaveData = new PlayerSaveData();
		ReadObject<T>(reader, playerSaveData);
		return playerSaveData;
	}
}
