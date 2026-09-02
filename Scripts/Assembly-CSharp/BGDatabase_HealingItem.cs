using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_HealingItem : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_HealingItem(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_HealingItem(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldInt _ufle12jhs77_Keys;

	private static BGFieldString _ufle12jhs77_Name;

	private static BGFieldInt _ufle12jhs77_HealingValuePercent;

	private static BGFieldBool _ufle12jhs77_IsUsable;

	private static BGFieldInt _ufle12jhs77_MaxItemInSlot;

	private static BGFieldString _ufle12jhs77_CraftingStation;

	private static BGFieldString _ufle12jhs77_CraftingMaterial0;

	private static BGFieldString _ufle12jhs77_CraftingMaterial1;

	private static BGFieldString _ufle12jhs77_CraftingMaterial2;

	private static BGFieldString _ufle12jhs77_CombineItem0;

	private static BGFieldString _ufle12jhs77_CombineItem1;

	private static BGFieldInt _ufle12jhs77_BaseKey;

	private static BGKey _edeht3sdad33_id;

	private static readonly BGDatabase_GameConfig.Factory _factory0_PFS = new BGDatabase_GameConfig.Factory();

	private static readonly BGDatabase_Character.Factory _factory1_PFS = new BGDatabase_Character.Factory();

	private static readonly BGDatabase_Enemy.Factory _factory2_PFS = new BGDatabase_Enemy.Factory();

	private static readonly BGDatabase_Weapon.Factory _factory3_PFS = new BGDatabase_Weapon.Factory();

	private static readonly Factory _factory4_PFS = new Factory();

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

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(5545224965769968995uL, 9793418675717903507uL), () =>
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

	public int HealingValuePercent
	{
		get
		{
			return _HealingValuePercent[base.Index];
		}
		set
		{
			_HealingValuePercent[base.Index] = value;
		}
	}

	public bool IsUsable
	{
		get
		{
			return _IsUsable[base.Index];
		}
		set
		{
			_IsUsable[base.Index] = value;
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

	public string CombineItem0
	{
		get
		{
			return _CombineItem0[base.Index];
		}
		set
		{
			_CombineItem0[base.Index] = value;
		}
	}

	public string CombineItem1
	{
		get
		{
			return _CombineItem1[base.Index];
		}
		set
		{
			_CombineItem1[base.Index] = value;
		}
	}

	public int BaseKey
	{
		get
		{
			return _BaseKey[base.Index];
		}
		set
		{
			_BaseKey[base.Index] = value;
		}
	}

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5620321761106905634uL, 13154643156365820061uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldInt _Keys => _ufle12jhs77_Keys ?? (_ufle12jhs77_Keys = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4732527263346007770uL, 7306439916902472077uL), () =>
	{
		_ufle12jhs77_Keys = null;
	}));

	public static BGFieldString _Name => _ufle12jhs77_Name ?? (_ufle12jhs77_Name = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5355224253082579040uL, 7052323854496551848uL), () =>
	{
		_ufle12jhs77_Name = null;
	}));

	public static BGFieldInt _HealingValuePercent => _ufle12jhs77_HealingValuePercent ?? (_ufle12jhs77_HealingValuePercent = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5338929144136094351uL, 9022002974140120501uL), () =>
	{
		_ufle12jhs77_HealingValuePercent = null;
	}));

	public static BGFieldBool _IsUsable => _ufle12jhs77_IsUsable ?? (_ufle12jhs77_IsUsable = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4803240418325506078uL, 18344618639518855615uL), () =>
	{
		_ufle12jhs77_IsUsable = null;
	}));

	public static BGFieldInt _MaxItemInSlot => _ufle12jhs77_MaxItemInSlot ?? (_ufle12jhs77_MaxItemInSlot = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5024553683974943778uL, 14612212726144372661uL), () =>
	{
		_ufle12jhs77_MaxItemInSlot = null;
	}));

	public static BGFieldString _CraftingStation => _ufle12jhs77_CraftingStation ?? (_ufle12jhs77_CraftingStation = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5157882968428848911uL, 6705232563832864900uL), () =>
	{
		_ufle12jhs77_CraftingStation = null;
	}));

	public static BGFieldString _CraftingMaterial0 => _ufle12jhs77_CraftingMaterial0 ?? (_ufle12jhs77_CraftingMaterial0 = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5554103004709154243uL, 8349954048306215554uL), () =>
	{
		_ufle12jhs77_CraftingMaterial0 = null;
	}));

	public static BGFieldString _CraftingMaterial1 => _ufle12jhs77_CraftingMaterial1 ?? (_ufle12jhs77_CraftingMaterial1 = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4829784907671610552uL, 2548169439112226991uL), () =>
	{
		_ufle12jhs77_CraftingMaterial1 = null;
	}));

	public static BGFieldString _CraftingMaterial2 => _ufle12jhs77_CraftingMaterial2 ?? (_ufle12jhs77_CraftingMaterial2 = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4706543933794409468uL, 11189868306710384059uL), () =>
	{
		_ufle12jhs77_CraftingMaterial2 = null;
	}));

	public static BGFieldString _CombineItem0 => _ufle12jhs77_CombineItem0 ?? (_ufle12jhs77_CombineItem0 = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5129655985128454895uL, 17139456308200450178uL), () =>
	{
		_ufle12jhs77_CombineItem0 = null;
	}));

	public static BGFieldString _CombineItem1 => _ufle12jhs77_CombineItem1 ?? (_ufle12jhs77_CombineItem1 = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4835335904816608330uL, 6757083836252537534uL), () =>
	{
		_ufle12jhs77_CombineItem1 = null;
	}));

	public static BGFieldInt _BaseKey => _ufle12jhs77_BaseKey ?? (_ufle12jhs77_BaseKey = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4621910518301828274uL, 7339134229033602970uL), () =>
	{
		_ufle12jhs77_BaseKey = null;
	}));

	public static BGKey _id => _edeht3sdad33_id ?? (_edeht3sdad33_id = BGCodeGenUtils.GetKey(MetaDefault, new BGId(4678779588918620091uL, 10539466825601480614uL), () =>
	{
		_edeht3sdad33_id = null;
	}));

	private BGDatabase_HealingItem()
		: base(MetaDefault)
	{
	}

	private BGDatabase_HealingItem(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_HealingItem(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_HealingItem(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_HealingItem FindEntity(Predicate<BGDatabase_HealingItem> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_HealingItem> FindEntities(Predicate<BGDatabase_HealingItem> filter, List<BGDatabase_HealingItem> result = null, Comparison<BGDatabase_HealingItem> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_HealingItem> action, Predicate<BGDatabase_HealingItem> filter = null, Comparison<BGDatabase_HealingItem> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_HealingItem GetEntity(BGId entityId)
	{
		return (BGDatabase_HealingItem)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_HealingItem GetEntity(int index)
	{
		return (BGDatabase_HealingItem)MetaDefault[index];
	}

	public static BGDatabase_HealingItem GetEntity(string entityName)
	{
		return (BGDatabase_HealingItem)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_HealingItem NewEntity()
	{
		return (BGDatabase_HealingItem)MetaDefault.NewEntity();
	}

	public static BGDatabase_HealingItem NewEntity(BGId entityId)
	{
		return (BGDatabase_HealingItem)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_HealingItem NewEntity(Action<BGDatabase_HealingItem> callback)
	{
		return (BGDatabase_HealingItem)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_HealingItem)entity);
		}));
	}

	public static BGDatabase_HealingItem GetEntityByKeyid(int Keys)
	{
		return (BGDatabase_HealingItem)_id.GetEntityByKey(Keys);
	}
}
