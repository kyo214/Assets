namespace Toked.StatusEffect;

public static class StatusEffectScriptableObjectExtension
{
	public static StatusEffectScriptableObject CloneStatusEffectSO(this StatusEffectScriptableObject statusEffectScriptableObject, bool destroyOnRemove)
	{
		StatusEffectScriptableObject statusEffectScriptableObject2 = statusEffectScriptableObject.Clone();
		statusEffectScriptableObject2.DestroyOnRemove = destroyOnRemove;
		statusEffectScriptableObject2.AdditionalCloneSoData(statusEffectScriptableObject);
		return statusEffectScriptableObject2;
	}

	public static StatusEffectScriptableObject CloneStatusEffectSO(this StatusEffectScriptableObject statusEffectScriptableObject, bool destroyOnRemove, string additionalName)
	{
		StatusEffectScriptableObject statusEffectScriptableObject2 = statusEffectScriptableObject.CloneStatusEffectSO(destroyOnRemove);
		statusEffectScriptableObject2.StatusEffectData.SetAdditionalName(additionalName);
		return statusEffectScriptableObject2;
	}
}
