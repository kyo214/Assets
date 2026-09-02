using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Toked.StatusEffect;

public abstract class CurseItemStatusEffectScriptableObjectBase : StatusEffectScriptableObject, IItemEffect
{
	[SerializeField]
	private int _itemCurseId = -1;

	[SerializeField]
	private bool _overrideUniqueItemId;

	[SerializeField]
	private int _uniqueItemId = -1;

	[SerializeField]
	private GameObject _itemEffectParticle;

	public int ItemId
	{
		get
		{
			return _itemCurseId;
		}
		set
		{
			_itemCurseId = value;
		}
	}

	public int UniqueItemId
	{
		get
		{
			return _uniqueItemId;
		}
		set
		{
			_uniqueItemId = value;
		}
	}

	public virtual void Init(int itemId, int uniqueItemId)
	{
		_itemCurseId = itemId;
		UniqueItemId = uniqueItemId;
	}

	public GameObject GetItemEffectParticle()
	{
		return _itemEffectParticle;
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return null;
		CheckHaveAntiStatusEffect(statusEffect);
		GameObject effectParticlePrefab = _statusEffectData.EffectParticlePrefab;
		if ((bool)effectParticlePrefab)
		{
			statusEffect.statusEffectGameObject = Object.Instantiate(effectParticlePrefab, statusEffectController.PlayerController.characterRenderController.PlayerParticleTransform);
		}
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if ((bool)statusEffect.statusEffectGameObject)
		{
			Object.Destroy(statusEffect.statusEffectGameObject);
		}
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
}
