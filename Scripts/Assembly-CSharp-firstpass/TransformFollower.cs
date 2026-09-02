using System.Collections.Generic;
using DarkTonic.MasterAudio;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
	[Tooltip("This is for diagnostic purposes only. Do not change or assign this field.")]
	public Transform RuntimeFollowingTransform;

	private GameObject _goToFollow;

	private Transform _trans;

	private GameObject _go;

	private SphereCollider _collider;

	private string _soundType;

	private string _variationName;

	private bool _willFollowSource;

	private bool _isInsideTrigger;

	private bool _hasPlayedSound;

	private float _playVolume;

	private bool _positionAtClosestColliderPoint;

	private MasterAudio.AmbientSoundExitMode _exitMode;

	private float _exitFadeTime;

	private MasterAudio.AmbientSoundReEnterMode _reEnterMode;

	private float _reEnterFadeTime;

	private readonly List<Collider> _actorColliders = new List<Collider>();

	private Vector3 _lastListenerPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);

	private readonly Dictionary<Collider, Vector3> _lastPositionByCollider = new Dictionary<Collider, Vector3>();

	private PlaySoundResult playingVariation;

	private PlaySoundResult fadingVariation;

	public SphereCollider Trigger
	{
		get
		{
			if (_collider != null)
			{
				return _collider;
			}
			_collider = GameObj.AddComponent<SphereCollider>();
			_collider.isTrigger = true;
			return _collider;
		}
	}

	public GameObject GameObj
	{
		get
		{
			if (_go != null)
			{
				return _go;
			}
			_go = base.gameObject;
			return _go;
		}
	}

	public Transform Trans
	{
		get
		{
			if (_trans == null)
			{
				_trans = base.transform;
			}
			return _trans;
		}
	}

	private void Awake()
	{
		if (!(Trigger == null))
		{
			_ = _actorColliders.Count;
		}
		if (!(_lastListenerPos == Vector3.zero) && playingVariation == null)
		{
			_ = _positionAtClosestColliderPoint;
		}
	}

	private void OnDisable()
	{
		AmbientUtil.RemoveTransformFollower(this);
		PerformTriggerExit();
	}

	public void UpdateAudioVariation(SoundGroupVariation newVariation)
	{
		playingVariation.ActingVariation = newVariation;
	}

	public void StartFollowing(Transform transToFollow, string soundType, string variationName, float volume, float trigRadius, bool willFollowSource, bool positionAtClosestColliderPoint, bool useTopCollider, bool useChildColliders, MasterAudio.AmbientSoundExitMode exitMode, float exitFadeTime, MasterAudio.AmbientSoundReEnterMode reEnterMode, float reEnterFadeTime)
	{
		RuntimeFollowingTransform = transToFollow;
		_goToFollow = transToFollow.gameObject;
		Trigger.radius = trigRadius;
		_soundType = soundType;
		_variationName = variationName;
		_playVolume = volume;
		_willFollowSource = willFollowSource;
		_exitMode = exitMode;
		_exitFadeTime = exitFadeTime;
		_reEnterMode = reEnterMode;
		_reEnterFadeTime = reEnterFadeTime;
		_lastPositionByCollider.Clear();
		if (useTopCollider)
		{
			Collider component = transToFollow.GetComponent<Collider>();
			if (component != null)
			{
				_actorColliders.Add(component);
				_lastPositionByCollider.Add(component, transToFollow.position);
			}
		}
		if (useChildColliders && transToFollow != null)
		{
			for (int i = 0; i < transToFollow.childCount; i++)
			{
				Collider component2 = transToFollow.GetChild(i).GetComponent<Collider>();
				if (component2 != null)
				{
					_actorColliders.Add(component2);
					_lastPositionByCollider.Add(component2, transToFollow.position);
				}
			}
		}
		_lastListenerPos = MasterAudio.ListenerTrans.position;
		int num = 0;
		if ((_actorColliders.Count == 0 && num == 0) & positionAtClosestColliderPoint)
		{
			Debug.Log("Can't follow collider of '" + transToFollow.name + "' because it doesn't have any colliders.");
			return;
		}
		_positionAtClosestColliderPoint = positionAtClosestColliderPoint;
		if (_positionAtClosestColliderPoint)
		{
			RecalcClosestColliderPosition(forceRecalc: true);
			MasterAudio.QueueTransformFollowerForColliderPositionRecalc(this);
		}
	}

	private void StopFollowing()
	{
		RuntimeFollowingTransform = null;
		Object.Destroy(GameObj);
	}

	private void PlaySound()
	{
		bool flag = !string.IsNullOrEmpty(_variationName);
		bool flag2 = _positionAtClosestColliderPoint || _exitMode == MasterAudio.AmbientSoundExitMode.FadeSound;
		bool flag3 = _willFollowSource && !_positionAtClosestColliderPoint;
		if (fadingVariation != null && fadingVariation.ActingVariation != null)
		{
			MasterAudio.AmbientSoundReEnterMode ambientSoundReEnterMode = _reEnterMode;
			if (!fadingVariation.ActingVariation.IsPlaying)
			{
				ambientSoundReEnterMode = MasterAudio.AmbientSoundReEnterMode.StopExistingSound;
			}
			switch (ambientSoundReEnterMode)
			{
			case MasterAudio.AmbientSoundReEnterMode.FadeInSameSound:
				fadingVariation.ActingVariation.FadeToVolume(_playVolume, _reEnterFadeTime);
				playingVariation = fadingVariation;
				fadingVariation = null;
				_hasPlayedSound = true;
				return;
			case MasterAudio.AmbientSoundReEnterMode.StopExistingSound:
				fadingVariation.ActingVariation.Stop();
				break;
			}
		}
		if (flag3)
		{
			if (flag2)
			{
				if (flag)
				{
					playingVariation = MasterAudio.PlaySound3DFollowTransform(_soundType, RuntimeFollowingTransform, _playVolume, 1f, 0f, _variationName);
				}
				else
				{
					playingVariation = MasterAudio.PlaySound3DFollowTransform(_soundType, RuntimeFollowingTransform, _playVolume);
				}
			}
			else if (flag)
			{
				MasterAudio.PlaySound3DFollowTransformAndForget(_soundType, RuntimeFollowingTransform, _playVolume, 1f, 0f, _variationName);
			}
			else
			{
				MasterAudio.PlaySound3DFollowTransformAndForget(_soundType, RuntimeFollowingTransform, _playVolume);
			}
		}
		else if (flag2)
		{
			if (flag)
			{
				playingVariation = MasterAudio.PlaySound3DAtTransform(_soundType, RuntimeFollowingTransform, _playVolume, 1f, 0f, _variationName);
			}
			else
			{
				playingVariation = MasterAudio.PlaySound3DAtTransform(_soundType, RuntimeFollowingTransform, _playVolume);
			}
		}
		else if (flag)
		{
			MasterAudio.PlaySound3DAtTransformAndForget(_soundType, RuntimeFollowingTransform, _playVolume, 1f, 0f, _variationName);
		}
		else
		{
			MasterAudio.PlaySound3DAtTransformAndForget(_soundType, RuntimeFollowingTransform, _playVolume);
		}
		fadingVariation = null;
		_hasPlayedSound = true;
	}

	public void ManualUpdate()
	{
		if (RuntimeFollowingTransform == null || !DTMonoHelper.IsActive(_goToFollow))
		{
			StopFollowing();
			return;
		}
		if (!_positionAtClosestColliderPoint)
		{
			Trans.position = RuntimeFollowingTransform.position;
		}
		if (_isInsideTrigger && !_hasPlayedSound)
		{
			PlaySound();
		}
	}

	public bool RecalcClosestColliderPosition(bool forceRecalc = false)
	{
		Vector3 position = MasterAudio.ListenerTrans.position;
		bool flag = _lastListenerPos != position;
		Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		bool flag2 = false;
		int count = _actorColliders.Count;
		int num = 0;
		if (count > 0)
		{
			float num2 = float.MaxValue;
			if (count == 1)
			{
				Collider collider = _actorColliders[0];
				if (collider == null)
				{
					return false;
				}
				Vector3 position2 = collider.transform.position;
				if (!forceRecalc && _lastPositionByCollider[collider] == position2 && !flag)
				{
					return false;
				}
				flag2 = true;
				vector = collider.ClosestPoint(position);
				_lastPositionByCollider[collider] = position2;
			}
			else
			{
				for (int i = 0; i < _actorColliders.Count; i++)
				{
					Collider collider2 = _actorColliders[i];
					Vector3 position3 = collider2.transform.position;
					if (forceRecalc || !(_lastPositionByCollider[collider2] == position3) || flag)
					{
						flag2 = true;
						Vector3 vector2 = collider2.ClosestPoint(position);
						float sqrMagnitude = (position - vector2).sqrMagnitude;
						if (sqrMagnitude < num2)
						{
							vector = vector2;
							num2 = sqrMagnitude;
						}
						_lastPositionByCollider[collider2] = position3;
					}
				}
			}
		}
		else if (num <= 0)
		{
			return false;
		}
		if (!flag2)
		{
			return false;
		}
		Trans.position = vector;
		Trans.LookAt(MasterAudio.ListenerTrans);
		if (playingVariation != null && playingVariation.ActingVariation != null)
		{
			playingVariation.ActingVariation.MoveToAmbientColliderPosition(vector, this);
		}
		_lastListenerPos = position;
		return true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(RuntimeFollowingTransform == null) && !(other == null) && !(base.name == "~ListenerFollower~") && !(other.name != "~ListenerFollower~"))
		{
			_isInsideTrigger = true;
			PlaySound();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!(RuntimeFollowingTransform == null) && !(other == null) && !(other.name != "~ListenerFollower~"))
		{
			PerformTriggerExit();
		}
	}

	private void PerformTriggerExit()
	{
		_isInsideTrigger = false;
		_hasPlayedSound = false;
		if (!(MasterAudio.GrabGroup(_soundType, logIfMissing: false) == null))
		{
			switch (_exitMode)
			{
			case MasterAudio.AmbientSoundExitMode.StopSound:
				MasterAudio.StopSoundGroupOfTransform(RuntimeFollowingTransform, _soundType);
				break;
			case MasterAudio.AmbientSoundExitMode.FadeSound:
				MasterAudio.FadeOutSoundGroupOfTransform(RuntimeFollowingTransform, _soundType, _exitFadeTime);
				break;
			}
			fadingVariation = playingVariation;
			playingVariation = null;
		}
	}
}
