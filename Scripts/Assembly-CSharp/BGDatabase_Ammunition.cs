using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_Ammunition : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_Ammunition(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_Ammunition(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldInt _ufle12jhs77_Keys;

	private static BGFieldString _ufle12jhs77_Name;

	private static BGFieldInt _ufle12jhs77_Amount;

	private static BGFieldInt _ufle12jhs77_MaxItemInSlot;

	private static BGFieldString _ufle12jhs77_CraftingStation;

	private static BGFieldString _ufle12jhs77_CraftingMaterial0;

	private static BGFieldString _ufle12jhs77_CraftingMaterial1;

	private static BGFieldString _ufle12jhs77_CraftingMaterial2;

	private static BGKey _edeht3sdad33_id;

	private static readonly BGDatabase_GameConfig.Factory _factory0_PFS = new BGDatabase_GameConfig.Factory();

	private static readonly BGDatabase_Character.Factory _factory1_PFS = new BGDatabase_Character.Factory();

	private static readonly BGDatabase_Enemy.Factory _factory2_PFS = new BGDatabase_Enemy.Factory();

	private static readonly BGDatabase_Weapon.Factory _factory3_PFS = new BGDatabase_Weapon.Factory();

	private static readonly BGDatabase_HealingItem.Factory _factory4_PFS = new BGDatabase_HealingItem.Factory();

	private static readonly BGDatabase_Item.Factory _factory5_PFS = new BGDatabase_Item.Factory();

	private static readonly Factory _factory6_PFS = new Factory();

	private static readonly BGDatabase_CraftMaterial.Factory _factory7_PFS = new BGDatabase_CraftMaterial.Factory();

	private static readonly BGDatabase_CraftRecipe.Factory _factory8_PFS = new BGDatabase_CraftRecipe.Factory();

	private static readonly BGDatabase_Wave.Factory _factory9_PFS = new BGDatabase_Wave.Factory();

	private static readonly BGDatabase_ItemConvert.Factory _factory10_PFS = new BGDatabase_ItemConvert.Factory();

	private static readonly BGDatabase_Skill.Factory _factory11_PFS = new BGDatabase_Skill.Factory();

	private static readonly BGDatabase_Perks.Factory _factory12_PFS = new BGDatabase_Perks.Factory();

	private static readonly BGDatabase_DefenseWave.Factory _factory13_PFS = new BGDatabase_DefenseWave.Factory();

	private static readonly BGDatabase_ItemDismantle.Factory _factory14_PFS = new BGDatabase_ItemDismantle.Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(4965629275832171743uL, 11579501939893864106uL), () =>
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

	public int Amount
	{
		get
		{
			return _Amount[base.Index];
		}
		set
		{
			_Amount[base.Index] = value;
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

	public string CraftingStation
	{
		get
		{
			return _CraftingStation[base.Index];
		}
		set
		{
			_CraftingStation[base.Index] = value;
		}
	}

	public string CraftingMaterial0
	{
		get
		{
			return _CraftingMaterial0[base.Index];
		}
		set
		{
			_CraftingMaterial0[base.Index] = value;
		}
	}

	public string CraftingMaterial1
	{
		get
		{
			return _CraftingMaterial1[base.Index];
		}
		set
		{
			_CraftingMaterial1[base.Index] = value;
		}
	}

	public string CraftingMaterial2
	{
		get
		{
			return _CraftingMaterial2[base.Index];
		}
		set
		{
			_CraftingMaterial2[base.Index] = value;
		}
	}

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5156277604013125209uL, 5390219226191903623uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldInt _Keys => _ufle12jhs77_Keys ?? (_ufle12jhs77_Keys = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4774080995343596890uL, 1276819324287118730uL), () =>
	{
		_ufle12jhs77_Keys = null;
	}));

	public static BGFieldString _Name => _ufle12jhs77_Name ?? (_ufle12jhs77_Name = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5362569222236157003uL, 15556427910819670462uL), () =>
	{
		_ufle12jhs77_Name = null;
	}));

	public static BGFieldInt _Amount => _ufle12jhs77_Amount ?? (_ufle12jhs77_Amount = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4833896433290412177uL, 11688765558541655740uL), () =>
	{
		_ufle12jhs77_Amount = null;
	}));

	public static BGFieldInt _MaxItemInSlot => _ufle12jhs77_MaxItemInSlot ?? (_ufle12jhs77_MaxItemInSlot = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5084251512937805821uL, 12844717911492999596uL), () =>
	{
		_ufle12jhs77_MaxItemInSlot = null;
	}));

	public static BGFieldString _CraftingStation => _ufle12jhs77_CraftingStation ?? (_ufle12jhs77_CraftingStation = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5456065757849663167uL, 12708412143248332954uL), () =>
	{
		_ufle12jhs77_CraftingStation = null;
	}));

	public static BGFieldString _CraftingMaterial0 => _ufle12jhs77_CraftingMaterial0 ?? (_ufle12jhs77_CraftingMaterial0 = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5300055843341918292uL, 5181397848342068617uL), () =>
	{
		_ufle12jhs77_CraftingMaterial0 = null;
	}));

	public static BGFieldString _CraftingMaterial1 => _ufle12jhs77_CraftingMaterial1 ?? (_ufle12jhs77_CraftingMaterial1 = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4931930274119221398uL, 7733578611064313499uL), () =>
	{
		_ufle12jhs77_CraftingMaterial1 = null;
	}));

	public static BGFieldString _CraftingMaterial2 => _ufle12jhs77_CraftingMaterial2 ?? (_ufle12jhs77_CraftingMaterial2 = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4757352013591779711uL, 16192175675122059147uL), () =>
	{
		_ufle12jhs77_CraftingMaterial2 = null;
	}));

	public static BGKey _id => _edeht3sdad33_id ?? (_edeht3sdad33_id = BGCodeGenUtils.GetKey(MetaDefault, new BGId(4620589353085193309uL, 8917064366062512015uL), () =>
	{
		_edeht3sdad33_id = null;
	}));

	private BGDatabase_Ammunition()
		: base(MetaDefault)
	{
	}

	private BGDatabase_Ammunition(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_Ammunition(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_Ammunition(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_Ammunition FindEntity(Predicate<BGDatabase_Ammunition> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_Ammunition> FindEntities(Predicate<BGDatabase_Ammunition> filter, List<BGDatabase_Ammunition> result = null, Comparison<BGDatabase_Ammunition> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_Ammunition> action, Predicate<BGDatabase_Ammunition> filter = null, Comparison<BGDatabase_Ammunition> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_Ammunition GetEntity(BGId entityId)
	{
		return (BGDatabase_Ammunition)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_Ammunition GetEntity(int index)
	{
		return (BGDatabase_Ammunition)MetaDefault[index];
	}

	public static BGDatabase_Ammunition GetEntity(string entityName)
	{
		return (BGDatabase_Ammunition)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_Ammunition NewEntity()
	{
		return (BGDatabase_Ammunition)MetaDefault.NewEntity();
	}

	public static BGDatabase_Ammunition NewEntity(BGId entityId)
	{
		return (BGDatabase_Ammunition)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_Ammunition NewEntity(Action<BGDatabase_Ammunition> callback)
	{
		return (BGDatabase_Ammunition)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_Ammunition)entity);
		}));
	}

	public static BGDatabase_Ammunition GetEntityByKeyid(int Keys)
	{
		return (BGDatabase_Ammunition)_id.GetEntityByKey(Keys);
	}
}
