namespace Toked.StatusEffect;

public interface IEffectable
{
	void ApplyStatus(PlayerController playerController, StatusEffectScriptableObject statusEffectScriptableObject, bool executeEvent = true);
}
