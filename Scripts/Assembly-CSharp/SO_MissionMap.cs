using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using _Modules.Cutscene.Scripts;

[CreateAssetMenu(fileName = "MapData", menuName = "WMO/ScriptableObjects/MapData", order = 1)]
public class SO_MissionMap : ScriptableObject
{
	public int MissionID;

	public int MissionIDByMap;

	public string SceneName;

	[TermsPopup("")]
	public string MapNameLocalization;

	[TermsPopup("")]
	public string DescLocalization;

	public bool IsFixedMissionObjective;

	public SO_MissionObjective MissionObjective;

	public List<SO_MissionModifierEffect> ListModifier = new List<SO_MissionModifierEffect>();

	public Sprite MapImage;

	public int Difficulty;

	public bool IsEasyMap;

	public bool IsCleared;

	public bool IsHide;

	public bool IsLocked;

	public bool IsBoss;

	public bool IsLastMap;

	public bool AlwaysLocked;

	public int SkillPointReward = 1;

	public int TotalMeleeWeapon;

	public List<ListTotalWeaponDifficulty> ListTotalRangeWeapon = new List<ListTotalWeaponDifficulty>();

	public int TotalSpecialWeapon;

	public int MinSpecialWeapon;

	public int TotalPlayerSpawningPosition;

	public int PlayerSpawningIdx;

	public List<WeaponMapType> ListWeapon = new List<WeaponMapType>();

	public List<SO_MissionMap> ListRequiredMapToUnlock = new List<SO_MissionMap>();

	public List<SO_MissionMap> ListPossibleMapToUnlock = new List<SO_MissionMap>();

	public bool transferMainMaterialInventoryToInGame;

	public bool pickupSharedMaterial;

	public bool customStartCutsceneLobby;

	public int buildVersion;

	public bool isInstantiate;

	public CutsceneScriptableObject cutsceneScriptableObject;

	public List<SO_MissionMap> ListCombinationMapFrom = new List<SO_MissionMap>();

	public Sprite GetMapImage()
	{
		if (!IsLocked)
		{
			return MapImage;
		}
		return null;
	}
}
