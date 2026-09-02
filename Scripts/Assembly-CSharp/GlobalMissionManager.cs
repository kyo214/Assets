using System.Collections.Generic;
using UnityEngine;

public class GlobalMissionManager : MonoBehaviour
{
	public List<SO_MissionObjective> ListAllMissionObjective = new List<SO_MissionObjective>();

	public List<SO_MissionObjective> ListRandomizeMissionObjectiveLv1 = new List<SO_MissionObjective>();

	public List<SO_MissionObjective> ListRandomizeMissionObjectiveLv2 = new List<SO_MissionObjective>();

	public List<SO_MissionObjective> ListRandomizeMissionObjectiveLv3 = new List<SO_MissionObjective>();

	public List<SO_MissionObjective> ListRandomizeMissionObjectiveLv4 = new List<SO_MissionObjective>();

	public List<SO_MissionMap> ListAllMission = new List<SO_MissionMap>();

	public List<SO_MissionModifierEffect> ListAllMissionModifier = new List<SO_MissionModifierEffect>();

	public SO_MissionModifierStatus ModMultiplyTotalZombiesHorde;

	public SO_MissionModifierStatus ModEnableMoreInitZombies;

	public SO_MissionModifierStatus ModMultiplyHpZombies;

	public SO_MissionModifierStatus ModMultiplySpeedZombies;

	public SO_MissionModifierStatus ModEnableMoreInitElite;

	public SO_MissionModifierStatus ModEnableExplosionsHorde;

	public SO_MissionModifierStatus ModToxicSpill;

	public SO_MissionModifierStatus ModNoAmmoLoot;

	public SO_MissionModifierStatus ModNoHealingItem;

	public SO_MissionModifierStatus ModExplodingZombie;

	public SO_MissionModifierStatus ModToxinZombie;

	public SO_MissionModifierStatus ModCursedItem;

	public SO_MissionMap BattleRoyaleMissionSO;

	public static GlobalMissionManager Instance { get; private set; }

	public SO_MissionModifierEffect GetMissionModifier(int id)
	{
		for (int i = 0; i < ListAllMissionModifier.Count; i++)
		{
			if (id == ListAllMissionModifier[i].ID)
			{
				return ListAllMissionModifier[i];
			}
		}
		return null;
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		Instance = this;
		for (int i = 0; i < ListAllMissionObjective.Count; i++)
		{
			ListAllMissionObjective[i].ID = i;
		}
		for (int num = ListAllMissionModifier.Count - 1; num > 0; num--)
		{
			ListAllMissionModifier[num].ID = num;
		}
	}
}
