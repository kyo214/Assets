using System;
using System.Collections.Generic;
using BansheeGz.BGDatabase;
using Toked.Crafting;

public class BGDatabase_CraftRecipe : BGEntity
{
	public class Factory : EntityFactory
	{
		public BGEntity NewEntity(BGMetaEntity meta)
		{
			return new BGDatabase_CraftRecipe(meta);
		}

		public BGEntity NewEntity(BGMetaEntity meta, BGId id)
		{
			return new BGDatabase_CraftRecipe(meta, id);
		}
	}

	private static BGMetaRow _metaDefault;

	private static BGFieldEntityName _ufle12jhs77_name;

	private static BGFieldString _ufle12jhs77_IdRecipe;

	private static BGFieldEnum _ufle12jhs77_RecipeItemType;

	private static BGFieldEnum _ufle12jhs77_RecipeCategoryType;

	private static BGFieldEnum _ufle12jhs77_CraftStation;

	private static BGFieldString _ufle12jhs77_RecipeSpritePath;

	private static BGFieldInt _ufle12jhs77_ItemCraftId;

	private static BGFieldListString _ufle12jhs77_CraftingIngredientsList;

	private static BGFieldInt _ufle12jhs77_CraftAmount;

	private static BGFieldString _ufle12jhs77_ItemNameLocalizeId;

	private static BGFieldString _ufle12jhs77_ItemDescriptionLocalizeId;

	private static readonly BGDatabase_GameConfig.Factory _factory0_PFS = new BGDatabase_GameConfig.Factory();

	private static readonly BGDatabase_Character.Factory _factory1_PFS = new BGDatabase_Character.Factory();

	private static readonly BGDatabase_Enemy.Factory _factory2_PFS = new BGDatabase_Enemy.Factory();

	private static readonly BGDatabase_Weapon.Factory _factory3_PFS = new BGDatabase_Weapon.Factory();

	private static readonly BGDatabase_HealingItem.Factory _factory4_PFS = new BGDatabase_HealingItem.Factory();

	private static readonly BGDatabase_Item.Factory _factory5_PFS = new BGDatabase_Item.Factory();

	private static readonly BGDatabase_Ammunition.Factory _factory6_PFS = new BGDatabase_Ammunition.Factory();

	private static readonly BGDatabase_CraftMaterial.Factory _factory7_PFS = new BGDatabase_CraftMaterial.Factory();

	private static readonly Factory _factory8_PFS = new Factory();

	private static readonly BGDatabase_Wave.Factory _factory9_PFS = new BGDatabase_Wave.Factory();

	private static readonly BGDatabase_ItemConvert.Factory _factory10_PFS = new BGDatabase_ItemConvert.Factory();

	private static readonly BGDatabase_Skill.Factory _factory11_PFS = new BGDatabase_Skill.Factory();

	private static readonly BGDatabase_Perks.Factory _factory12_PFS = new BGDatabase_Perks.Factory();

	private static readonly BGDatabase_DefenseWave.Factory _factory13_PFS = new BGDatabase_DefenseWave.Factory();

	private static readonly BGDatabase_ItemDismantle.Factory _factory14_PFS = new BGDatabase_ItemDismantle.Factory();

	public static BGMetaRow MetaDefault => _metaDefault ?? (_metaDefault = BGCodeGenUtils.GetMeta<BGMetaRow>(new BGId(4712725997132184694uL, 4945246706734754459uL), () =>
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

	public string IdRecipe
	{
		get
		{
			return _IdRecipe[base.Index];
		}
		set
		{
			_IdRecipe[base.Index] = value;
		}
	}

	public CraftRecipeScriptableObject.RecipeItemType RecipeItemType
	{
		get
		{
			return (CraftRecipeScriptableObject.RecipeItemType)_RecipeItemType.GetStoredValue(base.Index);
		}
		set
		{
			_RecipeItemType.SetStoredValue(base.Index, (int)value);
		}
	}

	public CraftRecipeScriptableObject.RecipeCategoryType RecipeCategoryType
	{
		get
		{
			return (CraftRecipeScriptableObject.RecipeCategoryType)_RecipeCategoryType.GetStoredValue(base.Index);
		}
		set
		{
			_RecipeCategoryType.SetStoredValue(base.Index, (int)value);
		}
	}

	public CraftRecipeScriptableObject.CraftStationType CraftStation
	{
		get
		{
			return (CraftRecipeScriptableObject.CraftStationType)_CraftStation.GetStoredValue(base.Index);
		}
		set
		{
			_CraftStation.SetStoredValue(base.Index, (int)value);
		}
	}

	public string RecipeSpritePath
	{
		get
		{
			return _RecipeSpritePath[base.Index];
		}
		set
		{
			_RecipeSpritePath[base.Index] = value;
		}
	}

	public int ItemCraftId
	{
		get
		{
			return _ItemCraftId[base.Index];
		}
		set
		{
			_ItemCraftId[base.Index] = value;
		}
	}

	public List<string> CraftingIngredientsList
	{
		get
		{
			return _CraftingIngredientsList[base.Index];
		}
		set
		{
			_CraftingIngredientsList[base.Index] = value;
		}
	}

	public int CraftAmount
	{
		get
		{
			return _CraftAmount[base.Index];
		}
		set
		{
			_CraftAmount[base.Index] = value;
		}
	}

	public string ItemNameLocalizeId
	{
		get
		{
			return _ItemNameLocalizeId[base.Index];
		}
		set
		{
			_ItemNameLocalizeId[base.Index] = value;
		}
	}

	public string ItemDescriptionLocalizeId
	{
		get
		{
			return _ItemDescriptionLocalizeId[base.Index];
		}
		set
		{
			_ItemDescriptionLocalizeId[base.Index] = value;
		}
	}

	public static BGFieldEntityName _name => _ufle12jhs77_name ?? (_ufle12jhs77_name = BGCodeGenUtils.GetField<BGFieldEntityName>(MetaDefault, new BGId(5163479434646313789uL, 8867807962086242176uL), () =>
	{
		_ufle12jhs77_name = null;
	}));

	public static BGFieldString _IdRecipe => _ufle12jhs77_IdRecipe ?? (_ufle12jhs77_IdRecipe = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5618041772518080279uL, 17109657625892057749uL), () =>
	{
		_ufle12jhs77_IdRecipe = null;
	}));

	public static BGFieldEnum _RecipeItemType => _ufle12jhs77_RecipeItemType ?? (_ufle12jhs77_RecipeItemType = BGCodeGenUtils.GetField<BGFieldEnum>(MetaDefault, new BGId(5635413627671042650uL, 4625210253242481537uL), () =>
	{
		_ufle12jhs77_RecipeItemType = null;
	}));

	public static BGFieldEnum _RecipeCategoryType => _ufle12jhs77_RecipeCategoryType ?? (_ufle12jhs77_RecipeCategoryType = BGCodeGenUtils.GetField<BGFieldEnum>(MetaDefault, new BGId(4910638153108763124uL, 10607809028316355257uL), () =>
	{
		_ufle12jhs77_RecipeCategoryType = null;
	}));

	public static BGFieldEnum _CraftStation => _ufle12jhs77_CraftStation ?? (_ufle12jhs77_CraftStation = BGCodeGenUtils.GetField<BGFieldEnum>(MetaDefault, new BGId(4828290090703998117uL, 13543412414121832101uL), () =>
	{
		_ufle12jhs77_CraftStation = null;
	}));

	public static BGFieldString _RecipeSpritePath => _ufle12jhs77_RecipeSpritePath ?? (_ufle12jhs77_RecipeSpritePath = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5088568327776870005uL, 12802045265324414362uL), () =>
	{
		_ufle12jhs77_RecipeSpritePath = null;
	}));

	public static BGFieldInt _ItemCraftId => _ufle12jhs77_ItemCraftId ?? (_ufle12jhs77_ItemCraftId = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4979254015098708082uL, 6082993387921829031uL), () =>
	{
		_ufle12jhs77_ItemCraftId = null;
	}));

	public static BGFieldListString _CraftingIngredientsList => _ufle12jhs77_CraftingIngredientsList ?? (_ufle12jhs77_CraftingIngredientsList = BGCodeGenUtils.GetField<BGFieldListString>(MetaDefault, new BGId(5158746161390809341uL, 17210701389474786957uL), () =>
	{
		_ufle12jhs77_CraftingIngredientsList = null;
	}));

	public static BGFieldInt _CraftAmount => _ufle12jhs77_CraftAmount ?? (_ufle12jhs77_CraftAmount = BGCodeGenUtils.GetField<BGFieldInt>(MetaDefault, new BGId(4695483374229527506uL, 3653892171393539489uL), () =>
	{
		_ufle12jhs77_CraftAmount = null;
	}));

	public static BGFieldString _ItemNameLocalizeId => _ufle12jhs77_ItemNameLocalizeId ?? (_ufle12jhs77_ItemNameLocalizeId = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(5126758784586355679uL, 17881172834793055416uL), () =>
	{
		_ufle12jhs77_ItemNameLocalizeId = null;
	}));

	public static BGFieldString _ItemDescriptionLocalizeId => _ufle12jhs77_ItemDescriptionLocalizeId ?? (_ufle12jhs77_ItemDescriptionLocalizeId = BGCodeGenUtils.GetField<BGFieldString>(MetaDefault, new BGId(4753602507737118829uL, 11143164695014379664uL), () =>
	{
		_ufle12jhs77_ItemDescriptionLocalizeId = null;
	}));

	public List<BGDatabase_ItemDismantle> RelatedItemDismantleListUsingMaterialValueRelation => BGCodeGenUtils.GetRelatedInbound<BGDatabase_ItemDismantle>(BGDatabase_ItemDismantle._MaterialValue, base.Id);

	private BGDatabase_CraftRecipe()
		: base(MetaDefault)
	{
	}

	private BGDatabase_CraftRecipe(BGId id)
		: base(MetaDefault, id)
	{
	}

	private BGDatabase_CraftRecipe(BGMetaEntity meta)
		: base(meta)
	{
	}

	private BGDatabase_CraftRecipe(BGMetaEntity meta, BGId id)
		: base(meta, id)
	{
	}

	public static BGDatabase_CraftRecipe FindEntity(Predicate<BGDatabase_CraftRecipe> filter)
	{
		return BGCodeGenUtils.FindEntity(MetaDefault, filter);
	}

	public static List<BGDatabase_CraftRecipe> FindEntities(Predicate<BGDatabase_CraftRecipe> filter, List<BGDatabase_CraftRecipe> result = null, Comparison<BGDatabase_CraftRecipe> sort = null)
	{
		return BGCodeGenUtils.FindEntities(MetaDefault, filter, result, sort);
	}

	public static void ForEachEntity(Action<BGDatabase_CraftRecipe> action, Predicate<BGDatabase_CraftRecipe> filter = null, Comparison<BGDatabase_CraftRecipe> sort = null)
	{
		BGCodeGenUtils.ForEachEntity(MetaDefault, action, filter, sort);
	}

	public static BGDatabase_CraftRecipe GetEntity(BGId entityId)
	{
		return (BGDatabase_CraftRecipe)MetaDefault.GetEntity(entityId);
	}

	public static BGDatabase_CraftRecipe GetEntity(int index)
	{
		return (BGDatabase_CraftRecipe)MetaDefault[index];
	}

	public static BGDatabase_CraftRecipe GetEntity(string entityName)
	{
		return (BGDatabase_CraftRecipe)MetaDefault.GetEntity(entityName);
	}

	public static BGDatabase_CraftRecipe NewEntity()
	{
		return (BGDatabase_CraftRecipe)MetaDefault.NewEntity();
	}

	public static BGDatabase_CraftRecipe NewEntity(BGId entityId)
	{
		return (BGDatabase_CraftRecipe)MetaDefault.NewEntity(entityId);
	}

	public static BGDatabase_CraftRecipe NewEntity(Action<BGDatabase_CraftRecipe> callback)
	{
		return (BGDatabase_CraftRecipe)MetaDefault.NewEntity(new BGMetaEntity.NewEntityContext((BGEntity entity) =>
		{
			callback((BGDatabase_CraftRecipe)entity);
		}));
	}
}
