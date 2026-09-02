using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;

public class BGDatabase_Skill : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_Skill(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_Skill(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldString _ufle12jhs77_Id;

	private static BGFieldString _ufle12jhs77_SkillSpritePath;

	private static BGFieldString _ufle12jhs77_SkillNameLocalizeId;

	private static BGFieldString _ufle12jhs77_SkillDescriptionLocalizeId;

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

	private static readonly Factory _factory11_PFS = new Factory();

	private static readonly BGDatabase_Perks.Factory _factory12_PFS = new BGDatabase_Perks.Factory();

	private static readonly BGDatabase_DefenseWave.Factory _factory13_PFS = new BGDatabase_DefenseWave.Factory();

	private static readonly BGDatabase_ItemDismantle.Factory _factory14_PFS = new BGDatabase_ItemDismantle.Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(4875482473428388540uL, 8227689077599942801uL), () =>
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

	public string SkillSpritePath
	{
		get
		{
			return _SkillSpritePath[base.Index];
		}
		set
		{
			_SkillSpritePath[base.Index] = value;
		}
	}

	public string SkillNameLocalizeId
	{
		get
		{
			return _SkillNameLocalizeId[base.Index];
		}
		set
		{
			_SkillNameLocalizeId[base.Index] = value;
		}
	}

	public string SkillDescriptionLocalizeId
	{
		get
		{
			return _SkillDescriptionLocalizeId[base.Index];
		}
		set
		{
			_SkillDescriptionLocalizeId[base.Index] = value;
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

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5140497469582647005uL, 5627518702669492900uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldString _Id => _ufle12jhs77_Id ?? (_ufle12jhs77_Id = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5570345961936843029uL, 17546364120330088115uL), () =>
	{
		_ufle12jhs77_Id = null;
	}));

	public static BGFieldString _SkillSpritePath => _ufle12jhs77_SkillSpritePath ?? (_ufle12jhs77_SkillSpritePath = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5105428908266702629uL, 10726479432288269992uL), () =>
	{
		_ufle12jhs77_SkillSpritePath = null;
	}));

	public static BGFieldString _SkillNameLocalizeId => _ufle12jhs77_SkillNameLocalizeId ?? (_ufle12jhs77_SkillNameLocalizeId = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4823708512861708927uL, 15049539059719470003uL), () =>
	{
		_ufle12jhs77_SkillNameLocalizeId = null;
	}));

	public static BGFieldString _SkillDescriptionLocalizeId => _ufle12jhs77_SkillDescriptionLocalizeId ?? (_ufle12jhs77_SkillDescriptionLocalizeId = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5257403748169801580uL, 8206747900274291388uL), () =>
	{
		_ufle12jhs77_SkillDescriptionLocalizeId = null;
	}));

	public static BGFieldListString _SkillEffectSoPath => _ufle12jhs77_SkillEffectSoPath ?? (_ufle12jhs77_SkillEffectSoPath = BGCodeGenUtils.GetField<BGFieldListString>(MetaDefault, new BGId(4886879471462344268uL, 17925577913936809402uL), () =>
	{
		_ufle12jhs77_SkillEffectSoPath = null;
	}));

	private BGDatabase_Skill()
		: base(MetaDefault)
	{
	}

	private BGDatabase_Skill(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_Skill(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_Skill(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_Skill FindEntity(Predicate<BGDatabase_Skill> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_Skill> FindEntities(Predicate<BGDatabase_Skill> filter, List<BGDatabase_Skill> result = null, Comparison<BGDatabase_Skill> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_Skill> action, Predicate<BGDatabase_Skill> filter = null, Comparison<BGDatabase_Skill> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_Skill GetEntity(BGId entityId)
	{
		return (BGDatabase_Skill)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_Skill GetEntity(int index)
	{
		return (BGDatabase_Skill)MetaDefault[index];
	}

	public static BGDatabase_Skill GetEntity(string entityName)
	{
		return (BGDatabase_Skill)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_Skill NewEntity()
	{
		return (BGDatabase_Skill)MetaDefault.NewEntity();
	}

	public static BGDatabase_Skill NewEntity(BGId entityId)
	{
		return (BGDatabase_Skill)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_Skill NewEntity(Action<BGDatabase_Skill> callback)
	{
		return (BGDatabase_Skill)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_Skill)entity);
		}));
	}
}
