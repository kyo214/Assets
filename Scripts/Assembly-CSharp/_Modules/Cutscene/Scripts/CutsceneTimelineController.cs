using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace _Modules.Cutscene.Scripts;

[RequireComponent(typeof(PlayableDirector), typeof(SignalReceiver))]
public class CutsceneTimelineController : MonoBehaviour, ICutscene
{
	[SerializeField]
	protected string _timelineId;

	[SerializeField]
	protected PlayableDirector _playableDirector;

	private CinematicBlackBarController _blackBarController;

	[SerializeField]
	protected bool _cinematicBlackBar = true;

	public UnityEvent OnStartEvent;

	public UnityEvent OnCompleteEvent;

	public string TimeLineId
	{
		get
		{
			if (_timelineId.IsNullOrWhitespace())
			{
				_timelineId = base.gameObject.name;
			}
			return _timelineId;
		}
	}

	public PlayableDirector PlayableDirector => _playableDirector;

	public float PlayableDirectorDuration => (float)_playableDirector.playableAsset.duration;

	protected CinematicBlackBarController BlackBarController
	{
		get
		{
			if (_blackBarController == null)
			{
				_blackBarController = GenericSingleton<CutsceneManager>.Instance.CinematicBlackBarController;
			}
			return _blackBarController;
		}
	}

	private void OnEnable()
	{
		CutsceneTimelineManager.AddPayableDirectorDictionary(this);
	}

	private void OnDisable()
	{
		CutsceneTimelineManager.RemovePayableDirectorDictionary(this);
	}

	public virtual void Play()
	{
		_playableDirector.Play();
	}

	public virtual void Skip()
	{
		_playableDirector.time = GetOnCompleteEventTime();
	}

	public virtual void Stop()
	{
		_playableDirector.Stop();
	}

	public virtual void OnStart()
	{
		if (_cinematicBlackBar)
		{
			BlackBarController.ShowBar(null);
		}
		OnStartEvent?.Invoke();
	}

	public virtual void OnComplete()
	{
		if (_cinematicBlackBar)
		{
			BlackBarController.HideBar(null);
		}
		OnCompleteEvent?.Invoke();
	}

	public double GetOnStartEventTime()
	{
		return GetMarker(0)?.time ?? 0.0;
	}

	public double GetOnCompleteEventTime()
	{
		return GetMarker(1)?.time ?? _playableDirector.playableAsset.duration;
	}

	private IMarker GetMarker(int index)
	{
		return ((TimelineAsset)_playableDirector.playableAsset).markerTrack.GetMarker(index);
	}
}
