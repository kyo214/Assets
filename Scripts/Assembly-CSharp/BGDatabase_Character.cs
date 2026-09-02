using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_Character : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_Character(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_Character(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldString _ufle12jhs77_Keys;

	private static BGFieldString _ufle12jhs77_Name;

	private static BGFieldBool _ufle12jhs77_PlayableChar;

	private static BGFieldFloat _ufle12jhs77_Health;

	private static BGFieldFloat _ufle12jhs77_MaxSanity;

	private static BGFieldFloat _ufle12jhs77_Stamina;

	private static BGFieldFloat _ufle12jhs77_StaminaRegen;

	private static BGFieldFloat _ufle12jhs77_DelayStaminaRegen;

	private static BGFieldFloat _ufle12jhs77_DodgeStamina;

	private static BGFieldFloat _ufle12jhs77_MoveSpeed;

	private static BGFieldFloat _ufle12jhs77_SprintSpeed;

	private static BGFieldFloat _ufle12jhs77_MoveBackwardSpeed;

	private static BGFieldFloat _ufle12jhs77_MoveAimSpeed;

	private static BGFieldInt _ufle12jhs77_MaxInventory;

	private static BGFieldInt _ufle12jhs77_Inventory0;

	private static BGFieldInt _ufle12jhs77_Inventory1;

	private static BGFieldInt _ufle12jhs77_Inventory2;

	private static BGFieldInt _ufle12jhs77_Inventory3;

	private static BGFieldInt _ufle12jhs77_Inventory4;

	private static BGFieldInt _ufle12jhs77_Inventory5;

	private static BGKey _edeht3sdad33_id;

	private static readonly BGDatabase_GameConfig.Factory _factory0_PFS = new BGDatabase_GameConfig.Factory();

	private static readonly Factory _factory1_PFS = new Factory();

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

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(4631556735898439114uL, 5037283757451391914uL), () =>
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

	public bool PlayableChar
	{
		get
		{
			return _PlayableChar[base.Index];
		}
		set
		{
			_PlayableChar[base.Index] = value;
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

	public float MaxSanity
	{
		get
		{
			return _MaxSanity[base.Index];
		}
		set
		{
			_MaxSanity[base.Index] = value;
		}
	}

	public float Stamina
	{
		get
		{
			return _Stamina[base.Index];
		}
		set
		{
			_Stamina[base.Index] = value;
		}
	}

	public float StaminaRegen
	{
		get
		{
			return _StaminaRegen[base.Index];
		}
		set
		{
			_StaminaRegen[base.Index] = value;
		}
	}

	public float DelayStaminaRegen
	{
		get
		{
			return _DelayStaminaRegen[base.Index];
		}
		set
		{
			_DelayStaminaRegen[base.Index] = value;
		}
	}

	public float DodgeStamina
	{
		get
		{
			return _DodgeStamina[base.Index];
		}
		set
		{
			_DodgeStamina[base.Index] = value;
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

	public float SprintSpeed
	{
		get
		{
			return _SprintSpeed[base.Index];
		}
		set
		{
			_SprintSpeed[base.Index] = value;
		}
	}

	public float MoveBackwardSpeed
	{
		get
		{
			return _MoveBackwardSpeed[base.Index];
		}
		set
		{
			_MoveBackwardSpeed[base.Index] = value;
		}
	}

	public float MoveAimSpeed
	{
		get
		{
			return _MoveAimSpeed[base.Index];
		}
		set
		{
			_MoveAimSpeed[base.Index] = value;
		}
	}

	public int MaxInventory
	{
		get
		{
			return _MaxInventory[base.Index];
		}
		set
		{
			_MaxInventory[base.Index] = value;
		}
	}

	public int Inventory0
	{
		get
		{
			return _Inventory0[base.Index];
		}
		set
		{
			_Inventory0[base.Index] = value;
		}
	}

	public int Inventory1
	{
		get
		{
			return _Inventory1[base.Index];
		}
		set
		{
			_Inventory1[base.Index] = value;
		}
	}

	public int Inventory2
	{
		get
		{
			return _Inventory2[base.Index];
		}
		set
		{
			_Inventory2[base.Index] = value;
		}
	}

	public int Inventory3
	{
		get
		{
			return _Inventory3[base.Index];
		}
		set
		{
			_Inventory3[base.Index] = value;
		}
	}

	public int Inventory4
	{
		get
		{
			return _Inventory4[base.Index];
		}
		set
		{
			_Inventory4[base.Index] = value;
		}
	}

	public int Inventory5
	{
		get
		{
			return _Inventory5[base.Index];
		}
		set
		{
			_Inventory5[base.Index] = value;
		}
	}

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5058016257974130892uL, 5123265637171252123uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldString _Keys => _ufle12jhs77_Keys ?? (_ufle12jhs77_Keys = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5662950663439138629uL, 8132422473044737178uL), () =>
	{
		_ufle12jhs77_Keys = null;
	}));

	public static BGFieldString _Name => _ufle12jhs77_Name ?? (_ufle12jhs77_Name = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5374680099094836867uL, 17605647497243902616uL), () =>
	{
		_ufle12jhs77_Name = null;
	}));

	public static BGFieldBool _PlayableChar => _ufle12jhs77_PlayableChar ?? (_ufle12jhs77_PlayableChar = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5224129626809484101uL, 16893800715485867910uL), () =>
	{
		_ufle12jhs77_PlayableChar = null;
	}));

	public static BGFieldFloat _Health => _ufle12jhs77_Health ?? (_ufle12jhs77_Health = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4801903505834064510uL, 5331718167248650655uL), () =>
	{
		_ufle12jhs77_Health = null;
	}));

	public static BGFieldFloat _MaxSanity => _ufle12jhs77_MaxSanity ?? (_ufle12jhs77_MaxSanity = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4617481519215560638uL, 2762648036813361543uL), () =>
	{
		_ufle12jhs77_MaxSanity = null;
	}));

	public static BGFieldFloat _Stamina => _ufle12jhs77_Stamina ?? (_ufle12jhs77_Stamina = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5684641003687531897uL, 11719532560268114337uL), () =>
	{
		_ufle12jhs77_Stamina = null;
	}));

	public static BGFieldFloat _StaminaRegen => _ufle12jhs77_StaminaRegen ?? (_ufle12jhs77_StaminaRegen = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4659258040149107314uL, 2361057205244283034uL), () =>
	{
		_ufle12jhs77_StaminaRegen = null;
	}));

	public static BGFieldFloat _DelayStaminaRegen => _ufle12jhs77_DelayStaminaRegen ?? (_ufle12jhs77_DelayStaminaRegen = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4788814383655875507uL, 8276442098480499874uL), () =>
	{
		_ufle12jhs77_DelayStaminaRegen = null;
	}));

	public static BGFieldFloat _DodgeStamina => _ufle12jhs77_DodgeStamina ?? (_ufle12jhs77_DodgeStamina = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5055559394265753850uL, 15414458268485768887uL), () =>
	{
		_ufle12jhs77_DodgeStamina = null;
	}));

	public static BGFieldFloat _MoveSpeed => _ufle12jhs77_MoveSpeed ?? (_ufle12jhs77_MoveSpeed = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4774636303909903406uL, 12040612418464580536uL), () =>
	{
		_ufle12jhs77_MoveSpeed = null;
	}));

	public static BGFieldFloat _SprintSpeed => _ufle12jhs77_SprintSpeed ?? (_ufle12jhs77_SprintSpeed = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(4844405225797206172uL, 6001812902619711628uL), () =>
	{
		_ufle12jhs77_SprintSpeed = null;
	}));

	public static BGFieldFloat _MoveBackwardSpeed => _ufle12jhs77_MoveBackwardSpeed ?? (_ufle12jhs77_MoveBackwardSpeed = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5481835712679847584uL, 17452194991715702146uL), () =>
	{
		_ufle12jhs77_MoveBackwardSpeed = null;
	}));

	public static BGFieldFloat _MoveAimSpeed => _ufle12jhs77_MoveAimSpeed ?? (_ufle12jhs77_MoveAimSpeed = BGCodeGenUtils.GetField<BGFieldFloat>(MetaDefault, new BGId(5566041475000284904uL, 9827736156544745655uL), () =>
	{
		_ufle12jhs77_MoveAimSpeed = null;
	}));

	public static BGFieldInt _MaxInventory => _ufle12jhs77_MaxInventory ?? (_ufle12jhs77_MaxInventory = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4991587026878267749uL, 17072390057180748720uL), () =>
	{
		_ufle12jhs77_MaxInventory = null;
	}));

	public static BGFieldInt _Inventory0 => _ufle12jhs77_Inventory0 ?? (_ufle12jhs77_Inventory0 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5167985571347887876uL, 12199080384104943500uL), () =>
	{
		_ufle12jhs77_Inventory0 = null;
	}));

	public static BGFieldInt _Inventory1 => _ufle12jhs77_Inventory1 ?? (_ufle12jhs77_Inventory1 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4787497076031285470uL, 182639177973577404uL), () =>
	{
		_ufle12jhs77_Inventory1 = null;
	}));

	public static BGFieldInt _Inventory2 => _ufle12jhs77_Inventory2 ?? (_ufle12jhs77_Inventory2 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5260188094431791151uL, 13521188946918343831uL), () =>
	{
		_ufle12jhs77_Inventory2 = null;
	}));

	public static BGFieldInt _Inventory3 => _ufle12jhs77_Inventory3 ?? (_ufle12jhs77_Inventory3 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5266721998136199955uL, 17154003081782716848uL), () =>
	{
		_ufle12jhs77_Inventory3 = null;
	}));

	public static BGFieldInt _Inventory4 => _ufle12jhs77_Inventory4 ?? (_ufle12jhs77_Inventory4 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4736381040796300433uL, 11632501998480590253uL), () =>
	{
		_ufle12jhs77_Inventory4 = null;
	}));

	public static BGFieldInt _Inventory5 => _ufle12jhs77_Inventory5 ?? (_ufle12jhs77_Inventory5 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4979238490636594916uL, 752973961232334995uL), () =>
	{
		_ufle12jhs77_Inventory5 = null;
	}));

	public static BGKey _id => _edeht3sdad33_id ?? (_edeht3sdad33_id = BGCodeGenUtils.GetKey(MetaDefault, new BGId(4718635540277274893uL, 12879911005372909986uL), () =>
	{
		_edeht3sdad33_id = null;
	}));

	private BGDatabase_Character()
		: base(MetaDefault)
	{
	}

	private BGDatabase_Character(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_Character(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_Character(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_Character FindEntity(Predicate<BGDatabase_Character> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_Character> FindEntities(Predicate<BGDatabase_Character> filter, List<BGDatabase_Character> result = null, Comparison<BGDatabase_Character> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_Character> action, Predicate<BGDatabase_Character> filter = null, Comparison<BGDatabase_Character> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_Character GetEntity(BGId entityId)
	{
		return (BGDatabase_Character)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_Character GetEntity(int index)
	{
		return (BGDatabase_Character)MetaDefault[index];
	}

	public static BGDatabase_Character GetEntity(string entityName)
	{
		return (BGDatabase_Character)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_Character NewEntity()
	{
		return (BGDatabase_Character)MetaDefault.NewEntity();
	}

	public static BGDatabase_Character NewEntity(BGId entityId)
	{
		return (BGDatabase_Character)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_Character NewEntity(Action<BGDatabase_Character> callback)
	{
		return (BGDatabase_Character)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_Character)entity);
		}));
	}

	public static BGDatabase_Character GetEntityByKeyid(string Keys)
	{
		return (BGDatabase_Character)_id.GetEntityByKey(Keys);
	}
}
