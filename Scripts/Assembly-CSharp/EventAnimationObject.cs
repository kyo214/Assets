using Toked;
using UnityEngine;

public class EventAnimationObject : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	private PlayerController playerController;

	private void Start()
	{
		if ((object)animator == null)
		{
			animator = GetComponent<Animator>();
		}
	}

	public void TriggerParameter(string param)
	{
		animator.SetTrigger(param);
	}

	public void PlaySFXOnObjectPosition(string filename)
	{
		AudioManager.PlaySFXTransform(filename, base.transform, isLocalPlayerTrigger: false);
	}

	public void PlaySFXGlobalNoPosition(string filename)
	{
		AudioManager.PlaySFX(filename);
	}

	public void StopAnimation()
	{
		animator.speed = 0f;
	}

	public void DeactivateObject()
	{
		base.gameObject.SetActive(value: false);
	}
}
