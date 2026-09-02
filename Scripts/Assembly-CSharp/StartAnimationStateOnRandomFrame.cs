using UnityEngine;

public class StartAnimationStateOnRandomFrame : MonoBehaviour
{
	private Animator animator;

	private void Start()
	{
		animator = GetComponent<Animator>();
		AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
		if (currentAnimatorClipInfo.Length != 0)
		{
			float length = currentAnimatorClipInfo[0].clip.length;
			float normalizedTime = Random.Range(0f, length) / length;
			animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, normalizedTime);
		}
	}
}
