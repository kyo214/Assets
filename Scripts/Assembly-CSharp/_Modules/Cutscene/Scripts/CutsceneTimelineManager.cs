using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Modules.Cutscene.Scripts;

public class CutsceneTimelineManager : MonoBehaviour
{
	[SerializeField]
	private CutsceneManager _cutsceneManager;

	private static Dictionary<string, CutsceneTimelineController> playableDirectorDictionary = new Dictionary<string, CutsceneTimelineController>();

	private CutsceneTimelineController _currentPlayableDirector;

	public void Play(string key, bool skippable, Action onCompleteAction)
	{
		if (playableDirectorDictionary.ContainsKey(key))
		{
			_currentPlayableDirector = playableDirectorDictionary[key];
			StartCoroutine(DoPlayCutscene(_currentPlayableDirector, skippable, onCompleteAction));
		}
	}

	private IEnumerator DoPlayCutscene(CutsceneTimelineController cutsceneTimelineController, bool skippable, Action onCompleteAction)
	{
		cutsceneTimelineController.Play();
		yield return WaitUntilEndTimeOrSkipped(cutsceneTimelineController, skippable);
		onCompleteAction?.Invoke();
	}

	public static void AddPayableDirectorDictionary(CutsceneTimelineController cutsceneTimelineController)
	{
		playableDirectorDictionary.TryAdd(cutsceneTimelineController.TimeLineId, cutsceneTimelineController);
	}

	public static void RemovePayableDirectorDictionary(CutsceneTimelineController cutsceneTimelineController)
	{
		if (playableDirectorDictionary.ContainsKey(cutsceneTimelineController.TimeLineId))
		{
			playableDirectorDictionary.Remove(cutsceneTimelineController.TimeLineId);
		}
	}

	public void StopCutscene()
	{
		if ((bool)_currentPlayableDirector)
		{
			_currentPlayableDirector.Stop();
		}
	}

	public void SkipCutscene()
	{
		if ((bool)_currentPlayableDirector)
		{
			_currentPlayableDirector.Skip();
		}
	}

	private IEnumerator WaitUntilEndTimeOrSkipped(CutsceneTimelineController cutsceneTimelineController, bool skippable)
	{
		float endTime = Time.unscaledTime + cutsceneTimelineController.PlayableDirectorDuration;
		bool isSkip = false;
		bool isPressed = false;
		while (Time.unscaledTime < endTime)
		{
			yield return null;
			if (!skippable)
			{
				continue;
			}
			if (isPressed)
			{
				if (_cutsceneManager.AllSkip)
				{
					isSkip = true;
					break;
				}
			}
			else if (_cutsceneManager.GetSkipInput())
			{
				isPressed = true;
				_cutsceneManager.SetSkipCutsceneNetwork(setActive: true);
			}
		}
		if (isSkip)
		{
			cutsceneTimelineController.Skip();
		}
	}
}
