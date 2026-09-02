using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_Wave : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_Wave(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_Wave(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldInt _ufle12jhs77_EnemyKey;

	private static BGFieldInt _ufle12jhs77_Wave1;

	private static BGFieldInt _ufle12jhs77_Wave2;

	private static BGFieldInt _ufle12jhs77_Wave3;

	private static BGFieldInt _ufle12jhs77_Wave4;

	private static BGFieldInt _ufle12jhs77_Wave5;

	private static BGFieldInt _ufle12jhs77_Wave6;

	private static BGFieldInt _ufle12jhs77_Wave7;

	private static BGFieldInt _ufle12jhs77_Wave8;

	private static BGFieldString _ufle12jhs77_Type;

	private static BGKey _edeht3sdad33_id;

	private static readonly BGDatabase_GameConfig.Factory _factory0_PFS = new BGDatabase_GameConfig.Factory();

	private static readonly BGDatabase_Character.Factory _factory1_PFS = new BGDatabase_Character.Factory();

	private static readonly BGDatabase_Enemy.Factory _factory2_PFS = new BGDatabase_Enemy.Factory();

	private static readonly BGDatabase_Weapon.Factory _factory3_PFS = new BGDatabase_Weapon.Factory();

	private static readonly BGDatabase_HealingItem.Factory _factory4_PFS = new BGDatabase_HealingItem.Factory();

	private static readonly BGDatabase_Item.Factory _factory5_PFS = new BGDatabase_Item.Factory();

	private static readonly BGDatabase_Ammunition.Factory _factory6_PFS = new BGDatabase_Ammunition.Factory();

	private static readonly BGDatabase_CraftMaterial.Factory _factory7_PFS = new BGDatabase_CraftMaterial.Factory();

	private static readonly BGDatabase_CraftRecipe.Factory _factory8_PFS = new BGDatabase_CraftRecipe.Factory();

	private static readonly Factory _factory9_PFS = new Factory();

	private static readonly BGDatabase_ItemConvert.Factory _factory10_PFS = new BGDatabase_ItemConvert.Factory();

	private static readonly BGDatabase_Skill.Factory _factory11_PFS = new BGDatabase_Skill.Factory();

	private static readonly BGDatabase_Perks.Factory _factory12_PFS = new BGDatabase_Perks.Factory();

	private static readonly BGDatabase_DefenseWave.Factory _factory13_PFS = new BGDatabase_DefenseWave.Factory();

	private static readonly BGDatabase_ItemDismantle.Factory _factory14_PFS = new BGDatabase_ItemDismantle.Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(4865825476637431606uL, 16858510848478381753uL), () =>
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

	public int EnemyKey
	{
		get
		{
			return _EnemyKey[base.Index];
		}
		set
		{
			_EnemyKey[base.Index] = value;
		}
	}

	public int Wave1
	{
		get
		{
			return _Wave1[base.Index];
		}
		set
		{
			_Wave1[base.Index] = value;
		}
	}

	public int Wave2
	{
		get
		{
			return _Wave2[base.Index];
		}
		set
		{
			_Wave2[base.Index] = value;
		}
	}

	public int Wave3
	{
		get
		{
			return _Wave3[base.Index];
		}
		set
		{
			_Wave3[base.Index] = value;
		}
	}

	public int Wave4
	{
		get
		{
			return _Wave4[base.Index];
		}
		set
		{
			_Wave4[base.Index] = value;
		}
	}

	public int Wave5
	{
		get
		{
			return _Wave5[base.Index];
		}
		set
		{
			_Wave5[base.Index] = value;
		}
	}

	public int Wave6
	{
		get
		{
			return _Wave6[base.Index];
		}
		set
		{
			_Wave6[base.Index] = value;
		}
	}

	public int Wave7
	{
		get
		{
			return _Wave7[base.Index];
		}
		set
		{
			_Wave7[base.Index] = value;
		}
	}

	public int Wave8
	{
		get
		{
			return _Wave8[base.Index];
		}
		set
		{
			_Wave8[base.Index] = value;
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

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5238842803130914486uL, 8187089847448872878uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldInt _EnemyKey => _ufle12jhs77_EnemyKey ?? (_ufle12jhs77_EnemyKey = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4766115636131893256uL, 14985070417547009464uL), () =>
	{
		_ufle12jhs77_EnemyKey = null;
	}));

	public static BGFieldInt _Wave1 => _ufle12jhs77_Wave1 ?? (_ufle12jhs77_Wave1 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5481400335764173428uL, 15587739953250656163uL), () =>
	{
		_ufle12jhs77_Wave1 = null;
	}));

	public static BGFieldInt _Wave2 => _ufle12jhs77_Wave2 ?? (_ufle12jhs77_Wave2 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5408488127713762393uL, 6193946227238894235uL), () =>
	{
		_ufle12jhs77_Wave2 = null;
	}));

	public static BGFieldInt _Wave3 => _ufle12jhs77_Wave3 ?? (_ufle12jhs77_Wave3 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4691909672069280585uL, 1611919867462528443uL), () =>
	{
		_ufle12jhs77_Wave3 = null;
	}));

	public static BGFieldInt _Wave4 => _ufle12jhs77_Wave4 ?? (_ufle12jhs77_Wave4 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4967471110549605005uL, 8076236399413997195uL), () =>
	{
		_ufle12jhs77_Wave4 = null;
	}));

	public static BGFieldInt _Wave5 => _ufle12jhs77_Wave5 ?? (_ufle12jhs77_Wave5 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5222188336002235199uL, 16762473927663956112uL), () =>
	{
		_ufle12jhs77_Wave5 = null;
	}));

	public static BGFieldInt _Wave6 => _ufle12jhs77_Wave6 ?? (_ufle12jhs77_Wave6 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5332243920665509901uL, 12250437555499340686uL), () =>
	{
		_ufle12jhs77_Wave6 = null;
	}));

	public static BGFieldInt _Wave7 => _ufle12jhs77_Wave7 ?? (_ufle12jhs77_Wave7 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5717090420224749646uL, 17849045886221454495uL), () =>
	{
		_ufle12jhs77_Wave7 = null;
	}));

	public static BGFieldInt _Wave8 => _ufle12jhs77_Wave8 ?? (_ufle12jhs77_Wave8 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4793253554399953246uL, 18202522105157230481uL), () =>
	{
		_ufle12jhs77_Wave8 = null;
	}));

	public static BGFieldString _Type => _ufle12jhs77_Type ?? (_ufle12jhs77_Type = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5143335887875783679uL, 16001577099836094372uL), () =>
	{
		_ufle12jhs77_Type = null;
	}));

	public static BGKey _id => _edeht3sdad33_id ?? (_edeht3sdad33_id = BGCodeGenUtils.GetKey(MetaDefault, new BGId(5460535143810248869uL, 13150581204330361523uL), () =>
	{
		_edeht3sdad33_id = null;
	}));

	private BGDatabase_Wave()
		: base(MetaDefault)
	{
	}

	private BGDatabase_Wave(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_Wave(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_Wave(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_Wave FindEntity(Predicate<BGDatabase_Wave> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_Wave> FindEntities(Predicate<BGDatabase_Wave> filter, List<BGDatabase_Wave> result = null, Comparison<BGDatabase_Wave> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_Wave> action, Predicate<BGDatabase_Wave> filter = null, Comparison<BGDatabase_Wave> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_Wave GetEntity(BGId entityId)
	{
		return (BGDatabase_Wave)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_Wave GetEntity(int index)
	{
		return (BGDatabase_Wave)MetaDefault[index];
	}

	public static BGDatabase_Wave GetEntity(string entityName)
	{
		return (BGDatabase_Wave)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_Wave NewEntity()
	{
		return (BGDatabase_Wave)MetaDefault.NewEntity();
	}

	public static BGDatabase_Wave NewEntity(BGId entityId)
	{
		return (BGDatabase_Wave)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_Wave NewEntity(Action<BGDatabase_Wave> callback)
	{
		return (BGDatabase_Wave)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_Wave)entity);
		}));
	}

	public static BGDatabase_Wave GetEntityByKeyid(int EnemyKey)
	{
		return (BGDatabase_Wave)_id.GetEntityByKey(EnemyKey);
	}
}
