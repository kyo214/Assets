using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "UnlockSkillAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/ActivateRegenHP", order = 0)]
public class ActivateRegenHP : SkillEffectBaseAction
{
	[Header("Regen Settings")]
	[SerializeField]
	private float regenRate = 1f;

	[SerializeField]
	private float regenThreshold = 30f;

	[SerializeField]
	private float regenInterval = 1f;

	private Coroutine regenCoroutine;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		UniTaskUtil.DelayedCall(playerController, 2f, () =>
		{
			playerController.data.StartRegenHp(regenRate, regenThreshold, regenInterval);
		}).Forget();
	}
}
