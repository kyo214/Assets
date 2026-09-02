using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public static EnemySpawner Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public EnemyController SpawnEnemy(PosEnemy pos, Transform posEnter, int type, bool isHorde, Vector3 spawnPos = default(Vector3))
	{
		EnemyController enemyController = null;
		if (pos != null)
		{
			spawnPos = pos.transform.position;
		}
		foreach (EnemyController item in GameManager.Instance.arrEnemyController)
		{
			if (item.network.IsNonActive() && item.network.GetHealth() <= 0f && item.data.type == type && !item.network.GetIsHovering())
			{
				enemyController = item;
				if ((bool)pos)
				{
					pos.lastEnemySpawned = item;
					item.LastPosEnemy = pos;
				}
				item.network.SetIsHorde(isHorde);
				item.isAlwaysChasing = isHorde;
				item.transform.position = spawnPos;
				item.NetworkPos.WritePosition(spawnPos);
				item.data.Init(isOnCreated: false);
				item.Init();
				item.bodyCollider.isTrigger = false;
				item.network.ExecInit(spawnPos);
				item.isWaveSpawned = true;
				item.object2D.position = item.transform.position;
				item.attack.SpawnedChasing(playerSighted: false, posEnter);
				item.attack.fov.SetDisable(value: true);
				item.isSpriteInactive = true;
				item.HideSprite();
				item.network.networkPhoton.isDeaf = false;
				if (type < 100)
				{
					item.network.networkPhoton.isMoveToJump = true;
				}
				if (isHorde)
				{
					item.movement.timerChangeAngle.StopDuration();
					item.movement.timerChangeState.StopDuration();
				}
				Transform child = item.object2D.GetChild(0);
				child.DOKill();
				child.localPosition = new Vector3(child.localPosition.x, 0.742f, child.localPosition.z);
				item.object2D.DOLocalRotate(new Vector3(0f, CameraGame.Instance.camRotate, 0f), 0.1f).SetEase(Ease.OutQuad);
				item.network.SetIsHovering(value: false);
				break;
			}
		}
		if (enemyController == null)
		{
			EnemyController component = GameManager.Instance.SpawnEnemyPhoton(pos, type, isOtherType: false, spawnPos).GetComponent<EnemyController>();
			enemyController = component;
			if ((bool)pos)
			{
				pos.lastEnemySpawned = component;
				component.LastPosEnemy = pos;
			}
			component.data.type = type;
			component.network.SetIsHorde(isHorde);
			component.isAlwaysChasing = isHorde;
			component.bodyCollider.isTrigger = false;
			component.isWaveSpawned = true;
			component.object2D.position = component.transform.position;
			if (type < 100)
			{
				component.attack.SpawnedChasing(playerSighted: false, posEnter);
				component.attack.fov.SetDisable(value: true);
				component.network.ExecInit(spawnPos);
				component.network.networkPhoton.isMoveToJump = true;
			}
			else
			{
				component.network.ExecInit(new Vector3(spawnPos.x, 0.742f, spawnPos.y));
				component.movement.StartMove().Forget();
			}
			component.isSpriteInactive = true;
			component.HideSprite();
			component.network.networkPhoton.isDeaf = false;
			component.network.SetIsHovering(value: false);
			Transform child2 = component.object2D.GetChild(0);
			child2.DOKill();
			child2.localPosition = new Vector3(child2.localPosition.x, 0.742f, child2.localPosition.z);
			component.object2D.DOLocalRotate(new Vector3(0f, CameraGame.Instance.camRotate, 0f), 0.1f).SetEase(Ease.OutQuad);
			if (isHorde)
			{
				component.movement.timerChangeAngle.StopDuration();
				component.movement.timerChangeState.StopDuration();
			}
		}
		return enemyController;
	}

	private void MultiplyEnemyHealthEndlessDefense(EnemyController enemy)
	{
		if ((bool)GameManagerPhoton.Instance.CurrentMission && GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning && !GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsCarRepairingOnStart)
		{
			int num = Mathf.FloorToInt((float)GameManager.Instance.waveManager.levelHorde / 3f);
			float health = enemy.network.GetHealth();
			enemy.network.SetHealth(health + health * ((float)num * WaveEnemyManager.SCALING_ENEMY_HP_DEFENSE));
		}
	}
}
