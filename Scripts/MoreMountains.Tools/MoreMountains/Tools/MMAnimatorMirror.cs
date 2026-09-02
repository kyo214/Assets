using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMAnimatorMirror : MonoBehaviour
{
	public struct MMAnimatorMirrorBind
	{
		public int ParameterHash;

		public AnimatorControllerParameterType ParameterType;
	}

	[Header("Bindings")]
	public Animator SourceAnimator;

	public Animator TargetAnimator;

	protected AnimatorControllerParameter[] _sourceParameters;

	protected AnimatorControllerParameter[] _targetParameters;

	protected List<MMAnimatorMirrorBind> _updateParameters;

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		if (TargetAnimator == null)
		{
			TargetAnimator = base.gameObject.GetComponent<Animator>();
		}
		if (TargetAnimator == null || SourceAnimator == null)
		{
			return;
		}
		int parameterCount = SourceAnimator.parameterCount;
		_sourceParameters = new AnimatorControllerParameter[parameterCount];
		for (int i = 0; i < parameterCount; i++)
		{
			_sourceParameters[i] = SourceAnimator.GetParameter(i);
		}
		parameterCount = TargetAnimator.parameterCount;
		_targetParameters = new AnimatorControllerParameter[parameterCount];
		for (int j = 0; j < parameterCount; j++)
		{
			_targetParameters[j] = TargetAnimator.GetParameter(j);
		}
		_updateParameters = new List<MMAnimatorMirrorBind>();
		AnimatorControllerParameter[] sourceParameters = _sourceParameters;
		foreach (AnimatorControllerParameter animatorControllerParameter in sourceParameters)
		{
			AnimatorControllerParameter[] targetParameters = _targetParameters;
			foreach (AnimatorControllerParameter animatorControllerParameter2 in targetParameters)
			{
				if (animatorControllerParameter.name == animatorControllerParameter2.name)
				{
					MMAnimatorMirrorBind item = new MMAnimatorMirrorBind
					{
						ParameterHash = animatorControllerParameter.nameHash,
						ParameterType = animatorControllerParameter.type
					};
					_updateParameters.Add(item);
				}
			}
		}
	}

	protected virtual void Update()
	{
		Mirror();
	}

	protected virtual void Mirror()
	{
		if (TargetAnimator == null || SourceAnimator == null)
		{
			return;
		}
		foreach (MMAnimatorMirrorBind updateParameter in _updateParameters)
		{
			switch (updateParameter.ParameterType)
			{
			case AnimatorControllerParameterType.Bool:
				TargetAnimator.SetBool(updateParameter.ParameterHash, SourceAnimator.GetBool(updateParameter.ParameterHash));
				break;
			case AnimatorControllerParameterType.Float:
				TargetAnimator.SetFloat(updateParameter.ParameterHash, SourceAnimator.GetFloat(updateParameter.ParameterHash));
				break;
			case AnimatorControllerParameterType.Int:
				TargetAnimator.SetInteger(updateParameter.ParameterHash, SourceAnimator.GetInteger(updateParameter.ParameterHash));
				break;
			case AnimatorControllerParameterType.Trigger:
				if (SourceAnimator.GetBool(updateParameter.ParameterHash))
				{
					TargetAnimator.SetTrigger(updateParameter.ParameterHash);
				}
				break;
			}
		}
	}
}
