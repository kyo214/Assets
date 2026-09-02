using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using I2.Loc;
using Toked;
using UnityEngine;
using _Modules.GameSystem.BaseScripts.Difficulty;

public class WaveEnemyManager : MonoBehaviour
{
	public XTimer spawnTimer;

	public XTimer waveTimer;

	public XTimer roamingTimer;

	public XTimer hordeTimer;

	public XTimer hordingTimer;

	public XTimer cueHordeTimer;

	public XTimer buildUpHordeTimer;

	public XTimer spawnHordeTimer;

	public XTimer spawnEliteDelayTimer;

	public List<PosEnemy> arrWavePosEnemy = new List<PosEnemy>();

	public int levelHorde;

	public int levelWave;

	public bool disabled;

	public int maxEnemySpawnOnLevel;

	public int totEnemyOnGame;

	public int totEnemyHorde;

	public int timeWave;

	public int timeRoaming;

	public int timeHorde;

	public int totEliteSpawn;

	public int roamingGroup;

	public float timeSpawnEnemy;

	public float multiplyPerPlayer;

	public int ctrEnemyHorde;

	public bool isSpawningHorde;

	[SerializeField]
	public List<WavePerEnemy> ListEnemyWave = new List<WavePerEnemy>();

	public List<WavePerEnemy> ListEliteWave = new List<WavePerEnemy>();

	public const int MAX_LEVEL_DEFENSE_WAVE = 15;

	public const int MAX_LEVEL_NORMAL_WAVE = 8;

	public static float SCALING_ENEMY_HP_DEFENSE = 0.05f;

	public bool isSpawningHordeSpecial;

	private void Start()
	{
		DifficultyData difficultyData = GameModes.Instance.GetDifficultyData();
		multiplyPerPlayer = BGDatabase_GameConfig.GetEntity(GameModes.Instance.modeGame).MultiplyPerPlayer;
		totEnemyOnGame = BGDatabase_GameConfig.GetEntityByKeyid(GameModes.Instance.modeGame).TotEnemyOnGame;
		maxEnemySpawnOnLevel = BGDatabase_GameConfig.GetEntityByKeyid(GameModes.Instance.modeGame).MaxEnemySpawnOnLevel;
		timeWave = BGDatabase_GameConfig.GetEntityByKeyid(GameModes.Instance.modeGame).TimerFirstWave + difficultyData.WaveAdditionalTime;
		timeRoaming = BGDatabase_GameConfig.GetEntityByKeyid(GameModes.Instance.modeGame).TimerEnemyRoaming;
		timeSpawnEnemy = BGDatabase_GameConfig.GetEntity(GameModes.Instance.modeGame).TimerSpawnEnemy;
		timeHorde = BGDatabase_GameConfig.GetEntityByKeyid(GameModes.Instance.modeGame).TimerHorde + difficultyData.HordeAdditionalTime;
		if (NetworkGameManager.Instance.isServer)
		{
			GameManagerPhoton.Instance.Wave = 0;
			levelWave = GameManagerPhoton.Instance.CurrentMission.MissionObjective.StartingTableWave - 1;
		}
		if (totEnemyOnGame < 0)
		{
			totEnemyOnGame = 9999999;
		}
		if (maxEnemySpawnOnLevel < 0)
		{
			maxEnemySpawnOnLevel = 9999999;
		}
		spawnTimer.StartDuration(timeSpawnEnemy / (1f + (float)(NetworkGameManager.Instance.arrPlayerController.Count - 1) * multiplyPerPlayer));
		waveTimer.StartDuration(timeWave);
		roamingTimer.StartDuration(timeRoaming);
		levelHorde = GameModes.Instance.GetDifficultyData().StartIndexHorde;
	}

	public void UpdateNetwork()
	{
		if (roamingTimer.isCompleted() && NetworkGameManager.Instance.isServer && !disabled)
		{
			roamingTimer.StartDuration(timeRoaming);
			foreach (RoomCollider item in GameManager.Instance.arrRoom)
			{
				if (!item.isRevealedByAllPlayer || !item.canHorde || GetTotalenemyRoom(item.RoomName, roamingCheck: true) < item.minEnemyRoaming + item.minEnemyStay || !GameManager.Instance.isHordeMode)
				{
					continue;
				}
				PlayerController randomPlayer = NetworkGameManager.Instance.GetRandomPlayer(isHaveHealth: true);
				int num = item.minEnemyRoaming;
				roamingGroup++;
				foreach (EnemyController item2 in item.listEnemy)
				{
					if (num > 0 && !item2.isRoaming && !item2.network.GetIsHovering() && item2.network.GetHealth() > 0f && !item2.isDead && !item2.isFakeDead)
					{
						item2.attack.RoamingToPlayer(randomPlayer);
						item2.roamingGroup = roamingGroup;
						num--;
					}
				}
			}
		}
		if (cueHordeTimer.isCompleted() && !disabled && NetworkGameManager.Instance.isServer)
		{
			GameManager.Instance.gameManagerPhoton.RpcExecAlertHorde();
		}
		if (buildUpHordeTimer.isCompleted() && !disabled)
		{
			AudioManager.StopBGM();
			AudioManager.PlayBGM("Horde", "HordeBuildUp", 0f, isLooping: false);
			AudioManager.DisableAmbient();
			foreach (SoundTriggerController item3 in GameManager.Instance.arrSoundTrigger)
			{
				if (!item3.IsDialogue)
				{
					item3.gameObject.SetActive(value: false);
				}
				else if (item3.EventSound != null && item3.EventSound.triggerSound != null)
				{
					item3.EventSound.triggerSound.SoundEvents[0].volume = -90f;
				}
			}
		}
		if (hordingTimer.isCompleted() && !disabled && NetworkGameManager.Instance.isServer && GameManager.Instance.isHordeMode)
		{
			if (GameManager.Instance.gameManagerPhoton.objectiveComplete || GameManager.Instance.isInfiniteHordeMode)
			{
				if (GameManager.Instance.waveManager.levelHorde < 7)
				{
					GameManager.Instance.waveManager.levelHorde++;
				}
				ctrEnemyHorde = 0;
				GameManager.Instance.waveManager.InitHorde();
				if (GameManager.Instance.isInfiniteHordeMode)
				{
					hordingTimer.StartDuration(50f);
				}
				else
				{
					hordingTimer.StartDuration(60f);
				}
			}
			else
			{
				GameManager.Instance.isHordeMode = false;
				GameManager.Instance.gameManagerPhoton.RpcExecDisableHorde();
				GameManager.Instance.waveManager.InitHorde();
			}
		}
		if (hordeTimer.isCompleted() && !disabled && LobbyManager.Instance == null)
		{
			ExecuteHorde();
			if (NetworkGameManager.Instance.isServer)
			{
				if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning)
				{
					if (!GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsCarRepairingOnStart && levelWave == GameManagerPhoton.Instance.CurrentMission.MissionObjective.MaxWave)
					{
						hordingTimer.StartDuration(150f);
					}
					else
					{
						hordingTimer.StartDuration(80f);
					}
				}
				else if (GameManager.Instance.gameManagerPhoton.objectiveComplete)
				{
					hordingTimer.StartDuration(60f);
				}
				else
				{
					hordingTimer.StartDuration(50f);
				}
				ctrEnemyHorde = 0;
				if (ctrEnemyHorde < totEnemyHorde)
				{
					spawnHordeTimer.StartDuration(Random.Range(1f, 1.2f));
				}
			}
			if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission && GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning)
			{
				GameManagerPhoton.Instance.Wave++;
				if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.MaxWave <= 0 || GameManagerPhoton.Instance.Wave < GameManagerPhoton.Instance.CurrentMission.MissionObjective.MaxWave)
				{
					GameManager.Instance.waveManager.hordeTimer.StartDuration((float)timeHorde + GameManagerPhoton.Instance.CurrentMission.MissionObjective.DelayHorde);
				}
			}
		}
		if (spawnHordeTimer.isCompleted() && GameManager.Instance.isHordeMode && NetworkGameManager.Instance.isServer && !disabled)
		{
			if (GameManager.Instance.isInfiniteHordeMode || ((bool)GameManagerPhoton.Instance && GameManagerPhoton.Instance.objectiveComplete))
			{
				spawnHordeTimer.StartDuration(GameModes.Instance.GetDifficultyData().DelayEnemySpawn);
			}
			else
			{
				spawnHordeTimer.StartDuration(1f);
			}
			isSpawningHorde = true;
			if (ctrEnemyHorde < totEnemyHorde)
			{
				ctrEnemyHorde++;
				SpawnEnemyHorde();
			}
			if (ctrEnemyHorde >= totEnemyHorde)
			{
				if (GameManager.Instance.isInfiniteHordeMode)
				{
					int num2 = 0;
					if (num2 < 7 * ((NetworkGameManager.Instance.arrPlayerController.Count - 1) * 3))
					{
						ctrEnemyHorde = num2;
						totEnemyHorde = 7 * ((NetworkGameManager.Instance.arrPlayerController.Count - 1) * 3);
					}
				}
				else if (GameManager.Instance.gameManagerPhoton.objectiveComplete)
				{
					int num3 = 0;
					if (num3 < 5 * ((NetworkGameManager.Instance.arrPlayerController.Count - 1) * 3))
					{
						ctrEnemyHorde = num3;
						totEnemyHorde = 5 * ((NetworkGameManager.Instance.arrPlayerController.Count - 1) * 3);
					}
				}
				else
				{
					isSpawningHorde = false;
				}
			}
		}
		if (!hordeTimer.isRunning)
		{
			return;
		}
		float interval = hordeTimer.interval;
		if (!(interval > 0f) || !(interval <= 10f))
		{
			interval = hordeTimer.interval;
			if (!(interval > 50f) || !(interval <= 60f))
			{
				return;
			}
		}
		if (GameModes.Instance.modeGame == "Defense" && (bool)UIGameManager.Instance.TextTimeIncomingWave && !UIGameManager.Instance.TextTimeIncomingWave.transform.parent.gameObject.activeSelf)
		{
			GameManager.Instance.gameManagerPhoton.RpcExecIncomingWave((byte)hordeTimer.interval);
		}
	}

	public void TimerIncomingWave()
	{
		if (hordeTimer.interval >= 0.5f && hordeTimer.interval < 5.5f)
		{
			AudioManager.PlaySFXVol("ui_countdown", 0.5f);
		}
		UIGameManager.Instance.TextTimeIncomingWave.transform.parent.gameObject.SetActive(value: true);
		if (hordeTimer.interval < 5f)
		{
			UIGameManager.Instance.TextTimeIncomingWave.color = Color.red;
			UIGameManager.Instance.TextTimeIncomingWave.transform.DOShakePosition(5f, 1.5f, 0, 90f, snapping: false, fadeOut: false);
		}
		else
		{
			UIGameManager.Instance.TextTimeIncomingWave.color = Color.white;
		}
		UIGameManager.Instance.TextTimeIncomingWave.text = LocalizationManager.GetTranslation("Menu/TimeIncomingWave");
		UIGameManager.Instance.TextTimeIncomingWave.text = UIGameManager.Instance.TextTimeIncomingWave.text.Replace("(x)", Mathf.RoundToInt(hordeTimer.interval).ToString());
		if (UIGameManager.Instance.uiHordeIncoming.gameObject.activeSelf)
		{
			UIGameManager.Instance.TextTimeIncomingWave.rectTransform.anchoredPosition = new Vector2(UIGameManager.Instance.TextTimeIncomingWave.rectTransform.anchoredPosition.x, 133f);
		}
		else
		{
			UIGameManager.Instance.TextTimeIncomingWave.rectTransform.anchoredPosition = new Vector2(UIGameManager.Instance.TextTimeIncomingWave.rectTransform.anchoredPosition.x, 168f);
		}
		if (hordeTimer.interval > 60f || !hordeTimer.isRunning)
		{
			UIGameManager.Instance.TextTimeIncomingWave.transform.parent.gameObject.SetActive(value: false);
		}
		else
		{
			Invoke("TimerIncomingWave", 1f);
		}
	}

	public void ExecuteHorde()
	{
		if (!(LobbyManager.Instance == null))
		{
			return;
		}
		GameManager.Instance.isHordeMode = true;
		isSpawningHorde = true;
		UIGameManager.Instance.uiHordeIncoming.SetActive(value: true);
		if (GameManagerPhoton.Instance.objectiveComplete)
		{
			UIGameManager.Instance.LabelHordeInfiniteIncoming.SetActive(value: true);
			UIGameManager.Instance.LabelHordeIncoming.SetActive(value: false);
		}
		foreach (SoundTriggerController item in GameManager.Instance.arrSoundTrigger)
		{
			if (!item.IsDialogue)
			{
				item.gameObject.SetActive(value: false);
			}
			else if (item.EventSound != null && item.EventSound.triggerSound != null)
			{
				item.EventSound.triggerSound.SoundEvents[0].volume = -90f;
			}
		}
		AudioManager.PlaySFX("enemy-scream");
		AudioManager.BGMSetLoop(isLooping: true);
		if (GameManager.Instance.isInfiniteHordeMode || GameManagerPhoton.Instance.objectiveComplete)
		{
			CameraGame.Instance.SetColorAdjustmentEffect(new Color(1f, 0.85f, 0.85f), 3f);
			GameManager.Instance.BloodPool.gameObject.SetActive(value: true);
			GameManager.Instance.BloodPool.DOScale(new Vector3(100f, 0.01f, 100f), 40f);
		}
		if (!AudioManager.isPlayingBGM("HordeIsComing"))
		{
			AudioManager.DisableAmbient();
			if (GameManager.Instance.isInfiniteHordeMode || GameManagerPhoton.Instance.objectiveComplete)
			{
				AudioManager.PlayBGM("Horde", "EndlessHorde");
			}
			else
			{
				AudioManager.PlayBGM("Horde", "HordeIsComing", 0f, isLooping: true, savePlaylistName: true);
			}
		}
	}

	public void InitHorde(bool isInit = false, int initDifficultyWave = -1)
	{
		if (!NetworkGameManager.Instance.isServer)
		{
			return;
		}
		if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning)
		{
			if (isInit)
			{
				GameManager.Instance.waveManager.hordeTimer.StartDuration(20f);
			}
		}
		else
		{
			if ((bool)MissionManager.Instance && !MissionManager.Instance.IsTimerCountdownMode)
			{
				cueHordeTimer.StartDuration((float)timeHorde - 47.5f);
			}
			if (timeHorde > 180)
			{
				timeHorde -= 15;
			}
		}
		ListEnemyWave.Clear();
		totEnemyHorde = 0;
		if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning && !GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsCarRepairingOnStart)
		{
			if (levelWave >= 15)
			{
				levelWave = 14;
			}
			BGDatabase_DefenseWave.ForEachEntity((BGDatabase_DefenseWave entity) =>
			{
				WavePerEnemy wavePerEnemy = new WavePerEnemy(entity.EnemyKey, 0);
				if (entity.GetType().GetProperty("Wave" + Mathf.RoundToInt(levelWave + 1))?.GetValue(entity) != null)
				{
					wavePerEnemy.Total = (int)entity.GetType().GetProperty("Wave" + Mathf.FloorToInt(levelWave + 1))?.GetValue(entity);
				}
				if (wavePerEnemy.EnemyKey < 100)
				{
					wavePerEnemy.Total += Mathf.RoundToInt((float)(wavePerEnemy.Total * (NetworkGameManager.Instance.arrPlayerController.Count - 1)) * GameModes.Instance.WaveMultiplierByPlayer);
					wavePerEnemy.Total = Mathf.RoundToInt((float)wavePerEnemy.Total * GlobalMissionManager.Instance.ModMultiplyTotalZombiesHorde.CurrentValue);
					wavePerEnemy.Total = Mathf.RoundToInt((float)wavePerEnemy.Total * GameModes.Instance.GetDifficultyData().EnemyHordeMultiplier);
				}
				Debug.Log(wavePerEnemy.EnemyKey + " - " + wavePerEnemy.Total);
				if (wavePerEnemy.Total > 0)
				{
					if (wavePerEnemy.EnemyKey < 100)
					{
						ListEnemyWave.Add(wavePerEnemy);
						totEnemyHorde += wavePerEnemy.Total;
					}
					else
					{
						ListEliteWave.Add(wavePerEnemy);
					}
				}
			});
			levelWave++;
			return;
		}
		levelWave = levelHorde;
		if (initDifficultyWave != -1)
		{
			isSpawningHordeSpecial = true;
			levelWave = initDifficultyWave;
		}
		if (levelWave >= 8)
		{
			levelWave = 7;
		}
		BGDatabase_Wave.ForEachEntity((BGDatabase_Wave entity) =>
		{
			WavePerEnemy wavePerEnemy = new WavePerEnemy(entity.EnemyKey, 0);
			if (entity.GetType().GetProperty("Wave" + Mathf.RoundToInt(levelWave + 1))?.GetValue(entity) != null)
			{
				wavePerEnemy.Total = (int)entity.GetType().GetProperty("Wave" + Mathf.FloorToInt(levelWave + 1))?.GetValue(entity);
			}
			if (wavePerEnemy.EnemyKey < 100)
			{
				wavePerEnemy.Total += Mathf.RoundToInt((float)(wavePerEnemy.Total * (NetworkGameManager.Instance.arrPlayerController.Count - 1)) * GameModes.Instance.WaveMultiplierByPlayer);
				wavePerEnemy.Total = Mathf.RoundToInt((float)wavePerEnemy.Total * GlobalMissionManager.Instance.ModMultiplyTotalZombiesHorde.CurrentValue);
				wavePerEnemy.Total = Mathf.RoundToInt((float)wavePerEnemy.Total * GameModes.Instance.GetDifficultyData().EnemyHordeMultiplier);
			}
			if (wavePerEnemy.Total > 0)
			{
				if (wavePerEnemy.EnemyKey < 100)
				{
					ListEnemyWave.Add(wavePerEnemy);
					totEnemyHorde += wavePerEnemy.Total;
				}
				else
				{
					ListEliteWave.Add(wavePerEnemy);
				}
			}
		});
	}

	private int GetTotalenemyRoom(string roomName, bool roamingCheck)
	{
		int num = 0;
		foreach (RoomCollider item in GameManager.Instance.arrRoom)
		{
			if (!(item.RoomName == roomName))
			{
				continue;
			}
			if (roamingCheck)
			{
				foreach (EnemyController item2 in item.listEnemy)
				{
					if (!item2.isRoaming && !item2.network.GetIsHovering())
					{
						num++;
					}
				}
			}
			else
			{
				num = item.listEnemy.Count;
			}
		}
		return num;
	}

	private void SpawnEnemyHorde()
	{
		int type = 0;
		if (ListEnemyWave.Count > 0)
		{
			int index = Random.Range(0, ListEnemyWave.Count);
			type = ListEnemyWave[index].EnemyKey;
			ListEnemyWave[index].Total--;
			if (ListEnemyWave[index].Total <= 0)
			{
				ListEnemyWave.RemoveAt(index);
			}
		}
		PlayerController randomPlayer = NetworkGameManager.Instance.GetRandomPlayer(isHaveHealth: true);
		Vector3 vector = Vector3.zero;
		if ((bool)randomPlayer)
		{
			vector = randomPlayer.transform.position;
		}
		List<PosEnemy> list = ((!GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning || GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsCarRepairingOnStart) ? FindNearestWavePos(3, vector) : FindNearestWavePos(-1, vector));
		int count = list.Count;
		for (int i = 1; i < count; i++)
		{
			list.Add(list[i]);
		}
		List<int> list2 = new List<int>();
		bool flag = true;
		if (ListEliteWave.Count > 0 && Random.Range(0, 6) < 5 && totEnemyHorde - ctrEnemyHorde >= 9)
		{
			flag = false;
		}
		if (flag)
		{
			int num = 100;
			if (ListEliteWave.Count > 0)
			{
				num = ListEliteWave[0].EnemyKey;
				ListEliteWave[0].Total--;
				totEliteSpawn++;
				if (ListEliteWave[0].Total <= 0)
				{
					ListEliteWave.RemoveAt(0);
				}
			}
			if (totEliteSpawn >= 1)
			{
				for (int j = 0; j < GameManager.Instance.arrInitPosEnemy.Count; j++)
				{
					if (GameManager.Instance.arrInitPosEnemy[j].roomCollider != null && MathFunc.Distance(GameManager.Instance.arrInitPosEnemy[j].transform.position, vector) <= 40f && GameManager.Instance.arrInitPosEnemy[j].roomCollider.isRevealedByAllPlayer && GameManager.Instance.arrInitPosEnemy[j].posType == PosEnemy.PosType.PosEliteHorde)
					{
						list2.Add(j);
					}
				}
				if (list2.Count > 0)
				{
					if (list2.Count >= totEliteSpawn)
					{
						int num2 = 0;
						foreach (EnemyController item in GameManager.Instance.arrEnemyController)
						{
							if (item.isElite && item.network.GetIsHorde() && item.network.GetHealth() > 0f)
							{
								num2++;
							}
						}
						int index2 = totEliteSpawn - 1;
						Debug.Log("Spawn Elilte " + num);
						if (GameModes.Instance.modeGame == "Defense")
						{
							GameManager.Instance.gameManagerPhoton.RpcExecSpawnPortal((byte)list2[index2], (byte)num);
						}
						else if (totEliteSpawn < 2 && (bool)GameManagerPhoton.Instance)
						{
							GameManager.Instance.gameManagerPhoton.RpcExecSpawnPortal((byte)list2[index2], (byte)num);
						}
					}
					totEliteSpawn--;
				}
				else
				{
					totEliteSpawn--;
				}
			}
		}
		if (list.Count > 0)
		{
			int index3 = Random.Range(0, list.Count);
			EnemySpawner.Instance.SpawnEnemy(list[index3], list[index3].posEnter[Random.Range(0, list[index3].posEnter.Count)].transform, type, isHorde: true);
		}
		list.Clear();
	}

	private List<PosEnemy> FindNearestWavePos(int count, Vector3 playerPos)
	{
		float num = 1600f;
		if (count <= -1)
		{
			List<PosEnemy> list = new List<PosEnemy>();
			for (int i = 0; i < arrWavePosEnemy.Count; i++)
			{
				PosEnemy posEnemy = arrWavePosEnemy[i];
				if (!(posEnemy.roomCollider == null) && posEnemy.isSpawnable && posEnemy.roomCollider.isRevealedByAllPlayer && posEnemy.canSpawnHordeType && (posEnemy.transform.position - playerPos).sqrMagnitude <= num)
				{
					list.Add(posEnemy);
				}
			}
			list.Sort((PosEnemy a, PosEnemy b) =>
			{
				float sqrMagnitude2 = (a.transform.position - playerPos).sqrMagnitude;
				float sqrMagnitude3 = (b.transform.position - playerPos).sqrMagnitude;
				return sqrMagnitude2.CompareTo(sqrMagnitude3);
			});
			return list;
		}
		List<PosEnemy> list2 = new List<PosEnemy>(count);
		List<float> list3 = new List<float>(count);
		for (int num2 = 0; num2 < arrWavePosEnemy.Count; num2++)
		{
			PosEnemy posEnemy2 = arrWavePosEnemy[num2];
			if (posEnemy2.roomCollider == null || !posEnemy2.isSpawnable || !posEnemy2.roomCollider.isRevealedByAllPlayer || !posEnemy2.canSpawnHordeType)
			{
				continue;
			}
			float sqrMagnitude = (posEnemy2.transform.position - playerPos).sqrMagnitude;
			if (sqrMagnitude > num)
			{
				continue;
			}
			if (list2.Count < count)
			{
				list2.Add(posEnemy2);
				list3.Add(sqrMagnitude);
				continue;
			}
			int index = 0;
			float num3 = list3[0];
			for (int num4 = 1; num4 < list3.Count; num4++)
			{
				if (list3[num4] > num3)
				{
					num3 = list3[num4];
					index = num4;
				}
			}
			if (sqrMagnitude < num3)
			{
				list2[index] = posEnemy2;
				list3[index] = sqrMagnitude;
			}
		}
		list2.Sort((PosEnemy a, PosEnemy b) =>
		{
			float sqrMagnitude2 = (a.transform.position - playerPos).sqrMagnitude;
			float sqrMagnitude3 = (b.transform.position - playerPos).sqrMagnitude;
			return sqrMagnitude2.CompareTo(sqrMagnitude3);
		});
		return list2;
	}

	private void SpawnEnemyRandom()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < arrWavePosEnemy.Count; i++)
		{
			int num = Random.Range(0, 100);
			if (arrWavePosEnemy[i].roomCollider != null && arrWavePosEnemy[i].isSpawnable && GetTotalenemyRoom(arrWavePosEnemy[i].roomCollider.RoomName, roamingCheck: true) < arrWavePosEnemy[i].roomCollider.maxEnemy && ((arrWavePosEnemy[i].roomCollider.isRevealedByAllPlayer && num < 80) || (!arrWavePosEnemy[i].roomCollider.isRevealedByAllPlayer && num >= 80)))
			{
				list.Add(i);
			}
		}
		int index = Random.Range(0, list.Count);
		if (list.Count > 0)
		{
			EnemySpawner.Instance.SpawnEnemy(arrWavePosEnemy[list[index]], arrWavePosEnemy[list[index]].posEnter[Random.Range(0, arrWavePosEnemy[list[index]].posEnter.Count)].transform, 0, isHorde: false);
		}
	}

	public void AlertHorde(int cueDuration)
	{
		AudioManager.PlaySFX("cue-hordeComing");
		if (cueDuration > 10)
		{
			buildUpHordeTimer.StartDuration(10f);
		}
		hordeTimer.StartDuration(cueDuration);
	}

	public void HordeDisable()
	{
		GameManager.Instance.waveManager.spawnTimer.StartDuration(Random.Range(1, 3));
		GameManager.Instance.isHordeMode = false;
		UIGameManager.Instance.uiHordeIncoming.SetActive(value: false);
		AudioManager.StopBGM(5f, isPlayPrevBGM: true);
		if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning)
		{
			if (GameManagerPhoton.Instance.Wave == GameManagerPhoton.Instance.CurrentMission.MissionObjective.MaxWave)
			{
				foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
				{
					item.network.playerPhoton.IsSurvive = true;
				}
				if (NetworkGameManager.Instance.isServer)
				{
					foreach (ItemPickable item2 in GameManager.Instance.arrItemPickable)
					{
						if (item2.itemType != "Material" && item2.transform.parent != null && item2.itemCollider.enabled)
						{
							ItemSpawn itemSpawn = new ItemSpawn();
							itemSpawn.IDItem = item2.itemID;
							itemSpawn.Amount = item2.amount;
							itemSpawn.Ammo = item2.ammo;
							itemSpawn.Durability = item2.durability;
							if ((bool)GameManager.Instance.MapTransform)
							{
								itemSpawn.Pos = item2.transform.parent.position - GameManager.Instance.MapTransform.position;
							}
							else
							{
								itemSpawn.Pos = item2.transform.parent.position;
							}
							GameManagerPhoton.Instance.ListItemSpawnToLobby.Add(itemSpawn);
						}
					}
				}
				UniTaskUtil.DelayedCall(this, 3f, () =>
				{
					foreach (PlayerController item3 in NetworkGameManager.Instance.arrPlayerController)
					{
						if (item3.network.playerPhoton.health <= 0)
						{
							item3.network.playerPhoton.health = 1;
						}
					}
					GameManager.Instance.TriggerWin(usingGameCutscene: true);
				}).Forget();
			}
			levelHorde++;
		}
		else if (!isSpawningHordeSpecial && levelHorde < 7)
		{
			levelHorde++;
		}
		isSpawningHordeSpecial = false;
		UniTaskUtil.DelayedCall(this, 5f, ChangeMusicToInit).Forget();
	}

	private void ChangeMusicToInit()
	{
		if (AudioManager.Instance.BGMFixed)
		{
			return;
		}
		AudioManager.EnableAmbient();
		foreach (SoundTriggerController item in GameManager.Instance.arrSoundTrigger)
		{
			item.gameObject.SetActive(value: true);
			if (item.IsDialogue && item.EventSound != null && item.EventSound.triggerSound != null)
			{
				item.EventSound.triggerSound.SoundEvents[0].volume = 100f;
			}
		}
	}

	public void DisableHorde(EnemyController enemyController)
	{
		if (!enemyController.network.GetIsHorde())
		{
			return;
		}
		enemyController.network.SetIsHorde(value: false);
		int num = 0;
		int num2 = 0;
		foreach (EnemyController item in GameManager.Instance.arrEnemyController)
		{
			if (item != enemyController && !item.network.IsNonActive() && item.network.GetHealth() > 0f && !item.isDead)
			{
				if (item.network.GetIsHorde())
				{
					num++;
				}
				if (item.GetCurrentStateHash() == AnimatorHashManager.ChasingHash || item.GetCurrentStateHash() == AnimatorHashManager.AttackingHash || item.GetCurrentStateHash() == AnimatorHashManager.AlertChasingHash)
				{
					num2++;
				}
			}
		}
		if (num == 0 && num2 == 0 && GameManager.Instance.isHordeMode && !GameManager.Instance.waveManager.isSpawningHorde && !GameManager.Instance.isInfiniteHordeMode && !GameManager.Instance.gameManagerPhoton.objectiveComplete)
		{
			GameManager.Instance.isHordeMode = false;
			GameManager.Instance.gameManagerPhoton.RpcExecDisableHorde();
			InitHorde();
		}
	}
}
