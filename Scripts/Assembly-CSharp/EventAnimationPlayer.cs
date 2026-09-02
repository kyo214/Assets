using Toked;
using UnityEngine;

public class EventAnimationPlayer : MonoBehaviour
{
	private Animator animator;

	private PlayerController playerController;

	private void Start()
	{
		animator = GetComponent<Animator>();
		playerController = base.transform.parent.parent.transform.GetComponent<PlayerController>();
	}

	public void OnPlaySFX(string filename)
	{
		if (NetworkGameManager.Instance.ownPlayer != null && !NetworkGameManager.Instance.ownPlayer.IsGhost)
		{
			if (filename == "Steps")
			{
				AudioManager.PlaySFXTransform("steps-" + NetworkGameManager.Instance.ownPlayer.soundStepType.ToString().ToLower(), base.transform.parent, isLocalPlayerTrigger: false);
			}
			else
			{
				AudioManager.PlaySFXTransform(filename, base.transform.parent, isLocalPlayerTrigger: false);
			}
		}
	}

	public void OnPlayVOMale(string filename)
	{
		if (playerController.network.isLocalPlayer && playerController.IsMale)
		{
			AudioManager.PlaySFXTransform(filename, base.transform.parent, isLocalPlayerTrigger: false);
		}
	}

	public void OnPlayVOFemale(string filename)
	{
		if (playerController.network.isLocalPlayer && !playerController.IsMale)
		{
			AudioManager.PlaySFXTransform(filename, base.transform.parent, isLocalPlayerTrigger: false);
		}
	}

	public void StopAnimation()
	{
		animator.speed = 0f;
	}

	public void StopMeleeAnimation()
	{
		if (playerController.isLMBDown && !playerController.isAttackMeleeSwing)
		{
			animator.speed = 0f;
			animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0.83f);
		}
	}

	public void ShowMeleeCollider()
	{
	}

	public void DisactivateObject()
	{
		base.gameObject.SetActive(value: false);
	}
}
