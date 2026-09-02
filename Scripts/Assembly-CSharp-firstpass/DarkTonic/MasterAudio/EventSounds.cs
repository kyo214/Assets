using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio.Multiplayer;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkTonic.MasterAudio;

[AudioScriptOrder(-30)]
[AddComponentMenu("Dark Tonic/Master Audio/Event Sounds")]
public class EventSounds : MonoBehaviour, ICustomEventReceiver
{
	public enum UnityUIVersion
	{
		Legacy = 0,
		uGUI = 1
	}

	public enum EventType
	{
		OnStart = 0,
		OnVisible = 1,
		OnInvisible = 2,
		OnCollision = 3,
		OnTriggerEnter = 4,
		OnTriggerExit = 5,
		OnMouseEnter = 6,
		OnMouseClick = 7,
		OnSpawned = 8,
		OnDespawned = 9,
		OnEnable = 10,
		OnDisable = 11,
		OnCollision2D = 12,
		OnTriggerEnter2D = 13,
		OnTriggerExit2D = 14,
		OnParticleCollision = 15,
		UserDefinedEvent = 16,
		OnCollisionExit = 17,
		OnCollisionExit2D = 18,
		OnMouseUp = 19,
		OnMouseExit = 20,
		OnMouseDrag = 21,
		NGUIOnClick = 22,
		NGUIMouseDown = 23,
		NGUIMouseUp = 24,
		NGUIMouseEnter = 25,
		NGUIMouseExit = 26,
		MechanimStateChanged = 27,
		UnitySliderChanged = 28,
		UnityButtonClicked = 29,
		UnityPointerDown = 30,
		UnityPointerUp = 31,
		UnityPointerEnter = 32,
		UnityPointerExit = 33,
		UnityDrag = 34,
		UnityDrop = 35,
		UnityScroll = 36,
		UnityUpdateSelected = 37,
		UnitySelect = 38,
		UnityDeselect = 39,
		UnityMove = 40,
		UnityInitializePotentialDrag = 41,
		UnityBeginDrag = 42,
		UnityEndDrag = 43,
		UnitySubmit = 44,
		UnityCancel = 45,
		UnityToggle = 46,
		OnTriggerStay = 47,
		OnTriggerStay2D = 48,
		CodeTriggeredEvent1 = 49,
		CodeTriggeredEvent2 = 50
	}

	public enum GlidePitchType
	{
		None = 0,
		RaisePitch = 1,
		LowerPitch = 2
	}

	public enum VariationType
	{
		PlaySpecific = 0,
		PlayRandom = 1
	}

	public enum PreviousSoundStopMode
	{
		None = 0,
		Stop = 1,
		FadeOut = 2
	}

	public enum RetriggerLimMode
	{
		None = 0,
		FrameBased = 1,
		TimeBased = 2
	}

	public MasterAudio.SoundSpawnLocationMode soundSpawnMode = MasterAudio.SoundSpawnLocationMode.AttachToCaller;

	public bool disableSounds;

	public bool showPoolManager;

	public bool showNGUI;

	public bool multiplayerBroadcast;

	public AudioEvent eventToGizmo;

	public UnityUIVersion unityUIMode = UnityUIVersion.uGUI;

	public bool logMissingEvents = true;

	public static readonly List<EventType> DisallowedMultBroadcastEventType = new List<EventType>
	{
		EventType.OnSpawned,
		EventType.OnDespawned
	};

	public static List<MasterAudio.EventSoundFunctionType> CommandTypesExcludedFromMultiplayerBroadcast = new List<MasterAudio.EventSoundFunctionType> { MasterAudio.EventSoundFunctionType.PersistentSettingsControl };

	public static List<string> LayerTagFilterEvents = new List<string>
	{
		EventType.OnCollision.ToString(),
		EventType.OnTriggerEnter.ToString(),
		EventType.OnTriggerExit.ToString(),
		EventType.OnCollision2D.ToString(),
		EventType.OnTriggerEnter2D.ToString(),
		EventType.OnTriggerExit2D.ToString(),
		EventType.OnParticleCollision.ToString(),
		EventType.OnCollisionExit.ToString(),
		EventType.OnCollisionExit2D.ToString()
	};

	public static List<MasterAudio.PlaylistCommand> PlaylistCommandsWithAll = new List<MasterAudio.PlaylistCommand>
	{
		MasterAudio.PlaylistCommand.FadeToVolume,
		MasterAudio.PlaylistCommand.Pause,
		MasterAudio.PlaylistCommand.PlayNextSong,
		MasterAudio.PlaylistCommand.PlayRandomSong,
		MasterAudio.PlaylistCommand.Resume,
		MasterAudio.PlaylistCommand.Stop,
		MasterAudio.PlaylistCommand.Mute,
		MasterAudio.PlaylistCommand.Unmute,
		MasterAudio.PlaylistCommand.ToggleMute,
		MasterAudio.PlaylistCommand.Restart,
		MasterAudio.PlaylistCommand.StopLoopingCurrentSong,
		MasterAudio.PlaylistCommand.StopPlaylistAfterCurrentSong
	};

	public AudioEventGroup startSound;

	public AudioEventGroup visibleSound;

	public AudioEventGroup invisibleSound;

	public AudioEventGroup collisionSound;

	public AudioEventGroup collisionExitSound;

	public AudioEventGroup triggerSound;

	public AudioEventGroup triggerExitSound;

	public AudioEventGroup triggerStaySound;

	public AudioEventGroup mouseEnterSound;

	public AudioEventGroup mouseExitSound;

	public AudioEventGroup mouseClickSound;

	public AudioEventGroup mouseUpSound;

	public AudioEventGroup mouseDragSound;

	public AudioEventGroup spawnedSound;

	public AudioEventGroup despawnedSound;

	public AudioEventGroup enableSound;

	public AudioEventGroup disableSound;

	public AudioEventGroup collision2dSound;

	public AudioEventGroup collisionExit2dSound;

	public AudioEventGroup triggerEnter2dSound;

	public AudioEventGroup triggerStay2dSound;

	public AudioEventGroup triggerExit2dSound;

	public AudioEventGroup particleCollisionSound;

	public AudioEventGroup nguiOnClickSound;

	public AudioEventGroup nguiMouseDownSound;

	public AudioEventGroup nguiMouseUpSound;

	public AudioEventGroup nguiMouseEnterSound;

	public AudioEventGroup nguiMouseExitSound;

	public AudioEventGroup codeTriggeredEvent1Sound;

	public AudioEventGroup codeTriggeredEvent2Sound;

	public AudioEventGroup unitySliderChangedSound;

	public AudioEventGroup unityButtonClickedSound;

	public AudioEventGroup unityPointerDownSound;

	public AudioEventGroup unityDragSound;

	public AudioEventGroup unityPointerUpSound;

	public AudioEventGroup unityPointerEnterSound;

	public AudioEventGroup unityPointerExitSound;

	public AudioEventGroup unityDropSound;

	public AudioEventGroup unityScrollSound;

	public AudioEventGroup unityUpdateSelectedSound;

	public AudioEventGroup unitySelectSound;

	public AudioEventGroup unityDeselectSound;

	public AudioEventGroup unityMoveSound;

	public AudioEventGroup unityInitializePotentialDragSound;

	public AudioEventGroup unityBeginDragSound;

	public AudioEventGroup unityEndDragSound;

	public AudioEventGroup unitySubmitSound;

	public AudioEventGroup unityCancelSound;

	public AudioEventGroup unityToggleSound;

	public List<AudioEventGroup> userDefinedSounds = new List<AudioEventGroup>();

	public List<AudioEventGroup> mechanimStateChangedSounds = new List<AudioEventGroup>();

	public bool useStartSound;

	public bool useVisibleSound;

	public bool useInvisibleSound;

	public bool useCollisionSound;

	public bool useCollisionExitSound;

	public bool useTriggerEnterSound;

	public bool useTriggerExitSound;

	public bool useTriggerStaySound;

	public bool useMouseEnterSound;

	public bool useMouseExitSound;

	public bool useMouseClickSound;

	public bool useMouseUpSound;

	public bool useMouseDragSound;

	public bool useSpawnedSound;

	public bool useDespawnedSound;

	public bool useEnableSound;

	public bool useDisableSound;

	public bool useCollision2dSound;

	public bool useCollisionExit2dSound;

	public bool useTriggerEnter2dSound;

	public bool useTriggerStay2dSound;

	public bool useTriggerExit2dSound;

	public bool useParticleCollisionSound;

	public bool useNguiOnClickSound;

	public bool useNguiMouseDownSound;

	public bool useNguiMouseUpSound;

	public bool useNguiMouseEnterSound;

	public bool useNguiMouseExitSound;

	public bool useCodeTriggeredEvent1Sound;

	public bool useCodeTriggeredEvent2Sound;

	public bool useUnitySliderChangedSound;

	public bool useUnityButtonClickedSound;

	public bool useUnityPointerDownSound;

	public bool useUnityDragSound;

	public bool useUnityPointerUpSound;

	public bool useUnityPointerEnterSound;

	public bool useUnityPointerExitSound;

	public bool useUnityDropSound;

	public bool useUnityScrollSound;

	public bool useUnityUpdateSelectedSound;

	public bool useUnitySelectSound;

	public bool useUnityDeselectSound;

	public bool useUnityMoveSound;

	public bool useUnityInitializePotentialDragSound;

	public bool useUnityBeginDragSound;

	public bool useUnityEndDragSound;

	public bool useUnitySubmitSound;

	public bool useUnityCancelSound;

	public bool useUnityToggleSound;

	private Slider _slider;

	private Toggle _toggle;

	private Button _button;

	private bool _isVisible;

	private bool _needsCoroutine;

	private float? _triggerEnterTime;

	private float? _triggerEnter2dTime;

	private bool _mouseDragSoundPlayed;

	private PlaySoundResult _mouseDragResult;

	private Transform _trans;

	private readonly List<AudioEventGroup> _validMechanimStateChangedSounds = new List<AudioEventGroup>();

	private Animator _anim;

	private AudioEventGroup eventsToPlayDuringStart;

	private bool startHappened;

	private bool IsSetToUGUI => unityUIMode != UnityUIVersion.Legacy;

	private bool IsSetToLegacyUI => unityUIMode == UnityUIVersion.Legacy;

	private Transform Trans
	{
		get
		{
			if (_trans != null)
			{
				return _trans;
			}
			_trans = base.transform;
			return _trans;
		}
	}

	private void Awake()
	{
		_trans = base.transform;
		_anim = GetComponent<Animator>();
		_slider = GetComponent<Slider>();
		_button = GetComponent<Button>();
		_toggle = GetComponent<Toggle>();
		if (IsSetToUGUI)
		{
			AddUGUIComponents();
		}
		SpawnedOrAwake();
	}

	protected virtual void SpawnedOrAwake()
	{
		_isVisible = false;
		_validMechanimStateChangedSounds.Clear();
		_needsCoroutine = false;
		if (disableSounds || _anim == null)
		{
			return;
		}
		for (int i = 0; i < mechanimStateChangedSounds.Count; i++)
		{
			AudioEventGroup audioEventGroup = mechanimStateChangedSounds[i];
			if (audioEventGroup.mechanimEventActive && !string.IsNullOrEmpty(audioEventGroup.mechanimStateName))
			{
				_needsCoroutine = true;
				_validMechanimStateChangedSounds.Add(audioEventGroup);
			}
		}
	}

	private IEnumerator CoUpdate()
	{
		while (true)
		{
			yield return MasterAudio.EndOfFrameDelay;
			for (int i = 0; i < _validMechanimStateChangedSounds.Count; i++)
			{
				AudioEventGroup audioEventGroup = _validMechanimStateChangedSounds[i];
				if (!_anim.GetCurrentAnimatorStateInfo(0).IsName(audioEventGroup.mechanimStateName))
				{
					audioEventGroup.mechEventPlayedForState = false;
				}
				else if (!audioEventGroup.mechEventPlayedForState)
				{
					audioEventGroup.mechEventPlayedForState = true;
					PlaySounds(audioEventGroup, EventType.MechanimStateChanged);
				}
			}
		}
	}

	private void Start()
	{
		CheckForIllegalCustomEvents();
		if (useStartSound)
		{
			PlaySounds(startSound, EventType.OnStart);
		}
		if (eventsToPlayDuringStart != null && !startHappened)
		{
			PlaySounds(eventsToPlayDuringStart, EventType.OnStart);
		}
		eventsToPlayDuringStart = null;
		startHappened = true;
	}

	private void OnBecameVisible()
	{
		if (useVisibleSound && !_isVisible)
		{
			_isVisible = true;
			PlaySounds(visibleSound, EventType.OnVisible);
		}
	}

	private void OnBecameInvisible()
	{
		if (useInvisibleSound)
		{
			_isVisible = false;
			PlaySounds(invisibleSound, EventType.OnInvisible);
		}
	}

	private void OnEnable()
	{
		if (_slider != null)
		{
			_slider.onValueChanged.AddListener(SliderChanged);
			RestorePersistentSliders();
		}
		if (_button != null)
		{
			_button.onClick.AddListener(ButtonClicked);
		}
		if (_toggle != null)
		{
			_toggle.onValueChanged.AddListener(ToggleChanged);
		}
		_mouseDragResult = null;
		RegisterReceiver();
		if (_needsCoroutine)
		{
			StopAllCoroutines();
			StartCoroutine(CoUpdate());
		}
		if (!useEnableSound)
		{
			return;
		}
		if (!startHappened)
		{
			bool flag = false;
			for (int i = 0; i < enableSound.SoundEvents.Count; i++)
			{
				if (enableSound.SoundEvents[i].currentSoundFunctionType == MasterAudio.EventSoundFunctionType.PlaylistControl)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				eventsToPlayDuringStart = enableSound;
				return;
			}
		}
		PlaySounds(enableSound, EventType.OnEnable);
	}

	private void RestorePersistentSliders()
	{
		if (!useUnitySliderChangedSound)
		{
			return;
		}
		foreach (AudioEvent soundEvent in unitySliderChangedSound.SoundEvents)
		{
			if (soundEvent.currentSoundFunctionType != MasterAudio.EventSoundFunctionType.PersistentSettingsControl || soundEvent.targetVolMode != AudioEvent.TargetVolumeMode.UseSliderValue)
			{
				continue;
			}
			switch (soundEvent.currentPersistentSettingsCommand)
			{
			case MasterAudio.PersistentSettingsCommand.SetMusicVolume:
			{
				float? musicVolume = PersistentAudioSettings.MusicVolume;
				if (musicVolume.HasValue)
				{
					_slider.value = musicVolume.Value;
				}
				break;
			}
			case MasterAudio.PersistentSettingsCommand.SetBusVolume:
				if (!soundEvent.allSoundTypesForBusCmd)
				{
					float? busVolume = PersistentAudioSettings.GetBusVolume(soundEvent.busName);
					if (busVolume.HasValue)
					{
						_slider.value = busVolume.Value;
					}
				}
				break;
			case MasterAudio.PersistentSettingsCommand.SetMixerVolume:
			{
				float? mixerVolume = PersistentAudioSettings.MixerVolume;
				if (mixerVolume.HasValue)
				{
					_slider.value = mixerVolume.Value;
				}
				break;
			}
			case MasterAudio.PersistentSettingsCommand.SetGroupVolume:
				if (!soundEvent.allSoundTypesForGroupCmd)
				{
					float? groupVolume = PersistentAudioSettings.GetGroupVolume(soundEvent.soundType);
					if (groupVolume.HasValue)
					{
						_slider.value = groupVolume.Value;
					}
				}
				break;
			}
		}
	}

	private void OnDisable()
	{
		if (!(MasterAudio.SafeInstance == null) && !MasterAudio.AppIsShuttingDown)
		{
			if (_slider != null)
			{
				_slider.onValueChanged.RemoveListener(SliderChanged);
			}
			if (_button != null)
			{
				_button.onClick.RemoveListener(ButtonClicked);
			}
			if (_toggle != null)
			{
				_toggle.onValueChanged.RemoveListener(ToggleChanged);
			}
			UnregisterReceiver();
			if (useDisableSound && !MasterAudio.AppIsShuttingDown)
			{
				PlaySounds(disableSound, EventType.OnDisable);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (useCollisionSound && (!collisionSound.useLayerFilter || collisionSound.matchingLayers.Contains(collision.gameObject.layer)) && (!collisionSound.useTagFilter || collisionSound.matchingTags.Contains(collision.gameObject.tag)))
		{
			PlaySounds(collisionSound, EventType.OnCollision);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (useCollisionExitSound && (!collisionExitSound.useLayerFilter || collisionExitSound.matchingLayers.Contains(collision.gameObject.layer)) && (!collisionExitSound.useTagFilter || collisionExitSound.matchingTags.Contains(collision.gameObject.tag)))
		{
			PlaySounds(collisionExitSound, EventType.OnCollisionExit);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		_triggerEnterTime = Time.realtimeSinceStartup;
		if (useTriggerEnterSound && (!triggerSound.useLayerFilter || triggerSound.matchingLayers.Contains(other.gameObject.layer)) && (!triggerSound.useTagFilter || triggerSound.matchingTags.Contains(other.gameObject.tag)))
		{
			PlaySounds(triggerSound, EventType.OnTriggerEnter);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (useTriggerStaySound)
		{
			float num = 0f;
			num = (_triggerEnterTime.HasValue ? (Time.realtimeSinceStartup - _triggerEnterTime.Value) : 0f);
			if (!(triggerStaySound.triggerStayForTime > num) && PlaySounds(triggerStaySound, EventType.OnTriggerStay) && triggerStaySound.doesTriggerStayRepeat)
			{
				_triggerEnterTime = Time.realtimeSinceStartup;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		_triggerEnterTime = null;
		if (useTriggerExitSound && (!triggerExitSound.useLayerFilter || triggerExitSound.matchingLayers.Contains(other.gameObject.layer)) && (!triggerExitSound.useTagFilter || triggerExitSound.matchingTags.Contains(other.gameObject.tag)))
		{
			PlaySounds(triggerExitSound, EventType.OnTriggerExit);
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		if (useParticleCollisionSound && (!particleCollisionSound.useLayerFilter || particleCollisionSound.matchingLayers.Contains(other.gameObject.layer)) && (!particleCollisionSound.useTagFilter || particleCollisionSound.matchingTags.Contains(other.gameObject.tag)))
		{
			PlaySounds(particleCollisionSound, EventType.OnParticleCollision);
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
		if (IsSetToUGUI && useUnityPointerEnterSound)
		{
			PlaySounds(unityPointerEnterSound, EventType.UnityPointerEnter);
		}
	}

	public void OnPointerExit(PointerEventData data)
	{
		if (IsSetToUGUI && useUnityPointerExitSound)
		{
			PlaySounds(unityPointerExitSound, EventType.UnityPointerExit);
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (IsSetToUGUI && useUnityPointerDownSound)
		{
			PlaySounds(unityPointerDownSound, EventType.UnityPointerDown);
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		if (IsSetToUGUI && useUnityPointerUpSound)
		{
			PlaySounds(unityPointerUpSound, EventType.UnityPointerUp);
		}
	}

	private void OnDrag(Vector2 delta)
	{
	}

	public void OnDrag(PointerEventData data)
	{
		if (IsSetToUGUI && useUnityDragSound)
		{
			PlaySounds(unityDragSound, EventType.UnityDrag);
		}
	}

	private void OnDrop(GameObject go)
	{
	}

	public void OnDrop(PointerEventData data)
	{
		if (IsSetToUGUI && useUnityDropSound)
		{
			PlaySounds(unityDropSound, EventType.UnityDrop);
		}
	}

	public void OnScroll(PointerEventData data)
	{
		if (IsSetToUGUI && useUnityScrollSound)
		{
			PlaySounds(unityScrollSound, EventType.UnityScroll);
		}
	}

	public void OnUpdateSelected(BaseEventData data)
	{
		if (IsSetToUGUI && useUnityUpdateSelectedSound)
		{
			PlaySounds(unityUpdateSelectedSound, EventType.UnityUpdateSelected);
		}
	}

	private void OnSelect(bool isSelected)
	{
	}

	public void OnSelect(BaseEventData data)
	{
		if (IsSetToUGUI && useUnitySelectSound)
		{
			PlaySounds(unitySelectSound, EventType.UnitySelect);
		}
	}

	public void OnDeselect(BaseEventData data)
	{
		if (IsSetToUGUI && useUnityDeselectSound)
		{
			PlaySounds(unityDeselectSound, EventType.UnityDeselect);
		}
	}

	public void OnMove(AxisEventData data)
	{
		if (IsSetToUGUI && useUnityMoveSound)
		{
			PlaySounds(unityMoveSound, EventType.UnityMove);
		}
	}

	public void OnInitializePotentialDrag(PointerEventData data)
	{
		if (IsSetToUGUI && useUnityInitializePotentialDragSound)
		{
			PlaySounds(unityInitializePotentialDragSound, EventType.UnityInitializePotentialDrag);
		}
	}

	public void OnBeginDrag(PointerEventData data)
	{
		if (IsSetToUGUI && useUnityBeginDragSound)
		{
			PlaySounds(unityBeginDragSound, EventType.UnityBeginDrag);
		}
	}

	public void OnEndDrag(PointerEventData data)
	{
		if (IsSetToUGUI && useUnityEndDragSound)
		{
			PlaySounds(unityEndDragSound, EventType.UnityEndDrag);
		}
	}

	public void OnSubmit(BaseEventData data)
	{
		if (IsSetToUGUI && useUnitySubmitSound)
		{
			PlaySounds(unitySubmitSound, EventType.UnitySubmit);
		}
	}

	public void OnCancel(BaseEventData data)
	{
		if (IsSetToUGUI && useUnityCancelSound)
		{
			PlaySounds(unityCancelSound, EventType.UnityCancel);
		}
	}

	private void SliderChanged(float newValue)
	{
		if (useUnitySliderChangedSound)
		{
			unitySliderChangedSound.sliderValue = newValue;
			PlaySounds(unitySliderChangedSound, EventType.UnitySliderChanged);
		}
	}

	private void ToggleChanged(bool newValue)
	{
		if (useUnityToggleSound)
		{
			PlaySounds(unityToggleSound, EventType.UnityToggle);
		}
	}

	private void ButtonClicked()
	{
		if (useUnityButtonClickedSound)
		{
			PlaySounds(unityButtonClickedSound, EventType.UnityButtonClicked);
		}
	}

	private void OnMouseEnter()
	{
		if (IsSetToLegacyUI && useMouseEnterSound)
		{
			PlaySounds(mouseEnterSound, EventType.OnMouseEnter);
		}
	}

	private void OnMouseExit()
	{
		if (IsSetToLegacyUI && useMouseExitSound)
		{
			PlaySounds(mouseExitSound, EventType.OnMouseExit);
		}
	}

	private void OnMouseDown()
	{
		if (IsSetToLegacyUI && useMouseClickSound)
		{
			PlaySounds(mouseClickSound, EventType.OnMouseClick);
		}
	}

	private void OnMouseUp()
	{
		if (IsSetToLegacyUI && useMouseUpSound)
		{
			PlaySounds(mouseUpSound, EventType.OnMouseUp);
		}
		if (useMouseDragSound)
		{
			switch (mouseUpSound.mouseDragStopMode)
			{
			case PreviousSoundStopMode.Stop:
				if (_mouseDragResult != null && (_mouseDragResult.SoundPlayed || _mouseDragResult.SoundScheduled))
				{
					_mouseDragResult.ActingVariation.Stop(stopEndDetection: true);
				}
				break;
			case PreviousSoundStopMode.FadeOut:
				if (_mouseDragResult != null && (_mouseDragResult.SoundPlayed || _mouseDragResult.SoundScheduled))
				{
					_mouseDragResult.ActingVariation.FadeToVolume(0f, mouseUpSound.mouseDragFadeOutTime);
				}
				break;
			}
			_mouseDragResult = null;
		}
		_mouseDragSoundPlayed = false;
	}

	private void OnMouseDrag()
	{
		if (IsSetToLegacyUI && useMouseDragSound && !_mouseDragSoundPlayed)
		{
			PlaySounds(mouseDragSound, EventType.OnMouseDrag);
			_mouseDragSoundPlayed = true;
		}
	}

	private void OnPress(bool isDown)
	{
		if (!showNGUI)
		{
			return;
		}
		if (isDown)
		{
			if (useNguiMouseDownSound)
			{
				PlaySounds(nguiMouseDownSound, EventType.NGUIMouseDown);
			}
		}
		else if (useNguiMouseUpSound)
		{
			PlaySounds(nguiMouseUpSound, EventType.NGUIMouseUp);
		}
	}

	private void OnClick()
	{
		if (showNGUI && useNguiOnClickSound)
		{
			PlaySounds(nguiOnClickSound, EventType.NGUIOnClick);
		}
	}

	private void OnHover(bool isOver)
	{
		if (!showNGUI)
		{
			return;
		}
		if (isOver)
		{
			if (useNguiMouseEnterSound)
			{
				PlaySounds(nguiMouseEnterSound, EventType.NGUIMouseEnter);
			}
		}
		else if (useNguiMouseExitSound)
		{
			PlaySounds(nguiMouseExitSound, EventType.NGUIMouseExit);
		}
	}

	private void OnSpawned()
	{
		SpawnedOrAwake();
		if (showPoolManager && useSpawnedSound)
		{
			PlaySounds(spawnedSound, EventType.OnSpawned);
		}
	}

	private void OnDespawned()
	{
		if (showPoolManager && useDespawnedSound)
		{
			PlaySounds(despawnedSound, EventType.OnDespawned);
		}
	}

	public void ActivateCodeTriggeredEvent1()
	{
		if (useCodeTriggeredEvent1Sound)
		{
			PlaySounds(codeTriggeredEvent1Sound, EventType.CodeTriggeredEvent1);
		}
	}

	public void ActivateCodeTriggeredEvent2()
	{
		if (useCodeTriggeredEvent2Sound)
		{
			PlaySounds(codeTriggeredEvent2Sound, EventType.CodeTriggeredEvent2);
		}
	}

	public void CalculateRadius(AudioEvent anEvent)
	{
		AudioSource namedOrFirstAudioSource = GetNamedOrFirstAudioSource(anEvent);
		if (namedOrFirstAudioSource == null)
		{
			anEvent.colliderMaxDistance = 0f;
		}
		else
		{
			anEvent.colliderMaxDistance = namedOrFirstAudioSource.maxDistance;
		}
	}

	public AudioSource GetNamedOrFirstAudioSource(AudioEvent anEvent)
	{
		if (string.IsNullOrEmpty(anEvent.soundType))
		{
			anEvent.colliderMaxDistance = 0f;
			return null;
		}
		if (MasterAudio.SafeInstance == null)
		{
			anEvent.colliderMaxDistance = 0f;
			return null;
		}
		Transform transform = MasterAudio.Instance.transform.Find(anEvent.soundType);
		if (transform == null)
		{
			anEvent.colliderMaxDistance = 0f;
			return null;
		}
		Transform transform2 = null;
		switch (anEvent.variationType)
		{
		case VariationType.PlayRandom:
			transform2 = transform.GetChild(0);
			break;
		case VariationType.PlaySpecific:
			transform2 = transform.transform.Find(anEvent.variationName);
			break;
		}
		if (transform2 == null)
		{
			anEvent.colliderMaxDistance = 0f;
			return null;
		}
		return transform2.GetComponent<AudioSource>();
	}

	public List<AudioSource> GetAllVariationAudioSources(AudioEvent anEvent)
	{
		if (string.IsNullOrEmpty(anEvent.soundType))
		{
			anEvent.colliderMaxDistance = 0f;
			return null;
		}
		if (MasterAudio.SafeInstance == null)
		{
			anEvent.colliderMaxDistance = 0f;
			return null;
		}
		Transform transform = MasterAudio.Instance.transform.Find(anEvent.soundType);
		if (transform == null)
		{
			anEvent.colliderMaxDistance = 0f;
			return null;
		}
		List<AudioSource> list = new List<AudioSource>(transform.childCount);
		for (int i = 0; i < transform.childCount; i++)
		{
			AudioSource component = transform.GetChild(i).GetComponent<AudioSource>();
			list.Add(component);
		}
		return list;
	}

	public AudioEventGroup GetMechanimAudioEventGroup(string stateName)
	{
		for (int i = 0; i < _validMechanimStateChangedSounds.Count; i++)
		{
			AudioEventGroup audioEventGroup = _validMechanimStateChangedSounds[i];
			if (audioEventGroup.mechanimStateName == stateName)
			{
				return audioEventGroup;
			}
		}
		return null;
	}

	public bool PlaySounds(AudioEventGroup eventGrp, EventType eType)
	{
		if (!CheckForRetriggerLimit(eventGrp))
		{
			return false;
		}
		if (MasterAudio.SafeInstance == null)
		{
			return false;
		}
		switch (eventGrp.retriggerLimitMode)
		{
		case RetriggerLimMode.FrameBased:
			eventGrp.triggeredLastFrame = AudioUtil.FrameCount;
			break;
		case RetriggerLimMode.TimeBased:
			eventGrp.triggeredLastTime = AudioUtil.Time;
			break;
		}
		if (!MasterAudio.AppIsShuttingDown && MasterAudio.IsWarming)
		{
			AudioEvent aEvent = new AudioEvent();
			PerformSingleAction(eventGrp, aEvent, eType);
			return true;
		}
		for (int i = 0; i < eventGrp.SoundEvents.Count; i++)
		{
			PerformSingleAction(eventGrp, eventGrp.SoundEvents[i], eType);
		}
		return true;
	}

	private void OnDrawGizmos()
	{
		if (!(MasterAudio.SafeInstance == null) && MasterAudio.Instance.showRangeSoundGizmos && eventToGizmo != null && eventToGizmo.colliderMaxDistance != 0f)
		{
			Color color = Color.green;
			if (MasterAudio.SafeInstance != null)
			{
				color = MasterAudio.Instance.rangeGizmoColor;
			}
			Gizmos.color = color;
			Gizmos.DrawWireSphere(base.transform.position, eventToGizmo.colliderMaxDistance);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!(MasterAudio.SafeInstance == null) && MasterAudio.Instance.showSelectedRangeSoundGizmos && eventToGizmo != null && eventToGizmo.colliderMaxDistance != 0f)
		{
			Color color = Color.green;
			if (MasterAudio.SafeInstance != null)
			{
				color = MasterAudio.Instance.selectedRangeGizmoColor;
			}
			Gizmos.color = color;
			Gizmos.DrawWireSphere(base.transform.position, eventToGizmo.colliderMaxDistance);
		}
	}

	private static bool CheckForRetriggerLimit(AudioEventGroup grp)
	{
		switch (grp.retriggerLimitMode)
		{
		case RetriggerLimMode.FrameBased:
			if (grp.triggeredLastFrame > 0 && AudioUtil.FrameCount - grp.triggeredLastFrame < grp.limitPerXFrm)
			{
				return false;
			}
			break;
		case RetriggerLimMode.TimeBased:
			if (grp.triggeredLastTime > 0f && AudioUtil.Time - grp.triggeredLastTime < grp.limitPerXSec)
			{
				return false;
			}
			break;
		}
		return true;
	}

	private bool AllPlayersShouldHearAction(AudioEventGroup grp, EventType eType)
	{
		if (DisallowedMultBroadcastEventType.Contains(eType))
		{
			return false;
		}
		if (multiplayerBroadcast || grp.multiplayerBroadcast)
		{
			return MasterAudioMultiplayerAdapter.CanSendRPCs;
		}
		return false;
	}

	private void PerformSingleAction(AudioEventGroup grp, AudioEvent aEvent, EventType eType)
	{
		if (disableSounds || MasterAudio.AppIsShuttingDown || MasterAudio.SafeInstance == null)
		{
			return;
		}
		bool flag = eType == EventType.UnitySliderChanged && aEvent.targetVolMode == AudioEvent.TargetVolumeMode.UseSliderValue;
		float num = aEvent.volume;
		string soundType = aEvent.soundType;
		float? pitch = aEvent.pitch;
		if (!aEvent.useFixedPitch)
		{
			pitch = null;
		}
		PlaySoundResult playSoundResult = null;
		bool flag2 = AllPlayersShouldHearAction(grp, eType);
		MasterAudio.SoundSpawnLocationMode soundSpawnLocationMode = soundSpawnMode;
		if (eType == EventType.OnDisable || eType == EventType.OnDespawned)
		{
			soundSpawnLocationMode = MasterAudio.SoundSpawnLocationMode.CallerLocation;
		}
		bool flag3 = eType == EventType.OnMouseDrag || aEvent.glidePitchType != GlidePitchType.None;
		switch (aEvent.currentSoundFunctionType)
		{
		case MasterAudio.EventSoundFunctionType.PlaySound:
		{
			string text2 = null;
			if (aEvent.variationType == VariationType.PlaySpecific)
			{
				text2 = aEvent.variationName;
			}
			if (flag)
			{
				num = grp.sliderValue;
			}
			switch (soundSpawnLocationMode)
			{
			case MasterAudio.SoundSpawnLocationMode.CallerLocation:
				if (flag3)
				{
					playSoundResult = ((!flag2) ? MasterAudio.PlaySound3DAtTransform(soundType, Trans, num, pitch, aEvent.delaySound, text2) : MasterAudioMultiplayerAdapter.PlaySound3DAtTransform(soundType, Trans, num, pitch, aEvent.delaySound, text2));
					break;
				}
				if (flag2)
				{
					playSoundResult = MasterAudioMultiplayerAdapter.PlaySound3DAtTransform(soundType, Trans, num, pitch, aEvent.delaySound, text2);
					break;
				}
				playSoundResult = MasterAudio.PlaySound3DAtTransform(soundType, Trans, num, pitch, aEvent.delaySound, text2);
				if (playSoundResult != null)
				{
					aEvent.variationName = playSoundResult.ActingVariation.name;
				}
				break;
			case MasterAudio.SoundSpawnLocationMode.AttachToCaller:
				if (flag3)
				{
					playSoundResult = ((!flag2) ? MasterAudio.PlaySound3DFollowTransform(soundType, Trans, num, pitch, aEvent.delaySound, text2) : MasterAudioMultiplayerAdapter.PlaySound3DFollowTransform(soundType, Trans, num, pitch, aEvent.delaySound, text2));
				}
				else if (flag2)
				{
					MasterAudioMultiplayerAdapter.PlaySound3DFollowTransformAndForget(soundType, Trans, num, pitch, aEvent.delaySound, text2);
				}
				else
				{
					MasterAudio.PlaySound3DFollowTransformAndForget(soundType, Trans, num, pitch, aEvent.delaySound, text2);
				}
				break;
			case MasterAudio.SoundSpawnLocationMode.MasterAudioLocation:
				if (flag3)
				{
					playSoundResult = ((!flag2) ? MasterAudio.PlaySound(soundType, num, pitch, aEvent.delaySound, text2) : MasterAudioMultiplayerAdapter.PlaySound(Trans, soundType, num, pitch, aEvent.delaySound, text2));
				}
				else if (flag2)
				{
					MasterAudioMultiplayerAdapter.PlaySoundAndForget(Trans, soundType, num, pitch, aEvent.delaySound, text2);
				}
				else
				{
					MasterAudio.PlaySoundAndForget(soundType, num, pitch, aEvent.delaySound, text2);
				}
				break;
			}
			if (playSoundResult != null && playSoundResult.ActingVariation != null && aEvent.glidePitchType != GlidePitchType.None)
			{
				switch (aEvent.glidePitchType)
				{
				case GlidePitchType.RaisePitch:
					if (!string.IsNullOrEmpty(aEvent.theCustomEventName))
					{
						playSoundResult.ActingVariation.GlideByPitch(aEvent.targetGlidePitch, aEvent.pitchGlideTime, () =>
						{
							MasterAudio.FireCustomEvent(aEvent.theCustomEventName, _trans);
						});
					}
					else
					{
						playSoundResult.ActingVariation.GlideByPitch(aEvent.targetGlidePitch, aEvent.pitchGlideTime);
					}
					break;
				case GlidePitchType.LowerPitch:
					if (!string.IsNullOrEmpty(aEvent.theCustomEventName))
					{
						playSoundResult.ActingVariation.GlideByPitch(aEvent.targetGlidePitch, aEvent.pitchGlideTime, () =>
						{
							MasterAudio.FireCustomEvent(aEvent.theCustomEventName, _trans);
						});
					}
					else
					{
						playSoundResult.ActingVariation.GlideByPitch(0f - aEvent.targetGlidePitch, aEvent.pitchGlideTime);
					}
					break;
				}
			}
			if (eType == EventType.OnMouseDrag)
			{
				_mouseDragResult = playSoundResult;
			}
			break;
		}
		case MasterAudio.EventSoundFunctionType.PlaylistControl:
			playSoundResult = new PlaySoundResult
			{
				ActingVariation = null,
				SoundPlayed = true,
				SoundScheduled = false
			};
			if (string.IsNullOrEmpty(aEvent.playlistControllerName))
			{
				aEvent.playlistControllerName = "~only~";
			}
			switch (aEvent.currentPlaylistCommand)
			{
			case MasterAudio.PlaylistCommand.None:
				playSoundResult.SoundPlayed = false;
				break;
			case MasterAudio.PlaylistCommand.Restart:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.RestartAllPlaylists(Trans);
					}
					else
					{
						MasterAudio.RestartAllPlaylists();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.RestartPlaylist(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.RestartPlaylist(aEvent.playlistControllerName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.Start:
				if (!(aEvent.playlistControllerName == "[None]") && !(aEvent.playlistName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StartPlaylist(Trans, aEvent.playlistControllerName, aEvent.playlistName);
					}
					else
					{
						MasterAudio.StartPlaylist(aEvent.playlistControllerName, aEvent.playlistName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.ChangePlaylist:
				if (string.IsNullOrEmpty(aEvent.playlistName))
				{
					Debug.Log("You have not specified a Playlist name for Event Sounds on '" + _trans.name + "'.");
					playSoundResult.SoundPlayed = false;
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.ChangePlaylistByName(Trans, aEvent.playlistControllerName, aEvent.playlistName, aEvent.startPlaylist);
					}
					else
					{
						MasterAudio.ChangePlaylistByName(aEvent.playlistControllerName, aEvent.playlistName, aEvent.startPlaylist);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.StopLoopingCurrentSong:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopLoopingAllCurrentSongs(Trans);
					}
					else
					{
						MasterAudio.StopLoopingAllCurrentSongs();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopLoopingCurrentSong(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.StopLoopingCurrentSong(aEvent.playlistControllerName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.StopPlaylistAfterCurrentSong:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopAllPlaylistsAfterCurrentSongs(Trans);
					}
					else
					{
						MasterAudio.StopAllPlaylistsAfterCurrentSongs();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopPlaylistAfterCurrentSong(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.StopPlaylistAfterCurrentSong(aEvent.playlistControllerName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.FadeToVolume:
			{
				float num4 = (flag ? grp.sliderValue : aEvent.fadeVolume);
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.FadeAllPlaylistsToVolume(Trans, num4, aEvent.fadeTime);
					}
					else
					{
						MasterAudio.FadeAllPlaylistsToVolume(num4, aEvent.fadeTime);
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.FadePlaylistToVolume(Trans, aEvent.playlistControllerName, num4, aEvent.fadeTime);
					}
					else
					{
						MasterAudio.FadePlaylistToVolume(aEvent.playlistControllerName, num4, aEvent.fadeTime);
					}
				}
				break;
			}
			case MasterAudio.PlaylistCommand.Mute:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.MuteAllPlaylists(Trans);
					}
					else
					{
						MasterAudio.MuteAllPlaylists();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.MutePlaylist(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.MutePlaylist(aEvent.playlistControllerName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.Unmute:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnmuteAllPlaylists(Trans);
					}
					else
					{
						MasterAudio.UnmuteAllPlaylists();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnmutePlaylist(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.UnmutePlaylist(aEvent.playlistControllerName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.ToggleMute:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.ToggleMuteAllPlaylists(Trans);
					}
					else
					{
						MasterAudio.ToggleMuteAllPlaylists();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.ToggleMutePlaylist(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.ToggleMutePlaylist(aEvent.playlistControllerName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.PlaySong:
				if (string.IsNullOrEmpty(aEvent.clipName))
				{
					Debug.Log("You have not specified a song name for Event Sounds on '" + _trans.name + "'.");
					playSoundResult.SoundPlayed = false;
				}
				else
				{
					if (aEvent.playlistControllerName == "[None]")
					{
						break;
					}
					if (flag2)
					{
						if (!MasterAudioMultiplayerAdapter.TriggerPlaylistClip(Trans, aEvent.playlistControllerName, aEvent.clipName))
						{
							playSoundResult.SoundPlayed = false;
						}
					}
					else if (!MasterAudio.TriggerPlaylistClip(aEvent.playlistControllerName, aEvent.clipName))
					{
						playSoundResult.SoundPlayed = false;
					}
				}
				break;
			case MasterAudio.PlaylistCommand.AddSongToQueue:
				playSoundResult.SoundPlayed = false;
				if (string.IsNullOrEmpty(aEvent.clipName))
				{
					Debug.Log("You have not specified a song name for Event Sounds on '" + _trans.name + "'.");
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.QueuePlaylistClip(Trans, aEvent.playlistControllerName, aEvent.clipName);
					}
					else
					{
						MasterAudio.QueuePlaylistClip(aEvent.playlistControllerName, aEvent.clipName);
					}
					playSoundResult.SoundPlayed = true;
				}
				break;
			case MasterAudio.PlaylistCommand.PlayRandomSong:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.TriggerRandomClipAllPlaylists(Trans);
					}
					else
					{
						MasterAudio.TriggerRandomClipAllPlaylists();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.TriggerRandomPlaylistClip(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.TriggerRandomPlaylistClip(aEvent.playlistControllerName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.PlayNextSong:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.TriggerNextClipAllPlaylists(Trans);
					}
					else
					{
						MasterAudio.TriggerNextClipAllPlaylists();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.TriggerNextPlaylistClip(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.TriggerNextPlaylistClip(aEvent.playlistControllerName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.Pause:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.PauseAllPlaylists(Trans);
					}
					else
					{
						MasterAudio.PauseAllPlaylists();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.PausePlaylist(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.PausePlaylist(aEvent.playlistControllerName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.Stop:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopAllPlaylists(Trans);
					}
					else
					{
						MasterAudio.StopAllPlaylists();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopPlaylist(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.StopPlaylist(aEvent.playlistControllerName);
					}
				}
				break;
			case MasterAudio.PlaylistCommand.Resume:
				if (aEvent.allPlaylistControllersForGroupCmd)
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnpauseAllPlaylists(Trans);
					}
					else
					{
						MasterAudio.UnpauseAllPlaylists();
					}
				}
				else if (!(aEvent.playlistControllerName == "[None]"))
				{
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnpausePlaylist(Trans, aEvent.playlistControllerName);
					}
					else
					{
						MasterAudio.UnpausePlaylist(aEvent.playlistControllerName);
					}
				}
				break;
			}
			break;
		case MasterAudio.EventSoundFunctionType.GroupControl:
		{
			playSoundResult = new PlaySoundResult
			{
				ActingVariation = null,
				SoundPlayed = true,
				SoundScheduled = false
			};
			string text3 = string.Empty;
			List<string> list6 = new List<string>();
			if (!aEvent.allSoundTypesForGroupCmd || MasterAudio.GroupCommandsWithNoAllGroupSelector.Contains(aEvent.currentSoundGroupCommand))
			{
				list6.Add(aEvent.soundType);
			}
			else
			{
				list6.AddRange(MasterAudio.RuntimeSoundGroupNames);
				if (flag2)
				{
					text3 = "[All]";
				}
			}
			for (int num6 = 0; num6 < list6.Count; num6++)
			{
				string text4 = list6[num6];
				if (!string.IsNullOrEmpty(text3))
				{
					text4 = text3;
				}
				switch (aEvent.currentSoundGroupCommand)
				{
				case MasterAudio.SoundGroupCommand.None:
					playSoundResult.SoundPlayed = false;
					break;
				case MasterAudio.SoundGroupCommand.ToggleSoundGroup:
					if (MasterAudio.IsSoundGroupPlaying(text4))
					{
						if (flag2)
						{
							MasterAudioMultiplayerAdapter.FadeOutAllOfSound(Trans, text4, aEvent.fadeTime);
						}
						else
						{
							MasterAudio.FadeOutAllOfSound(text4, aEvent.fadeTime);
						}
						break;
					}
					switch (soundSpawnLocationMode)
					{
					case MasterAudio.SoundSpawnLocationMode.CallerLocation:
						if (flag2)
						{
							MasterAudioMultiplayerAdapter.PlaySound3DAtTransformAndForget(soundType, Trans, num, pitch, aEvent.delaySound);
						}
						else
						{
							MasterAudio.PlaySound3DAtTransformAndForget(soundType, Trans, num, pitch, aEvent.delaySound);
						}
						break;
					case MasterAudio.SoundSpawnLocationMode.AttachToCaller:
						if (flag2)
						{
							MasterAudioMultiplayerAdapter.PlaySound3DFollowTransformAndForget(soundType, Trans, num, pitch, aEvent.delaySound);
						}
						else
						{
							MasterAudio.PlaySound3DFollowTransformAndForget(soundType, Trans, num, pitch, aEvent.delaySound);
						}
						break;
					case MasterAudio.SoundSpawnLocationMode.MasterAudioLocation:
						if (flag2)
						{
							MasterAudioMultiplayerAdapter.PlaySoundAndForget(Trans, soundType, num, pitch, aEvent.delaySound);
						}
						else
						{
							MasterAudio.PlaySoundAndForget(soundType, num, pitch, aEvent.delaySound);
						}
						break;
					}
					break;
				case MasterAudio.SoundGroupCommand.ToggleSoundGroupOfTransform:
					if (MasterAudio.IsTransformPlayingSoundGroup(text4, _trans))
					{
						if (flag2)
						{
							MasterAudioMultiplayerAdapter.FadeOutSoundGroupOfTransform(Trans, text4, aEvent.fadeTime);
						}
						else
						{
							MasterAudio.FadeOutSoundGroupOfTransform(Trans, text4, aEvent.fadeTime);
						}
						break;
					}
					switch (soundSpawnLocationMode)
					{
					case MasterAudio.SoundSpawnLocationMode.CallerLocation:
						if (flag2)
						{
							MasterAudioMultiplayerAdapter.PlaySound3DAtTransformAndForget(soundType, Trans, num, pitch, aEvent.delaySound);
						}
						else
						{
							MasterAudio.PlaySound3DAtTransformAndForget(soundType, Trans, num, pitch, aEvent.delaySound);
						}
						break;
					case MasterAudio.SoundSpawnLocationMode.AttachToCaller:
						if (flag2)
						{
							MasterAudioMultiplayerAdapter.PlaySound3DFollowTransformAndForget(soundType, Trans, num, pitch, aEvent.delaySound);
						}
						else
						{
							MasterAudio.PlaySound3DFollowTransformAndForget(soundType, Trans, num, pitch, aEvent.delaySound);
						}
						break;
					case MasterAudio.SoundSpawnLocationMode.MasterAudioLocation:
						if (flag2)
						{
							MasterAudioMultiplayerAdapter.PlaySoundAndForget(Trans, soundType, num, pitch, aEvent.delaySound);
						}
						else
						{
							MasterAudio.PlaySoundAndForget(soundType, num, pitch, aEvent.delaySound);
						}
						break;
					}
					break;
				case MasterAudio.SoundGroupCommand.RefillSoundGroupPool:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.RefillSoundGroupPool(Trans, text4);
					}
					else
					{
						MasterAudio.RefillSoundGroupPool(text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.FadeToVolume:
				{
					float num7 = (flag ? grp.sliderValue : aEvent.fadeVolume);
					bool flag6 = aEvent.fireCustomEventAfterFade && !string.IsNullOrEmpty(aEvent.theCustomEventName);
					if (flag2)
					{
						if (flag6)
						{
							MasterAudioMultiplayerAdapter.FadeSoundGroupToVolume(Trans, text4, num7, aEvent.fadeTime, () =>
							{
								MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
							}, aEvent.stopAfterFade, aEvent.restoreVolumeAfterFade);
						}
						else
						{
							MasterAudioMultiplayerAdapter.FadeSoundGroupToVolume(Trans, text4, num7, aEvent.fadeTime, null, aEvent.stopAfterFade, aEvent.restoreVolumeAfterFade);
						}
					}
					else if (flag6)
					{
						MasterAudio.FadeSoundGroupToVolume(text4, num7, aEvent.fadeTime, () =>
						{
							MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
						}, aEvent.stopAfterFade, aEvent.restoreVolumeAfterFade);
					}
					else
					{
						MasterAudio.FadeSoundGroupToVolume(text4, num7, aEvent.fadeTime, null, aEvent.stopAfterFade, aEvent.restoreVolumeAfterFade);
					}
					break;
				}
				case MasterAudio.SoundGroupCommand.FadeOutAllOfSound:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.FadeOutAllOfSound(Trans, text4, aEvent.fadeTime);
					}
					else
					{
						MasterAudio.FadeOutAllOfSound(text4, aEvent.fadeTime);
					}
					break;
				case MasterAudio.SoundGroupCommand.Mute:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.MuteGroup(Trans, text4);
					}
					else
					{
						MasterAudio.MuteGroup(text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.Pause:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.PauseSoundGroup(Trans, text4);
					}
					else
					{
						MasterAudio.PauseSoundGroup(text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.Solo:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.SoloGroup(Trans, text4);
					}
					else
					{
						MasterAudio.SoloGroup(text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.StopAllOfSound:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopAllOfSound(Trans, text4);
					}
					else
					{
						MasterAudio.StopAllOfSound(text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.Unmute:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnmuteGroup(Trans, text4);
					}
					else
					{
						MasterAudio.UnmuteGroup(text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.Unpause:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnpauseSoundGroup(Trans, text4);
					}
					else
					{
						MasterAudio.UnpauseSoundGroup(text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.Unsolo:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnsoloGroup(Trans, text4);
					}
					else
					{
						MasterAudio.UnsoloGroup(text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.StopAllSoundsOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopAllSoundsOfTransform(Trans);
					}
					else
					{
						MasterAudio.StopAllSoundsOfTransform(Trans);
					}
					break;
				case MasterAudio.SoundGroupCommand.StopSoundGroupOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopSoundGroupOfTransform(Trans, text4);
					}
					else
					{
						MasterAudio.StopSoundGroupOfTransform(Trans, text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.PauseAllSoundsOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.PauseAllSoundsOfTransform(Trans);
					}
					else
					{
						MasterAudio.PauseAllSoundsOfTransform(Trans);
					}
					break;
				case MasterAudio.SoundGroupCommand.PauseSoundGroupOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.PauseSoundGroupOfTransform(Trans, text4);
					}
					else
					{
						MasterAudio.PauseSoundGroupOfTransform(Trans, text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.UnpauseAllSoundsOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnpauseAllSoundsOfTransform(Trans);
					}
					else
					{
						MasterAudio.UnpauseAllSoundsOfTransform(Trans);
					}
					break;
				case MasterAudio.SoundGroupCommand.UnpauseSoundGroupOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnpauseSoundGroupOfTransform(Trans, text4);
					}
					else
					{
						MasterAudio.UnpauseSoundGroupOfTransform(Trans, text4);
					}
					break;
				case MasterAudio.SoundGroupCommand.FadeOutSoundGroupOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.FadeOutSoundGroupOfTransform(Trans, text4, aEvent.fadeTime);
					}
					else
					{
						MasterAudio.FadeOutSoundGroupOfTransform(Trans, text4, aEvent.fadeTime);
					}
					break;
				case MasterAudio.SoundGroupCommand.FadeOutAllSoundsOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.FadeOutAllSoundsOfTransform(Trans, aEvent.fadeTime);
					}
					else
					{
						MasterAudio.FadeOutAllSoundsOfTransform(Trans, aEvent.fadeTime);
					}
					break;
				case MasterAudio.SoundGroupCommand.RouteToBus:
				{
					string text5 = aEvent.busName;
					if (text5 == "[None]")
					{
						text5 = null;
					}
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.RouteGroupToBus(Trans, text4, text5);
					}
					else
					{
						MasterAudio.RouteGroupToBus(text4, text5);
					}
					break;
				}
				case MasterAudio.SoundGroupCommand.GlideByPitch:
				{
					bool flag7 = !string.IsNullOrEmpty(aEvent.theCustomEventName);
					switch (aEvent.glidePitchType)
					{
					case GlidePitchType.RaisePitch:
						if (flag2)
						{
							if (flag7)
							{
								MasterAudioMultiplayerAdapter.GlideSoundGroupByPitch(Trans, text4, aEvent.targetGlidePitch, aEvent.pitchGlideTime, () =>
								{
									MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
								});
							}
							else
							{
								MasterAudioMultiplayerAdapter.GlideSoundGroupByPitch(Trans, text4, aEvent.targetGlidePitch, aEvent.pitchGlideTime, null);
							}
						}
						else if (flag7)
						{
							MasterAudio.GlideSoundGroupByPitch(text4, aEvent.targetGlidePitch, aEvent.pitchGlideTime, () =>
							{
								MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
							});
						}
						else
						{
							MasterAudio.GlideSoundGroupByPitch(text4, aEvent.targetGlidePitch, aEvent.pitchGlideTime);
						}
						break;
					case GlidePitchType.LowerPitch:
						if (flag2)
						{
							if (flag7)
							{
								MasterAudioMultiplayerAdapter.GlideSoundGroupByPitch(Trans, text4, 0f - aEvent.targetGlidePitch, aEvent.pitchGlideTime, () =>
								{
									MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
								});
							}
							else
							{
								MasterAudioMultiplayerAdapter.GlideSoundGroupByPitch(Trans, text4, 0f - aEvent.targetGlidePitch, aEvent.pitchGlideTime, null);
							}
						}
						else if (flag7)
						{
							MasterAudio.GlideSoundGroupByPitch(text4, 0f - aEvent.targetGlidePitch, aEvent.pitchGlideTime, () =>
							{
								MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
							});
						}
						else
						{
							MasterAudio.GlideSoundGroupByPitch(text4, 0f - aEvent.targetGlidePitch, aEvent.pitchGlideTime);
						}
						break;
					}
					break;
				}
				case MasterAudio.SoundGroupCommand.StopOldSoundGroupVoices:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopOldSoundGroupVoices(Trans, text4, aEvent.minAge);
					}
					else
					{
						MasterAudio.StopOldSoundGroupVoices(text4, aEvent.minAge);
					}
					break;
				case MasterAudio.SoundGroupCommand.FadeOutOldSoundGroupVoices:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.FadeOutOldSoundGroupVoices(Trans, text4, aEvent.minAge, aEvent.fadeTime);
					}
					else
					{
						MasterAudio.FadeOutOldSoundGroupVoices(text4, aEvent.minAge, aEvent.fadeTime);
					}
					break;
				}
				if (flag2)
				{
					break;
				}
			}
			break;
		}
		case MasterAudio.EventSoundFunctionType.BusControl:
		{
			playSoundResult = new PlaySoundResult
			{
				ActingVariation = null,
				SoundPlayed = true,
				SoundScheduled = false
			};
			string text = string.Empty;
			List<string> list5 = new List<string>();
			if (!aEvent.allSoundTypesForBusCmd)
			{
				list5.Add(aEvent.busName);
			}
			else
			{
				list5.AddRange(MasterAudio.RuntimeBusNames);
				if (flag2)
				{
					text = "[All]";
				}
			}
			for (int l = 0; l < list5.Count; l++)
			{
				string busName2 = list5[l];
				if (!string.IsNullOrEmpty(text))
				{
					busName2 = text;
				}
				switch (aEvent.currentBusCommand)
				{
				case MasterAudio.BusCommand.None:
					playSoundResult.SoundPlayed = false;
					break;
				case MasterAudio.BusCommand.FadeToVolume:
				{
					float num5 = (flag ? grp.sliderValue : aEvent.fadeVolume);
					bool flag4 = aEvent.fireCustomEventAfterFade && !string.IsNullOrEmpty(aEvent.theCustomEventName);
					if (flag2)
					{
						if (flag4)
						{
							MasterAudioMultiplayerAdapter.FadeBusToVolume(Trans, busName2, num5, aEvent.fadeTime, () =>
							{
								MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
							}, aEvent.stopAfterFade, aEvent.restoreVolumeAfterFade);
						}
						else
						{
							MasterAudioMultiplayerAdapter.FadeBusToVolume(Trans, busName2, num5, aEvent.fadeTime, null, aEvent.stopAfterFade, aEvent.restoreVolumeAfterFade);
						}
					}
					else if (flag4)
					{
						MasterAudio.FadeBusToVolume(busName2, num5, aEvent.fadeTime, () =>
						{
							MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
						}, aEvent.stopAfterFade, aEvent.restoreVolumeAfterFade);
					}
					else
					{
						MasterAudio.FadeBusToVolume(busName2, num5, aEvent.fadeTime, null, aEvent.stopAfterFade, aEvent.restoreVolumeAfterFade);
					}
					break;
				}
				case MasterAudio.BusCommand.GlideByPitch:
				{
					bool flag5 = !string.IsNullOrEmpty(aEvent.theCustomEventName);
					switch (aEvent.glidePitchType)
					{
					case GlidePitchType.RaisePitch:
						if (flag2)
						{
							if (flag5)
							{
								MasterAudioMultiplayerAdapter.GlideBusByPitch(Trans, busName2, aEvent.targetGlidePitch, aEvent.pitchGlideTime, () =>
								{
									MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
								});
							}
							else
							{
								MasterAudioMultiplayerAdapter.GlideBusByPitch(Trans, busName2, aEvent.targetGlidePitch, aEvent.pitchGlideTime, null);
							}
						}
						else if (flag5)
						{
							MasterAudio.GlideBusByPitch(busName2, aEvent.targetGlidePitch, aEvent.pitchGlideTime, () =>
							{
								MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
							});
						}
						else
						{
							MasterAudio.GlideBusByPitch(busName2, aEvent.targetGlidePitch, aEvent.pitchGlideTime);
						}
						break;
					case GlidePitchType.LowerPitch:
						if (flag2)
						{
							if (flag5)
							{
								MasterAudioMultiplayerAdapter.GlideBusByPitch(Trans, busName2, 0f - aEvent.targetGlidePitch, aEvent.pitchGlideTime, () =>
								{
									MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
								});
							}
							else
							{
								MasterAudioMultiplayerAdapter.GlideBusByPitch(Trans, busName2, 0f - aEvent.targetGlidePitch, aEvent.pitchGlideTime, null);
							}
						}
						else if (flag5)
						{
							MasterAudio.GlideBusByPitch(busName2, 0f - aEvent.targetGlidePitch, aEvent.pitchGlideTime, () =>
							{
								MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
							});
						}
						else
						{
							MasterAudio.GlideBusByPitch(busName2, 0f - aEvent.targetGlidePitch, aEvent.pitchGlideTime);
						}
						break;
					}
					break;
				}
				case MasterAudio.BusCommand.Pause:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.PauseBus(Trans, busName2);
					}
					else
					{
						MasterAudio.PauseBus(busName2);
					}
					break;
				case MasterAudio.BusCommand.Stop:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopBus(Trans, busName2);
					}
					else
					{
						MasterAudio.StopBus(busName2);
					}
					break;
				case MasterAudio.BusCommand.Unpause:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnpauseBus(Trans, busName2);
					}
					else
					{
						MasterAudio.UnpauseBus(busName2);
					}
					break;
				case MasterAudio.BusCommand.Mute:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.MuteBus(Trans, busName2);
					}
					else
					{
						MasterAudio.MuteBus(busName2);
					}
					break;
				case MasterAudio.BusCommand.Unmute:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnmuteBus(Trans, busName2);
					}
					else
					{
						MasterAudio.UnmuteBus(busName2);
					}
					break;
				case MasterAudio.BusCommand.ToggleMute:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.ToggleMuteBus(Trans, busName2);
					}
					else
					{
						MasterAudio.ToggleMuteBus(busName2);
					}
					break;
				case MasterAudio.BusCommand.Solo:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.SoloBus(Trans, busName2);
					}
					else
					{
						MasterAudio.SoloBus(busName2);
					}
					break;
				case MasterAudio.BusCommand.Unsolo:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnsoloBus(Trans, busName2);
					}
					else
					{
						MasterAudio.UnsoloBus(busName2);
					}
					break;
				case MasterAudio.BusCommand.ChangePitch:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.ChangeBusPitch(Trans, busName2, aEvent.pitch);
					}
					else
					{
						MasterAudio.ChangeBusPitch(busName2, aEvent.pitch);
					}
					break;
				case MasterAudio.BusCommand.PauseBusOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.PauseBusOfTransform(Trans, busName2);
					}
					else
					{
						MasterAudio.PauseBusOfTransform(Trans, aEvent.busName);
					}
					break;
				case MasterAudio.BusCommand.UnpauseBusOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.UnpauseBusOfTransform(Trans, busName2);
					}
					else
					{
						MasterAudio.UnpauseBusOfTransform(Trans, aEvent.busName);
					}
					break;
				case MasterAudio.BusCommand.StopBusOfTransform:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopBusOfTransform(Trans, busName2);
					}
					else
					{
						MasterAudio.StopBusOfTransform(Trans, aEvent.busName);
					}
					break;
				case MasterAudio.BusCommand.StopOldBusVoices:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.StopOldBusVoices(Trans, busName2, aEvent.minAge);
					}
					else
					{
						MasterAudio.StopOldBusVoices(busName2, aEvent.minAge);
					}
					break;
				case MasterAudio.BusCommand.FadeOutOldBusVoices:
					if (flag2)
					{
						MasterAudioMultiplayerAdapter.FadeOutOldBusVoices(Trans, busName2, aEvent.minAge, aEvent.fadeTime);
					}
					else
					{
						MasterAudio.FadeOutOldBusVoices(busName2, aEvent.minAge, aEvent.fadeTime);
					}
					break;
				}
				if (flag2)
				{
					break;
				}
			}
			break;
		}
		case MasterAudio.EventSoundFunctionType.CustomEventControl:
			if (eType == EventType.UserDefinedEvent)
			{
				Debug.LogError("Custom Event Receivers cannot fire events. Occured in Transform '" + base.name + "'.");
			}
			else if (aEvent.currentCustomEventCommand == MasterAudio.CustomEventCommand.FireEvent)
			{
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.FireCustomEvent(aEvent.theCustomEventName, Trans);
				}
				else
				{
					MasterAudio.FireCustomEvent(aEvent.theCustomEventName, Trans);
				}
			}
			break;
		case MasterAudio.EventSoundFunctionType.GlobalControl:
			switch (aEvent.currentGlobalCommand)
			{
			case MasterAudio.GlobalCommand.PauseAudioListener:
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.AudioListenerPause(Trans);
				}
				else
				{
					AudioListener.pause = true;
				}
				break;
			case MasterAudio.GlobalCommand.UnpauseAudioListener:
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.AudioListenerUnpause(Trans);
				}
				else
				{
					AudioListener.pause = false;
				}
				break;
			case MasterAudio.GlobalCommand.SetMasterMixerVolume:
			{
				float num2 = (flag ? grp.sliderValue : aEvent.volume);
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.SetMasterMixerVolume(Trans, num2);
				}
				else
				{
					MasterAudio.MasterVolumeLevel = num2;
				}
				break;
			}
			case MasterAudio.GlobalCommand.SetMasterPlaylistVolume:
			{
				float num3 = (flag ? grp.sliderValue : aEvent.volume);
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.SetPlaylistMasterVolume(Trans, num3);
				}
				else
				{
					MasterAudio.PlaylistMasterVolume = num3;
				}
				break;
			}
			case MasterAudio.GlobalCommand.PauseMixer:
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.PauseMixer(Trans);
				}
				else
				{
					MasterAudio.PauseMixer();
				}
				break;
			case MasterAudio.GlobalCommand.UnpauseMixer:
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.UnpauseMixer(Trans);
				}
				else
				{
					MasterAudio.UnpauseMixer();
				}
				break;
			case MasterAudio.GlobalCommand.StopMixer:
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.StopMixer(Trans);
				}
				else
				{
					MasterAudio.StopMixer();
				}
				break;
			case MasterAudio.GlobalCommand.MuteEverything:
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.MuteEverything(Trans);
				}
				else
				{
					MasterAudio.MuteEverything();
				}
				break;
			case MasterAudio.GlobalCommand.UnmuteEverything:
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.UnmuteEverything(Trans);
				}
				else
				{
					MasterAudio.UnmuteEverything();
				}
				break;
			case MasterAudio.GlobalCommand.PauseEverything:
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.PauseEverything(Trans);
				}
				else
				{
					MasterAudio.PauseEverything();
				}
				break;
			case MasterAudio.GlobalCommand.UnpauseEverything:
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.UnpauseEverything(Trans);
				}
				else
				{
					MasterAudio.UnpauseEverything();
				}
				break;
			case MasterAudio.GlobalCommand.StopEverything:
				if (flag2)
				{
					MasterAudioMultiplayerAdapter.StopEverything(Trans);
				}
				else
				{
					MasterAudio.StopEverything();
				}
				break;
			}
			break;
		case MasterAudio.EventSoundFunctionType.UnityMixerControl:
			switch (aEvent.currentMixerCommand)
			{
			case MasterAudio.UnityMixerCommand.TransitionToSnapshot:
			{
				AudioMixerSnapshot snapshotToTransitionTo = aEvent.snapshotToTransitionTo;
				if (snapshotToTransitionTo != null)
				{
					snapshotToTransitionTo.audioMixer.updateMode = MasterAudio.Instance.mixerUpdateMode;
					snapshotToTransitionTo.audioMixer.TransitionToSnapshots(new AudioMixerSnapshot[1] { snapshotToTransitionTo }, new float[1] { 1f }, aEvent.snapshotTransitionTime);
				}
				break;
			}
			case MasterAudio.UnityMixerCommand.TransitionToSnapshotBlend:
			{
				List<AudioMixerSnapshot> list3 = new List<AudioMixerSnapshot>();
				List<float> list4 = new List<float>();
				AudioMixer audioMixer = null;
				for (int k = 0; k < aEvent.snapshotsToBlend.Count; k++)
				{
					AudioEvent.MA_SnapshotInfo mA_SnapshotInfo = aEvent.snapshotsToBlend[k];
					if (!(mA_SnapshotInfo.snapshot == null))
					{
						if (audioMixer == null)
						{
							audioMixer = mA_SnapshotInfo.snapshot.audioMixer;
						}
						else if (audioMixer != mA_SnapshotInfo.snapshot.audioMixer)
						{
							Debug.LogError("Snapshot '" + mA_SnapshotInfo.snapshot.name + "' isn't in the same Audio Mixer as the previous snapshot in EventSounds on GameObject '" + base.name + "'. Please make sure all the Snapshots to blend are on the same mixer.");
							break;
						}
						list3.Add(mA_SnapshotInfo.snapshot);
						list4.Add(mA_SnapshotInfo.weight);
					}
				}
				if (list3.Count > 0)
				{
					audioMixer.updateMode = MasterAudio.Instance.mixerUpdateMode;
					audioMixer.TransitionToSnapshots(list3.ToArray(), list4.ToArray(), aEvent.snapshotTransitionTime);
				}
				break;
			}
			}
			break;
		case MasterAudio.EventSoundFunctionType.PersistentSettingsControl:
			switch (aEvent.currentPersistentSettingsCommand)
			{
			case MasterAudio.PersistentSettingsCommand.SetBusVolume:
			{
				List<string> list2 = new List<string>();
				if (!aEvent.allSoundTypesForBusCmd)
				{
					list2.Add(aEvent.busName);
				}
				else
				{
					list2.AddRange(MasterAudio.RuntimeBusNames);
				}
				for (int j = 0; j < list2.Count; j++)
				{
					string busName = list2[j];
					float vol2 = (flag ? grp.sliderValue : aEvent.volume);
					PersistentAudioSettings.SetBusVolume(busName, vol2);
				}
				break;
			}
			case MasterAudio.PersistentSettingsCommand.SetGroupVolume:
			{
				List<string> list = new List<string>();
				if (!aEvent.allSoundTypesForGroupCmd)
				{
					list.Add(aEvent.soundType);
				}
				else
				{
					list.AddRange(MasterAudio.RuntimeSoundGroupNames);
				}
				for (int i = 0; i < list.Count; i++)
				{
					string grpName = list[i];
					float vol = (flag ? grp.sliderValue : aEvent.volume);
					PersistentAudioSettings.SetGroupVolume(grpName, vol);
				}
				break;
			}
			case MasterAudio.PersistentSettingsCommand.SetMixerVolume:
				PersistentAudioSettings.MixerVolume = (flag ? grp.sliderValue : aEvent.volume);
				break;
			case MasterAudio.PersistentSettingsCommand.SetMusicVolume:
				PersistentAudioSettings.MusicVolume = (flag ? grp.sliderValue : aEvent.volume);
				break;
			case MasterAudio.PersistentSettingsCommand.MixerMuteToggle:
				if (PersistentAudioSettings.MixerMuted.HasValue)
				{
					PersistentAudioSettings.MixerMuted = !PersistentAudioSettings.MixerMuted.Value;
				}
				else
				{
					PersistentAudioSettings.MixerMuted = true;
				}
				break;
			case MasterAudio.PersistentSettingsCommand.MusicMuteToggle:
				if (PersistentAudioSettings.MusicMuted.HasValue)
				{
					PersistentAudioSettings.MusicMuted = !PersistentAudioSettings.MusicMuted.Value;
				}
				else
				{
					PersistentAudioSettings.MusicMuted = true;
				}
				break;
			}
			break;
		}
	}

	private void LogIfCustomEventMissing(AudioEventGroup eventGroup)
	{
		if (!logMissingEvents || (eventGroup.isCustomEvent && (!eventGroup.customSoundActive || string.IsNullOrEmpty(eventGroup.customEventName))))
		{
			return;
		}
		for (int i = 0; i < eventGroup.SoundEvents.Count; i++)
		{
			AudioEvent audioEvent = eventGroup.SoundEvents[i];
			if (audioEvent.currentSoundFunctionType == MasterAudio.EventSoundFunctionType.CustomEventControl)
			{
				string theCustomEventName = audioEvent.theCustomEventName;
				if (!MasterAudio.CustomEventExists(theCustomEventName))
				{
					MasterAudio.LogWarning("Transform '" + base.name + "' is set up to receive or fire Custom Event '" + theCustomEventName + "', which does not exist in Master Audio.");
				}
			}
		}
	}

	public void CheckForIllegalCustomEvents()
	{
		if (useStartSound)
		{
			LogIfCustomEventMissing(startSound);
		}
		if (useVisibleSound)
		{
			LogIfCustomEventMissing(visibleSound);
		}
		if (useInvisibleSound)
		{
			LogIfCustomEventMissing(invisibleSound);
		}
		if (useCollisionSound)
		{
			LogIfCustomEventMissing(collisionSound);
		}
		if (useCollisionExitSound)
		{
			LogIfCustomEventMissing(collisionExitSound);
		}
		if (useTriggerEnterSound)
		{
			LogIfCustomEventMissing(triggerSound);
		}
		if (useTriggerExitSound)
		{
			LogIfCustomEventMissing(triggerExitSound);
		}
		if (useMouseEnterSound)
		{
			LogIfCustomEventMissing(mouseEnterSound);
		}
		if (useMouseExitSound)
		{
			LogIfCustomEventMissing(mouseExitSound);
		}
		if (useMouseClickSound)
		{
			LogIfCustomEventMissing(mouseClickSound);
		}
		if (useMouseDragSound)
		{
			LogIfCustomEventMissing(mouseDragSound);
		}
		if (useMouseUpSound)
		{
			LogIfCustomEventMissing(mouseUpSound);
		}
		if (useNguiMouseDownSound)
		{
			LogIfCustomEventMissing(nguiMouseDownSound);
		}
		if (useNguiMouseUpSound)
		{
			LogIfCustomEventMissing(nguiMouseUpSound);
		}
		if (useNguiOnClickSound)
		{
			LogIfCustomEventMissing(nguiOnClickSound);
		}
		if (useNguiMouseEnterSound)
		{
			LogIfCustomEventMissing(nguiMouseEnterSound);
		}
		if (useNguiMouseExitSound)
		{
			LogIfCustomEventMissing(nguiMouseExitSound);
		}
		if (useSpawnedSound)
		{
			LogIfCustomEventMissing(spawnedSound);
		}
		if (useDespawnedSound)
		{
			LogIfCustomEventMissing(despawnedSound);
		}
		if (useEnableSound)
		{
			LogIfCustomEventMissing(enableSound);
		}
		if (useDisableSound)
		{
			LogIfCustomEventMissing(disableSound);
		}
		if (useCollision2dSound)
		{
			LogIfCustomEventMissing(collision2dSound);
		}
		if (useCollisionExit2dSound)
		{
			LogIfCustomEventMissing(collisionExit2dSound);
		}
		if (useTriggerEnter2dSound)
		{
			LogIfCustomEventMissing(triggerEnter2dSound);
		}
		if (useTriggerExit2dSound)
		{
			LogIfCustomEventMissing(triggerExit2dSound);
		}
		if (useParticleCollisionSound)
		{
			LogIfCustomEventMissing(particleCollisionSound);
		}
		if (useUnitySliderChangedSound)
		{
			LogIfCustomEventMissing(unitySliderChangedSound);
		}
		if (useUnityButtonClickedSound)
		{
			LogIfCustomEventMissing(unityButtonClickedSound);
		}
		if (useUnityPointerDownSound)
		{
			LogIfCustomEventMissing(unityPointerDownSound);
		}
		if (useUnityDragSound)
		{
			LogIfCustomEventMissing(unityDragSound);
		}
		if (useUnityDropSound)
		{
			LogIfCustomEventMissing(unityDropSound);
		}
		if (useUnityPointerUpSound)
		{
			LogIfCustomEventMissing(unityPointerUpSound);
		}
		if (useUnityPointerEnterSound)
		{
			LogIfCustomEventMissing(unityPointerEnterSound);
		}
		if (useUnityPointerExitSound)
		{
			LogIfCustomEventMissing(unityPointerExitSound);
		}
		if (useUnityScrollSound)
		{
			LogIfCustomEventMissing(unityScrollSound);
		}
		if (useUnityUpdateSelectedSound)
		{
			LogIfCustomEventMissing(unityUpdateSelectedSound);
		}
		if (useUnitySelectSound)
		{
			LogIfCustomEventMissing(unitySelectSound);
		}
		if (useUnityDeselectSound)
		{
			LogIfCustomEventMissing(unityDeselectSound);
		}
		if (useUnityMoveSound)
		{
			LogIfCustomEventMissing(unityMoveSound);
		}
		if (useUnityInitializePotentialDragSound)
		{
			LogIfCustomEventMissing(unityInitializePotentialDragSound);
		}
		if (useUnityBeginDragSound)
		{
			LogIfCustomEventMissing(unityBeginDragSound);
		}
		if (useUnityEndDragSound)
		{
			LogIfCustomEventMissing(unityEndDragSound);
		}
		if (useUnitySubmitSound)
		{
			LogIfCustomEventMissing(unitySubmitSound);
		}
		if (useUnityCancelSound)
		{
			LogIfCustomEventMissing(unityCancelSound);
		}
		if (useCodeTriggeredEvent1Sound)
		{
			LogIfCustomEventMissing(codeTriggeredEvent1Sound);
		}
		if (useCodeTriggeredEvent2Sound)
		{
			LogIfCustomEventMissing(codeTriggeredEvent2Sound);
		}
		for (int i = 0; i < userDefinedSounds.Count; i++)
		{
			AudioEventGroup eventGroup = userDefinedSounds[i];
			LogIfCustomEventMissing(eventGroup);
		}
	}

	public void ReceiveEvent(string customEventName, Vector3 originPoint)
	{
		for (int i = 0; i < userDefinedSounds.Count; i++)
		{
			AudioEventGroup audioEventGroup = userDefinedSounds[i];
			if (audioEventGroup.customSoundActive && !string.IsNullOrEmpty(audioEventGroup.customEventName) && audioEventGroup.customEventName.Equals(customEventName))
			{
				PlaySounds(audioEventGroup, EventType.UserDefinedEvent);
			}
		}
	}

	public bool SubscribesToEvent(string customEventName)
	{
		for (int i = 0; i < userDefinedSounds.Count; i++)
		{
			AudioEventGroup audioEventGroup = userDefinedSounds[i];
			if (audioEventGroup.customSoundActive && !string.IsNullOrEmpty(audioEventGroup.customEventName) && audioEventGroup.customEventName.Equals(customEventName))
			{
				return true;
			}
		}
		return false;
	}

	public void RegisterReceiver()
	{
		if (userDefinedSounds.Count > 0)
		{
			MasterAudio.AddCustomEventReceiver(this, _trans);
		}
	}

	public void UnregisterReceiver()
	{
		if (userDefinedSounds.Count > 0)
		{
			MasterAudio.RemoveCustomEventReceiver(this);
		}
	}

	public IList<AudioEventGroup> GetAllEvents()
	{
		return userDefinedSounds.AsReadOnly();
	}

	private void AddUGUIComponents()
	{
		AddUGUIHandler<EventSoundsPointerEnterHandler>(useUnityPointerEnterSound);
		AddUGUIHandler<EventSoundsPointerExitHandler>(useUnityPointerExitSound);
		AddUGUIHandler<EventSoundsPointerDownHandler>(useUnityPointerDownSound);
		AddUGUIHandler<EventSoundsPointerUpHandler>(useUnityPointerUpSound);
		AddUGUIHandler<EventSoundsDragHandler>(useUnityDragSound);
		AddUGUIHandler<EventSoundsDropHandler>(useUnityDropSound);
		AddUGUIHandler<EventSoundsScrollHandler>(useUnityScrollSound);
		AddUGUIHandler<EventSoundsUpdateSelectedHandler>(useUnityUpdateSelectedSound);
		AddUGUIHandler<EventSoundsSelectHandler>(useUnitySelectSound);
		AddUGUIHandler<EventSoundsDeselectHandler>(useUnityDeselectSound);
		AddUGUIHandler<EventSoundsMoveHandler>(useUnityMoveSound);
		AddUGUIHandler<EventSoundsInitializePotentialDragHandler>(useUnityInitializePotentialDragSound);
		AddUGUIHandler<EventSoundsBeginDragHandler>(useUnityBeginDragSound);
		AddUGUIHandler<EventSoundsEndDragHandler>(useUnityEndDragSound);
		AddUGUIHandler<EventSoundsSubmitHandler>(useUnitySubmitSound);
		AddUGUIHandler<EventSoundsCancelHandler>(useUnityCancelSound);
	}

	private void AddUGUIHandler<T>(bool useSound) where T : EventSoundsUGUIHandler
	{
		if (useSound)
		{
			base.gameObject.AddComponent<T>().eventSounds = this;
		}
	}
}
