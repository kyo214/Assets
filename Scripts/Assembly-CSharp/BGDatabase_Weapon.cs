using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_Weapon : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_Weapon(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_Weapon(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldInt _ufle12jhs77_Keys;

	private static BGFieldString _ufle12jhs77_Name;

	private static BGFieldString _ufle12jhs77_WeaponType;

	private static BGFieldString _ufle12jhs77_Type;

	private static BGFieldString _ufle12jhs77_AttackType;

	private static BGFieldFloat _ufle12jhs77_floatRange;

	private static BGFieldFloat _ufle12jhs77_MaxRangeAccuracy;

	private static BGFieldFloat _ufle12jhs77_MinRangeAccuracy;

	private static BGFieldInt _ufle12jhs77_Durability;

	private static BGFieldFloat _ufle12jhs77_Damage;

	private static BGFieldInt _ufle12jhs77_NeedStamina;

	private static BGFieldInt _ufle12jhs77_AmmoTypeID;

	private static BGFieldInt _ufle12jhs77_MagazineSize;

	private static BGFieldFloat _ufle12jhs77_ReloadTime;

	private static BGFieldBool _ufle12jhs77_ReloadPerAmmo;

	private static BGFieldInt _ufle12jhs77_MaxItemInSlot;

	private static BGFieldBool _ufle12jhs77_IsAutoFire;

	private static BGFieldFloat _ufle12jhs77_AttackInterval;

	private static BGFieldInt _ufle12jhs77_ShotsPerAttack;

	private static BGFieldInt _ufle12jhs77_ShellsPerShot;

	private static BGFieldInt _ufle12jhs77_BulletPerShell;

	private static BGFieldFloat _ufle12jhs77_RadiusBulletSpread;

	private static BGFieldFloat _ufle12jhs77_BulletSize;

	private static BGFieldFloat _ufle12jhs77_ImpactAoESize;

	private static BGFieldBool _ufle12jhs77_KnockbackAnimTrigger;

	private static BGFieldBool _ufle12jhs77_HitAnimTrigger;

	private static BGFieldInt _ufle12jhs77_DeadEnemyType;

	private static BGFieldBool _ufle12jhs77_isPumpAction;

	private static BGFieldFloat _ufle12jhs77_StuntTime;

	private static BGFieldFloat _ufle12jhs77_DurShake;

	private static BGFieldFloat _ufle12jhs77_AmplitudeShake;

	private static BGFieldFloat _ufle12jhs77_AimSpeed;

	private static BGFieldFloat _ufle12jhs77_TimeReleaseAttack;

	private static BGFieldBool _ufle12jhs77_OneHitKnockback;

	private static BGFieldInt _ufle12jhs77_MaxHitEnemy;

	private static BGFieldListString _ufle12jhs77_CraftRecipe;

	private static BGFieldInt _ufle12jhs77_BaseWeaponID;

	private static BGFieldListString _ufle12jhs77_Buff;

	private static BGFieldBool _ufle12jhs77_IsSpecialWeapon;

	private static BGFieldInt _ufle12jhs77_MaxHitEnemyHalfCharge;

	private static BGFieldInt _ufle12jhs77_MaxHitEnemyFullCharge;

	private static BGFieldBool _ufle12jhs77_IsDisableHalfCharge;

	private static BGFieldString _ufle12jhs77_SpecialFullCharge;

	private static BGFieldInt _ufle12jhs77_DamageHalfCharge;

	private static BGFieldInt _ufle12jhs77_DamageFullCharge;

	private static BGFieldFloat _ufle12jhs77_ReleaseAttackAnimSpeed;

	private static BGFieldBool _ufle12jhs77_HeadOff;

	private static BGFieldBool _ufle12jhs77_IsTrainingWeapon;

	private static BGFieldBool _ufle12jhs77_DashBasicAttack;

	private static BGFieldBool _ufle12jhs77_isNotUsingGunPowder;

	private static BGKey _edeht3sdad33_id;

	private static readonly BGDatabase_GameConfig.Factory _factory0_PFS = new BGDatabase_GameConfig.Factory();

	private static readonly BGDatabase_Character.Factory _factory1_PFS = new BGDatabase_Character.Factory();

	private static readonly BGDatabase_Enemy.Factory _factory2_PFS = new BGDatabase_Enemy.Factory();

	private static readonly Factory _factory3_PFS = new Factory();

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

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(4890544861658418382uL, 14046862991279202435uL), () =>
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

	public int Keys
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

	public new string Name
	{
		get
		{
			return _Name[base.Index];
		}
		set
		{
			_Name[base.Index] = value;
		}
	}

	public string WeaponType
	{
		get
		{
			return _WeaponType[base.Index];
		}
		set
		{
			_WeaponType[base.Index] = value;
		}
	}

	public string Type
	{
		get
		{
			return _Type[base.Index];
		}
		set
		{
			_Type[base.Index] = value;
		}
	}

	public string AttackType
	{
		get
		{
			return _AttackType[base.Index];
		}
		set
		{
			_AttackType[base.Index] = value;
		}
	}

	public float floatRange
	{
		get
		{
			return _floatRange[base.Index];
		}
		set
		{
			_floatRange[base.Index] = value;
		}
	}

	public float MaxRangeAccuracy
	{
		get
		{
			return _MaxRangeAccuracy[base.Index];
		}
		set
		{
			_MaxRangeAccuracy[base.Index] = value;
		}
	}

	public float MinRangeAccuracy
	{
		get
		{
			return _MinRangeAccuracy[base.Index];
		}
		set
		{
			_MinRangeAccuracy[base.Index] = value;
		}
	}

	public int Durability
	{
		get
		{
			return _Durability[base.Index];
		}
		set
		{
			_Durability[base.Index] = value;
		}
	}

	public float Damage
	{
		get
		{
			return _Damage[base.Index];
		}
		set
		{
			_Damage[base.Index] = value;
		}
	}

	public int NeedStamina
	{
		get
		{
			return _NeedStamina[base.Index];
		}
		set
		{
			_NeedStamina[base.Index] = value;
		}
	}

	public int AmmoTypeID
	{
		get
		{
			return _AmmoTypeID[base.Index];
		}
		set
		{
			_AmmoTypeID[base.Index] = value;
		}
	}

	public int MagazineSize
	{
		get
		{
			return _MagazineSize[base.Index];
		}
		set
		{
			_MagazineSize[base.Index] = value;
		}
	}

	public float ReloadTime
	{
		get
		{
			return _ReloadTime[base.Index];
		}
		set
		{
			_ReloadTime[base.Index] = value;
		}
	}

	public bool ReloadPerAmmo
	{
		get
		{
			return _ReloadPerAmmo[base.Index];
		}
		set
		{
			_ReloadPerAmmo[base.Index] = value;
		}
	}

	public int MaxItemInSlot
	{
		get
		{
			return _MaxItemInSlot[base.Index];
		}
		set
		{
			_MaxItemInSlot[base.Index] = value;
		}
	}

	public bool IsAutoFire
	{
		get
		{
			return _IsAutoFire[base.Index];
		}
		set
		{
			_IsAutoFire[base.Index] = value;
		}
	}

	public float AttackInterval
	{
		get
		{
			return _AttackInterval[base.Index];
		}
		set
		{
			_AttackInterval[base.Index] = value;
		}
	}

	public int ShotsPerAttack
	{
		get
		{
			return _ShotsPerAttack[base.Index];
		}
		set
		{
			_ShotsPerAttack[base.Index] = value;
		}
	}

	public int ShellsPerShot
	{
		get
		{
			return _ShellsPerShot[base.Index];
		}
		set
		{
			_ShellsPerShot[base.Index] = value;
		}
	}

	public int BulletPerShell
	{
		get
		{
			return _BulletPerShell[base.Index];
		}
		set
		{
			_BulletPerShell[base.Index] = value;
		}
	}

	public float RadiusBulletSpread
	{
		get
		{
			return _RadiusBulletSpread[base.Index];
		}
		set
		{
			_RadiusBulletSpread[base.Index] = value;
		}
	}

	public float BulletSize
	{
		get
		{
			return _BulletSize[base.Index];
		}
		set
		{
			_BulletSize[base.Index] = value;
		}
	}

	public float ImpactAoESize
	{
		get
		{
			return _ImpactAoESize[base.Index];
		}
		set
		{
			_ImpactAoESize[base.Index] = value;
		}
	}

	public bool KnockbackAnimTrigger
	{
		get
		{
			return _KnockbackAnimTrigger[base.Index];
		}
		set
		{
			_KnockbackAnimTrigger[base.Index] = value;
		}
	}

	public bool HitAnimTrigger
	{
		get
		{
			return _HitAnimTrigger[base.Index];
		}
		set
		{
			_HitAnimTrigger[base.Index] = value;
		}
	}

	public int DeadEnemyType
	{
		get
		{
			return _DeadEnemyType[base.Index];
		}
		set
		{
			_DeadEnemyType[base.Index] = value;
		}
	}

	public bool isPumpAction
	{
		get
		{
			return _isPumpAction[base.Index];
		}
		set
		{
			_isPumpAction[base.Index] = value;
		}
	}

	public float StuntTime
	{
		get
		{
			return _StuntTime[base.Index];
		}
		set
		{
			_StuntTime[base.Index] = value;
		}
	}

	public float DurShake
	{
		get
		{
			return _DurShake[base.Index];
		}
		set
		{
			_DurShake[base.Index] = value;
		}
	}

	public float AmplitudeShake
	{
		get
		{
			return _AmplitudeShake[base.Index];
		}
		set
		{
			_AmplitudeShake[base.Index] = value;
		}
	}

	public float AimSpeed
	{
		get
		{
			return _AimSpeed[base.Index];
		}
		set
		{
			_AimSpeed[base.Index] = value;
		}
	}

	public float TimeReleaseAttack
	{
		get
		{
			return _TimeReleaseAttack[base.Index];
		}
		set
		{
			_TimeReleaseAttack[base.Index] = value;
		}
	}

	public bool OneHitKnockback
	{
		get
		{
			return _OneHitKnockback[base.Index];
		}
		set
		{
			_OneHitKnockback[base.Index] = value;
		}
	}

	public int MaxHitEnemy
	{
		get
		{
			return _MaxHitEnemy[base.Index];
		}
		set
		{
			_MaxHitEnemy[base.Index] = value;
		}
	}

	public List<string> CraftRecipe
	{
		get
		{
			return _CraftRecipe[base.Index];
		}
		set
		{
			_CraftRecipe[base.Index] = value;
		}
	}

	public int BaseWeaponID
	{
		get
		{
			return _BaseWeaponID[base.Index];
		}
		set
		{
			_BaseWeaponID[base.Index] = value;
		}
	}

	public List<string> Buff
	{
		get
		{
			return _Buff[base.Index];
		}
		set
		{
			_Buff[base.Index] = value;
		}
	}

	public bool IsSpecialWeapon
	{
		get
		{
			return _IsSpecialWeapon[base.Index];
		}
		set
		{
			_IsSpecialWeapon[base.Index] = value;
		}
	}

	public int MaxHitEnemyHalfCharge
	{
		get
		{
			return _MaxHitEnemyHalfCharge[base.Index];
		}
		set
		{
			_MaxHitEnemyHalfCharge[base.Index] = value;
		}
	}

	public int MaxHitEnemyFullCharge
	{
		get
		{
			return _MaxHitEnemyFullCharge[base.Index];
		}
		set
		{
			_MaxHitEnemyFullCharge[base.Index] = value;
		}
	}

	public bool IsDisableHalfCharge
	{
		get
		{
			return _IsDisableHalfCharge[base.Index];
		}
		set
		{
			_IsDisableHalfCharge[base.Index] = value;
		}
	}

	public string SpecialFullCharge
	{
		get
		{
			return _SpecialFullCharge[base.Index];
		}
		set
		{
			_SpecialFullCharge[base.Index] = value;
		}
	}

	public int DamageHalfCharge
	{
		get
		{
			return _DamageHalfCharge[base.Index];
		}
		set
		{
			_DamageHalfCharge[base.Index] = value;
		}
	}

	public int DamageFullCharge
	{
		get
		{
			return _DamageFullCharge[base.Index];
		}
		set
		{
			_DamageFullCharge[base.Index] = value;
		}
	}

	public float ReleaseAttackAnimSpeed
	{
		get
		{
			return _ReleaseAttackAnimSpeed[base.Index];
		}
		set
		{
			_ReleaseAttackAnimSpeed[base.Index] = value;
		}
	}

	public bool HeadOff
	{
		get
		{
			return _HeadOff[base.Index];
		}
		set
		{
			_HeadOff[base.Index] = value;
		}
	}

	public bool IsTrainingWeapon
	{
		get
		{
			return _IsTrainingWeapon[base.Index];
		}
		set
		{
			_IsTrainingWeapon[base.Index] = value;
		}
	}

	public bool DashBasicAttack
	{
		get
		{
			return _DashBasicAttack[base.Index];
		}
		set
		{
			_DashBasicAttack[base.Index] = value;
		}
	}

	public bool isNotUsingGunPowder
	{
		get
		{
			return _isNotUsingGunPowder[base.Index];
		}
		set
		{
			_isNotUsingGunPowder[base.Index] = value;
		}
	}

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5646577115052676290uL, 13340689542815402632uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldInt _Keys => _ufle12jhs77_Keys ?? (_ufle12jhs77_Keys = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5502054191967280154uL, 8323880852668785326uL), () =>
	{
		_ufle12jhs77_Keys = null;
	}));

	public static BGFieldString _Name => _ufle12jhs77_Name ?? (_ufle12jhs77_Name = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5284411814938075096uL, 3972926009216295559uL), () =>
	{
		_ufle12jhs77_Name = null;
	}));

	public static BGFieldString _WeaponType => _ufle12jhs77_WeaponType ?? (_ufle12jhs77_WeaponType = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4715675739707394851uL, 12305998655973473181uL), () =>
	{
		_ufle12jhs77_WeaponType = null;
	}));

	public static BGFieldString _Type => _ufle12jhs77_Type ?? (_ufle12jhs77_Type = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4836041553209695969uL, 9027130471581569166uL), () =>
	{
		_ufle12jhs77_Type = null;
	}));

	public static BGFieldString _AttackType => _ufle12jhs77_AttackType ?? (_ufle12jhs77_AttackType = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4849693020035006273uL, 2033073081267366544uL), () =>
	{
		_ufle12jhs77_AttackType = null;
	}));

	public static BGFieldFloat _floatRange => _ufle12jhs77_floatRange ?? (_ufle12jhs77_floatRange = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5182670796167986958uL, 3989701781978294950uL), () =>
	{
		_ufle12jhs77_floatRange = null;
	}));

	public static BGFieldFloat _MaxRangeAccuracy => _ufle12jhs77_MaxRangeAccuracy ?? (_ufle12jhs77_MaxRangeAccuracy = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4915839076683699370uL, 12870704474276725931uL), () =>
	{
		_ufle12jhs77_MaxRangeAccuracy = null;
	}));

	public static BGFieldFloat _MinRangeAccuracy => _ufle12jhs77_MinRangeAccuracy ?? (_ufle12jhs77_MinRangeAccuracy = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4703682165419227840uL, 13188928496025615518uL), () =>
	{
		_ufle12jhs77_MinRangeAccuracy = null;
	}));

	public static BGFieldInt _Durability => _ufle12jhs77_Durability ?? (_ufle12jhs77_Durability = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5008717763955774940uL, 5456338210424810430uL), () =>
	{
		_ufle12jhs77_Durability = null;
	}));

	public static BGFieldFloat _Damage => _ufle12jhs77_Damage ?? (_ufle12jhs77_Damage = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5221873015147578128uL, 5977443179731199362uL), () =>
	{
		_ufle12jhs77_Damage = null;
	}));

	public static BGFieldInt _NeedStamina => _ufle12jhs77_NeedStamina ?? (_ufle12jhs77_NeedStamina = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5498314555590784038uL, 5856272016451398814uL), () =>
	{
		_ufle12jhs77_NeedStamina = null;
	}));

	public static BGFieldInt _AmmoTypeID => _ufle12jhs77_AmmoTypeID ?? (_ufle12jhs77_AmmoTypeID = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5762190496340736829uL, 9443807547773825664uL), () =>
	{
		_ufle12jhs77_AmmoTypeID = null;
	}));

	public static BGFieldInt _MagazineSize => _ufle12jhs77_MagazineSize ?? (_ufle12jhs77_MagazineSize = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5681819827884922923uL, 7336599063843846577uL), () =>
	{
		_ufle12jhs77_MagazineSize = null;
	}));

	public static BGFieldFloat _ReloadTime => _ufle12jhs77_ReloadTime ?? (_ufle12jhs77_ReloadTime = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4902870626773747766uL, 6649833150796906393uL), () =>
	{
		_ufle12jhs77_ReloadTime = null;
	}));

	public static BGFieldBool _ReloadPerAmmo => _ufle12jhs77_ReloadPerAmmo ?? (_ufle12jhs77_ReloadPerAmmo = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5682888171981594686uL, 11554133718533046961uL), () =>
	{
		_ufle12jhs77_ReloadPerAmmo = null;
	}));

	public static BGFieldInt _MaxItemInSlot => _ufle12jhs77_MaxItemInSlot ?? (_ufle12jhs77_MaxItemInSlot = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4647948823702784884uL, 8474163153205184420uL), () =>
	{
		_ufle12jhs77_MaxItemInSlot = null;
	}));

	public static BGFieldBool _IsAutoFire => _ufle12jhs77_IsAutoFire ?? (_ufle12jhs77_IsAutoFire = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4977001086953134053uL, 1856024201047209914uL), () =>
	{
		_ufle12jhs77_IsAutoFire = null;
	}));

	public static BGFieldFloat _AttackInterval => _ufle12jhs77_AttackInterval ?? (_ufle12jhs77_AttackInterval = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4756547611702414999uL, 3201994194001797270uL), () =>
	{
		_ufle12jhs77_AttackInterval = null;
	}));

	public static BGFieldInt _ShotsPerAttack => _ufle12jhs77_ShotsPerAttack ?? (_ufle12jhs77_ShotsPerAttack = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4859842520659399667uL, 450477176666217905uL), () =>
	{
		_ufle12jhs77_ShotsPerAttack = null;
	}));

	public static BGFieldInt _ShellsPerShot => _ufle12jhs77_ShellsPerShot ?? (_ufle12jhs77_ShellsPerShot = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5702022215429993823uL, 2835919077923629961uL), () =>
	{
		_ufle12jhs77_ShellsPerShot = null;
	}));

	public static BGFieldInt _BulletPerShell => _ufle12jhs77_BulletPerShell ?? (_ufle12jhs77_BulletPerShell = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4703888785985454887uL, 6796555299883504788uL), () =>
	{
		_ufle12jhs77_BulletPerShell = null;
	}));

	public static BGFieldFloat _RadiusBulletSpread => _ufle12jhs77_RadiusBulletSpread ?? (_ufle12jhs77_RadiusBulletSpread = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4641374222074350057uL, 10892874542914387388uL), () =>
	{
		_ufle12jhs77_RadiusBulletSpread = null;
	}));

	public static BGFieldFloat _BulletSize => _ufle12jhs77_BulletSize ?? (_ufle12jhs77_BulletSize = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5271847946873618986uL, 15531158959639041465uL), () =>
	{
		_ufle12jhs77_BulletSize = null;
	}));

	public static BGFieldFloat _ImpactAoESize => _ufle12jhs77_ImpactAoESize ?? (_ufle12jhs77_ImpactAoESize = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4861427180449953484uL, 2062785211439066551uL), () =>
	{
		_ufle12jhs77_ImpactAoESize = null;
	}));

	public static BGFieldBool _KnockbackAnimTrigger => _ufle12jhs77_KnockbackAnimTrigger ?? (_ufle12jhs77_KnockbackAnimTrigger = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5315876166386862709uL, 7002337220533058713uL), () =>
	{
		_ufle12jhs77_KnockbackAnimTrigger = null;
	}));

	public static BGFieldBool _HitAnimTrigger => _ufle12jhs77_HitAnimTrigger ?? (_ufle12jhs77_HitAnimTrigger = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4715619869106091292uL, 16685601827097572255uL), () =>
	{
		_ufle12jhs77_HitAnimTrigger = null;
	}));

	public static BGFieldInt _DeadEnemyType => _ufle12jhs77_DeadEnemyType ?? (_ufle12jhs77_DeadEnemyType = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5176880328966876627uL, 2321435922134427571uL), () =>
	{
		_ufle12jhs77_DeadEnemyType = null;
	}));

	public static BGFieldBool _isPumpAction => _ufle12jhs77_isPumpAction ?? (_ufle12jhs77_isPumpAction = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4884612162320458489uL, 11138979620499841464uL), () =>
	{
		_ufle12jhs77_isPumpAction = null;
	}));

	public static BGFieldFloat _StuntTime => _ufle12jhs77_StuntTime ?? (_ufle12jhs77_StuntTime = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5074630961691036503uL, 4494350985057535666uL), () =>
	{
		_ufle12jhs77_StuntTime = null;
	}));

	public static BGFieldFloat _DurShake => _ufle12jhs77_DurShake ?? (_ufle12jhs77_DurShake = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5668445818537151459uL, 17763444889281319356uL), () =>
	{
		_ufle12jhs77_DurShake = null;
	}));

	public static BGFieldFloat _AmplitudeShake => _ufle12jhs77_AmplitudeShake ?? (_ufle12jhs77_AmplitudeShake = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4805468316822122416uL, 1731392778667412642uL), () =>
	{
		_ufle12jhs77_AmplitudeShake = null;
	}));

	public static BGFieldFloat _AimSpeed => _ufle12jhs77_AimSpeed ?? (_ufle12jhs77_AimSpeed = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5313206915935255148uL, 11349447588223362954uL), () =>
	{
		_ufle12jhs77_AimSpeed = null;
	}));

	public static BGFieldFloat _TimeReleaseAttack => _ufle12jhs77_TimeReleaseAttack ?? (_ufle12jhs77_TimeReleaseAttack = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4767904164053722228uL, 1333683379996444038uL), () =>
	{
		_ufle12jhs77_TimeReleaseAttack = null;
	}));

	public static BGFieldBool _OneHitKnockback => _ufle12jhs77_OneHitKnockback ?? (_ufle12jhs77_OneHitKnockback = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4914346586932637014uL, 18040322164490668939uL), () =>
	{
		_ufle12jhs77_OneHitKnockback = null;
	}));

	public static BGFieldInt _MaxHitEnemy => _ufle12jhs77_MaxHitEnemy ?? (_ufle12jhs77_MaxHitEnemy = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5763775904314228388uL, 15312570932520122500uL), () =>
	{
		_ufle12jhs77_MaxHitEnemy = null;
	}));

	public static BGFieldListString _CraftRecipe => _ufle12jhs77_CraftRecipe ?? (_ufle12jhs77_CraftRecipe = BGCodeGenUtils.GetField<BGFieldListString>(MetaDefault, new BGId(5095130978926782689uL, 14381674517381715885uL), () =>
	{
		_ufle12jhs77_CraftRecipe = null;
	}));

	public static BGFieldInt _BaseWeaponID => _ufle12jhs77_BaseWeaponID ?? (_ufle12jhs77_BaseWeaponID = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4800858518882701321uL, 13674717789564633227uL), () =>
	{
		_ufle12jhs77_BaseWeaponID = null;
	}));

	public static BGFieldListString _Buff => _ufle12jhs77_Buff ?? (_ufle12jhs77_Buff = BGCodeGenUtils.GetField<BGFieldListString>(MetaDefault, new BGId(5623079902283432791uL, 15847737648151702460uL), () =>
	{
		_ufle12jhs77_Buff = null;
	}));

	public static BGFieldBool _IsSpecialWeapon => _ufle12jhs77_IsSpecialWeapon ?? (_ufle12jhs77_IsSpecialWeapon = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5493511343106941012uL, 16360889966157607344uL), () =>
	{
		_ufle12jhs77_IsSpecialWeapon = null;
	}));

	public static BGFieldInt _MaxHitEnemyHalfCharge => _ufle12jhs77_MaxHitEnemyHalfCharge ?? (_ufle12jhs77_MaxHitEnemyHalfCharge = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5545464061429147136uL, 7334376613984817844uL), () =>
	{
		_ufle12jhs77_MaxHitEnemyHalfCharge = null;
	}));

	public static BGFieldInt _MaxHitEnemyFullCharge => _ufle12jhs77_MaxHitEnemyFullCharge ?? (_ufle12jhs77_MaxHitEnemyFullCharge = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4826879565165517331uL, 1172020673403405503uL), () =>
	{
		_ufle12jhs77_MaxHitEnemyFullCharge = null;
	}));

	public static BGFieldBool _IsDisableHalfCharge => _ufle12jhs77_IsDisableHalfCharge ?? (_ufle12jhs77_IsDisableHalfCharge = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5380102026109850068uL, 10821249401462373005uL), () =>
	{
		_ufle12jhs77_IsDisableHalfCharge = null;
	}));

	public static BGFieldString _SpecialFullCharge => _ufle12jhs77_SpecialFullCharge ?? (_ufle12jhs77_SpecialFullCharge = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4767813545144550371uL, 14106201470076776350uL), () =>
	{
		_ufle12jhs77_SpecialFullCharge = null;
	}));

	public static BGFieldInt _DamageHalfCharge => _ufle12jhs77_DamageHalfCharge ?? (_ufle12jhs77_DamageHalfCharge = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4834554704587085548uL, 7139288250058003366uL), () =>
	{
		_ufle12jhs77_DamageHalfCharge = null;
	}));

	public static BGFieldInt _DamageFullCharge => _ufle12jhs77_DamageFullCharge ?? (_ufle12jhs77_DamageFullCharge = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5554741742127369238uL, 14110735255664575375uL), () =>
	{
		_ufle12jhs77_DamageFullCharge = null;
	}));

	public static BGFieldFloat _ReleaseAttackAnimSpeed => _ufle12jhs77_ReleaseAttackAnimSpeed ?? (_ufle12jhs77_ReleaseAttackAnimSpeed = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5462195031443977134uL, 9037874653240698524uL), () =>
	{
		_ufle12jhs77_ReleaseAttackAnimSpeed = null;
	}));

	public static BGFieldBool _HeadOff => _ufle12jhs77_HeadOff ?? (_ufle12jhs77_HeadOff = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5100089143909585235uL, 14834246693389886130uL), () =>
	{
		_ufle12jhs77_HeadOff = null;
	}));

	public static BGFieldBool _IsTrainingWeapon => _ufle12jhs77_IsTrainingWeapon ?? (_ufle12jhs77_IsTrainingWeapon = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5516500495671645486uL, 17921751516194905270uL), () =>
	{
		_ufle12jhs77_IsTrainingWeapon = null;
	}));

	public static BGFieldBool _DashBasicAttack => _ufle12jhs77_DashBasicAttack ?? (_ufle12jhs77_DashBasicAttack = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5211860027687949112uL, 8911676921274788243uL), () =>
	{
		_ufle12jhs77_DashBasicAttack = null;
	}));

	public static BGFieldBool _isNotUsingGunPowder => _ufle12jhs77_isNotUsingGunPowder ?? (_ufle12jhs77_isNotUsingGunPowder = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5294424198037400930uL, 1159167244515698863uL), () =>
	{
		_ufle12jhs77_isNotUsingGunPowder = null;
	}));

	public static BGKey _id => _edeht3sdad33_id ?? (_edeht3sdad33_id = BGCodeGenUtils.GetKey(MetaDefault, new BGId(5618802727430380995uL, 12248221220620586116uL), () =>
	{
		_edeht3sdad33_id = null;
	}));

	private BGDatabase_Weapon()
		: base(MetaDefault)
	{
	}

	private BGDatabase_Weapon(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_Weapon(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_Weapon(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_Weapon FindEntity(Predicate<BGDatabase_Weapon> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_Weapon> FindEntities(Predicate<BGDatabase_Weapon> filter, List<BGDatabase_Weapon> result = null, Comparison<BGDatabase_Weapon> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_Weapon> action, Predicate<BGDatabase_Weapon> filter = null, Comparison<BGDatabase_Weapon> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_Weapon GetEntity(BGId entityId)
	{
		return (BGDatabase_Weapon)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_Weapon GetEntity(int index)
	{
		return (BGDatabase_Weapon)MetaDefault[index];
	}

	public static BGDatabase_Weapon GetEntity(string entityName)
	{
		return (BGDatabase_Weapon)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_Weapon NewEntity()
	{
		return (BGDatabase_Weapon)MetaDefault.NewEntity();
	}

	public static BGDatabase_Weapon NewEntity(BGId entityId)
	{
		return (BGDatabase_Weapon)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_Weapon NewEntity(Action<BGDatabase_Weapon> callback)
	{
		return (BGDatabase_Weapon)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_Weapon)entity);
		}));
	}

	public static BGDatabase_Weapon GetEntityByKeyid(int Keys)
	{
		return (BGDatabase_Weapon)_id.GetEntityByKey(Keys);
	}
}
