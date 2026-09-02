using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_ItemConvert : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_ItemConvert(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_ItemConvert(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldInt _ufle12jhs77_ItemId;

	private static BGFieldListString _ufle12jhs77_Material;

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

	private static readonly Factory _factory10_PFS = new Factory();

	private static readonly BGDatabase_Skill.Factory _factory11_PFS = new BGDatabase_Skill.Factory();

	private static readonly BGDatabase_Perks.Factory _factory12_PFS = new BGDatabase_Perks.Factory();

	private static readonly BGDatabase_DefenseWave.Factory _factory13_PFS = new BGDatabase_DefenseWave.Factory();

	private static readonly BGDatabase_ItemDismantle.Factory _factory14_PFS = new BGDatabase_ItemDismantle.Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(5233316909803647810uL, 8352028221668089475uL), () =>
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

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5607154579534627712uL, 16162620561711226044uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldInt _ItemId => _ufle12jhs77_ItemId ?? (_ufle12jhs77_ItemId = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(5453380839123995204uL, 12297182875870371503uL), () =>
	{
		_ufle12jhs77_ItemId = null;
	}));

	public static BGFieldListString _Material => _ufle12jhs77_Material ?? (_ufle12jhs77_Material = BGCodeGenUtils.GetField<BGFieldListString>(MetaDefault, new BGId(5599927428664658405uL, 7447705956108191633uL), () =>
	{
		_ufle12jhs77_Material = null;
	}));

	public static BGKey _Item => _edeht3sdad33_Item ?? (_edeht3sdad33_Item = BGCodeGenUtils.GetKey(MetaDefault, new BGId(5456923007315226256uL, 791588968882272899uL), () =>
	{
		_edeht3sdad33_Item = null;
	}));

	private BGDatabase_ItemConvert()
		: base(MetaDefault)
	{
	}

	private BGDatabase_ItemConvert(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_ItemConvert(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_ItemConvert(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_ItemConvert FindEntity(Predicate<BGDatabase_ItemConvert> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_ItemConvert> FindEntities(Predicate<BGDatabase_ItemConvert> filter, List<BGDatabase_ItemConvert> result = null, Comparison<BGDatabase_ItemConvert> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_ItemConvert> action, Predicate<BGDatabase_ItemConvert> filter = null, Comparison<BGDatabase_ItemConvert> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_ItemConvert GetEntity(BGId entityId)
	{
		return (BGDatabase_ItemConvert)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_ItemConvert GetEntity(int index)
	{
		return (BGDatabase_ItemConvert)MetaDefault[index];
	}

	public static BGDatabase_ItemConvert GetEntity(string entityName)
	{
		return (BGDatabase_ItemConvert)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_ItemConvert NewEntity()
	{
		return (BGDatabase_ItemConvert)MetaDefault.NewEntity();
	}

	public static BGDatabase_ItemConvert NewEntity(BGId entityId)
	{
		return (BGDatabase_ItemConvert)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_ItemConvert NewEntity(Action<BGDatabase_ItemConvert> callback)
	{
		return (BGDatabase_ItemConvert)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_ItemConvert)entity);
		}));
	}

	public static BGDatabase_ItemConvert GetEntityByKeyItem(int ItemId)
	{
		return (BGDatabase_ItemConvert)_Item.GetEntityByKey(ItemId);
	}

	public static List<BGDatabase_ItemConvert> GetEntitiesByKeyItem(int ItemId, List<BGDatabase_ItemConvert> result = null)
	{
		return _Item.GetEntitiesByKey(result, ItemId);
	}
}
