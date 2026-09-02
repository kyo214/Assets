using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dissonance;
using Fusion;
using Toked;
using UnityEngine;
using _Modules.CharacterSkin.Scripts;
using _Modules.Map.Scripts;

public class PlayerNetwork : MonoBehaviour
{
	public PlayerPhotonNetwork playerPhoton;

	public PlayerController playerController;

	public CharacterSkinPhotonNetwork characterSkinPhotonNetwork;

	public NetworkMecanimAnimator fsmUpperBodyPhoton;

	public CharControllerPhoton charControllerPhoton;

	public bool isLocalPlayer;

	public byte playerIdx;

	public SyncController syncController;

	public NetworkObject networkObj;

	public bool isDeadResult;

	private VoicePlayerState _voicePlayerState;

	public VoicePlayerState VoicePlayerState => _voicePlayerState ?? (_voicePlayerState = VoiceChatGlobalController.Instance.GetVoiceChat(playerPhoton.voiceChatName));

	private void Awake()
	{
		playerController = GetComponent<PlayerController>();
		playerController.fsmUpperBody = fsmUpperBodyPhoton.Animator;
		syncController = GetComponent<SyncController>();
	}

	public void UpdateNetwork()
	{
		if (!playerPhoton.disconnected)
		{
			if (networkObj == null)
			{
				networkObj = GetComponent<NetworkObject>();
			}
			Vector3 directionPlayer = GetAngledirection();
			float num = GetAngleInputNetwork();
			Vector3 rotatePlayer = new Vector3(Mathf.Sin(MathF.PI / 180f * num), 0f, Mathf.Cos(MathF.PI / 180f * num)).normalized;
			if (isLocalPlayer)
			{
				rotatePlayer = playerController.angleInput;
				directionPlayer = playerController.direction;
			}
			playerController.AnglePlayer(directionPlayer, rotatePlayer);
		}
	}

	private void Update()
	{
		if (!charControllerPhoton.DisableMoveTemporary)
		{
			MoveCharacter(GetAngledirection());
			return;
		}
		charControllerPhoton.CtrDelayMove--;
		if (charControllerPhoton.CtrDelayMove <= 0)
		{
			charControllerPhoton.DisableMoveTemporary = false;
		}
	}

	public Vector3 GetAngledirection()
	{
		float num = GetDirection();
		if (num != 9f)
		{
			num *= 45f;
			return new Vector3(Mathf.Sin(MathF.PI / 180f * num), 0f, Mathf.Cos(MathF.PI / 180f * num)).normalized;
		}
		return Vector3.zero;
	}

	public void MoveCharacter(Vector3 myDirection)
	{
		Vector3 newVelocity = Vector3.zero;
		if (!playerController.isDashing)
		{
			if (playerController.network.GetEnableControl() && playerController.enableMoveChar)
			{
				if (myDirection != Vector3.zero)
				{
					playerController.AnglePlayerAim(playerController.inputRotation, towardsFunctionOn: false);
				}
				newVelocity = myDirection * playerController.data.GetCurrentMoveSpeed();
			}
			else
			{
				newVelocity = Vector3.zero;
			}
		}
		else if (!playerController.isEntangled)
		{
			if (playerController.directionDash == Vector3.zero)
			{
				playerController.directionDash = myDirection;
			}
			newVelocity = ((!playerController.isDashingMove) ? (playerController.directionDash * 2.5f) : (playerController.directionDash * 7.5f));
			if (Time.timeScale == 0f)
			{
				newVelocity = Vector3.zero;
			}
		}
		if (playerController.initPos && GameManager.Instance != null && !NetworkGameManager.Instance.isSyncingMissionMap)
		{
			MapManager mapManager = GameManager.Instance.MapManager;
			mapManager.StartCoroutine(mapManager.InitMap());
			if ((bool)LobbyManager.Instance)
			{
				if (GlobalSaveData.instance.dialogueOnboardingShowed)
				{
					charControllerPhoton.SetPosition(mapManager.GetSpawnPosition(0, GetIDX()));
				}
				else
				{
					charControllerPhoton.SetPosition(mapManager.GetSpawnPosition(1, GetIDX()));
				}
			}
			else if (playerController.data.isSyncPosReconnect)
			{
				charControllerPhoton.SetPosition(playerController.network.playerPhoton.SyncCurrentPosition);
				playerController.data.isSyncPosReconnect = false;
			}
			else
			{
				charControllerPhoton.SetPosition(mapManager.GetSpawnPosition(GetIDX()));
				if (NetworkGameManager.Instance.isServer)
				{
					playerController.network.playerPhoton.SyncCurrentPosition = mapManager.GetSpawnPosition(GetIDX());
				}
			}
			if (playerController.network.isLocalPlayer)
			{
				Vector3 vector = new Vector3(5.4f, -7.9f, 5.3f);
				CameraGame.Instance.targetCursor.position = playerController.transform.position - vector;
			}
			playerController.initPos = false;
			UniTaskUtil.DelayedCall(this, 0.3f, () =>
			{
				if (NetworkGameManager.Instance.isServer)
				{
					NetworkGameManager.Instance.ownPlayer.network.playerPhoton.SyncCurrentPosition = NetworkGameManager.Instance.ownPlayer.transform.position;
				}
				else
				{
					NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcSetSyncPosition(NetworkGameManager.Instance.ownPlayer.transform.position);
				}
			}).Forget();
		}
		else if (charControllerPhoton.transform.position.y < -1f)
		{
			charControllerPhoton.SetPosition(new Vector3(charControllerPhoton.transform.position.x, 0f, charControllerPhoton.transform.position.z));
			charControllerPhoton.DisableMoveTemporary = true;
		}
		else
		{
			charControllerPhoton.SetKinematicVelocity(newVelocity, playerController.timeline.deltaTime);
		}
	}

	public void SetIdxPlayer()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			List<byte> list = new List<byte>();
			for (byte b = 0; b < 10; b++)
			{
				list.Add(b);
			}
			for (int i = 0; i < NetworkGameManager.Instance.arrPlayerController.Count; i++)
			{
				int index = list.IndexOf(NetworkGameManager.Instance.arrPlayerController[i].network.GetIDX());
				list.RemoveAt(index);
			}
			for (int j = 0; j < NetworkGameManager.Instance.arrPlayerDisconnected.Count; j++)
			{
				int index2 = list.IndexOf(NetworkGameManager.Instance.arrPlayerDisconnected[j].network.GetIDX());
				list.RemoveAt(index2);
			}
			playerIdx = list[0];
			playerPhoton.idx = list[0];
			NetworkGameManager.Instance.arrPlayerNetworkController[playerPhoton.idx] = playerController;
			playerPhoton.targetIdxCam = list[0];
			playerController.SetTargetIdxCamBeforeRevive(list[0]);
			if (!GameManager.Instance.isTestMode && GameManager.Instance.gameManagerPhoton != null)
			{
				GameManager.Instance.gameManagerPhoton.arrPlayerReady.Set(list[0], value: false);
			}
		}
	}

	public void SetTargetIdxCamTarget(byte idx)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			playerPhoton.targetIdxCam = idx;
		}
		else
		{
			playerPhoton.RpcSetTargetIdxCam(idx);
		}
	}

	public void SetPlayerName()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			playerPhoton.playerName = GlobalSaveData.instance.UserSaveData.UserName;
			playerPhoton.userUniqueId = GlobalSaveData.instance.UserSaveData.UserUniqueId;
		}
		else
		{
			playerPhoton.RpcSetPlayerName(GlobalSaveData.instance.UserSaveData.UserName, GlobalSaveData.instance.UserSaveData.UserUniqueId);
		}
	}

	public void AddSubHealth(float value, bool trueDamage = false, bool cantDead = false)
	{
		if (!trueDamage && value < 0f)
		{
			value = playerController.ArmorManager?.CalculateDamage(value) ?? value;
		}
		value *= 100f;
		if ((playerController.invincibleTimer.isRunning && !(value > 0f)) || GameManagerPhoton.Instance.IsWin || (bool)playerPhoton.disconnected)
		{
			return;
		}
		if (NetworkGameManager.Instance.isServer)
		{
			short num = playerPhoton.health;
			if (!playerController.IsGod || (playerController.IsGod && value > 0f))
			{
				num = ((!((float)num + value >= playerController.data.GetMaxHealth() * 100f)) ? ((short)(num + (short)value)) : ((short)(playerController.data.GetMaxHealth() * 100f)));
			}
			if (num <= 0)
			{
				num = (short)(cantDead ? 100 : 0);
			}
			playerPhoton.health = num;
		}
		else if (!playerController.IsGod || value > 0f)
		{
			playerPhoton.RpcAddHealth((short)value, cantDead);
		}
	}

	public bool IsDead()
	{
		if (GetHealth() <= 0f || (bool)playerPhoton.disconnected)
		{
			return !playerPhoton.IsSurvive;
		}
		return false;
	}

	public void SetHealth(float value, bool init = false)
	{
		value *= 100f;
		if (NetworkGameManager.Instance.isServer | init)
		{
			short num = (short)value;
			if (num < 0)
			{
				num = 0;
			}
			playerPhoton.health = num;
		}
		else if (playerPhoton.health != (short)value)
		{
			playerPhoton.RpcSetHealth((short)value);
		}
	}

	public void SetGodMode(bool isGodMode)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			playerPhoton.godMode = isGodMode;
		}
		else
		{
			playerPhoton.RpcSetGodMode(isGodMode);
		}
	}

	public void ShowBaloonChat(ChatType chatType, int itemID, int itemID2 = -1, int itemID3 = -1, int UIDItem = -1, byte playerTargetID = 10)
	{
		if (itemID3 >= 0)
		{
			playerPhoton.RpcShowBaloonChat3(GetIDX(), chatType, (short)itemID, (short)itemID2, (short)itemID3);
		}
		else if (itemID2 >= 0)
		{
			playerPhoton.RpcShowBaloonChat2(GetIDX(), chatType, (short)itemID, (short)itemID2);
		}
		else if (UIDItem >= 0)
		{
			playerPhoton.RpcShowBaloonChatUID(GetIDX(), chatType, (short)itemID, (short)UIDItem);
		}
		else
		{
			playerPhoton.RpcShowBaloonChat1(GetIDX(), chatType, (short)itemID, playerTargetID);
		}
	}

	public void SetUILobby(bool isActive)
	{
		playerPhoton.RpcSetUILobby(isActive, GetIDX());
	}

	public void SetWeapon1(int value, bool init = false)
	{
		if (NetworkGameManager.Instance.isServer | init)
		{
			playerPhoton.idWeapon1 = (short)value;
		}
		else
		{
			playerPhoton.RpcSetWeapon1((short)value);
		}
	}

	public void SetWeapon0(int value, bool init = false)
	{
		if (NetworkGameManager.Instance.isServer | init)
		{
			playerPhoton.idWeapon0 = (short)value;
		}
		else
		{
			playerPhoton.RpcSetWeapon0((short)value);
		}
	}

	public void SelectWeapon(int value, bool init = false)
	{
		if (NetworkGameManager.Instance.isServer | init)
		{
			playerPhoton.weaponSelect = (byte)value;
		}
		else
		{
			playerPhoton.RpcSelectWeapon((byte)value);
		}
	}

	public void SetPlayerReady(bool value)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			if (GameManager.Instance.gameManagerPhoton != null)
			{
				GameManager.Instance.gameManagerPhoton.arrPlayerReady.Set(GetIDX(), value);
			}
		}
		else
		{
			playerPhoton.RpcSetReady(value);
		}
	}

	public void SetPlayerAFK(bool value)
	{
	}

	public void SetInGame(bool value)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			playerPhoton.inGame = value;
		}
	}

	public void KillEnemy(Vector3 posEnemy, bool isFading, bool isElite)
	{
		if (!playerPhoton.playerNetwork.isLocalPlayer)
		{
			return;
		}
		if (NetworkGameManager.Instance.isServer)
		{
			playerController.ScorePlayerNetwork.IncreaseKill(isElite);
		}
		else
		{
			playerController.ScorePlayerNetwork.RpcAddKillEnemy(isElite);
		}
		int num = UnityEngine.Random.Range(0, 100);
		if ((double)num < (double)GameModes.Instance.chancePercentDropAmmo * 0.75 && playerController.weaponController.idWeaponRange > 0)
		{
			int ammoTypeID = BGDatabase_Weapon.GetEntityByKeyid(playerController.weaponController.idBaseWeaponRange).AmmoTypeID;
			short idxItem = (short)GameManager.Instance.GetIdxItemPool(ammoTypeID);
			if (NetworkGameManager.Instance.isServer)
			{
				GameManager.Instance.gameManagerPhoton.RpcDropItem(ammoTypeID, (byte)BGDatabase_Ammunition.GetEntityByKeyid(ammoTypeID).Amount, 0, MathFunc.EncodeVector3ToULong(posEnemy), idxItem);
			}
			else
			{
				playerPhoton.RpcDropItem(ammoTypeID, (byte)BGDatabase_Ammunition.GetEntityByKeyid(ammoTypeID).Amount, 0, MathFunc.EncodeVector3ToULong(posEnemy), idxItem);
			}
		}
		else if (num < GameModes.Instance.chancePercentDropAmmo)
		{
			int[] array = new int[3] { 100, 102, 103 };
			int num2 = UnityEngine.Random.Range(0, array.Length);
			short idxItem2 = (short)GameManager.Instance.GetIdxItemPool(num2);
			if (NetworkGameManager.Instance.isServer)
			{
				GameManager.Instance.gameManagerPhoton.RpcDropItem(array[num2], (byte)BGDatabase_Ammunition.GetEntityByKeyid(array[num2]).Amount, 0, MathFunc.EncodeVector3ToULong(posEnemy), idxItem2);
			}
			else
			{
				playerPhoton.RpcDropItem(array[num2], (byte)BGDatabase_Ammunition.GetEntityByKeyid(array[num2]).Amount, 0, MathFunc.EncodeVector3ToULong(posEnemy), idxItem2);
			}
		}
		else if (num < GameModes.Instance.chancePercentDropScraps + GameModes.Instance.chancePercentDropGunPowder + GameModes.Instance.chancePercentDropAmmo)
		{
			num = UnityEngine.Random.Range(0, GameModes.Instance.chancePercentDropScraps + GameModes.Instance.chancePercentDropGunPowder + GameModes.Instance.chancePercentDropAmmo);
			int num3 = 0;
			if (num < GameModes.Instance.chancePercentDropScraps)
			{
				num3 = 400;
			}
			else if (num - GameModes.Instance.chancePercentDropScraps < GameModes.Instance.chancePercentDropGunPowder)
			{
				num3 = 402;
			}
			else if (num - GameModes.Instance.chancePercentDropScraps - GameModes.Instance.chancePercentDropGunPowder < GameModes.Instance.chancePercentDropChemical)
			{
				num3 = 404;
			}
			short idxItem3 = (short)GameManager.Instance.GetIdxItemPool(num3);
			if (NetworkGameManager.Instance.isServer)
			{
				GameManager.Instance.gameManagerPhoton.RpcDropItem(num3, 0, 0, MathFunc.EncodeVector3ToULong(posEnemy), idxItem3, isFading);
			}
			else
			{
				playerPhoton.RpcDropItem(num3, 0, 0, MathFunc.EncodeVector3ToULong(posEnemy), idxItem3, isFading);
			}
		}
	}

	public void SetDropItemFromPlayer(int uIDItem, int amount, int ammo, int idxInventory, bool isQuickDrop = false, int uniqueID = -1)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			if (uniqueID == -1)
			{
				uniqueID = GameManager.Instance.GetIdxItemPool(uIDItem, isQuickDrop);
			}
			GameManager.Instance.gameManagerPhoton.RpcDropItemFromPlayer(uIDItem, (byte)amount, (byte)ammo, GetIDX(), (short)uniqueID, (byte)idxInventory);
		}
		else
		{
			playerPhoton.RpcDropItemFromPlayer(uIDItem, (byte)amount, (byte)ammo, GetIDX(), (byte)idxInventory, isQuickDrop, (short)uniqueID);
		}
	}

	public void SetDropItem(int uIDItem, int amount, int ammo, Vector3 pos, bool isSwapWeapon, bool isSpreading = true)
	{
		int idxItemPool = GameManager.Instance.GetIdxItemPool(uIDItem);
		if (NetworkGameManager.Instance.isServer)
		{
			GameManager.Instance.gameManagerPhoton.RpcDropItem(uIDItem, (byte)amount, (byte)ammo, MathFunc.EncodeVector3ToULong(pos), (short)idxItemPool, isFading: false, isSpreading);
		}
		else
		{
			playerPhoton.RpcDropItem(uIDItem, (byte)amount, (byte)ammo, MathFunc.EncodeVector3ToULong(pos), (short)idxItemPool, isFading: false, isSpreading);
		}
	}

	public void SetUnlockItem(int uniqueId)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			GameManager.Instance.gameManagerPhoton.RpcUnlockItem((byte)uniqueId);
		}
		else
		{
			playerPhoton.RpcUnlockItem((byte)uniqueId);
		}
	}

	public void SetSpawnItem(int IDItem, Vector3 pos, int amount = -1, int ammo = -1, bool isSpread = false)
	{
		if (amount == -1 && ammo == -1)
		{
			SetSpawnItem(IDItem, pos, isSpread);
		}
		else if (ammo != -1)
		{
			SetSpawnItemAmmo(IDItem, pos, amount, ammo, isSpread);
		}
		else
		{
			SetSpawnItemAmount(IDItem, pos, amount, isSpread);
		}
	}

	private void SetSpawnItem(int IDItem, Vector3 pos, bool isSpread = false)
	{
		int num = GameManager.Instance.GetIdxItemPool(IDItem);
		if (NetworkGameManager.Instance.isServer)
		{
			if (num < 0 && GameManager.Instance.arrItemPickable.Count > 0)
			{
				GameManager.Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
				List<ItemPickable> arrItemPickable = GameManager.Instance.arrItemPickable;
				num = (short)(arrItemPickable[arrItemPickable.Count - 1].uniqueID + 1);
			}
			GameManager.Instance.gameManagerPhoton.RpcSpawnItem(IDItem, MathFunc.EncodeVector3ToULong(pos), (short)num, isSpread);
		}
		else
		{
			playerPhoton.RpcSpawnItem(IDItem, pos, (short)num, isSpread);
		}
	}

	public void SetSpawnItemAmmo(int IDItem, Vector3 pos, int amount, int ammo, bool isSpread = false)
	{
		int num = GameManager.Instance.GetIdxItemPool(IDItem);
		if (NetworkGameManager.Instance.isServer)
		{
			if (num < 0 && GameManager.Instance.arrItemPickable.Count > 0)
			{
				GameManager.Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
				List<ItemPickable> arrItemPickable = GameManager.Instance.arrItemPickable;
				num = (short)(arrItemPickable[arrItemPickable.Count - 1].uniqueID + 1);
			}
			GameManager.Instance.gameManagerPhoton.RpcSpawnItemAmountAmmo(IDItem, MathFunc.EncodeVector3ToULong(pos), (short)num, (byte)amount, (byte)ammo, isSpread);
		}
		else
		{
			playerPhoton.RpcSpawnItemAmountAmmo(IDItem, pos, (short)num, (byte)amount, (byte)ammo, isSpread);
		}
	}

	public void SetSpawnItemAmount(int IDItem, Vector3 pos, int amount, bool isSpread = false)
	{
		int num = GameManager.Instance.GetIdxItemPool(IDItem);
		if (NetworkGameManager.Instance.isServer)
		{
			if (num < 0 && GameManager.Instance.arrItemPickable.Count > 0)
			{
				GameManager.Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
				List<ItemPickable> arrItemPickable = GameManager.Instance.arrItemPickable;
				num = (short)(arrItemPickable[arrItemPickable.Count - 1].uniqueID + 1);
			}
			GameManager.Instance.gameManagerPhoton.RpcSpawnItemAmount(IDItem, MathFunc.EncodeVector3ToULong(pos), (short)num, (byte)amount, isSpread);
		}
		else
		{
			playerPhoton.RpcSpawnItemAmount(IDItem, pos, (short)num, (byte)amount, isSpread);
		}
	}

	public void UnequipWeapon0()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			playerPhoton.idWeapon0 = -1;
		}
		else
		{
			playerPhoton.RpcUnequipWeapon0();
		}
	}

	public void UnequipWeapon1()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			playerPhoton.idWeapon1 = -1;
		}
		else
		{
			playerPhoton.RpcUnequipWeapon1();
		}
	}

	public void ExecAddInventory(int iD, int idxInventory, int amount, int uniqueID = -1, int durability = -1)
	{
		if (durability > 0)
		{
			playerPhoton.RpcAddInventory((short)iD, (byte)idxInventory, (byte)amount, GetIDX(), (short)uniqueID, (short)durability);
		}
		else
		{
			playerPhoton.RpcAddInventory((short)iD, (byte)idxInventory, (byte)amount, GetIDX(), (short)uniqueID, -1);
		}
	}

	public void ExecSyncDataInventory(int idxInventory, int amount)
	{
		playerPhoton.RpcSyncDataInventory((byte)idxInventory, (byte)amount, GetIDX());
	}

	public void ExecSwapItem(int idx1, int idx2)
	{
		playerPhoton.RpcSwapItem((byte)idx1, (byte)idx2, GetIDX());
	}

	public void ExecRemoveObject(int uniqueID)
	{
		playerPhoton.RpcRemoveObject((short)uniqueID);
	}

	public void ExecRemoveInventory(int idx)
	{
		playerPhoton.RpcRemoveInventory((byte)idx);
	}

	public void ExecRemoveInventoryData(int idx)
	{
		playerPhoton.RpcRemoveInventoryData((byte)idx);
	}

	public void ExecRemoveInventoryDuplicate(int idx, int itemAmount)
	{
		playerPhoton.RpcRemoveInventoryDuplicate((byte)idx, (byte)itemAmount);
	}

	public void ExecInteractObject(short uniqueID, bool triggerOnReverse = false, bool isForceInteract = false)
	{
		if (isLocalPlayer)
		{
			playerPhoton.RpcItemInteract(uniqueID, GetIDX(), triggerOnReverse, isForceInteract);
		}
		else
		{
			GameManager.Instance.ItemInteract(uniqueID, GetIDX(), triggerOnReverse, isForceInteract);
		}
	}

	public void ExecStartProgressInteract(short uniqueID, byte playerID)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			GameManager.Instance.gameManagerPhoton.RpcStartProgressInteract(uniqueID, playerID);
		}
		else
		{
			playerPhoton.RpcStartProgressInteract(uniqueID, playerID);
		}
	}

	public void ExecStopProgressInteract(short uniqueID)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			GameManager.Instance.gameManagerPhoton.RpcStopProgressInteract(uniqueID, GetIDX());
		}
		else
		{
			playerPhoton.RpcStopProgressInteract(uniqueID);
		}
	}

	public void ExecStopProgressInteract()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			GameManager.Instance.gameManagerPhoton.RpcStopProgressInteract(GetIDX());
		}
		else
		{
			playerPhoton.RpcStopProgressInteract();
		}
	}

	public void ExecStopProgressInteractNoItem()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			GameManager.Instance.gameManagerPhoton.RpcStopProgressInteract(GetIDX());
		}
		else
		{
			playerPhoton.RpcStopProgressInteract();
		}
	}

	public void ExecHurtEffect(byte idx, bool isCloseInventory = true, bool isGreenBloodScreen = false)
	{
		if (!playerController.invincibleTimer.isRunning)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				GameManager.Instance.gameManagerPhoton.RpcExecHitEffect(idx, isCloseInventory, isGreenBloodScreen);
			}
			else
			{
				playerPhoton.RpcExecHitEffect(idx, isCloseInventory);
			}
		}
	}

	public void SetAimDirection(short aimDirection)
	{
		playerPhoton.RpcSetAimDirection(GetIDX(), aimDirection);
	}

	public void ExecAttackTriggered(short aimDirection)
	{
		if ((!GlobalOptionsManager.Instance.usingWeaponSelect && playerController.isAiming) || (GlobalOptionsManager.Instance.usingWeaponSelect && playerController.weaponController.weaponSelect == 1))
		{
			if (playerController.data.arrInventory[playerController.weaponController.idxWeaponRange].Ammo > 0)
			{
				playerPhoton.RpcExecAttackTriggered(GetIDX(), (byte)playerController.data.arrInventory[playerController.weaponController.idxWeaponRange].Ammo, aimDirection);
			}
			else
			{
				playerController.weaponController.TriggerReload();
			}
		}
		else if ((!GlobalOptionsManager.Instance.usingWeaponSelect || (GlobalOptionsManager.Instance.usingWeaponSelect && playerController.weaponController.weaponSelect == 0)) && playerController.data.arrInventory.Count > 0 && playerController.data.GetStamina() > 0f && !playerController.fsmUpperBody.GetBool("isMelee") && !playerController.isRMBDown)
		{
			playerPhoton.RpcExecAttackTriggered(GetIDX(), (byte)playerController.data.arrInventory[playerController.weaponController.idxWeaponMelee].Ammo, aimDirection);
		}
	}

	public void ExecThrowingTriggered(short aimDirection)
	{
		playerPhoton.RpcExecThrowingTriggered(GetIDX(), (byte)playerController.data.arrInventory[playerController.weaponController.idxWeaponRange].Ammo, aimDirection);
	}

	public void ExecReleaseAttack()
	{
		if (!GameManager.Instance.quitGame)
		{
			playerPhoton.RpcReleaseAttack(GetIDX());
		}
	}

	public void StopShoot()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			playerController.data.arrInventory[playerController.weaponController.idxWeaponRange].Ammo = 0;
		}
		else
		{
			playerPhoton.RpcStopShoot(GetIDX());
		}
	}

	public IEnumerator ShowResultScene()
	{
		GameManager.Instance.AStarPath.enabled = false;
		if ((bool)GameManagerPhoton.Instance)
		{
			GameManagerPhoton.Instance.RpcSyncTimer();
		}
		if (GameManager.Instance != null)
		{
			GameManager.Instance.waveManager.disabled = true;
			GameManager.Instance.gameOver = true;
		}
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			item.network.charControllerPhoton.enabled = false;
			item.reviveTimer.StopDuration();
		}
		float seconds = 1f;
		if (!GameManagerPhoton.Instance.IsWin)
		{
			seconds = 2f;
		}
		yield return new WaitForSeconds(seconds);
		UIGameManager.Instance.fadeBlack.DOFade(1f, 0.4f);
		yield return new WaitForSeconds(0.4f);
		if (GameManager.Instance != null)
		{
			GameManager.Instance.arrEnemyController.Clear();
			GameManager.Instance.gameManagerPhoton.showResult = true;
		}
		foreach (EnemyController item2 in GameManager.Instance.arrEnemyController)
		{
			if (item2 != null)
			{
				item2.isDestroyed = true;
				if (NetworkGameManager.Instance.isServer)
				{
					PhotonMultiplayerManager.Instance.DespawnObject(item2.gameObject);
				}
			}
		}
	}

	public void SetTriggerInteractableObject()
	{
		bool[] array = new bool[GameManager.Instance.arrItemInteractable.Count];
		bool[] array2 = new bool[GameManager.Instance.arrItemInteractable.Count];
		bool[] array3 = new bool[GameManager.Instance.arrItemInteractable.Count];
		short[] array4 = new short[20];
		int[] array5 = new int[20];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if (GameManager.Instance.arrItemInteractable[i].needItemToActivate)
			{
				array4[num] = (short)GameManager.Instance.arrItemInteractable[i].UniqueID;
				for (int j = 0; j < GameManager.Instance.arrItemInteractable[i].listItemToActivate.Count; j++)
				{
					array5[num] += Mathf.FloorToInt(Mathf.Pow(1000f, j) * (float)GameManager.Instance.arrItemInteractable[i].listItemToActivate[j]);
				}
				num++;
			}
			array[i] = GameManager.Instance.arrItemInteractable[i].triggerOnReverse;
			if (GameManager.Instance.arrItemInteractable[i].isTriggered)
			{
				array3[i] = GameManager.Instance.arrItemInteractable[i].isTriggered;
			}
			if ((bool)GameManager.Instance.arrItemInteractable[i].boxCollider)
			{
				array2[i] = GameManager.Instance.arrItemInteractable[i].boxCollider.enabled;
			}
			else
			{
				array2[i] = false;
			}
			if (GameManager.Instance.arrItemInteractable[i].isNeedSyncStateAnimator)
			{
				playerPhoton.RpcSetInteractableObject((short)GameManager.Instance.arrItemInteractable[i].UniqueID, array[i], array2[i], array3[i], GameManager.Instance.arrItemInteractable[i].animatorTrigger1.GetCurrentAnimatorStateInfo(0).shortNameHash);
			}
			else
			{
				playerPhoton.RpcSetInteractableObject((short)GameManager.Instance.arrItemInteractable[i].UniqueID, array[i], array2[i], array3[i]);
			}
		}
		for (int k = 0; k < num; k++)
		{
			if (k == num - 1)
			{
				playerPhoton.RpcSetInteractableObjectKeyItem(array4[k], array5[k], isLastItem: true);
			}
			else
			{
				playerPhoton.RpcSetInteractableObjectKeyItem(array4[k], array5[k]);
			}
		}
	}

	public void ExecSyncEventTrigger()
	{
		for (int i = 0; i < GameManager.Instance.arrEventTrigger.Count; i++)
		{
			if (GameManager.Instance.arrEventTrigger[i].UniqueID >= 0)
			{
				playerPhoton.RpcSyncEventTrigger((short)GameManager.Instance.arrEventTrigger[i].UniqueID, GameManager.Instance.arrEventTrigger[i].IsCollided);
			}
		}
	}

	public void ExecSyncPickableObject()
	{
		bool[] array = new bool[GameManager.Instance.arrItemPickable.Count];
		short[] array2 = new short[GameManager.Instance.arrItemPickable.Count];
		byte[] array3 = new byte[GameManager.Instance.arrItemPickable.Count];
		byte[] array4 = new byte[GameManager.Instance.arrItemPickable.Count];
		ulong[] array5 = new ulong[GameManager.Instance.arrItemPickable.Count];
		bool[] array6 = new bool[GameManager.Instance.arrItemPickable.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = GameManager.Instance.arrItemPickable[i].itemCollider.enabled;
			array2[i] = (short)GameManager.Instance.arrItemPickable[i].itemID;
			array3[i] = (byte)GameManager.Instance.arrItemPickable[i].uniqueID;
			if (GameManager.Instance.arrItemPickable[i].itemType == "Weapon")
			{
				array4[i] = (byte)GameManager.Instance.arrItemPickable[i].ammo;
			}
			else if (GameManager.Instance.arrItemPickable[i].durability > 0)
			{
				array4[i] = (byte)GameManager.Instance.arrItemPickable[i].durability;
			}
			else
			{
				array4[i] = GameManager.Instance.arrItemPickable[i].amount;
			}
			if ((bool)GameManager.Instance.arrItemPickable[i].rigidbody)
			{
				array5[i] = MathFunc.EncodeVector3ToULong(GameManager.Instance.arrItemPickable[i].rigidbody.transform.localPosition);
			}
			else
			{
				array5[i] = MathFunc.EncodeVector3ToULong(GameManager.Instance.arrItemPickable[i].transform.position);
			}
			if ((bool)GameManager.Instance.arrItemPickable[i].itemMap)
			{
				array6[i] = GameManager.Instance.arrItemPickable[i].itemMap.enabled;
			}
			bool flag = i == array.Length - 1;
			if (flag)
			{
				playerPhoton.RpcSyncPickableObject((byte)i, array[i], array2[i], array3[i], array4[i], array5[i], array6[i], flag);
			}
			else
			{
				playerPhoton.RpcSyncPickableObject((byte)i, array[i], array2[i], array3[i], array4[i], array5[i], array6[i]);
			}
		}
		foreach (int item in GameManagerPhoton.Instance.ListItemUIDLobbyPickedUp)
		{
			playerPhoton.RpcSyncLobbyObjectPickedUp((byte)item);
		}
	}

	public void ExecSyncInventory(bool isTargettedLocalPlayer = false, bool isToAllPlayer = false, bool isToHostOnly = false)
	{
		Debug.Log("SyncInventory");
		short[] array = new short[12];
		short[] array2 = new short[12];
		short[] array3 = new short[12];
		short[] array4 = new short[12];
		short[] array5 = new short[12];
		short[] array6 = new short[12];
		short[] array7 = new short[12];
		short[] array8 = new short[12];
		byte[] array9 = new byte[12];
		byte[] array10 = new byte[12];
		byte[] array11 = new byte[12];
		byte[] array12 = new byte[12];
		short[] array13 = new short[12];
		short[] array14 = new short[12];
		short[] array15 = new short[12];
		short[] array16 = new short[12];
		short num = -1;
		short num2 = -1;
		byte b = 0;
		short num3 = 0;
		for (int i = 0; i < 4; i++)
		{
			PlayerController player = NetworkGameManager.Instance.GetPlayer(i);
			if (!(player != null))
			{
				continue;
			}
			for (int j = 0; j < player.data.arrInventory.Count; j++)
			{
				if (player.data.arrInventory[j].ID <= 0 && j == 0)
				{
					player.data.ResetDefaultMelee();
				}
				num = (short)player.data.arrInventory[j].UniqueID;
				num2 = (short)player.data.arrInventory[j].ID;
				b = (byte)player.data.arrInventory[j].Amount;
				string itemType = player.data.arrInventory[j].ItemType;
				num3 = (short)player.data.arrInventory[j].Durability;
				if (itemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(num2) != null && BGDatabase_Weapon.GetEntityByKeyid(num2).WeaponType == "Range")
				{
					b = (byte)player.data.arrInventory[j].Ammo;
				}
				if (player.network.GetIDX() == 0)
				{
					array[j] = num;
					array5[j] = num2;
					array9[j] = b;
					array13[j] = num3;
				}
				if (player.network.GetIDX() == 1)
				{
					array2[j] = num;
					array6[j] = num2;
					array10[j] = b;
					array14[j] = num3;
				}
				if (player.network.GetIDX() == 2)
				{
					array3[j] = num;
					array7[j] = num2;
					array11[j] = b;
					array15[j] = num3;
				}
				if (player.network.GetIDX() == 3)
				{
					array4[j] = num;
					array8[j] = num2;
					array12[j] = b;
					array16[j] = num3;
				}
			}
		}
		if (isToHostOnly)
		{
			playerPhoton.RpcSyncInventoryToHost(isTargettedLocalPlayer, array, array5, array2, array6, array3, array7, array4, array8, array9, array10, array11, array12, array13, array14, array15, array16, isToAllPlayer);
		}
		else
		{
			playerPhoton.RpcSyncInventory(isTargettedLocalPlayer, array, array5, array2, array6, array3, array7, array4, array8, array9, array10, array11, array12, array13, array14, array15, array16, isToAllPlayer);
		}
	}

	public void ExecSyncInventoryLocalPlayerToAll()
	{
		Debug.Log("SyncInventoryLocalPlayerToAll");
		short[] array = new short[12];
		byte[] array2 = new byte[12];
		short[] array3 = new short[12];
		short num = -1;
		byte b = 0;
		short num2 = 0;
		if (playerController != null)
		{
			for (int i = 0; i < playerController.data.arrInventory.Count; i++)
			{
				num = (short)playerController.data.arrInventory[i].ID;
				b = (byte)playerController.data.arrInventory[i].Amount;
				string itemType = playerController.data.arrInventory[i].ItemType;
				num2 = (short)playerController.data.arrInventory[i].Durability;
				if (itemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(num) != null && BGDatabase_Weapon.GetEntityByKeyid(num).WeaponType == "Range")
				{
					b = (byte)playerController.data.arrInventory[i].Ammo;
				}
				array[i] = num;
				array2[i] = b;
				array3[i] = num2;
			}
		}
		playerPhoton.RpcSyncInventoryLocalToAll(array, array2, array3);
	}

	public void ExecSyncItemBox(PlayerController playerJoin, short timer, bool isForLocalPlayer = true)
	{
		short[] array = new short[playerJoin.ItemBoxController.arrItem.Count];
		byte[] array2 = new byte[playerJoin.ItemBoxController.arrItem.Count];
		short[] array3 = new short[playerJoin.ItemBoxController.arrItem.Count];
		for (int i = 0; i < playerJoin.ItemBoxController.arrItem.Count; i++)
		{
			array[i] = (short)playerJoin.ItemBoxController.arrItem[i].ID;
			array2[i] = (byte)playerJoin.ItemBoxController.arrItem[i].Amount;
			array3[i] = (short)playerJoin.ItemBoxController.arrItem[i].Durability;
			if (playerJoin.ItemBoxController.arrItem[i].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(array[i]).WeaponType == "Range")
			{
				array2[i] = (byte)playerJoin.ItemBoxController.arrItem[i].Ammo;
			}
		}
		playerJoin.network.playerPhoton.RpcSyncItemBox(array, array2, array3, timer, isForLocalPlayer);
	}

	public void ExecSyncMission()
	{
		for (int i = 0; i < GameManagerPhoton.Instance.ListMission.Count; i++)
		{
			byte missionID = (byte)GameManagerPhoton.Instance.ListMission[i].MissionID;
			byte missionIDByMap = (byte)GameManagerPhoton.Instance.ListMission[i].MissionIDByMap;
			bool isCleared = GameManagerPhoton.Instance.ListMission[i].IsCleared;
			bool isLocked = GameManagerPhoton.Instance.ListMission[i].IsLocked;
			bool isHide = GameManagerPhoton.Instance.ListMission[i].IsHide;
			byte objectiveID = (byte)GameManagerPhoton.Instance.ListMission[i].MissionObjective.ID;
			MapModifierStruct[] array = new MapModifierStruct[GameManagerPhoton.Instance.ListMission[i].ListModifier.Count];
			for (int j = 0; j < GameManagerPhoton.Instance.ListMission[i].ListModifier.Count; j++)
			{
				array[j].idMissionModifier = (byte)GameManagerPhoton.Instance.ListMission[i].ListModifier[j].ID;
			}
			byte spawnIdx = (byte)GameManagerPhoton.Instance.ListMission[i].PlayerSpawningIdx;
			bool flag = false;
			WeaponMapStruct[] array2 = new WeaponMapStruct[GameManagerPhoton.Instance.ListMission[i].ListWeapon.Count];
			for (int k = 0; k < GameManagerPhoton.Instance.ListMission[i].ListWeapon.Count; k++)
			{
				array2[k].Weapon = GameManagerPhoton.Instance.ListMission[i].ListWeapon[k].Weapon;
				array2[k].WeaponType = GameManagerPhoton.Instance.ListMission[i].ListWeapon[k].WeaponType;
			}
			int num = 0;
			if (GameManagerPhoton.Instance.CurrentMission == GameManagerPhoton.Instance.ListMission[i])
			{
				flag = true;
				num = GameManagerPhoton.Instance.CurrentMission.ListPossibleMapToUnlock.Count;
			}
			byte[] array3 = new byte[num];
			if (flag)
			{
				for (int l = 0; l < GameManagerPhoton.Instance.CurrentMission.ListPossibleMapToUnlock.Count; l++)
				{
					array3[l] = (byte)GameManagerPhoton.Instance.CurrentMission.ListPossibleMapToUnlock[l].MissionIDByMap;
				}
			}
			if (i == 0)
			{
				playerPhoton.RpcSyncMission(missionID, missionIDByMap, isCleared, isLocked, isHide, flag, array2, spawnIdx, objectiveID, array, array3, isLastMissionList: false, isFirstMissionList: true);
			}
			else if (i == GameManagerPhoton.Instance.ListMission.Count - 1)
			{
				playerPhoton.RpcSyncMission(missionID, missionIDByMap, isCleared, isLocked, isHide, flag, array2, spawnIdx, objectiveID, array, array3, isLastMissionList: true);
			}
			else
			{
				playerPhoton.RpcSyncMission(missionID, missionIDByMap, isCleared, isLocked, isHide, flag, array2, spawnIdx, objectiveID, array, array3);
			}
		}
	}

	public void ExecSyncModifier()
	{
		MapModifierStruct[] array = new MapModifierStruct[GameManagerPhoton.Instance.CurrentMission.ListModifier.Count];
		for (int i = 0; i < GameManagerPhoton.Instance.CurrentMission.ListModifier.Count; i++)
		{
			array[i].idMissionModifier = (byte)GameManagerPhoton.Instance.CurrentMission.ListModifier[i].ID;
		}
		playerPhoton.RpcSyncModifier(array);
	}

	public void ExecSyncMaxHealth()
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			playerPhoton.RpcSyncMaxHealthLocal(item.network.GetIDX(), item.data.GetMaxHealth());
		}
	}

	public void ExecSyncMaxStamina()
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			playerPhoton.RpcSyncMaxStaminaLocal(item.network.GetIDX(), item.data.GetMaxStamina());
		}
	}

	public void ExecSyncMap(PlayerController playerJoin)
	{
		for (int i = 0; i < GameManager.Instance.arrRoom.Count; i++)
		{
			byte stateRoom = 0;
			if (GameManager.Instance.arrRoom[i].isComplete)
			{
				stateRoom = 2;
			}
			else if (GameManager.Instance.arrRoom[i].isRevealed || GameManager.Instance.arrRoom[i].isRevealedByAllPlayer)
			{
				stateRoom = 1;
			}
			playerJoin.network.playerPhoton.RpcSyncRoom((byte)i, stateRoom);
		}
	}

	public void ExecAddRemoveItemBoxToServer(int idxItemBox, bool isRemove = false)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			return;
		}
		if (!isRemove)
		{
			short num = 0;
			short num2;
			byte amount;
			if (GameModes.Instance.isItemBoxGlobal)
			{
				num2 = (short)ItemBoxNetwork.instance.arrItem.Get(idxItemBox).ID;
				amount = (byte)ItemBoxNetwork.instance.arrItem.Get(idxItemBox).Amount;
				if (ItemBoxNetwork.instance.GetItemType(ItemBoxNetwork.instance.arrItem.Get(idxItemBox).ID) == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(num2).WeaponType == "Range")
				{
					amount = (byte)ItemBoxNetwork.instance.arrItem.Get(idxItemBox).Ammo;
				}
				num = (short)ItemBoxNetwork.instance.arrItem.Get(idxItemBox).Durability;
			}
			else
			{
				num2 = (short)playerController.ItemBoxController.arrItem[idxItemBox].ID;
				amount = (byte)playerController.ItemBoxController.arrItem[idxItemBox].Amount;
				if (playerController.ItemBoxController.arrItem[idxItemBox].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(num2).WeaponType == "Range")
				{
					amount = (byte)playerController.ItemBoxController.arrItem[idxItemBox].Ammo;
				}
				num = (short)playerController.ItemBoxController.arrItem[idxItemBox].Durability;
			}
			playerPhoton.RpcAddItemBox(num2, amount, num);
		}
		else
		{
			playerPhoton.RpcRemoveItemBox((byte)idxItemBox);
		}
	}

	public void SetEnableControl(bool value)
	{
		if (playerController.isEntangled)
		{
			return;
		}
		if (NetworkGameManager.Instance.isServer)
		{
			playerPhoton.enableControl = value;
			if (value)
			{
				playerPhoton.IsInteractingPuzzle = false;
			}
		}
		else
		{
			playerPhoton.RpcSetEnableControl(value);
			if (value)
			{
				playerPhoton.RpcSetInteractingPuzzle(value: false);
			}
		}
	}

	public void SetLife(byte newLife)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			playerPhoton.Life = newLife;
		}
		else
		{
			playerPhoton.RpcSetLife(newLife);
		}
	}

	public bool GetEnableControl()
	{
		bool result = false;
		if (!GameManager.Instance.quitGame)
		{
			result = playerPhoton.enableControl;
		}
		return result;
	}

	public void HitEnemy(byte idxEnemy, float value, byte animationType)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			EnemyController enemy = GameManager.Instance.GetEnemy(idxEnemy);
			enemy.network.AddSubHealth(0f - value);
			if (enemy.network.GetIsJumping() || enemy.network.networkPhoton.isMoveToJump)
			{
				return;
			}
			if (enemy.network.networkPhoton.isDeaf)
			{
				float maxDistance = MathFunc.Distance(enemy.colliderFOV.position, new Vector3(playerController.weaponPos.position.x, enemy.colliderFOV.position.y, playerController.weaponPos.position.z));
				Vector3 normalized = (enemy.colliderFOV.position - new Vector3(playerController.weaponPos.position.x, enemy.colliderFOV.position.y, playerController.weaponPos.position.z)).normalized;
				if (!Physics.Raycast(new Vector3(playerController.weaponPos.position.x, enemy.colliderFOV.position.y, playerController.weaponPos.position.z), normalized, maxDistance, GameManager.Instance.layerGrenade))
				{
					enemy.ChasingPlayer(playerController);
				}
			}
			else
			{
				enemy.ChasingPlayer(playerController);
			}
		}
		else
		{
			playerPhoton.RpcHitEnemy(idxEnemy, (short)value, animationType);
		}
	}

	public void ExecThrowPose()
	{
		playerController.fsmUpperBody.SetBool(playerController.IsThrowingAnim, value: true);
		playerPhoton.RpcThrowPose(GetIDX());
	}

	public void ExecCancelThrow()
	{
		playerController.fsmUpperBody.SetBool(playerController.IsThrowingAnim, value: false);
		playerController.SetAnimUpperSpeed(1f);
		playerController.isThrowing = false;
		playerPhoton.RpcCancelThrow(GetIDX());
	}

	public void ExecSyncItemMap(short uidItem)
	{
		playerPhoton.RpcSyncItemMap(uidItem);
	}

	public void ExecThrowGrenade(Vector3 posGrenade)
	{
		playerPhoton.RpcThrowGrenade(GetIDX(), posGrenade, (byte)playerController.data.idThrowable);
	}

	public void ExecGrenadeLauncher(Vector3 direction)
	{
		playerPhoton.RpcExecGrenadeLauncher(GetIDX(), direction);
	}

	public void ExecEnTangled(int idxEnemy, int angleAnim)
	{
		playerPhoton.RpcEnTangled(GetIDX(), (byte)idxEnemy, (short)angleAnim);
	}

	public void ExecReleaseEnTangled()
	{
		playerPhoton.RpcReleaseEnTangled();
	}

	public void StartSprint()
	{
		playerPhoton.RpcStartSprint(GetIDX());
	}

	public void StopSprint()
	{
		playerPhoton.RpcStopSprint(GetIDX());
	}

	public void SetUnlimitedStamina(bool isUnlimitedStamina)
	{
		playerPhoton.RpcTonicStamina(isUnlimitedStamina);
	}

	public byte GetIDX()
	{
		return playerIdx;
	}

	public string GetPlayerName()
	{
		return playerPhoton.playerName;
	}

	public string GetUserUniqueID()
	{
		return playerPhoton.userUniqueId;
	}

	public int GetDirection()
	{
		return Mathf.FloorToInt(playerPhoton.dataInputMove / 10);
	}

	public int GetAngleInputNetwork()
	{
		return playerPhoton.dataInputMove % 10 * 45;
	}

	public short GetIdWeapon0()
	{
		return playerPhoton.idWeapon0;
	}

	public short GetIdWeapon1()
	{
		return playerPhoton.idWeapon1;
	}

	public float GetHealth()
	{
		return (float)playerPhoton.health / 100f;
	}

	public byte GetLife()
	{
		return playerPhoton.Life;
	}

	public bool GetReadyLobby()
	{
		return GameManager.Instance.gameManagerPhoton.arrPlayerReady[GetIDX()];
	}

	public bool GetAFKPlayer()
	{
		return false;
	}

	public bool GetInGame()
	{
		return playerPhoton.inGame;
	}

	public byte GetIdxTargetCam()
	{
		return playerPhoton.targetIdxCam;
	}

	public void EquipWeaponInventory(int idxInventory)
	{
		playerPhoton.RPCEquipWeaponInventory((byte)idxInventory);
	}

	public void ExecSyncAmmoWeapon(int ammo)
	{
		playerPhoton.RPCSyncAmmoWeapon((byte)ammo);
	}

	public void ExecSyncAmmoWeaponInventory(int idxInventory, int ammo)
	{
		playerPhoton.RPCSyncAmmoWeaponInventory((byte)idxInventory, (byte)ammo);
	}

	public void ExecSubtractAmmo()
	{
		playerPhoton.RPCSubtractAmmoWeapon();
	}

	public void ExecSyncAmountInventory(int idxInventory, int amount)
	{
		playerPhoton.RPCSyncAmountInventory((byte)idxInventory, (byte)amount);
	}

	public void ExecSetAdditionalSpeed(float speed)
	{
		playerPhoton.RPCSetAdditionalSpeed(speed);
	}
}
