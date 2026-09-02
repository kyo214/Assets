using UnityEngine;
using UnityEngine.Playables;

namespace Timeline.CustomTrack.DeactivateTrack;

public class DeactivationMixerPlayable : PlayableBehaviour
{
	private DeactivationTrack.PostPlaybackState m_PostPlaybackState;

	private bool m_BoundGameObjectInitialStateIsActive;

	private GameObject m_BoundGameObject;

	public DeactivationTrack.PostPlaybackState postPlaybackState
	{
		get
		{
			return m_PostPlaybackState;
		}
		set
		{
			m_PostPlaybackState = value;
		}
	}

	public static ScriptPlayable<DeactivationMixerPlayable> Create(PlayableGraph graph, int inputCount)
	{
		return ScriptPlayable<DeactivationMixerPlayable>.Create(graph, inputCount);
	}

	public override void OnPlayableDestroy(Playable playable)
	{
		if (!(m_BoundGameObject == null))
		{
			switch (m_PostPlaybackState)
			{
			case DeactivationTrack.PostPlaybackState.Active:
				m_BoundGameObject.SetActive(value: true);
				break;
			case DeactivationTrack.PostPlaybackState.Inactive:
				m_BoundGameObject.SetActive(value: false);
				break;
			case DeactivationTrack.PostPlaybackState.Revert:
				m_BoundGameObject.SetActive(m_BoundGameObjectInitialStateIsActive);
				break;
			case DeactivationTrack.PostPlaybackState.LeaveAsIs:
				break;
			}
		}
	}

	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if (m_BoundGameObject == null)
		{
			m_BoundGameObject = playerData as GameObject;
			m_BoundGameObjectInitialStateIsActive = m_BoundGameObject != null && m_BoundGameObject.activeSelf;
		}
		if (m_BoundGameObject == null)
		{
			return;
		}
		int inputCount = playable.GetInputCount();
		bool active = true;
		for (int i = 0; i < inputCount; i++)
		{
			if (playable.GetInputWeight(i) > 0f)
			{
				active = false;
				break;
			}
		}
		m_BoundGameObject.SetActive(active);
	}
}
