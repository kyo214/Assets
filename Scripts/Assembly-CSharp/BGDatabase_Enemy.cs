using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_Enemy : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_Enemy(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_Enemy(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldString _ufle12jhs77_Keys;

	private static BGFieldString _ufle12jhs77_Name;

	private static BGFieldFloat _ufle12jhs77_Health;

	private static BGFieldFloat _ufle12jhs77_MoveSpeed;

	private static BGFieldFloat _ufle12jhs77_Damage;

	private static BGFieldFloat _ufle12jhs77_AggroSpeed;

	private static BGFieldFloat _ufle12jhs77_AggroSpeedHorde;

	private static BGFieldFloat _ufle12jhs77_DistanceAggro2;

	private static BGFieldFloat _ufle12jhs77_AggroSpeed2;

	private static BGFieldFloat _ufle12jhs77_AggroDelay;

	private static BGFieldFloat _ufle12jhs77_MinTimeRandomAngle;

	private static BGFieldFloat _ufle12jhs77_MaxTimeRandomAngle;

	private static BGFieldFloat _ufle12jhs77_MinTimeRandomState;

	private static BGFieldFloat _ufle12jhs77_MaxTimeRandomState;

	private static BGFieldFloat _ufle12jhs77_SoundDistTolerance;

	private static BGFieldFloat _ufle12jhs77_DistConeView;

	private static BGFieldInt _ufle12jhs77_AngleConeView;

	private static BGFieldFloat _ufle12jhs77_DistChasing;

	private static BGFieldInt _ufle12jhs77_TimeOutChasing;

	private static BGFieldFloat _ufle12jhs77_DelayAttack;

	private static BGFieldFloat _ufle12jhs77_KnockBackDistanceMultiply;

	private static BGFieldFloat _ufle12jhs77_AttackMoveSpeed;

	private static BGKey _edeht3sdad33_id;

	private static readonly BGDatabase_GameConfig.Factory _factory0_PFS = new BGDatabase_GameConfig.Factory();

	private static readonly BGDatabase_Character.Factory _factory1_PFS = new BGDatabase_Character.Factory();

	private static readonly Factory _factory2_PFS = new Factory();

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

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(5146673813551562359uL, 15922953764518477749uL), () =>
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

	public float Health
	{
		get
		{
			return _Health[base.Index];
		}
		set
		{
			_Health[base.Index] = value;
		}
	}

	public float MoveSpeed
	{
		get
		{
			return _MoveSpeed[base.Index];
		}
		set
		{
			_MoveSpeed[base.Index] = value;
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

	public float AggroSpeed
	{
		get
		{
			return _AggroSpeed[base.Index];
		}
		set
		{
			_AggroSpeed[base.Index] = value;
		}
	}

	public float AggroSpeedHorde
	{
		get
		{
			return _AggroSpeedHorde[base.Index];
		}
		set
		{
			_AggroSpeedHorde[base.Index] = value;
		}
	}

	public float DistanceAggro2
	{
		get
		{
			return _DistanceAggro2[base.Index];
		}
		set
		{
			_DistanceAggro2[base.Index] = value;
		}
	}

	public float AggroSpeed2
	{
		get
		{
			return _AggroSpeed2[base.Index];
		}
		set
		{
			_AggroSpeed2[base.Index] = value;
		}
	}

	public float AggroDelay
	{
		get
		{
			return _AggroDelay[base.Index];
		}
		set
		{
			_AggroDelay[base.Index] = value;
		}
	}

	public float MinTimeRandomAngle
	{
		get
		{
			return _MinTimeRandomAngle[base.Index];
		}
		set
		{
			_MinTimeRandomAngle[base.Index] = value;
		}
	}

	public float MaxTimeRandomAngle
	{
		get
		{
			return _MaxTimeRandomAngle[base.Index];
		}
		set
		{
			_MaxTimeRandomAngle[base.Index] = value;
		}
	}

	public float MinTimeRandomState
	{
		get
		{
			return _MinTimeRandomState[base.Index];
		}
		set
		{
			_MinTimeRandomState[base.Index] = value;
		}
	}

	public float MaxTimeRandomState
	{
		get
		{
			return _MaxTimeRandomState[base.Index];
		}
		set
		{
			_MaxTimeRandomState[base.Index] = value;
		}
	}

	public float SoundDistTolerance
	{
		get
		{
			return _SoundDistTolerance[base.Index];
		}
		set
		{
			_SoundDistTolerance[base.Index] = value;
		}
	}

	public float DistConeView
	{
		get
		{
			return _DistConeView[base.Index];
		}
		set
		{
			_DistConeView[base.Index] = value;
		}
	}

	public int AngleConeView
	{
		get
		{
			return _AngleConeView[base.Index];
		}
		set
		{
			_AngleConeView[base.Index] = value;
		}
	}

	public float DistChasing
	{
		get
		{
			return _DistChasing[base.Index];
		}
		set
		{
			_DistChasing[base.Index] = value;
		}
	}

	public int TimeOutChasing
	{
		get
		{
			return _TimeOutChasing[base.Index];
		}
		set
		{
			_TimeOutChasing[base.Index] = value;
		}
	}

	public float DelayAttack
	{
		get
		{
			return _DelayAttack[base.Index];
		}
		set
		{
			_DelayAttack[base.Index] = value;
		}
	}

	public float KnockBackDistanceMultiply
	{
		get
		{
			return _KnockBackDistanceMultiply[base.Index];
		}
		set
		{
			_KnockBackDistanceMultiply[base.Index] = value;
		}
	}

	public float AttackMoveSpeed
	{
		get
		{
			return _AttackMoveSpeed[base.Index];
		}
		set
		{
			_AttackMoveSpeed[base.Index] = value;
		}
	}

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5693724167883258389uL, 10919044718633200569uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldString _Keys => _ufle12jhs77_Keys ?? (_ufle12jhs77_Keys = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5460835491226700885uL, 14294902482019085740uL), () =>
	{
		_ufle12jhs77_Keys = null;
	}));

	public static BGFieldString _Name => _ufle12jhs77_Name ?? (_ufle12jhs77_Name = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5669915707052512539uL, 5300915505948151725uL), () =>
	{
		_ufle12jhs77_Name = null;
	}));

	public static BGFieldFloat _Health => _ufle12jhs77_Health ?? (_ufle12jhs77_Health = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5012630368761423246uL, 3926114164217694875uL), () =>
	{
		_ufle12jhs77_Health = null;
	}));

	public static BGFieldFloat _MoveSpeed => _ufle12jhs77_MoveSpeed ?? (_ufle12jhs77_MoveSpeed = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4677544954592547107uL, 5357137471760138407uL), () =>
	{
		_ufle12jhs77_MoveSpeed = null;
	}));

	public static BGFieldFloat _Damage => _ufle12jhs77_Damage ?? (_ufle12jhs77_Damage = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4645876425729208432uL, 143092917986207422uL), () =>
	{
		_ufle12jhs77_Damage = null;
	}));

	public static BGFieldFloat _AggroSpeed => _ufle12jhs77_AggroSpeed ?? (_ufle12jhs77_AggroSpeed = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5390942877526474715uL, 8768613091046503320uL), () =>
	{
		_ufle12jhs77_AggroSpeed = null;
	}));

	public static BGFieldFloat _AggroSpeedHorde => _ufle12jhs77_AggroSpeedHorde ?? (_ufle12jhs77_AggroSpeedHorde = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4968230726807737389uL, 3131829316074237365uL), () =>
	{
		_ufle12jhs77_AggroSpeedHorde = null;
	}));

	public static BGFieldFloat _DistanceAggro2 => _ufle12jhs77_DistanceAggro2 ?? (_ufle12jhs77_DistanceAggro2 = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4800795369399525410uL, 5996820664516076686uL), () =>
	{
		_ufle12jhs77_DistanceAggro2 = null;
	}));

	public static BGFieldFloat _AggroSpeed2 => _ufle12jhs77_AggroSpeed2 ?? (_ufle12jhs77_AggroSpeed2 = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5563866742378924951uL, 3880473550009337732uL), () =>
	{
		_ufle12jhs77_AggroSpeed2 = null;
	}));

	public static BGFieldFloat _AggroDelay => _ufle12jhs77_AggroDelay ?? (_ufle12jhs77_AggroDelay = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4917880773404694337uL, 7499204985415901831uL), () =>
	{
		_ufle12jhs77_AggroDelay = null;
	}));

	public static BGFieldFloat _MinTimeRandomAngle => _ufle12jhs77_MinTimeRandomAngle ?? (_ufle12jhs77_MinTimeRandomAngle = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4698790128899055586uL, 7598462991353653931uL), () =>
	{
		_ufle12jhs77_MinTimeRandomAngle = null;
	}));

	public static BGFieldFloat _MaxTimeRandomAngle => _ufle12jhs77_MaxTimeRandomAngle ?? (_ufle12jhs77_MaxTimeRandomAngle = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4837880309245262218uL, 4936284133229270455uL), () =>
	{
		_ufle12jhs77_MaxTimeRandomAngle = null;
	}));

	public static BGFieldFloat _MinTimeRandomState => _ufle12jhs77_MinTimeRandomState ?? (_ufle12jhs77_MinTimeRandomState = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5037613511600282880uL, 16635738781716764295uL), () =>
	{
		_ufle12jhs77_MinTimeRandomState = null;
	}));

	public static BGFieldFloat _MaxTimeRandomState => _ufle12jhs77_MaxTimeRandomState ?? (_ufle12jhs77_MaxTimeRandomState = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5624844640679288683uL, 16705206681679792061uL), () =>
	{
		_ufle12jhs77_MaxTimeRandomState = null;
	}));

	public static BGFieldFloat _SoundDistTolerance => _ufle12jhs77_SoundDistTolerance ?? (_ufle12jhs77_SoundDistTolerance = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5022835619627455468uL, 855792015822598066uL), () =>
	{
		_ufle12jhs77_SoundDistTolerance = null;
	}));

	public static BGFieldFloat _DistConeView => _ufle12jhs77_DistConeView ?? (_ufle12jhs77_DistConeView = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4650942170551388633uL, 2694881692836332955uL), () =>
	{
		_ufle12jhs77_DistConeView = null;
	}));

	public static BGFieldInt _AngleConeView => _ufle12jhs77_AngleConeView ?? (_ufle12jhs77_AngleConeView = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5709300251332771865uL, 10712234735024557240uL), () =>
	{
		_ufle12jhs77_AngleConeView = null;
	}));

	public static BGFieldFloat _DistChasing => _ufle12jhs77_DistChasing ?? (_ufle12jhs77_DistChasing = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5323320996178077807uL, 15802095774021124783uL), () =>
	{
		_ufle12jhs77_DistChasing = null;
	}));

	public static BGFieldInt _TimeOutChasing => _ufle12jhs77_TimeOutChasing ?? (_ufle12jhs77_TimeOutChasing = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4755541263508798852uL, 10357220809110944680uL), () =>
	{
		_ufle12jhs77_TimeOutChasing = null;
	}));

	public static BGFieldFloat _DelayAttack => _ufle12jhs77_DelayAttack ?? (_ufle12jhs77_DelayAttack = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5216245645106622794uL, 3098015960733015201uL), () =>
	{
		_ufle12jhs77_DelayAttack = null;
	}));

	public static BGFieldFloat _KnockBackDistanceMultiply => _ufle12jhs77_KnockBackDistanceMultiply ?? (_ufle12jhs77_KnockBackDistanceMultiply = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5319894726180794523uL, 7281604167049932959uL), () =>
	{
		_ufle12jhs77_KnockBackDistanceMultiply = null;
	}));

	public static BGFieldFloat _AttackMoveSpeed => _ufle12jhs77_AttackMoveSpeed ?? (_ufle12jhs77_AttackMoveSpeed = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4624547484182848404uL, 15760845312067697283uL), () =>
	{
		_ufle12jhs77_AttackMoveSpeed = null;
	}));

	public static BGKey _id => _edeht3sdad33_id ?? (_edeht3sdad33_id = BGCodeGenUtils.GetKey(MetaDefault, new BGId(5010993509602680502uL, 2804953816022300301uL), () =>
	{
		_edeht3sdad33_id = null;
	}));

	private BGDatabase_Enemy()
		: base(MetaDefault)
	{
	}

	private BGDatabase_Enemy(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_Enemy(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_Enemy(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_Enemy FindEntity(Predicate<BGDatabase_Enemy> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_Enemy> FindEntities(Predicate<BGDatabase_Enemy> filter, List<BGDatabase_Enemy> result = null, Comparison<BGDatabase_Enemy> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_Enemy> action, Predicate<BGDatabase_Enemy> filter = null, Comparison<BGDatabase_Enemy> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_Enemy GetEntity(BGId entityId)
	{
		return (BGDatabase_Enemy)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_Enemy GetEntity(int index)
	{
		return (BGDatabase_Enemy)MetaDefault[index];
	}

	public static BGDatabase_Enemy GetEntity(string entityName)
	{
		return (BGDatabase_Enemy)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_Enemy NewEntity()
	{
		return (BGDatabase_Enemy)MetaDefault.NewEntity();
	}

	public static BGDatabase_Enemy NewEntity(BGId entityId)
	{
		return (BGDatabase_Enemy)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_Enemy NewEntity(Action<BGDatabase_Enemy> callback)
	{
		return (BGDatabase_Enemy)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_Enemy)entity);
		}));
	}

	public static BGDatabase_Enemy GetEntityByKeyid(string Keys)
	{
		return (BGDatabase_Enemy)_id.GetEntityByKey(Keys);
	}
}
