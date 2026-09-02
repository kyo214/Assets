using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class AnimationJukebox : MonoBehaviour
{
	public Animator targetAnimator;

	[HideInInspector]
	public List<AnimationClip> clips = new List<AnimationClip>();

	[Header("Playback Settings")]
	[Range(0f, 2f)]
	public float playbackSpeed = 1f;

	public bool loop = true;

	private PlayableGraph _graph;

	private AnimationClipPlayable _clipPlayable;

	private void OnDisable()
	{
		if (_graph.IsValid())
		{
			_graph.Destroy();
		}
	}

	private void Update()
	{
		if (!_graph.IsValid() || !_clipPlayable.IsValid())
		{
			return;
		}
		_clipPlayable.SetSpeed(playbackSpeed);
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			_clipPlayable.SetTime(0.0);
			if (!_graph.IsPlaying())
			{
				_graph.Play();
			}
		}
		double num = _clipPlayable.GetAnimationClip().length;
		if (_clipPlayable.GetTime() >= num)
		{
			if (loop)
			{
				_clipPlayable.SetTime(0.0);
				_graph.Play();
			}
			else
			{
				_clipPlayable.SetTime(num);
				_graph.Stop();
			}
		}
	}

	public void PlayClip(AnimationClip clip)
	{
		if (!(targetAnimator == null) && !(clip == null))
		{
			if (_graph.IsValid())
			{
				_graph.Destroy();
			}
			_graph = PlayableGraph.Create("JukeboxGraph");
			_graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
			AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "Animation", targetAnimator);
			_clipPlayable = AnimationClipPlayable.Create(_graph, clip);
			_clipPlayable.SetDuration(clip.length);
			if (!loop)
			{
				_clipPlayable.SetDuration(clip.length);
			}
			output.SetSourcePlayable(_clipPlayable);
			_graph.Play();
			Debug.Log("Playing: " + clip.name);
		}
	}

	public void Stop()
	{
		if (_graph.IsValid())
		{
			_graph.Destroy();
		}
	}
}
