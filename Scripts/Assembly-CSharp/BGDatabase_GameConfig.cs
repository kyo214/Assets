using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_GameConfig : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_GameConfig(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_GameConfig(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldString _ufle12jhs77_Keys;

	private static BGFieldFloat _ufle12jhs77_DecSanity1ValuePerSec;

	private static BGFieldFloat _ufle12jhs77_DecSanity2ValuePerSec;

	private static BGFieldBool _ufle12jhs77_FriendlyFire;

	private static BGFieldFloat _ufle12jhs77_FriendlyFireDmgMultiply;

	private static BGFieldFloat _ufle12jhs77_TimerSpawnEnemy;

	private static BGFieldInt _ufle12jhs77_TimerEnemyRoaming;

	private static BGFieldInt _ufle12jhs77_TimerHorde;

	private static BGFieldString _ufle12jhs77_WaveType;

	private static BGFieldInt _ufle12jhs77_MaxEnemySpawnOnLevel;

	private static BGFieldInt _ufle12jhs77_TotEnemyOnGame;

	private static BGFieldFloat _ufle12jhs77_ReviveTime;

	private static BGFieldInt _ufle12jhs77_TotEnemyFirstWave;

	private static BGFieldInt _ufle12jhs77_TotEnemyFirstHorde;

	private static BGFieldInt _ufle12jhs77_TimerFirstWave;

	private static BGFieldFloat _ufle12jhs77_IncTotEnemyPerWave;

	private static BGFieldFloat _ufle12jhs77_IncHpEnemyPerWave;

	private static BGFieldFloat _ufle12jhs77_IncAttackEnemyPerWave;

	private static BGFieldFloat _ufle12jhs77_IncTimerSpawnPerWave;

	private static BGFieldFloat _ufle12jhs77_IncTimerWavePerWave;

	private static BGFieldFloat _ufle12jhs77_MultiplyPerPlayer;

	private static BGFieldBool _ufle12jhs77_EnemyAlwaysChasing;

	private static BGFieldInt _ufle12jhs77_ChancePercentDropAmmo;

	private static BGFieldBool _ufle12jhs77_WeaponInBackpack;

	private static BGFieldInt _ufle12jhs77_ChanceDropScraps;

	private static BGFieldInt _ufle12jhs77_ChanceDropGunPowder;

	private static BGFieldInt _ufle12jhs77_ChanceDropChemical;

	private static BGKey _edeht3sdad33_id;

	private static readonly Factory _factory0_PFS = new Factory();

	private static readonly BGDatabase_Character.Factory _factory1_PFS = new BGDatabase_Character.Factory();

	private static readonly BGDatabase_Enemy.Factory _factory2_PFS = new BGDatabase_Enemy.Factory();

	private static readonly BGDatabase_Weapon.Factory _factory3_PFS = new BGDatabase_Weapon.Factory();

	private static readonly BGDatabase_HealingItem.Factory _factory4_PFS = new BGDatabase_HealingItem.Factory();

	private static readonly BGDatabase_Item.Factory _factory5_PFS = new BGDatabase_Item.Factory();

	private static readonly BGDatabase_Ammunition.Factory _factory6_PFS = new BGDatabase_Ammunition.Factory();

	private static readonly BGDatabase_CraftMaterial.Factory _factory7_PFS = new BGDatabase_CraftMaterial.Factory();

	private static readonly BGDatabase_CraftRecipe.Factory _factory8_PFS = new BGDatabase_CraftRecipe.Factory();

	private static readonly BGDatabase_Wave.Factory _factory9_PFS = new BGDatabase_Wave.Factory();

	private static readonly BGDatabase_ItemConvert.Factory _factory10_PFS = new BGDatabase_ItemConvert.Factory();

	private static readonly BGDatabase_Skill.Factory _factory11_PFS = new BGDatabase_Skill.Factory();

	private static readonly BGDatabase_Perks.Factory _factory12_PFS = new BGDatabase_Perks.Factory();

	private static readonly BGDatabase_DefenseWave.Factory _factory13_PFS = new BGDatabase_DefenseWave.Factory();

	private static readonly BGDatabase_ItemDismantle.Factory _factory14_PFS = new BGDatabase_ItemDismantle.Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(5642178347097702877uL, 2300392337623708331uL), () =>
	{
		_metaDefault = null;
	}));

	public static BGRepoEvents Events => BGRepo.I.Events;

	public static int CountEntities => MetaDefault.CountEntities;

	public string name
	{
		get
		{
			return _name[base.Index];
		}
		set
		{
			_name[base.Index] = value;
		}
	}

	public string Keys
	{
		get
		{
			return _Keys[base.Index];
		}
		set
		{
			_Keys[base.Index] = value;
		}
	}

	public float DecSanity1ValuePerSec
	{
		get
		{
			return _DecSanity1ValuePerSec[base.Index];
		}
		set
		{
			_DecSanity1ValuePerSec[base.Index] = value;
		}
	}

	public float DecSanity2ValuePerSec
	{
		get
		{
			return _DecSanity2ValuePerSec[base.Index];
		}
		set
		{
			_DecSanity2ValuePerSec[base.Index] = value;
		}
	}

	public bool FriendlyFire
	{
		get
		{
			return _FriendlyFire[base.Index];
		}
		set
		{
			_FriendlyFire[base.Index] = value;
		}
	}

	public float FriendlyFireDmgMultiply
	{
		get
		{
			return _FriendlyFireDmgMultiply[base.Index];
		}
		set
		{
			_FriendlyFireDmgMultiply[base.Index] = value;
		}
	}

	public float TimerSpawnEnemy
	{
		get
		{
			return _TimerSpawnEnemy[base.Index];
		}
		set
		{
			_TimerSpawnEnemy[base.Index] = value;
		}
	}

	public int TimerEnemyRoaming
	{
		get
		{
			return _TimerEnemyRoaming[base.Index];
		}
		set
		{
			_TimerEnemyRoaming[base.Index] = value;
		}
	}

	public int TimerHorde
	{
		get
		{
			return _TimerHorde[base.Index];
		}
		set
		{
			_TimerHorde[base.Index] = value;
		}
	}

	public string WaveType
	{
		get
		{
			return _WaveType[base.Index];
		}
		set
		{
			_WaveType[base.Index] = value;
		}
	}

	public int MaxEnemySpawnOnLevel
	{
		get
		{
			return _MaxEnemySpawnOnLevel[base.Index];
		}
		set
		{
			_MaxEnemySpawnOnLevel[base.Index] = value;
		}
	}

	public int TotEnemyOnGame
	{
		get
		{
			return _TotEnemyOnGame[base.Index];
		}
		set
		{
			_TotEnemyOnGame[base.Index] = value;
		}
	}

	public float ReviveTime
	{
		get
		{
			return _ReviveTime[base.Index];
		}
		set
		{
			_ReviveTime[base.Index] = value;
		}
	}

	public int TotEnemyFirstWave
	{
		get
		{
			return _TotEnemyFirstWave[base.Index];
		}
		set
		{
			_TotEnemyFirstWave[base.Index] = value;
		}
	}

	public int TotEnemyFirstHorde
	{
		get
		{
			return _TotEnemyFirstHorde[base.Index];
		}
		set
		{
			_TotEnemyFirstHorde[base.Index] = value;
		}
	}

	public int TimerFirstWave
	{
		get
		{
			return _TimerFirstWave[base.Index];
		}
		set
		{
			_TimerFirstWave[base.Index] = value;
		}
	}

	public float IncTotEnemyPerWave
	{
		get
		{
			return _IncTotEnemyPerWave[base.Index];
		}
		set
		{
			_IncTotEnemyPerWave[base.Index] = value;
		}
	}

	public float IncHpEnemyPerWave
	{
		get
		{
			return _IncHpEnemyPerWave[base.Index];
		}
		set
		{
			_IncHpEnemyPerWave[base.Index] = value;
		}
	}

	public float IncAttackEnemyPerWave
	{
		get
		{
			return _IncAttackEnemyPerWave[base.Index];
		}
		set
		{
			_IncAttackEnemyPerWave[base.Index] = value;
		}
	}

	public float IncTimerSpawnPerWave
	{
		get
		{
			return _IncTimerSpawnPerWave[base.Index];
		}
		set
		{
			_IncTimerSpawnPerWave[base.Index] = value;
		}
	}

	public float IncTimerWavePerWave
	{
		get
		{
			return _IncTimerWavePerWave[base.Index];
		}
		set
		{
			_IncTimerWavePerWave[base.Index] = value;
		}
	}

	public float MultiplyPerPlayer
	{
		get
		{
			return _MultiplyPerPlayer[base.Index];
		}
		set
		{
			_MultiplyPerPlayer[base.Index] = value;
		}
	}

	public bool EnemyAlwaysChasing
	{
		get
		{
			return _EnemyAlwaysChasing[base.Index];
		}
		set
		{
			_EnemyAlwaysChasing[base.Index] = value;
		}
	}

	public int ChancePercentDropAmmo
	{
		get
		{
			return _ChancePercentDropAmmo[base.Index];
		}
		set
		{
			_ChancePercentDropAmmo[base.Index] = value;
		}
	}

	public bool WeaponInBackpack
	{
		get
		{
			return _WeaponInBackpack[base.Index];
		}
		set
		{
			_WeaponInBackpack[base.Index] = value;
		}
	}

	public int ChanceDropScraps
	{
		get
		{
			return _ChanceDropScraps[base.Index];
		}
		set
		{
			_ChanceDropScraps[base.Index] = value;
		}
	}

	public int ChanceDropGunPowder
	{
		get
		{
			return _ChanceDropGunPowder[base.Index];
		}
		set
		{
			_ChanceDropGunPowder[base.Index] = value;
		}
	}

	public int ChanceDropChemical
	{
		get
		{
			return _ChanceDropChemical[base.Index];
		}
		set
		{
			_ChanceDropChemical[base.Index] = value;
		}
	}

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(4626912653440325474uL, 12284522718951533448uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldString _Keys => _ufle12jhs77_Keys ?? (_ufle12jhs77_Keys = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5573210809675114035uL, 10114489210062048689uL), () =>
	{
		_ufle12jhs77_Keys = null;
	}));

	public static BGFieldFloat _DecSanity1ValuePerSec => _ufle12jhs77_DecSanity1ValuePerSec ?? (_ufle12jhs77_DecSanity1ValuePerSec = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5007593875009169426uL, 15732406955472884381uL), () =>
	{
		_ufle12jhs77_DecSanity1ValuePerSec = null;
	}));

	public static BGFieldFloat _DecSanity2ValuePerSec => _ufle12jhs77_DecSanity2ValuePerSec ?? (_ufle12jhs77_DecSanity2ValuePerSec = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5445212103975075262uL, 2882915510463707327uL), () =>
	{
		_ufle12jhs77_DecSanity2ValuePerSec = null;
	}));

	public static BGFieldBool _FriendlyFire => _ufle12jhs77_FriendlyFire ?? (_ufle12jhs77_FriendlyFire = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5488357224337708021uL, 9352460639925059750uL), () =>
	{
		_ufle12jhs77_FriendlyFire = null;
	}));

	public static BGFieldFloat _FriendlyFireDmgMultiply => _ufle12jhs77_FriendlyFireDmgMultiply ?? (_ufle12jhs77_FriendlyFireDmgMultiply = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5624897652471205094uL, 18126170941101348014uL), () =>
	{
		_ufle12jhs77_FriendlyFireDmgMultiply = null;
	}));

	public static BGFieldFloat _TimerSpawnEnemy => _ufle12jhs77_TimerSpawnEnemy ?? (_ufle12jhs77_TimerSpawnEnemy = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4810289208714757999uL, 8352682728768977326uL), () =>
	{
		_ufle12jhs77_TimerSpawnEnemy = null;
	}));

	public static BGFieldInt _TimerEnemyRoaming => _ufle12jhs77_TimerEnemyRoaming ?? (_ufle12jhs77_TimerEnemyRoaming = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4669463872076768643uL, 6223396108930112949uL), () =>
	{
		_ufle12jhs77_TimerEnemyRoaming = null;
	}));

	public static BGFieldInt _TimerHorde => _ufle12jhs77_TimerHorde ?? (_ufle12jhs77_TimerHorde = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4663153065328313718uL, 10554353460050647720uL), () =>
	{
		_ufle12jhs77_TimerHorde = null;
	}));

	public static BGFieldString _WaveType => _ufle12jhs77_WaveType ?? (_ufle12jhs77_WaveType = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5539023127832128673uL, 3356813879068006307uL), () =>
	{
		_ufle12jhs77_WaveType = null;
	}));

	public static BGFieldInt _MaxEnemySpawnOnLevel => _ufle12jhs77_MaxEnemySpawnOnLevel ?? (_ufle12jhs77_MaxEnemySpawnOnLevel = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4706190544055098703uL, 9843207955204512677uL), () =>
	{
		_ufle12jhs77_MaxEnemySpawnOnLevel = null;
	}));

	public static BGFieldInt _TotEnemyOnGame => _ufle12jhs77_TotEnemyOnGame ?? (_ufle12jhs77_TotEnemyOnGame = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5091233700669125132uL, 18426623652674411441uL), () =>
	{
		_ufle12jhs77_TotEnemyOnGame = null;
	}));

	public static BGFieldFloat _ReviveTime => _ufle12jhs77_ReviveTime ?? (_ufle12jhs77_ReviveTime = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4918415198909812241uL, 6985858441078352539uL), () =>
	{
		_ufle12jhs77_ReviveTime = null;
	}));

	public static BGFieldInt _TotEnemyFirstWave => _ufle12jhs77_TotEnemyFirstWave ?? (_ufle12jhs77_TotEnemyFirstWave = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5520448352585805372uL, 17580708066588962699uL), () =>
	{
		_ufle12jhs77_TotEnemyFirstWave = null;
	}));

	public static BGFieldInt _TotEnemyFirstHorde => _ufle12jhs77_TotEnemyFirstHorde ?? (_ufle12jhs77_TotEnemyFirstHorde = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4946444381760791039uL, 8884443893041507712uL), () =>
	{
		_ufle12jhs77_TotEnemyFirstHorde = null;
	}));

	public static BGFieldInt _TimerFirstWave => _ufle12jhs77_TimerFirstWave ?? (_ufle12jhs77_TimerFirstWave = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4844799136798964254uL, 13460048856991293869uL), () =>
	{
		_ufle12jhs77_TimerFirstWave = null;
	}));

	public static BGFieldFloat _IncTotEnemyPerWave => _ufle12jhs77_IncTotEnemyPerWave ?? (_ufle12jhs77_IncTotEnemyPerWave = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5624797780524600630uL, 14704085195747771050uL), () =>
	{
		_ufle12jhs77_IncTotEnemyPerWave = null;
	}));

	public static BGFieldFloat _IncHpEnemyPerWave => _ufle12jhs77_IncHpEnemyPerWave ?? (_ufle12jhs77_IncHpEnemyPerWave = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5546717694727411525uL, 1519553625841612678uL), () =>
	{
		_ufle12jhs77_IncHpEnemyPerWave = null;
	}));

	public static BGFieldFloat _IncAttackEnemyPerWave => _ufle12jhs77_IncAttackEnemyPerWave ?? (_ufle12jhs77_IncAttackEnemyPerWave = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5635627118745579486uL, 12725148659696371606uL), () =>
	{
		_ufle12jhs77_IncAttackEnemyPerWave = null;
	}));

	public static BGFieldFloat _IncTimerSpawnPerWave => _ufle12jhs77_IncTimerSpawnPerWave ?? (_ufle12jhs77_IncTimerSpawnPerWave = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5570262246810994948uL, 9560812223114908056uL), () =>
	{
		_ufle12jhs77_IncTimerSpawnPerWave = null;
	}));

	public static BGFieldFloat _IncTimerWavePerWave => _ufle12jhs77_IncTimerWavePerWave ?? (_ufle12jhs77_IncTimerWavePerWave = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5760701678806135110uL, 6437572282423473046uL), () =>
	{
		_ufle12jhs77_IncTimerWavePerWave = null;
	}));

	public static BGFieldFloat _MultiplyPerPlayer => _ufle12jhs77_MultiplyPerPlayer ?? (_ufle12jhs77_MultiplyPerPlayer = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4951015431756209352uL, 680940975704528007uL), () =>
	{
		_ufle12jhs77_MultiplyPerPlayer = null;
	}));

	public static BGFieldBool _EnemyAlwaysChasing => _ufle12jhs77_EnemyAlwaysChasing ?? (_ufle12jhs77_EnemyAlwaysChasing = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4969177678219230418uL, 14487377069550526598uL), () =>
	{
		_ufle12jhs77_EnemyAlwaysChasing = null;
	}));

	public static BGFieldInt _ChancePercentDropAmmo => _ufle12jhs77_ChancePercentDropAmmo ?? (_ufle12jhs77_ChancePercentDropAmmo = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5070390224831551393uL, 5680182349138674600uL), () =>
	{
		_ufle12jhs77_ChancePercentDropAmmo = null;
	}));

	public static BGFieldBool _WeaponInBackpack => _ufle12jhs77_WeaponInBackpack ?? (_ufle12jhs77_WeaponInBackpack = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4694260941001223723uL, 9212357836707030432uL), () =>
	{
		_ufle12jhs77_WeaponInBackpack = null;
	}));

	public static BGFieldInt _ChanceDropScraps => _ufle12jhs77_ChanceDropScraps ?? (_ufle12jhs77_ChanceDropScraps = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4669336428754164663uL, 877284886418353077uL), () =>
	{
		_ufle12jhs77_ChanceDropScraps = null;
	}));

	public static BGFieldInt _ChanceDropGunPowder => _ufle12jhs77_ChanceDropGunPowder ?? (_ufle12jhs77_ChanceDropGunPowder = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5764425027527016703uL, 11880129741182268604uL), () =>
	{
		_ufle12jhs77_ChanceDropGunPowder = null;
	}));

	public static BGFieldInt _ChanceDropChemical => _ufle12jhs77_ChanceDropChemical ?? (_ufle12jhs77_ChanceDropChemical = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5251972150215873651uL, 10179599510975537282uL), () =>
	{
		_ufle12jhs77_ChanceDropChemical = null;
	}));

	public static BGKey _id => _edeht3sdad33_id ?? (_edeht3sdad33_id = BGCodeGenUtils.GetKey(MetaDefault, new BGId(5486829803295187822uL, 16346694171887973030uL), () =>
	{
		_edeht3sdad33_id = null;
	}));

	private BGDatabase_GameConfig()
		: base(MetaDefault)
	{
	}

	private BGDatabase_GameConfig(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_GameConfig(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_GameConfig(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_GameConfig FindEntity(Predicate<BGDatabase_GameConfig> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_GameConfig> FindEntities(Predicate<BGDatabase_GameConfig> filter, List<BGDatabase_GameConfig> result = null, Comparison<BGDatabase_GameConfig> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_GameConfig> action, Predicate<BGDatabase_GameConfig> filter = null, Comparison<BGDatabase_GameConfig> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_GameConfig GetEntity(BGId entityId)
	{
		return (BGDatabase_GameConfig)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_GameConfig GetEntity(int index)
	{
		return (BGDatabase_GameConfig)MetaDefault[index];
	}

	public static BGDatabase_GameConfig GetEntity(string entityName)
	{
		return (BGDatabase_GameConfig)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_GameConfig NewEntity()
	{
		return (BGDatabase_GameConfig)MetaDefault.NewEntity();
	}

	public static BGDatabase_GameConfig NewEntity(BGId entityId)
	{
		return (BGDatabase_GameConfig)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_GameConfig NewEntity(Action<BGDatabase_GameConfig> callback)
	{
		return (BGDatabase_GameConfig)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_GameConfig)entity);
		}));
	}

	public static BGDatabase_GameConfig GetEntityByKeyid(string Keys)
	{
		return (BGDatabase_GameConfig)_id.GetEntityByKey(Keys);
	}
}
