using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "TimeDilationStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/TimeDilationStatusEffectScriptableObject", order = 0)]
public class TimeDilationStatusEffectScriptableObject : CurseItemStatusEffectScriptableObjectBase
{
	[SerializeField]
	private PlayerStatsSO _movementSpeedPlayerStatsSo;

	[SerializeField]
	private float _speedMultiplier = -0.5f;

	private PlayerController pc;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return base.OnApplyEffect(statusEffectController, statusEffect);
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			pc = statusEffectController.PlayerController;
			SetSlowVoiceChatEffect(pc);
			pc.timeline.clock.localTimeScale += _speedMultiplier;
		}
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		base.RemoveEffect(statusEffectController, statusEffect);
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			pc = statusEffectController.PlayerController;
			ResetSlowVoiceChatEffect(pc);
			pc.timeline.clock.localTimeScale -= _speedMultiplier;
		}
	}

	private void SetSlowVoiceChatEffect(PlayerController playerController)
	{
		if (!playerController.network.isLocalPlayer)
		{
			VoiceSoundControl voiceSoundControl = VoiceChatGlobalController.Instance?.GetVoiceSoundControl(playerController.network.VoicePlayerState?.Name);
			if (voiceSoundControl != null)
			{
				voiceSoundControl.EnableSlowMotionEffect();
			}
		}
	}

	private void ResetSlowVoiceChatEffect(PlayerController playerController)
	{
		if (!playerController.network.isLocalPlayer)
		{
			VoiceSoundControl voiceSoundControl = VoiceChatGlobalController.Instance?.GetVoiceSoundControl(playerController.network.VoicePlayerState?.Name);
			if (voiceSoundControl != null)
			{
				voiceSoundControl.DisableSlowMotionEffect();
			}
		}
	}
}
