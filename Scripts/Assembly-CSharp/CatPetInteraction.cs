using Toked;
using UnityEngine;

public class CatPetInteraction : MonoBehaviour
{
	[SerializeField]
	private EnemyController _enemyController;

	private static readonly int IsWakeUp = Animator.StringToHash("IsWakeUp");

	private static readonly int IsSleep = Animator.StringToHash("IsSleep");

	private static readonly int IsResting = Animator.StringToHash("IsResting");

	public void ChangeStateToIdle()
	{
		if (_enemyController.GetCurrentStateHash() == AnimatorHashManager.IdleHash)
		{
			if (_enemyController.animator.GetBool(IsSleep))
			{
				_enemyController.animator.SetBool(IsWakeUp, value: true);
				_enemyController.animator.SetBool(IsResting, value: false);
				_enemyController.animator.SetBool(IsSleep, value: false);
				AudioManager.StopSFXTransform(_enemyController.transform);
			}
			else
			{
				AudioManager.PlaySFXTransform("cat-normal-pet", _enemyController.transform, isLocalPlayerTrigger: false);
			}
		}
		else if (_enemyController.GetCurrentStateHash() != AnimatorHashManager.AlertChasingHash)
		{
			AudioManager.PlaySFXTransform("cat-normal-pet", _enemyController.transform, isLocalPlayerTrigger: false);
			_enemyController.attack.fov.enabled = false;
			_enemyController.AIEnable = false;
			_enemyController.SetEnableAI(value: false);
			_enemyController.SetAISpeed(0f);
			_enemyController.SetState(EnemyState.Idle);
			if (_enemyController.attack.targetChasing != null)
			{
				_enemyController.movement.angleAnim = _enemyController.attack.AngleEnemy((_enemyController.attack.targetChasing.transform.position - _enemyController.attack.transform.position).normalized, _enemyController.movement.angleAnim);
			}
		}
	}
}
