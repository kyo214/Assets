using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_Item : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_Item(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_Item(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldInt _ufle12jhs77_Keys;

	private static BGFieldString _ufle12jhs77_Name;

	private static BGFieldString _ufle12jhs77_Alias;

	private static BGFieldBool _ufle12jhs77_IsUsable;

	private static BGFieldBool _ufle12jhs77_IsOpenable;

	private static BGFieldInt _ufle12jhs77_MaxItemInSlot;

	private static BGFieldString _ufle12jhs77_CombineItem0;

	private static BGFieldString _ufle12jhs77_CombineItem1;

	private static BGFieldString _ufle12jhs77_SpawnItemOpen;

	private static BGFieldString _ufle12jhs77_ShowPuzzleUI;

	private static BGFieldString _ufle12jhs77_UseFunction;

	private static BGFieldBool _ufle12jhs77_IsNotKeyItem;

	private static BGFieldListString _ufle12jhs77_ConvertMaterial;

	private static BGFieldBool _ufle12jhs77_CanConvertToMaterial;

	private static BGFieldInt _ufle12jhs77_Durability;

	private static BGKey _edeht3sdad33_id;

	private static readonly BGDatabase_GameConfig.Factory _factory0_PFS = new BGDatabase_GameConfig.Factory();

	private static readonly BGDatabase_Character.Factory _factory1_PFS = new BGDatabase_Character.Factory();

	private static readonly BGDatabase_Enemy.Factory _factory2_PFS = new BGDatabase_Enemy.Factory();

	private static readonly BGDatabase_Weapon.Factory _factory3_PFS = new BGDatabase_Weapon.Factory();

	private static readonly BGDatabase_HealingItem.Factory _factory4_PFS = new BGDatabase_HealingItem.Factory();

	private static readonly Factory _factory5_PFS = new Factory();

	private static readonly BGDatabase_Ammunition.Factory _factory6_PFS = new BGDatabase_Ammunition.Factory();

	private static readonly BGDatabase_CraftMaterial.Factory _factory7_PFS = new BGDatabase_CraftMaterial.Factory();

	private static readonly BGDatabase_CraftRecipe.Factory _factory8_PFS = new BGDatabase_CraftRecipe.Factory();

	private static readonly BGDatabase_Wave.Factory _factory9_PFS = new BGDatabase_Wave.Factory();

	private static readonly BGDatabase_ItemConvert.Factory _factory10_PFS = new BGDatabase_ItemConvert.Factory();

	private static readonly BGDatabase_Skill.Factory _factory11_PFS = new BGDatabase_Skill.Factory();

	private static readonly BGDatabase_Perks.Factory _factory12_PFS = new BGDatabase_Perks.Factory();

	private static readonly BGDatabase_DefenseWave.Factory _factory13_PFS = new BGDatabase_DefenseWave.Factory();

	private static readonly BGDatabase_ItemDismantle.Factory _factory14_PFS = new BGDatabase_ItemDismantle.Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(5120347978067412310uL, 10818504590044055467uL), () =>
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

	public string Alias
	{
		get
		{
			return _Alias[base.Index];
		}
		set
		{
			_Alias[base.Index] = value;
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

	public bool IsOpenable
	{
		get
		{
			return _IsOpenable[base.Index];
		}
		set
		{
			_IsOpenable[base.Index] = value;
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

	public string SpawnItemOpen
	{
		get
		{
			return _SpawnItemOpen[base.Index];
		}
		set
		{
			_SpawnItemOpen[base.Index] = value;
		}
	}

	public string ShowPuzzleUI
	{
		get
		{
			return _ShowPuzzleUI[base.Index];
		}
		set
		{
			_ShowPuzzleUI[base.Index] = value;
		}
	}

	public string UseFunction
	{
		get
		{
			return _UseFunction[base.Index];
		}
		set
		{
			_UseFunction[base.Index] = value;
		}
	}

	public bool IsNotKeyItem
	{
		get
		{
			return _IsNotKeyItem[base.Index];
		}
		set
		{
			_IsNotKeyItem[base.Index] = value;
		}
	}

	public List<string> ConvertMaterial
	{
		get
		{
			return _ConvertMaterial[base.Index];
		}
		set
		{
			_ConvertMaterial[base.Index] = value;
		}
	}

	public bool CanConvertToMaterial
	{
		get
		{
			return _CanConvertToMaterial[base.Index];
		}
		set
		{
			_CanConvertToMaterial[base.Index] = value;
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

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5243376740513088961uL, 15360798764809457584uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldInt _Keys => _ufle12jhs77_Keys ?? (_ufle12jhs77_Keys = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5515437945802011556uL, 6698331367984149916uL), () =>
	{
		_ufle12jhs77_Keys = null;
	}));

	public static BGFieldString _Name => _ufle12jhs77_Name ?? (_ufle12jhs77_Name = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5533898748970815478uL, 17732796265324442752uL), () =>
	{
		_ufle12jhs77_Name = null;
	}));

	public static BGFieldString _Alias => _ufle12jhs77_Alias ?? (_ufle12jhs77_Alias = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5065120333975416480uL, 2953270571175357066uL), () =>
	{
		_ufle12jhs77_Alias = null;
	}));

	public static BGFieldBool _IsUsable => _ufle12jhs77_IsUsable ?? (_ufle12jhs77_IsUsable = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4881740293763384784uL, 12321705101638562983uL), () =>
	{
		_ufle12jhs77_IsUsable = null;
	}));

	public static BGFieldBool _IsOpenable => _ufle12jhs77_IsOpenable ?? (_ufle12jhs77_IsOpenable = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5350779903877428998uL, 2720985613934981014uL), () =>
	{
		_ufle12jhs77_IsOpenable = null;
	}));

	public static BGFieldInt _MaxItemInSlot => _ufle12jhs77_MaxItemInSlot ?? (_ufle12jhs77_MaxItemInSlot = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4926540392631339173uL, 6129015185383352200uL), () =>
	{
		_ufle12jhs77_MaxItemInSlot = null;
	}));

	public static BGFieldString _CombineItem0 => _ufle12jhs77_CombineItem0 ?? (_ufle12jhs77_CombineItem0 = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5242772336701779558uL, 3917177689157152170uL), () =>
	{
		_ufle12jhs77_CombineItem0 = null;
	}));

	public static BGFieldString _CombineItem1 => _ufle12jhs77_CombineItem1 ?? (_ufle12jhs77_CombineItem1 = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4881036796083181549uL, 17850255985906597816uL), () =>
	{
		_ufle12jhs77_CombineItem1 = null;
	}));

	public static BGFieldString _SpawnItemOpen => _ufle12jhs77_SpawnItemOpen ?? (_ufle12jhs77_SpawnItemOpen = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5616378957010044569uL, 8003407476711760517uL), () =>
	{
		_ufle12jhs77_SpawnItemOpen = null;
	}));

	public static BGFieldString _ShowPuzzleUI => _ufle12jhs77_ShowPuzzleUI ?? (_ufle12jhs77_ShowPuzzleUI = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5288703177172459084uL, 7867439555271975074uL), () =>
	{
		_ufle12jhs77_ShowPuzzleUI = null;
	}));

	public static BGFieldString _UseFunction => _ufle12jhs77_UseFunction ?? (_ufle12jhs77_UseFunction = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5457162836345675802uL, 14915216124147027852uL), () =>
	{
		_ufle12jhs77_UseFunction = null;
	}));

	public static BGFieldBool _IsNotKeyItem => _ufle12jhs77_IsNotKeyItem ?? (_ufle12jhs77_IsNotKeyItem = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5763327000287905900uL, 12446837991491279766uL), () =>
	{
		_ufle12jhs77_IsNotKeyItem = null;
	}));

	public static BGFieldListString _ConvertMaterial => _ufle12jhs77_ConvertMaterial ?? (_ufle12jhs77_ConvertMaterial = BGCodeGenUtils.GetField<BGFieldListString>(MetaDefault, new BGId(5262286243466218524uL, 655281220902953357uL), () =>
	{
		_ufle12jhs77_ConvertMaterial = null;
	}));

	public static BGFieldBool _CanConvertToMaterial => _ufle12jhs77_CanConvertToMaterial ?? (_ufle12jhs77_CanConvertToMaterial = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4920377278852126806uL, 6250977568833571234uL), () =>
	{
		_ufle12jhs77_CanConvertToMaterial = null;
	}));

	public static BGFieldInt _Durability => _ufle12jhs77_Durability ?? (_ufle12jhs77_Durability = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5055965203466322805uL, 1082884662110542751uL), () =>
	{
		_ufle12jhs77_Durability = null;
	}));

	public static BGKey _id => _edeht3sdad33_id ?? (_edeht3sdad33_id = BGCodeGenUtils.GetKey(MetaDefault, new BGId(5005136311133254912uL, 7192177665668609951uL), () =>
	{
		_edeht3sdad33_id = null;
	}));

	private BGDatabase_Item()
		: base(MetaDefault)
	{
	}

	private BGDatabase_Item(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_Item(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_Item(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_Item FindEntity(Predicate<BGDatabase_Item> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_Item> FindEntities(Predicate<BGDatabase_Item> filter, List<BGDatabase_Item> result = null, Comparison<BGDatabase_Item> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_Item> action, Predicate<BGDatabase_Item> filter = null, Comparison<BGDatabase_Item> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_Item GetEntity(BGId entityId)
	{
		return (BGDatabase_Item)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_Item GetEntity(int index)
	{
		return (BGDatabase_Item)MetaDefault[index];
	}

	public static BGDatabase_Item GetEntity(string entityName)
	{
		return (BGDatabase_Item)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_Item NewEntity()
	{
		return (BGDatabase_Item)MetaDefault.NewEntity();
	}

	public static BGDatabase_Item NewEntity(BGId entityId)
	{
		return (BGDatabase_Item)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_Item NewEntity(Action<BGDatabase_Item> callback)
	{
		return (BGDatabase_Item)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_Item)entity);
		}));
	}

	public static BGDatabase_Item GetEntityByKeyid(int Keys)
	{
		return (BGDatabase_Item)_id.GetEntityByKey(Keys);
	}
}
