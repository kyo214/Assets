using Toked.StatusEffect;

namespace _Modules.Item.Scripts;

public static class ItemIntractableStatusEffectExtension
{
	public static ItemIntractableStatusEffect AddItemStatusEffect(this ItemPickable itemPickable, params StatusEffectScriptableObject[] statusEffectScriptableObjects)
	{
		if (!itemPickable)
		{
			return null;
		}
		ItemIntractableStatusEffect itemIntractableStatusEffect = (itemPickable.ItemIntractableStatusEffect = itemPickable.gameObject.AddComponent<ItemIntractableStatusEffect>());
		itemIntractableStatusEffect.ItemPickable = itemPickable;
		itemIntractableStatusEffect.StatusEffectScriptableObjectList.AddRange(statusEffectScriptableObjects);
		itemIntractableStatusEffect.ApplyItemEffect();
		return itemIntractableStatusEffect;
	}
}
