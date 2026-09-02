using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_DefenseWave : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_DefenseWave(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_DefenseWave(meta, id);
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

	private static BGFieldInt _ufle12jhs77_Wave9;

	private static BGFieldInt _ufle12jhs77_Wave10;

	private static BGFieldInt _ufle12jhs77_Wave11;

	private static BGFieldInt _ufle12jhs77_Wave12;

	private static BGFieldInt _ufle12jhs77_Wave13;

	private static BGFieldInt _ufle12jhs77_Wave14;

	private static BGFieldInt _ufle12jhs77_Wave15;

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

	private static readonly BGDatabase_Wave.Factory _factory9_PFS = new BGDatabase_Wave.Factory();

	private static readonly BGDatabase_ItemConvert.Factory _factory10_PFS = new BGDatabase_ItemConvert.Factory();

	private static readonly BGDatabase_Skill.Factory _factory11_PFS = new BGDatabase_Skill.Factory();

	private static readonly BGDatabase_Perks.Factory _factory12_PFS = new BGDatabase_Perks.Factory();

	private static readonly Factory _factory13_PFS = new Factory();

	private static readonly BGDatabase_ItemDismantle.Factory _factory14_PFS = new BGDatabase_ItemDismantle.Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(5226154992082462464uL, 6372010050916413581uL), () =>
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

	public int Wave9
	{
		get
		{
			return _Wave9[base.Index];
		}
		set
		{
			_Wave9[base.Index] = value;
		}
	}

	public int Wave10
	{
		get
		{
			return _Wave10[base.Index];
		}
		set
		{
			_Wave10[base.Index] = value;
		}
	}

	public int Wave11
	{
		get
		{
			return _Wave11[base.Index];
		}
		set
		{
			_Wave11[base.Index] = value;
		}
	}

	public int Wave12
	{
		get
		{
			return _Wave12[base.Index];
		}
		set
		{
			_Wave12[base.Index] = value;
		}
	}

	public int Wave13
	{
		get
		{
			return _Wave13[base.Index];
		}
		set
		{
			_Wave13[base.Index] = value;
		}
	}

	public int Wave14
	{
		get
		{
			return _Wave14[base.Index];
		}
		set
		{
			_Wave14[base.Index] = value;
		}
	}

	public int Wave15
	{
		get
		{
			return _Wave15[base.Index];
		}
		set
		{
			_Wave15[base.Index] = value;
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

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5728940978204227880uL, 5874155430603030177uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldInt _EnemyKey => _ufle12jhs77_EnemyKey ?? (_ufle12jhs77_EnemyKey = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5007135414303177775uL, 4089848476662588562uL), () =>
	{
		_ufle12jhs77_EnemyKey = null;
	}));

	public static BGFieldInt _Wave1 => _ufle12jhs77_Wave1 ?? (_ufle12jhs77_Wave1 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5433039888820422193uL, 9514502567631394693uL), () =>
	{
		_ufle12jhs77_Wave1 = null;
	}));

	public static BGFieldInt _Wave2 => _ufle12jhs77_Wave2 ?? (_ufle12jhs77_Wave2 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5497120397288388879uL, 8992474526028332425uL), () =>
	{
		_ufle12jhs77_Wave2 = null;
	}));

	public static BGFieldInt _Wave3 => _ufle12jhs77_Wave3 ?? (_ufle12jhs77_Wave3 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5188503429433469891uL, 2055336826928668843uL), () =>
	{
		_ufle12jhs77_Wave3 = null;
	}));

	public static BGFieldInt _Wave4 => _ufle12jhs77_Wave4 ?? (_ufle12jhs77_Wave4 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5748407440313152582uL, 7590746308308431264uL), () =>
	{
		_ufle12jhs77_Wave4 = null;
	}));

	public static BGFieldInt _Wave5 => _ufle12jhs77_Wave5 ?? (_ufle12jhs77_Wave5 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5498958770599120695uL, 14268133219962980742uL), () =>
	{
		_ufle12jhs77_Wave5 = null;
	}));

	public static BGFieldInt _Wave6 => _ufle12jhs77_Wave6 ?? (_ufle12jhs77_Wave6 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5055655492493540037uL, 15509969620921607855uL), () =>
	{
		_ufle12jhs77_Wave6 = null;
	}));

	public static BGFieldInt _Wave7 => _ufle12jhs77_Wave7 ?? (_ufle12jhs77_Wave7 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5418127979008839712uL, 14856376742329288345uL), () =>
	{
		_ufle12jhs77_Wave7 = null;
	}));

	public static BGFieldInt _Wave8 => _ufle12jhs77_Wave8 ?? (_ufle12jhs77_Wave8 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4982723925187680042uL, 3919181742217929654uL), () =>
	{
		_ufle12jhs77_Wave8 = null;
	}));

	public static BGFieldInt _Wave9 => _ufle12jhs77_Wave9 ?? (_ufle12jhs77_Wave9 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5008067515021245012uL, 15977431907829364135uL), () =>
	{
		_ufle12jhs77_Wave9 = null;
	}));

	public static BGFieldInt _Wave10 => _ufle12jhs77_Wave10 ?? (_ufle12jhs77_Wave10 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4672544712007396768uL, 17611202986144053695uL), () =>
	{
		_ufle12jhs77_Wave10 = null;
	}));

	public static BGFieldInt _Wave11 => _ufle12jhs77_Wave11 ?? (_ufle12jhs77_Wave11 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5572544237915242624uL, 15139548335287648930uL), () =>
	{
		_ufle12jhs77_Wave11 = null;
	}));

	public static BGFieldInt _Wave12 => _ufle12jhs77_Wave12 ?? (_ufle12jhs77_Wave12 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4918118333131159923uL, 9512123183219171983uL), () =>
	{
		_ufle12jhs77_Wave12 = null;
	}));

	public static BGFieldInt _Wave13 => _ufle12jhs77_Wave13 ?? (_ufle12jhs77_Wave13 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4696005262241751316uL, 4468812154431334589uL), () =>
	{
		_ufle12jhs77_Wave13 = null;
	}));

	public static BGFieldInt _Wave14 => _ufle12jhs77_Wave14 ?? (_ufle12jhs77_Wave14 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5190257751735139935uL, 17122918800283564459uL), () =>
	{
		_ufle12jhs77_Wave14 = null;
	}));

	public static BGFieldInt _Wave15 => _ufle12jhs77_Wave15 ?? (_ufle12jhs77_Wave15 = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5580588830844639683uL, 2203876605928877753uL), () =>
	{
		_ufle12jhs77_Wave15 = null;
	}));

	public static BGFieldString _Type => _ufle12jhs77_Type ?? (_ufle12jhs77_Type = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5113470918456570838uL, 2737500728250673561uL), () =>
	{
		_ufle12jhs77_Type = null;
	}));

	public static BGKey _id => _edeht3sdad33_id ?? (_edeht3sdad33_id = BGCodeGenUtils.GetKey(MetaDefault, new BGId(5520725612878447559uL, 17094368191415386755uL), () =>
	{
		_edeht3sdad33_id = null;
	}));

	private BGDatabase_DefenseWave()
		: base(MetaDefault)
	{
	}

	private BGDatabase_DefenseWave(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_DefenseWave(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_DefenseWave(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_DefenseWave FindEntity(Predicate<BGDatabase_DefenseWave> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_DefenseWave> FindEntities(Predicate<BGDatabase_DefenseWave> filter, List<BGDatabase_DefenseWave> result = null, Comparison<BGDatabase_DefenseWave> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_DefenseWave> action, Predicate<BGDatabase_DefenseWave> filter = null, Comparison<BGDatabase_DefenseWave> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_DefenseWave GetEntity(BGId entityId)
	{
		return (BGDatabase_DefenseWave)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_DefenseWave GetEntity(int index)
	{
		return (BGDatabase_DefenseWave)MetaDefault[index];
	}

	public static BGDatabase_DefenseWave GetEntity(string entityName)
	{
		return (BGDatabase_DefenseWave)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_DefenseWave NewEntity()
	{
		return (BGDatabase_DefenseWave)MetaDefault.NewEntity();
	}

	public static BGDatabase_DefenseWave NewEntity(BGId entityId)
	{
		return (BGDatabase_DefenseWave)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_DefenseWave NewEntity(Action<BGDatabase_DefenseWave> callback)
	{
		return (BGDatabase_DefenseWave)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_DefenseWave)entity);
		}));
	}

	public static BGDatabase_DefenseWave GetEntityByKeyid(int EnemyKey)
	{
		return (BGDatabase_DefenseWave)_id.GetEntityByKey(EnemyKey);
	}
}
