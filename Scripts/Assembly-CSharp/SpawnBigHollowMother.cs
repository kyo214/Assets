using Cysharp.Threading.Tasks;
using Toked;
using UnityEngine;

public class SpawnBigHollowMother : StateMachineBehaviour
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		AudioManager.PlayBGM("BGM", "AlphaHollowMother");
		AudioManager.SetBGMFixed(value: true);
		if (NetworkGameManager.Instance.isServer)
		{
			GameManager.Instance.gameManagerPhoton.RpcSpawnPortalPosition(MathFunc.EncodeVector3ToULong(animator.transform.position), 104);
			GameManager.Instance.waveManager.cueHordeTimer.interval = 0.1f;
			UniTaskUtil.DelayedCall(GameManager.Instance, 0.5f, () =>
			{
				GameManager.Instance.waveManager.buildUpHordeTimer.interval = 0.1f;
			}, ignoreTimeScale: false).Forget();
			UniTaskUtil.DelayedCall(GameManager.Instance, 1f, () =>
			{
				GameManager.Instance.waveManager.hordeTimer.interval = 0.1f;
			}, ignoreTimeScale: false).Forget();
		}
	}
}
