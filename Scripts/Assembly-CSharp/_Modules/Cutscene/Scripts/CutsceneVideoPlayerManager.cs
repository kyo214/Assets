using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace _Modules.Cutscene.Scripts;

public class CutsceneVideoPlayerManager : MonoBehaviour
{
	[SerializeField]
	private CutsceneManager _cutsceneManager;

	[SerializeField]
	private VideoPlayer _videoPlayer;

	[SerializeField]
	private Image _cutsceneImage;

	private Action onCompleteVideoEvent;

	private bool _isPlaying;

	public void Play(VideoClip videoClip, bool skippable, Action onCompleteVideo)
	{
		_videoPlayer.source = VideoSource.VideoClip;
		_videoPlayer.clip = videoClip;
		StartCoroutine(PlayVideoPlayer(skippable, onCompleteVideo));
	}

	public void Play(string path, bool skippable, Action onCompleteVideo)
	{
		_videoPlayer.source = VideoSource.Url;
		_videoPlayer.url = path;
		StartCoroutine(PlayVideoPlayer(skippable, onCompleteVideo));
	}

	private IEnumerator PlayVideoPlayer(bool skippable, Action onCompleteVideo)
	{
		_videoPlayer.enabled = true;
		_cutsceneImage.gameObject.SetActive(value: true);
		onCompleteVideoEvent = onCompleteVideo;
		_videoPlayer.Prepare();
		while (!_videoPlayer.isPrepared && !_videoPlayer.isPrepared)
		{
			yield return null;
		}
		_videoPlayer.Play();
		_isPlaying = true;
		yield return WaitUntilEndTimeOrSkipped(Time.unscaledTime + (float)_videoPlayer.length, skippable);
		StopVideoPlayer();
	}

	public void StopVideoPlayer()
	{
		onCompleteVideoEvent?.Invoke();
		_videoPlayer.Stop();
		_cutsceneImage.gameObject.SetActive(value: false);
		Reset();
		Debug.Log("stop");
	}

	private void Reset()
	{
		onCompleteVideoEvent = null;
		_videoPlayer.enabled = false;
		_videoPlayer.url = "";
		_videoPlayer.clip = null;
		_isPlaying = false;
	}

	private IEnumerator WaitUntilEndTimeOrSkipped(float endTime, bool skippable)
	{
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
					break;
				}
			}
			else if (_cutsceneManager.GetSkipInput())
			{
				isPressed = true;
				_cutsceneManager.SetSkipCutsceneNetwork(setActive: true);
			}
		}
	}
}
