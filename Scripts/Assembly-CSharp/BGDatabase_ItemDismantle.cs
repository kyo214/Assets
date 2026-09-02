using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_ItemDismantle : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_ItemDismantle(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_ItemDismantle(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldInt _ufle12jhs77_ItemId;

	private static BGFieldBool _ufle12jhs77_UseCustomMaterialValue;

	private static BGFieldBool _ufle12jhs77_AdditionalMaterialValue;

	private static BGFieldListString _ufle12jhs77_Material;

	private static BGFieldRelationMultiple _ufle12jhs77_MaterialValue;

	private static BGFieldBool _ufle12jhs77_UseDurability;

	private static BGKey _edeht3sdad33_Item;

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

	private static readonly BGDatabase_DefenseWave.Factory _factory13_PFS = new BGDatabase_DefenseWave.Factory();

	private static readonly Factory _factory14_PFS = new Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(5350960837070366938uL, 14862879954158392732uL), () =>
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

	public int ItemId
	{
		get
		{
			return _ItemId[base.Index];
		}
		set
		{
			_ItemId[base.Index] = value;
		}
	}

	public bool UseCustomMaterialValue
	{
		get
		{
			return _UseCustomMaterialValue[base.Index];
		}
		set
		{
			_UseCustomMaterialValue[base.Index] = value;
		}
	}

	public bool AdditionalMaterialValue
	{
		get
		{
			return _AdditionalMaterialValue[base.Index];
		}
		set
		{
			_AdditionalMaterialValue[base.Index] = value;
		}
	}

	public List<string> Material
	{
		get
		{
			return _Material[base.Index];
		}
		set
		{
			_Material[base.Index] = value;
		}
	}

	public List<BGDatabase_CraftRecipe> MaterialValue
	{
		get
		{
			return BGCodeGenUtils.MultipleRelationGet<BGDatabase_CraftRecipe>(_MaterialValue, base.Index);
		}
		set
		{
			BGCodeGenUtils.MultipleRelationSet(_MaterialValue, base.Index, value);
		}
	}

	public bool UseDurability
	{
		get
		{
			return _UseDurability[base.Index];
		}
		set
		{
			_UseDurability[base.Index] = value;
		}
	}

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5515881067392909750uL, 12219156349611985587uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldInt _ItemId => _ufle12jhs77_ItemId ?? (_ufle12jhs77_ItemId = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4752268778869515901uL, 12931935431214158513uL), () =>
	{
		_ufle12jhs77_ItemId = null;
	}));

	public static BGFieldBool _UseCustomMaterialValue => _ufle12jhs77_UseCustomMaterialValue ?? (_ufle12jhs77_UseCustomMaterialValue = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(5040764048955963612uL, 265856565088467609uL), () =>
	{
		_ufle12jhs77_UseCustomMaterialValue = null;
	}));

	public static BGFieldBool _AdditionalMaterialValue => _ufle12jhs77_AdditionalMaterialValue ?? (_ufle12jhs77_AdditionalMaterialValue = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4851713842087570508uL, 11621295583856285864uL), () =>
	{
		_ufle12jhs77_AdditionalMaterialValue = null;
	}));

	public static BGFieldListString _Material => _ufle12jhs77_Material ?? (_ufle12jhs77_Material = BGCodeGenUtils.GetField<BGFieldListString>(MetaDefault, new BGId(5039295586628085484uL, 6303701133812839604uL), () =>
	{
		_ufle12jhs77_Material = null;
	}));

	public static BGFieldRelationMultiple _MaterialValue => _ufle12jhs77_MaterialValue ?? (_ufle12jhs77_MaterialValue = BGCodeGenUtils.GetField<BGFieldRelationMultiple>(MetaDefault, new BGId(4836103892497275275uL, 4267875226009123236uL), () =>
	{
		_ufle12jhs77_MaterialValue = null;
	}));

	public static BGFieldBool _UseDurability => _ufle12jhs77_UseDurability ?? (_ufle12jhs77_UseDurability = BGCodeGenUtils.GetField<BGFieldBool>(MetaDefault, new BGId(4636044326206115255uL, 4212527256083985813uL), () =>
	{
		_ufle12jhs77_UseDurability = null;
	}));

	public static BGKey _Item => _edeht3sdad33_Item ?? (_edeht3sdad33_Item = BGCodeGenUtils.GetKey(MetaDefault, new BGId(5331509864832474603uL, 16758436369192984998uL), () =>
	{
		_edeht3sdad33_Item = null;
	}));

	private BGDatabase_ItemDismantle()
		: base(MetaDefault)
	{
	}

	private BGDatabase_ItemDismantle(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_ItemDismantle(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_ItemDismantle(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_ItemDismantle FindEntity(Predicate<BGDatabase_ItemDismantle> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_ItemDismantle> FindEntities(Predicate<BGDatabase_ItemDismantle> filter, List<BGDatabase_ItemDismantle> result = null, Comparison<BGDatabase_ItemDismantle> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_ItemDismantle> action, Predicate<BGDatabase_ItemDismantle> filter = null, Comparison<BGDatabase_ItemDismantle> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_ItemDismantle GetEntity(BGId entityId)
	{
		return (BGDatabase_ItemDismantle)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_ItemDismantle GetEntity(int index)
	{
		return (BGDatabase_ItemDismantle)MetaDefault[index];
	}

	public static BGDatabase_ItemDismantle GetEntity(string entityName)
	{
		return (BGDatabase_ItemDismantle)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_ItemDismantle NewEntity()
	{
		return (BGDatabase_ItemDismantle)MetaDefault.NewEntity();
	}

	public static BGDatabase_ItemDismantle NewEntity(BGId entityId)
	{
		return (BGDatabase_ItemDismantle)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_ItemDismantle NewEntity(Action<BGDatabase_ItemDismantle> callback)
	{
		return (BGDatabase_ItemDismantle)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_ItemDismantle)entity);
		}));
	}

	public void MaterialValue_Add(BGDatabase_CraftRecipe related)
	{
		BGCodeGenUtils.MultipleRelationAdd(_MaterialValue, base.Index, related);
	}

	public void MaterialValue_Remove(BGDatabase_CraftRecipe related)
	{
		BGCodeGenUtils.MultipleRelationRemove(_MaterialValue, base.Index, related);
	}

	public static BGDatabase_ItemDismantle GetEntityByKeyItem(int ItemId)
	{
		return (BGDatabase_ItemDismantle)_Item.GetEntityByKey(ItemId);
	}

	public static List<BGDatabase_ItemDismantle> GetEntitiesByKeyItem(int ItemId, List<BGDatabase_ItemDismantle> result = null)
	{
		return _Item.GetEntitiesByKey(result, ItemId);
	}
}
