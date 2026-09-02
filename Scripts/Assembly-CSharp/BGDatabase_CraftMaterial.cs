using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;
using Toked.Crafting;

public class BGDatabase_CraftMaterial : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_CraftMaterial(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_CraftMaterial(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldString _ufle12jhs77_Id;

	private static BGFieldInt _ufle12jhs77_ItemInventoryId;

	private static BGFieldString _ufle12jhs77_Name;

	private static BGFieldEnum _ufle12jhs77_MaterialType;

	private static BGFieldString _ufle12jhs77_MaterialSpritePath;

	private static BGFieldInt _ufle12jhs77_MinDropAmount;

	private static BGFieldInt _ufle12jhs77_MaxDropAmount;

	private static BGFieldString _ufle12jhs77_MaterialNameLocalizeId;

	private static BGFieldString _ufle12jhs77_MaterialDescriptionLocalizeId;

	private static BGKey _edeht3sdad33_ItemInventoryKey;

	private static readonly BGDatabase_GameConfig.Factory _factory0_PFS = new BGDatabase_GameConfig.Factory();

	private static readonly BGDatabase_Character.Factory _factory1_PFS = new BGDatabase_Character.Factory();

	private static readonly BGDatabase_Enemy.Factory _factory2_PFS = new BGDatabase_Enemy.Factory();

	private static readonly BGDatabase_Weapon.Factory _factory3_PFS = new BGDatabase_Weapon.Factory();

	private static readonly BGDatabase_HealingItem.Factory _factory4_PFS = new BGDatabase_HealingItem.Factory();

	private static readonly BGDatabase_Item.Factory _factory5_PFS = new BGDatabase_Item.Factory();

	private static readonly BGDatabase_Ammunition.Factory _factory6_PFS = new BGDatabase_Ammunition.Factory();

	private static readonly Factory _factory7_PFS = new Factory();

	private static readonly BGDatabase_CraftRecipe.Factory _factory8_PFS = new BGDatabase_CraftRecipe.Factory();

	private static readonly BGDatabase_Wave.Factory _factory9_PFS = new BGDatabase_Wave.Factory();

	private static readonly BGDatabase_ItemConvert.Factory _factory10_PFS = new BGDatabase_ItemConvert.Factory();

	private static readonly BGDatabase_Skill.Factory _factory11_PFS = new BGDatabase_Skill.Factory();

	private static readonly BGDatabase_Perks.Factory _factory12_PFS = new BGDatabase_Perks.Factory();

	private static readonly BGDatabase_DefenseWave.Factory _factory13_PFS = new BGDatabase_DefenseWave.Factory();

	private static readonly BGDatabase_ItemDismantle.Factory _factory14_PFS = new BGDatabase_ItemDismantle.Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(5283812163847618131uL, 3875530745284034204uL), () =>
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

	public new string Id
	{
		get
		{
			return _Id[base.Index];
		}
		set
		{
			_Id[base.Index] = value;
		}
	}

	public int ItemInventoryId
	{
		get
		{
			return _ItemInventoryId[base.Index];
		}
		set
		{
			_ItemInventoryId[base.Index] = value;
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

	public CraftMaterialScriptableObject.MaterialType MaterialType
	{
		get
		{
			return (CraftMaterialScriptableObject.MaterialType)_MaterialType.GetStoredValue(base.Index);
		}
		set
		{
			_MaterialType.SetStoredValue(base.Index, (int)value);
		}
	}

	public string MaterialSpritePath
	{
		get
		{
			return _MaterialSpritePath[base.Index];
		}
		set
		{
			_MaterialSpritePath[base.Index] = value;
		}
	}

	public int MinDropAmount
	{
		get
		{
			return _MinDropAmount[base.Index];
		}
		set
		{
			_MinDropAmount[base.Index] = value;
		}
	}

	public int MaxDropAmount
	{
		get
		{
			return _MaxDropAmount[base.Index];
		}
		set
		{
			_MaxDropAmount[base.Index] = value;
		}
	}

	public string MaterialNameLocalizeId
	{
		get
		{
			return _MaterialNameLocalizeId[base.Index];
		}
		set
		{
			_MaterialNameLocalizeId[base.Index] = value;
		}
	}

	public string MaterialDescriptionLocalizeId
	{
		get
		{
			return _MaterialDescriptionLocalizeId[base.Index];
		}
		set
		{
			_MaterialDescriptionLocalizeId[base.Index] = value;
		}
	}

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5256569866577255173uL, 3815744003654502079uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldString _Id => _ufle12jhs77_Id ?? (_ufle12jhs77_Id = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5408911165531447581uL, 8641987924793409427uL), () =>
	{
		_ufle12jhs77_Id = null;
	}));

	public static BGFieldInt _ItemInventoryId => _ufle12jhs77_ItemInventoryId ?? (_ufle12jhs77_ItemInventoryId = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5183243501621129874uL, 9484014343586335880uL), () =>
	{
		_ufle12jhs77_ItemInventoryId = null;
	}));

	public static BGFieldString _Name => _ufle12jhs77_Name ?? (_ufle12jhs77_Name = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5597896596819270690uL, 16921328324179971471uL), () =>
	{
		_ufle12jhs77_Name = null;
	}));

	public static BGFieldEnum _MaterialType => _ufle12jhs77_MaterialType ?? (_ufle12jhs77_MaterialType = BGCodeGenUtils.GetField<BGFieldEnum>(MetaDefault, new BGId(5158690048595218900uL, 13716355596267759534uL), () =>
	{
		_ufle12jhs77_MaterialType = null;
	}));

	public static BGFieldString _MaterialSpritePath => _ufle12jhs77_MaterialSpritePath ?? (_ufle12jhs77_MaterialSpritePath = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5562341778662309100uL, 6120281595214581936uL), () =>
	{
		_ufle12jhs77_MaterialSpritePath = null;
	}));

	public static BGFieldInt _MinDropAmount => _ufle12jhs77_MinDropAmount ?? (_ufle12jhs77_MinDropAmount = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5020066256427070920uL, 6023689238846444215uL), () =>
	{
		_ufle12jhs77_MinDropAmount = null;
	}));

	public static BGFieldInt _MaxDropAmount => _ufle12jhs77_MaxDropAmount ?? (_ufle12jhs77_MaxDropAmount = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5649452081059152057uL, 13883945539708589186uL), () =>
	{
		_ufle12jhs77_MaxDropAmount = null;
	}));

	public static BGFieldString _MaterialNameLocalizeId => _ufle12jhs77_MaterialNameLocalizeId ?? (_ufle12jhs77_MaterialNameLocalizeId = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4796853898753177428uL, 4039183697323213497uL), () =>
	{
		_ufle12jhs77_MaterialNameLocalizeId = null;
	}));

	public static BGFieldString _MaterialDescriptionLocalizeId => _ufle12jhs77_MaterialDescriptionLocalizeId ?? (_ufle12jhs77_MaterialDescriptionLocalizeId = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4754112326343701240uL, 17147632053204912042uL), () =>
	{
		_ufle12jhs77_MaterialDescriptionLocalizeId = null;
	}));

	public static BGKey _ItemInventoryKey => _edeht3sdad33_ItemInventoryKey ?? (_edeht3sdad33_ItemInventoryKey = BGCodeGenUtils.GetKey(MetaDefault, new BGId(5405260255483226186uL, 18195680878057042357uL), () =>
	{
		_edeht3sdad33_ItemInventoryKey = null;
	}));

	private BGDatabase_CraftMaterial()
		: base(MetaDefault)
	{
	}

	private BGDatabase_CraftMaterial(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_CraftMaterial(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_CraftMaterial(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_CraftMaterial FindEntity(Predicate<BGDatabase_CraftMaterial> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_CraftMaterial> FindEntities(Predicate<BGDatabase_CraftMaterial> filter, List<BGDatabase_CraftMaterial> result = null, Comparison<BGDatabase_CraftMaterial> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_CraftMaterial> action, Predicate<BGDatabase_CraftMaterial> filter = null, Comparison<BGDatabase_CraftMaterial> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_CraftMaterial GetEntity(BGId entityId)
	{
		return (BGDatabase_CraftMaterial)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_CraftMaterial GetEntity(int index)
	{
		return (BGDatabase_CraftMaterial)MetaDefault[index];
	}

	public static BGDatabase_CraftMaterial GetEntity(string entityName)
	{
		return (BGDatabase_CraftMaterial)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_CraftMaterial NewEntity()
	{
		return (BGDatabase_CraftMaterial)MetaDefault.NewEntity();
	}

	public static BGDatabase_CraftMaterial NewEntity(BGId entityId)
	{
		return (BGDatabase_CraftMaterial)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_CraftMaterial NewEntity(Action<BGDatabase_CraftMaterial> callback)
	{
		return (BGDatabase_CraftMaterial)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_CraftMaterial)entity);
		}));
	}

	public static BGDatabase_CraftMaterial GetEntityByKeyItemInventoryKey(int ItemInventoryId)
	{
		return (BGDatabase_CraftMaterial)_ItemInventoryKey.GetEntityByKey(ItemInventoryId);
	}
}
