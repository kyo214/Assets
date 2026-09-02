using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you change the material of the target renderer everytime it's played.")]
[FeedbackPath("Renderer/Material")]
public class MMFeedbackMaterial : MMFeedback
{
	public enum Methods
	{
		Sequential = 0,
		Random = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Material")]
	[Tooltip("the renderer to change material on")]
	public Renderer TargetRenderer;

	[FormerlySerializedAs("MaterialIndexes")]
	[Tooltip("the list of material indexes we want to change on the target renderer. If left empty, will only target the material at index 0")]
	public int[] RendererMaterialIndexes;

	[Header("Material Change")]
	[Tooltip("the selected method")]
	public Methods Method;

	[MMFEnumCondition("Method", new int[] { 0 })]
	[Tooltip("whether or not the sequential order should loop")]
	public bool Loop = true;

	[MMFEnumCondition("Method", new int[] { 1 })]
	[Tooltip("whether or not to always pick a new material in random mode")]
	public bool AlwaysNewMaterial = true;

	[Tooltip("the initial index to start with")]
	public int InitialIndex;

	[Tooltip("the list of materials to pick from")]
	public List<Material> Materials;

	[Header("Interpolation")]
	public bool InterpolateTransition;

	public float TransitionDuration = 1f;

	public AnimationCurve TransitionCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	protected int _currentIndex;

	protected float _startedAt;

	protected Coroutine[] _coroutines;

	protected Material[] _tempMaterials;

	public override float FeedbackDuration
	{
		get
		{
			if (!InterpolateTransition)
			{
				return 0f;
			}
			return TransitionDuration;
		}
		set
		{
			if (InterpolateTransition)
			{
				TransitionDuration = value;
			}
		}
	}

	public virtual float GetTime()
	{
		if (Timing.TimescaleMode != TimescaleModes.Scaled)
		{
			return Time.unscaledTime;
		}
		return Time.time;
	}

	public virtual float GetDeltaTime()
	{
		if (Timing.TimescaleMode != TimescaleModes.Scaled)
		{
			return Time.unscaledDeltaTime;
		}
		return Time.deltaTime;
	}

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		_currentIndex = InitialIndex;
		_tempMaterials = new Material[TargetRenderer.materials.Length];
		if (RendererMaterialIndexes == null)
		{
			RendererMaterialIndexes = new int[1];
		}
		if (RendererMaterialIndexes.Length == 0)
		{
			RendererMaterialIndexes = new int[1];
			RendererMaterialIndexes[0] = 0;
		}
		_coroutines = new Coroutine[RendererMaterialIndexes.Length];
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		if (Materials.Count == 0)
		{
			Debug.LogError("[MMFeedbackMaterial on " + base.name + "] The Materials array is empty.");
			return;
		}
		int index = DetermineNextIndex();
		if (Materials[index] == null)
		{
			Debug.LogError("[MMFeedbackMaterial on " + base.name + "] Attempting to switch to a null material.");
		}
		else if (InterpolateTransition)
		{
			for (int i = 0; i < RendererMaterialIndexes.Length; i++)
			{
				_coroutines[i] = StartCoroutine(TransitionMaterial(TargetRenderer.materials[RendererMaterialIndexes[i]], Materials[index], RendererMaterialIndexes[i]));
			}
		}
		else
		{
			ApplyMaterial(Materials[index]);
		}
	}

	protected virtual void ApplyMaterial(Material material)
	{
		_tempMaterials = TargetRenderer.materials;
		for (int i = 0; i < RendererMaterialIndexes.Length; i++)
		{
			_tempMaterials[RendererMaterialIndexes[i]] = material;
		}
		TargetRenderer.materials = _tempMaterials;
	}

	protected virtual void LerpMaterial(Material fromMaterial, Material toMaterial, float t, int materialIndex)
	{
		_tempMaterials = TargetRenderer.materials;
		for (int i = 0; i < RendererMaterialIndexes.Length; i++)
		{
			_tempMaterials[materialIndex].Lerp(fromMaterial, toMaterial, t);
		}
		TargetRenderer.materials = _tempMaterials;
	}

	protected virtual IEnumerator TransitionMaterial(Material originalMaterial, Material newMaterial, int materialIndex)
	{
		IsPlaying = true;
		_startedAt = GetTime();
		while (GetTime() - _startedAt < TransitionDuration)
		{
			float time = MMFeedbacksHelpers.Remap(GetTime() - _startedAt, 0f, TransitionDuration, 0f, 1f);
			float t = TransitionCurve.Evaluate(time);
			LerpMaterial(originalMaterial, newMaterial, t, materialIndex);
			yield return null;
		}
		float t2 = TransitionCurve.Evaluate(1f);
		LerpMaterial(originalMaterial, newMaterial, t2, materialIndex);
		IsPlaying = false;
	}

	protected virtual int DetermineNextIndex()
	{
		switch (Method)
		{
		case Methods.Random:
		{
			int num = Random.Range(0, Materials.Count);
			if (AlwaysNewMaterial)
			{
				while (_currentIndex == num)
				{
					num = Random.Range(0, Materials.Count);
				}
			}
			_currentIndex = num;
			return _currentIndex;
		}
		case Methods.Sequential:
			_currentIndex++;
			if (_currentIndex >= Materials.Count)
			{
				_currentIndex = ((!Loop) ? _currentIndex : 0);
			}
			return _currentIndex;
		default:
			return 0;
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		base.CustomStopFeedback(position, feedbacksIntensity);
		if (!Active || !FeedbackTypeAuthorized || _coroutines == null)
		{
			return;
		}
		IsPlaying = false;
		for (int i = 0; i < RendererMaterialIndexes.Length; i++)
		{
			if (_coroutines[i] != null)
			{
				StopCoroutine(_coroutines[i]);
			}
			_coroutines[i] = null;
		}
	}
}
