using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using _Modules.CharacterSkin;

namespace Toked.StatusEffect;

public abstract class UtilityItemStatusEffectScriptableObjectBase : StatusEffectScriptableObject, IItemEffect
{
	[SerializeField]
	private int _itemId = -1;

	[SerializeField]
	private SkinScriptableObject _headSkinScriptableObject;

	[SerializeField]
	private SkinScriptableObject _headAccSkinScriptableObject;

	[SerializeField]
	private SkinScriptableObject _bodySkinScriptableObject;

	[SerializeField]
	private SkinScriptableObject _bodyAccSkinScriptableObject;

	[SerializeField]
	private List<StatusEffectScriptableObject> _removetatusEffectsList = new List<StatusEffectScriptableObject>();

	public int ItemId
	{
		get
		{
			return _itemId;
		}
		set
		{
			_itemId = value;
		}
	}

	public int UniqueItemId { get; set; }

	public override bool CantClearEffectAfterFinishedMission => true;

	public virtual void Init(int itemId, int uniqueItemId)
	{
		_itemId = itemId;
		UniqueItemId = uniqueItemId;
	}

	public GameObject GetItemEffectParticle()
	{
		return null;
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return null;
		CheckHaveAntiStatusEffect(statusEffect);
		UpdateStatusEffectData(statusEffectController);
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			GameObject effectParticlePrefab = _statusEffectData.EffectParticlePrefab;
			if ((bool)effectParticlePrefab)
			{
				statusEffect.statusEffectGameObject = Object.Instantiate(effectParticlePrefab, statusEffectController.PlayerController.characterRenderController.PlayerParticleTransform);
			}
			SetHeadSkin(statusEffectController.PlayerController);
			SetHeadAccSkin(statusEffectController.PlayerController);
			SetBodySkin(statusEffectController.PlayerController);
			SetBodyAccSkin(statusEffectController.PlayerController);
		}
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if ((bool)statusEffect.statusEffectGameObject)
		{
			Object.Destroy(statusEffect.statusEffectGameObject);
		}
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			ResetHeadSkin(statusEffectController.PlayerController);
			ResetHeadAccSkin(statusEffectController.PlayerController);
			ResetBodySkin(statusEffectController.PlayerController);
			ResetBodyAccSkin(statusEffectController.PlayerController);
		}
		CheckHaveAntiStatusEffect(statusEffect);
		UpdateStatusEffectData(statusEffectController);
	}

	protected static IEnumerable GetItemId()
	{
		ValueDropdownList<int> result = new ValueDropdownList<int>();
		result.Add("None", -1);
		BGDatabase_Ammunition.ForEachEntity((BGDatabase_Ammunition data) =>
		{
			AddToList("Ammunition/" + data.Name, data.Keys);
		});
		BGDatabase_Weapon.ForEachEntity((BGDatabase_Weapon data) =>
		{
			AddToList("Weapon/" + data.Name, data.Keys);
		});
		BGDatabase_Item.ForEachEntity((BGDatabase_Item data) =>
		{
			AddToList("Item/" + data.Name, data.Keys);
		});
		BGDatabase_HealingItem.ForEachEntity((BGDatabase_HealingItem data) =>
		{
			AddToList("HealingItem/" + data.Name, data.Keys);
		});
		return result;
		void AddToList(string inspectorName, int value)
		{
			result.Add(inspectorName, value);
		}
	}

	protected void SetHeadSkin(PlayerController playerController)
	{
		if (_headSkinScriptableObject != null)
		{
			playerController.data.PlayerSkinData.SetHeadRender(_headSkinScriptableObject.CharacterSkinData, isUtility: true);
		}
	}

	protected void SetHeadAccSkin(PlayerController playerController)
	{
		if (_headAccSkinScriptableObject != null)
		{
			playerController.data.PlayerSkinData.SetHeadAccRender(_headAccSkinScriptableObject.CharacterSkinData, save: true);
		}
	}

	protected void SetBodySkin(PlayerController playerController)
	{
		if (_bodySkinScriptableObject != null)
		{
			playerController.data.PlayerSkinData.SetBodyRender(_bodySkinScriptableObject.CharacterSkinData, isUtility: true);
		}
	}

	protected void SetBodyAccSkin(PlayerController playerController)
	{
		if (_bodyAccSkinScriptableObject != null)
		{
			playerController.data.PlayerSkinData.SetBodyAccRender(_bodyAccSkinScriptableObject.CharacterSkinData, save: true);
		}
	}

	protected void ResetHeadSkin(PlayerController playerController)
	{
		if (_headSkinScriptableObject != null)
		{
			playerController.data.PlayerSkinData.ResetHeadRenderUtility(_headSkinScriptableObject.CharacterSkinData);
		}
	}

	protected void ResetHeadAccSkin(PlayerController playerController)
	{
		if (_headAccSkinScriptableObject != null)
		{
			playerController.data.PlayerSkinData.ResetHeadAccRenderUtility(_headAccSkinScriptableObject.CharacterSkinData);
		}
	}

	protected void ResetBodySkin(PlayerController playerController)
	{
		if (_bodySkinScriptableObject != null)
		{
			playerController.data.PlayerSkinData.ResetBodyRenderUtility(_bodySkinScriptableObject.CharacterSkinData);
		}
	}

	protected void ResetBodyAccSkin(PlayerController playerController)
	{
		if (_bodyAccSkinScriptableObject != null)
		{
			playerController.data.PlayerSkinData.ResetBodyAccRenderUtility(_bodyAccSkinScriptableObject.CharacterSkinData);
		}
	}

	private void UpdateStatusEffectData(StatusEffectController statusEffectController)
	{
		foreach (StatusEffectScriptableObject removetatusEffects in _removetatusEffectsList)
		{
			foreach (StatusEffectController.StatusEffect item in statusEffectController.GetAllStatusEffectsContainName(removetatusEffects?.StatusEffectData.BaseName))
			{
				item.statusEffectScriptableObject.CheckHaveAntiStatusEffect(item);
			}
		}
	}
}
