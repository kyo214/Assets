using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace _Modules.Cutscene.Scripts;

[CreateAssetMenu(fileName = "CutsceneScriptableObject", menuName = "WMO/Cutscene/Cutscene ScriptableObject", order = 0)]
public class CutsceneScriptableObject : ScriptableObject
{
	public enum VideoSourceType
	{
		VIDEOCLIP = 0,
		FILEPATH = 1
	}

	[SerializeField]
	private CutsceneType _cutsceneType;

	[SerializeField]
	private bool _skippable;

	[SerializeField]
	private string _playableDirectorId;

	[SerializeField]
	private VideoSourceType _videoSourceType;

	[SerializeField]
	private VideoClip _videoClip;

	[SerializeField]
	private string _videoClipPath;

	[SerializeField]
	private List<CustomCutsceneAction> _onBeforeStartActionList = new List<CustomCutsceneAction>();

	[SerializeField]
	private List<CustomCutsceneAction> _onCompletedActionList = new List<CustomCutsceneAction>();

	public CutsceneType CutsceneEnumType
	{
		get
		{
			return _cutsceneType;
		}
		set
		{
			_cutsceneType = value;
		}
	}

	public bool Skippable => _skippable;

	public string PlayableDirectorId
	{
		get
		{
			return _playableDirectorId;
		}
		set
		{
			_playableDirectorId = value;
		}
	}

	public VideoSourceType VideoSourceEnumType
	{
		get
		{
			return _videoSourceType;
		}
		set
		{
			_videoSourceType = value;
		}
	}

	public VideoClip VideoClip
	{
		get
		{
			return _videoClip;
		}
		set
		{
			_videoClip = value;
		}
	}

	public string VideoClipPath
	{
		get
		{
			return _videoClipPath;
		}
		set
		{
			_videoClipPath = value;
		}
	}

	public void InvokeOnBeforeStartAction()
	{
		foreach (CustomCutsceneAction onBeforeStartAction in _onBeforeStartActionList)
		{
			onBeforeStartAction?.Invoke();
		}
	}

	public void InvokeOnCompletedAction()
	{
		foreach (CustomCutsceneAction onCompletedAction in _onCompletedActionList)
		{
			onCompletedAction?.Invoke();
		}
	}
}
