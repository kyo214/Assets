using UnityEngine;

namespace MoreMountains.Tools;

[RequireComponent(typeof(Animator))]
[AddComponentMenu("More Mountains/Tools/Animation/MMOffsetAnimation")]
public class MMOffsetAnimation : MonoBehaviour
{
	public float MinimumRandomRange;

	public float MaximumRandomRange = 1f;

	public int AnimationLayerID;

	public bool OffsetOnStart = true;

	public bool DisableAfterOffset = true;

	protected Animator _animator;

	protected AnimatorStateInfo _stateInfo;

	protected virtual void Awake()
	{
		_animator = base.gameObject.GetComponent<Animator>();
	}

	protected virtual void Start()
	{
		OffsetCurrentAnimation();
	}

	public virtual void OffsetCurrentAnimation()
	{
		if (OffsetOnStart)
		{
			_stateInfo = _animator.GetCurrentAnimatorStateInfo(AnimationLayerID);
			_animator.Play(_stateInfo.fullPathHash, -1, Random.Range(MinimumRandomRange, MaximumRandomRange));
			if (DisableAfterOffset)
			{
				base.enabled = false;
			}
		}
	}
}
