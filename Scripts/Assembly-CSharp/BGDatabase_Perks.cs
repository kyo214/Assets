using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_Perks : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_Perks(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_Perks(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldString _ufle12jhs77_Id;

	private static BGFieldString _ufle12jhs77_PerkSpritePath;

	private static BGFieldString _ufle12jhs77_PerkNameLocalizeId;

	private static BGFieldString _ufle12jhs77_PerkDescriptionLocalizeId;

	private static BGFieldListString _ufle12jhs77_SkillEffectSoPath;

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

	private static readonly Factory _factory12_PFS = new Factory();

	private static readonly BGDatabase_DefenseWave.Factory _factory13_PFS = new BGDatabase_DefenseWave.Factory();

	private static readonly BGDatabase_ItemDismantle.Factory _factory14_PFS = new BGDatabase_ItemDismantle.Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(4953815419179254432uL, 3217539924419569793uL), () =>
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

	public string PerkSpritePath
	{
		get
		{
			return _PerkSpritePath[base.Index];
		}
		set
		{
			_PerkSpritePath[base.Index] = value;
		}
	}

	public string PerkNameLocalizeId
	{
		get
		{
			return _PerkNameLocalizeId[base.Index];
		}
		set
		{
			_PerkNameLocalizeId[base.Index] = value;
		}
	}

	public string PerkDescriptionLocalizeId
	{
		get
		{
			return _PerkDescriptionLocalizeId[base.Index];
		}
		set
		{
			_PerkDescriptionLocalizeId[base.Index] = value;
		}
	}

	public List<string> SkillEffectSoPath
	{
		get
		{
			return _SkillEffectSoPath[base.Index];
		}
		set
		{
			_SkillEffectSoPath[base.Index] = value;
		}
	}

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5396505377462010642uL, 16712744426566691482uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldString _Id => _ufle12jhs77_Id ?? (_ufle12jhs77_Id = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5666435845826157912uL, 2665546902460363393uL), () =>
	{
		_ufle12jhs77_Id = null;
	}));

	public static BGFieldString _PerkSpritePath => _ufle12jhs77_PerkSpritePath ?? (_ufle12jhs77_PerkSpritePath = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5200137817230798268uL, 18023659616570220463uL), () =>
	{
		_ufle12jhs77_PerkSpritePath = null;
	}));

	public static BGFieldString _PerkNameLocalizeId => _ufle12jhs77_PerkNameLocalizeId ?? (_ufle12jhs77_PerkNameLocalizeId = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5198810296188583674uL, 3785729888985386893uL), () =>
	{
		_ufle12jhs77_PerkNameLocalizeId = null;
	}));

	public static BGFieldString _PerkDescriptionLocalizeId => _ufle12jhs77_PerkDescriptionLocalizeId ?? (_ufle12jhs77_PerkDescriptionLocalizeId = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5742026601025987836uL, 13752299956893248644uL), () =>
	{
		_ufle12jhs77_PerkDescriptionLocalizeId = null;
	}));

	public static BGFieldListString _SkillEffectSoPath => _ufle12jhs77_SkillEffectSoPath ?? (_ufle12jhs77_SkillEffectSoPath = BGCodeGenUtils.GetField<BGFieldListString>(MetaDefault, new BGId(5279558671773412076uL, 14912769550019985584uL), () =>
	{
		_ufle12jhs77_SkillEffectSoPath = null;
	}));

	private BGDatabase_Perks()
		: base(MetaDefault)
	{
	}

	private BGDatabase_Perks(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_Perks(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_Perks(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_Perks FindEntity(Predicate<BGDatabase_Perks> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_Perks> FindEntities(Predicate<BGDatabase_Perks> filter, List<BGDatabase_Perks> result = null, Comparison<BGDatabase_Perks> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_Perks> action, Predicate<BGDatabase_Perks> filter = null, Comparison<BGDatabase_Perks> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_Perks GetEntity(BGId entityId)
	{
		return (BGDatabase_Perks)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_Perks GetEntity(int index)
	{
		return (BGDatabase_Perks)MetaDefault[index];
	}

	public static BGDatabase_Perks GetEntity(string entityName)
	{
		return (BGDatabase_Perks)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_Perks NewEntity()
	{
		return (BGDatabase_Perks)MetaDefault.NewEntity();
	}

	public static BGDatabase_Perks NewEntity(BGId entityId)
	{
		return (BGDatabase_Perks)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_Perks NewEntity(Action<BGDatabase_Perks> callback)
	{
		return (BGDatabase_Perks)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_Perks)entity);
		}));
	}
}
