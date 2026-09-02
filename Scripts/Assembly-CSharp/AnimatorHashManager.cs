using UnityEngine;

public static class AnimatorHashManager
{
	public static int IdleHash = Animator.StringToHash("Idle");

	public static int PatrolHash = Animator.StringToHash("Patrol");

	public static int AlertChasingHash = Animator.StringToHash("AlertChasing");

	public static int ChasingHash = Animator.StringToHash("Chasing");

	public static int AttackingHash = Animator.StringToHash("Attacking");

	public static int DeadHash = Animator.StringToHash("Dead");

	public static int HoveringHash = Animator.StringToHash("Hovering");

	public static bool HasParam(this Animator animator, string paramName)
	{
		AnimatorControllerParameter[] parameters = animator.parameters;
		for (int i = 0; i < parameters.Length; i++)
		{
			if (parameters[i].name == paramName)
			{
				return true;
			}
		}
		return false;
	}
}
