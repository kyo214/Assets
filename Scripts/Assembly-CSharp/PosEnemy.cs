using System.Collections.Generic;
using UnityEngine;

public class PosEnemy : MonoBehaviour
{
	public enum PosType
	{
		PosInitZombie = 0,
		PosInitFakeDeadZombie = 1,
		PosInitFloatingZombie = 2,
		PosInitElite = 3,
		PosZombieHorde = 4,
		PosEliteHorde = 5,
		PosOtherPrefab = 6
	}

	public List<Transform> posEnter = new List<Transform>();

	public PosType posType;

	public bool IsRandomizeElite;

	public EnumZombieType ZombieType;

	public int eliteType;

	public OtherPrefabType OtherType;

	public List<EliteType> ListRandomEliteType = new List<EliteType>();

	public bool isSpawnable;

	public bool isDeaf;

	public bool isDisableCollider;

	public int InitAngleEnemy;

	public bool DisableInEasyMap;

	[SerializeField]
	private SO_MissionModifierStatus modifierMission;

	public bool DisableInDemo;

	public bool isInvulnerable;

	public bool isAlwaysChasing;

	public bool IsMoveable = true;

	public EnemyController lastEnemySpawned;

	public RoomCollider roomCollider;

	public string roomName;

	public bool canSpawnHordeType = true;

	private void Start()
	{
		if (DisableInEasyMap)
		{
			if (((bool)GameManagerPhoton.Instance && GameManagerPhoton.Instance.CurrentMission.IsEasyMap) || (DisableInDemo && GameModes.Instance.isDemo))
			{
				base.gameObject.SetActive(value: false);
			}
			else
			{
				Init();
			}
		}
		else
		{
			Init();
		}
		CheckingModifier();
	}

	public void CheckingModifier()
	{
		if (!GameManagerPhoton.Instance || !(modifierMission != null))
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < GameManagerPhoton.Instance.CurrentMission.ListModifier.Count; i++)
		{
			for (int j = 0; j < GameManagerPhoton.Instance.CurrentMission.ListModifier[i].Modifier.Count; j++)
			{
				flag = GameManagerPhoton.Instance.CurrentMission.ListModifier[i].Modifier[j].ModifierStatus == modifierMission && modifierMission.CurrentValue >= 1f;
			}
		}
		if (flag)
		{
			base.gameObject.SetActive(value: true);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Init()
	{
		if (posType == PosType.PosInitElite && eliteType < 100)
		{
			eliteType += 100;
		}
		if (posEnter.Count <= 0 || (posEnter.Count > 0 && (posEnter[0] == null || !posEnter[0].gameObject.activeSelf)))
		{
			canSpawnHordeType = false;
		}
		IsMoveable = true;
		isAlwaysChasing = true;
		foreach (Transform item in posEnter)
		{
			item.position = new Vector3(item.position.x, 0f, item.position.z);
		}
		if (posType == PosType.PosZombieHorde)
		{
			GameManager.Instance.waveManager.arrWavePosEnemy.Add(this);
			foreach (RoomCollider item2 in GameManager.Instance.arrRoom)
			{
				Vector3 point = ((posEnter.Count <= 0 || !(posEnter[0] != null) || !posEnter[0].gameObject.activeSelf) ? base.transform.position : posEnter[0].position);
				bool flag = false;
				for (int i = 0; i < item2.boxColliders.Count; i++)
				{
					if (item2.boxColliders[i].bounds.Contains(point))
					{
						roomCollider = item2;
						roomName = item2.RoomName;
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		else
		{
			if (posType == PosType.PosInitFakeDeadZombie || posType == PosType.PosInitFloatingZombie)
			{
				IsMoveable = false;
			}
			if (posType != PosType.PosEliteHorde)
			{
				isAlwaysChasing = false;
			}
			if (posType == PosType.PosInitElite && IsRandomizeElite && ListRandomEliteType.Count > 0)
			{
				int index = Random.Range(0, ListRandomEliteType.Count);
				eliteType = (int)ListRandomEliteType[index];
			}
			GameManager.Instance.arrInitPosEnemy.Add(this);
			foreach (RoomCollider item3 in GameManager.Instance.arrRoom)
			{
				bool flag2 = false;
				for (int j = 0; j < item3.boxColliders.Count; j++)
				{
					if (item3.boxColliders[j].bounds.Contains(base.transform.position))
					{
						roomCollider = item3;
						roomName = item3.RoomName;
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					break;
				}
			}
		}
		Object.Destroy(GetComponent<MeshRenderer>());
		Object.Destroy(GetComponent<MeshFilter>());
	}

	private void OnDisable()
	{
		if (posType != PosType.PosZombieHorde)
		{
			GameManager.Instance.arrInitPosEnemy.Remove(this);
		}
	}
}
