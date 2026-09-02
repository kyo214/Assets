using System;
using System.Reflection;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMAnimationCurveGenerator : MonoBehaviour
{
	[Header("Save settings")]
	public string AnimationCurveFilePath = "Assets/MMTools/MMTween/Editor/";

	public string AnimationCurveFileName = "MMCurves.curves";

	[Header("Animation Curves")]
	public int Resolution = 50;

	public bool GenerateAntiCurves;

	[MMInspectorButton("GenerateAnimationCurvesAsset")]
	public bool GenerateAnimationCurvesButton;

	protected Type _scriptableObjectType;

	protected Keyframe _keyframe;

	protected MethodInfo _addMethodInfo;

	protected object[] _parameters;

	public virtual void GenerateAnimationCurvesAsset()
	{
		_scriptableObjectType = Type.GetType("UnityEditor.CurvePresetLibrary, UnityEditor");
		_addMethodInfo = _scriptableObjectType.GetMethod("Add");
		ScriptableObject asset = ScriptableObject.CreateInstance(_scriptableObjectType);
		foreach (MMTween.MMTweenCurve value in Enum.GetValues(typeof(MMTween.MMTweenCurve)))
		{
			CreateAnimationCurve(asset, value, Resolution, GenerateAntiCurves);
		}
	}

	protected virtual void CreateAnimationCurve(ScriptableObject asset, MMTween.MMTweenCurve curveType, int curveResolution, bool anti)
	{
		AnimationCurve animationCurve = new AnimationCurve();
		for (int i = 0; i < curveResolution; i++)
		{
			_keyframe.time = (float)i / ((float)curveResolution - 1f);
			if (anti)
			{
				_keyframe.value = MMTween.Tween(_keyframe.time, 0f, 1f, 1f, 0f, curveType);
			}
			else
			{
				_keyframe.value = MMTween.Tween(_keyframe.time, 0f, 1f, 0f, 1f, curveType);
			}
			animationCurve.AddKey(_keyframe);
		}
		for (int j = 0; j < curveResolution; j++)
		{
			animationCurve.SmoothTangents(j, 0f);
		}
		_parameters = new object[2]
		{
			animationCurve,
			curveType.ToString()
		};
		_addMethodInfo.Invoke(asset, _parameters);
	}
}
