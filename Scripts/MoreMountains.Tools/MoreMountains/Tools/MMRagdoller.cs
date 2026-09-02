using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Animation/MMRagdoller")]
public class MMRagdoller : MonoBehaviour
{
	public enum RagdollStates
	{
		Animated = 0,
		Ragdolling = 1,
		Blending = 2
	}

	[Header("Ragdoll")]
	public RagdollStates CurrentState;

	public float RagdollToMecanimBlendDuration = 0.5f;

	[Header("Rigidbodies")]
	public Rigidbody MainRigidbody;

	public bool ForceSleep = true;

	protected float _mecanimToGetUpTransitionTime = 0.05f;

	protected float _ragdollingEndTimestamp = -100f;

	protected Vector3 _ragdolledHipPosition;

	protected Vector3 _ragdolledHeadPosition;

	protected Vector3 _ragdolledFeetPosition;

	protected List<RagdollBodyPart> _bodyparts = new List<RagdollBodyPart>();

	protected Animator _animator;

	protected List<Component> _rigidbodiesTempList;

	protected Component[] _rigidbodies;

	protected HashSet<int> _animatorParameters;

	protected const string _getUpFromBackAnimationParameterName = "GetUpFromBack";

	protected int _getUpFromBackAnimationParameter;

	protected const string _getUpFromBellyAnimationParameterName = "GetUpFromBelly";

	protected int _getUpFromBellyAnimationParameter;

	public bool Ragdolling
	{
		get
		{
			return CurrentState != RagdollStates.Animated;
		}
		set
		{
			if (value)
			{
				if (CurrentState == RagdollStates.Animated)
				{
					SetIsKinematic(isKinematic: false);
					_animator.enabled = false;
					CurrentState = RagdollStates.Ragdolling;
					MMAnimatorExtensions.UpdateAnimatorBool(_animator, _getUpFromBackAnimationParameter, value: false, _animatorParameters);
					MMAnimatorExtensions.UpdateAnimatorBool(_animator, _getUpFromBellyAnimationParameter, value: false, _animatorParameters);
				}
			}
			else
			{
				if (CurrentState != RagdollStates.Ragdolling)
				{
					return;
				}
				SetIsKinematic(isKinematic: true);
				_ragdollingEndTimestamp = Time.time;
				_animator.enabled = true;
				CurrentState = RagdollStates.Blending;
				foreach (RagdollBodyPart bodypart in _bodyparts)
				{
					bodypart.StoredRotation = bodypart.BodyPartTransform.rotation;
					bodypart.StoredPosition = bodypart.BodyPartTransform.position;
				}
				_ragdolledFeetPosition = 0.5f * (_animator.GetBoneTransform(HumanBodyBones.LeftToes).position + _animator.GetBoneTransform(HumanBodyBones.RightToes).position);
				_ragdolledHeadPosition = _animator.GetBoneTransform(HumanBodyBones.Head).position;
				_ragdolledHipPosition = _animator.GetBoneTransform(HumanBodyBones.Hips).position;
				if (_animator.GetBoneTransform(HumanBodyBones.Hips).forward.y > 0f)
				{
					MMAnimatorExtensions.UpdateAnimatorBool(_animator, _getUpFromBackAnimationParameter, value: true, _animatorParameters);
				}
				else
				{
					MMAnimatorExtensions.UpdateAnimatorBool(_animator, _getUpFromBellyAnimationParameter, value: true, _animatorParameters);
				}
			}
		}
	}

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_rigidbodies = GetComponentsInChildren(typeof(Rigidbody));
		_rigidbodiesTempList = new List<Component>();
		Component[] rigidbodies = _rigidbodies;
		foreach (Component component in rigidbodies)
		{
			if (component.gameObject.MMGetComponentNoAlloc<MMRagdollerIgnore>() == null)
			{
				_rigidbodiesTempList.Add(component);
			}
		}
		_rigidbodies = null;
		_rigidbodies = _rigidbodiesTempList.ToArray();
		if (CurrentState == RagdollStates.Animated)
		{
			SetIsKinematic(isKinematic: true);
		}
		else
		{
			SetIsKinematic(isKinematic: false);
		}
		rigidbodies = GetComponentsInChildren(typeof(Transform));
		foreach (Component component2 in rigidbodies)
		{
			if (component2.transform != base.transform)
			{
				RagdollBodyPart item = new RagdollBodyPart
				{
					BodyPartTransform = (component2 as Transform)
				};
				_bodyparts.Add(item);
			}
		}
		_animator = base.gameObject.GetComponent<Animator>();
		RegisterAnimatorParameters();
	}

	protected virtual void RegisterAnimatorParameters()
	{
		_animatorParameters = new HashSet<int>();
		_getUpFromBackAnimationParameter = Animator.StringToHash("GetUpFromBack");
		_getUpFromBellyAnimationParameter = Animator.StringToHash("GetUpFromBelly");
		if (!(_animator == null))
		{
			if (_animator.MMHasParameterOfType("GetUpFromBack", AnimatorControllerParameterType.Bool))
			{
				_animatorParameters.Add(_getUpFromBackAnimationParameter);
			}
			if (_animator.MMHasParameterOfType("GetUpFromBelly", AnimatorControllerParameterType.Bool))
			{
				_animatorParameters.Add(_getUpFromBellyAnimationParameter);
			}
		}
	}

	protected virtual void SetIsKinematic(bool isKinematic)
	{
		Component[] rigidbodies = _rigidbodies;
		foreach (Component component in rigidbodies)
		{
			if (component.transform != base.transform)
			{
				(component as Rigidbody).detectCollisions = !isKinematic;
				(component as Rigidbody).isKinematic = isKinematic;
			}
		}
	}

	public virtual void ForceRigidbodiesToSleep()
	{
		Component[] rigidbodies = _rigidbodies;
		foreach (Component component in rigidbodies)
		{
			if (component.transform != base.transform)
			{
				(component as Rigidbody).Sleep();
			}
		}
	}

	protected virtual void LateUpdate()
	{
		if (CurrentState == RagdollStates.Animated && ForceSleep)
		{
			ForceRigidbodiesToSleep();
		}
		HandleBlending();
	}

	protected virtual void HandleBlending()
	{
		if (CurrentState != RagdollStates.Blending)
		{
			return;
		}
		if (Time.time <= _ragdollingEndTimestamp + _mecanimToGetUpTransitionTime)
		{
			Vector3 vector = _ragdolledHipPosition - _animator.GetBoneTransform(HumanBodyBones.Hips).position;
			Vector3 vector2 = base.transform.position + vector;
			RaycastHit[] array = Physics.RaycastAll(new Ray(vector2, Vector3.down));
			vector2.y = 0f;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (!raycastHit.transform.IsChildOf(base.transform))
				{
					vector2.y = Mathf.Max(vector2.y, raycastHit.point.y);
				}
			}
			base.transform.position = vector2;
			Vector3 vector3 = _ragdolledHeadPosition - _ragdolledFeetPosition;
			vector3.y = 0f;
			Vector3 vector4 = 0.5f * (_animator.GetBoneTransform(HumanBodyBones.LeftFoot).position + _animator.GetBoneTransform(HumanBodyBones.RightFoot).position);
			Vector3 vector5 = _animator.GetBoneTransform(HumanBodyBones.Head).position - vector4;
			vector5.y = 0f;
			base.transform.rotation *= Quaternion.FromToRotation(vector5.normalized, vector3.normalized);
		}
		float value = 1f - (Time.time - _ragdollingEndTimestamp - _mecanimToGetUpTransitionTime) / RagdollToMecanimBlendDuration;
		value = Mathf.Clamp01(value);
		foreach (RagdollBodyPart bodypart in _bodyparts)
		{
			if (bodypart.BodyPartTransform != base.transform)
			{
				if (bodypart.BodyPartTransform == _animator.GetBoneTransform(HumanBodyBones.Hips))
				{
					bodypart.BodyPartTransform.position = Vector3.Lerp(bodypart.BodyPartTransform.position, bodypart.StoredPosition, value);
				}
				bodypart.BodyPartTransform.rotation = Quaternion.Slerp(bodypart.BodyPartTransform.rotation, bodypart.StoredRotation, value);
			}
		}
		if (value == 0f)
		{
			CurrentState = RagdollStates.Animated;
		}
	}
}
