using System.Collections.Generic;
using UnityEngine;
using _Modules.GameSystem.BaseScripts.Difficulty;

public class EnemyData : MonoBehaviour
{
	public EnemyController enemyController;

	public int type;

	[SerializeField]
	private float health;

	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private float attackMoveSpeed;

	[SerializeField]
	private float soundDistTolerance;

	[SerializeField]
	private float distConeView;

	public float distAttack;

	[SerializeField]
	private int angleConeView;

	[SerializeField]
	public List<string> arrWeaponState = new List<string>();

	[SerializeField]
	public int weaponState;

	[SerializeField]
	private float initHealth;

	[SerializeField]
	private float initMoveSpeed;

	[SerializeField]
	private float initDamage;

	public float damage;

	public float aggroSpeed;

	public float distanceAggro2;

	public float aggroSpeed2;

	public float aggroDelay;

	public int timeOutChasing;

	public float minTimeRandomAngle;

	public float maxTimeRandomAngle;

	public float minTimeRandomState;

	public float maxTimeRandomState;

	public float distChasing;

	public float delayAttack;

	public bool initialized;

	public float knockBackDistanceMultiply;

	private void Start()
	{
		Init(isOnCreated: true);
	}

	public void Init(bool isOnCreated)
	{
		if (isOnCreated)
		{
			DifficultyData difficultyData = GameModes.Instance.GetDifficultyData();
			float multiplyPerPlayer = BGDatabase_GameConfig.GetEntityByKeyid(GameModes.Instance.modeGame).MultiplyPerPlayer;
			BGDatabase_Enemy entityByKeyid = BGDatabase_Enemy.GetEntityByKeyid("Enemy" + type);
			initMoveSpeed = entityByKeyid.MoveSpeed;
			attackMoveSpeed = entityByKeyid.AttackMoveSpeed;
			if (Mathf.Approximately(attackMoveSpeed, -1f))
			{
				attackMoveSpeed = entityByKeyid.MoveSpeed;
			}
			initMoveSpeed = Random.Range(initMoveSpeed * 0.8f, initMoveSpeed);
			initHealth = entityByKeyid.Health * difficultyData.EnemyHpMultiplier;
			knockBackDistanceMultiply = entityByKeyid.KnockBackDistanceMultiply;
			if (enemyController.isElite)
			{
				initHealth = initHealth * difficultyData.EnemyEliteHpMultiplier + (float)Mathf.RoundToInt(initHealth * (float)(NetworkGameManager.Instance.arrPlayerController.Count - 1) * multiplyPerPlayer);
			}
			initDamage = Mathf.FloorToInt(entityByKeyid.Damage * difficultyData.EnemyDamageMultiplier);
			moveSpeed = initMoveSpeed;
			health = initHealth;
			damage = initDamage;
			minTimeRandomAngle = entityByKeyid.MinTimeRandomAngle;
			maxTimeRandomAngle = entityByKeyid.MaxTimeRandomAngle;
			minTimeRandomState = entityByKeyid.MinTimeRandomState;
			maxTimeRandomState = entityByKeyid.MaxTimeRandomState;
			if (enemyController.isAlwaysChasing)
			{
				aggroSpeed = entityByKeyid.AggroSpeedHorde / 4f;
			}
			else
			{
				aggroSpeed = entityByKeyid.AggroSpeed / 4f;
			}
			aggroSpeed = Random.Range(aggroSpeed * 0.7f, aggroSpeed);
			distanceAggro2 = entityByKeyid.DistanceAggro2;
			aggroSpeed2 = entityByKeyid.AggroSpeed2 / 4f;
			aggroSpeed2 = Random.Range(aggroSpeed2 * 0.8f, aggroSpeed2);
			aggroDelay = entityByKeyid.AggroDelay;
			timeOutChasing = entityByKeyid.TimeOutChasing;
			soundDistTolerance = entityByKeyid.SoundDistTolerance;
			distConeView = entityByKeyid.DistConeView;
			angleConeView = entityByKeyid.AngleConeView;
			distChasing = entityByKeyid.DistChasing;
			delayAttack = entityByKeyid.DelayAttack;
			enemyController.attack.fov.viewRadius = distConeView;
			enemyController.attack.fov.viewAngle = angleConeView;
			MultiplyStatsModifierMap();
		}
		else
		{
			moveSpeed = initMoveSpeed;
			health = initHealth;
			damage = initDamage;
			MultiplyStatsModifierMap(isOnCreated: false);
		}
		enemyController.network.SetHealth(health);
		enemyController.SetAISpeed(aggroSpeed);
		initialized = true;
	}

	public void MultiplyStatsModifierMap(bool isOnCreated = true)
	{
		health = initHealth * GlobalMissionManager.Instance.ModMultiplyHpZombies.CurrentValue;
		moveSpeed *= GlobalMissionManager.Instance.ModMultiplySpeedZombies.CurrentValue;
		if (isOnCreated)
		{
			aggroSpeed *= GlobalMissionManager.Instance.ModMultiplySpeedZombies.CurrentValue;
			aggroSpeed2 *= GlobalMissionManager.Instance.ModMultiplySpeedZombies.CurrentValue;
		}
	}

	public float GetSpeed()
	{
		return moveSpeed / 4f;
	}

	public float GetAttackMoveSpeed()
	{
		return attackMoveSpeed / 4f;
	}
}
