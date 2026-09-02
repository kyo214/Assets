using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Tools;

[AddComponentMenu("")]
[ExecuteAlways]
public abstract class MMCinemachineZone : MonoBehaviour
{
	public enum Modes
	{
		Enable = 0,
		Priority = 1
	}

	[Header("Virtual Camera")]
	[Tooltip("whether to enable/disable virtual cameras, or to play on their priority for transitions")]
	public Modes Mode;

	[Tooltip("whether or not the camera in this zone should start active")]
	public bool CameraStartsActive;

	[Tooltip("the virtual camera associated to this zone (will try to grab one in children if none is set)")]
	public CinemachineVirtualCamera VirtualCamera;

	[Tooltip("when in priority mode, the priority this camera should have when the zone is active")]
	[MMEnumCondition("Mode", new int[] { 1 })]
	public int EnabledPriority = 10;

	[Tooltip("when in priority mode, the priority this camera should have when the zone is inactive")]
	[MMEnumCondition("Mode", new int[] { 1 })]
	public int DisabledPriority;

	[Header("Collisions")]
	[Tooltip("a layermask containing all the layers that should activate this zone")]
	public LayerMask TriggerMask;

	[Header("Confiner Setup")]
	[Tooltip("whether or not the zone should auto setup its camera's confiner on start - alternative is to manually click the ManualSetupConfiner, or do your own setup")]
	public bool SetupConfinerOnStart;

	[MMInspectorButton("ManualSetupConfiner")]
	public bool GenerateConfinerSetup;

	[Header("Events")]
	[Tooltip("a UnityEvent to trigger when entering the zone")]
	public UnityEvent OnEnterZoneEvent;

	[Tooltip("a UnityEvent to trigger when exiting the zone")]
	public UnityEvent OnExitZoneEvent;

	[Header("Debug")]
	[Tooltip("whether or not to draw shape gizmos to help visualize the zone's bounds")]
	public bool DrawGizmos = true;

	[Tooltip("the color of the gizmos to draw in edit mode")]
	public Color GizmosColor;

	protected GameObject _confinerGameObject;

	protected Vector3 _gizmoSize;

	protected virtual void Awake()
	{
		AlwaysInitialization();
		if (Application.isPlaying)
		{
			Initialization();
		}
	}

	protected virtual void AlwaysInitialization()
	{
		InitializeCollider();
	}

	protected virtual void Initialization()
	{
		if (VirtualCamera == null)
		{
			VirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
		}
		if (VirtualCamera == null)
		{
			Debug.LogWarning("[MMCinemachineZone2D] " + base.name + " : no virtual camera is attached to this zone. Set one in its inspector.");
		}
		SetupConfinerGameObject();
	}

	protected virtual void Start()
	{
		if (Application.isPlaying)
		{
			SetupConfiner();
			StartCoroutine(EnableCamera(CameraStartsActive, 1));
		}
	}

	protected abstract void InitializeCollider();

	protected abstract void SetupConfiner();

	protected virtual void ManualSetupConfiner()
	{
		Initialization();
		SetupConfiner();
	}

	protected virtual void SetupConfinerGameObject()
	{
		Transform transform = base.transform.Find("Confiner");
		if (transform != null)
		{
			Object.DestroyImmediate(transform.gameObject);
		}
		_confinerGameObject = new GameObject();
		_confinerGameObject.transform.localPosition = Vector3.zero;
		_confinerGameObject.transform.SetParent(base.transform);
		_confinerGameObject.name = "Confiner";
	}

	protected virtual IEnumerator EnableCamera(bool state, int frames)
	{
		if (!(VirtualCamera == null))
		{
			if (frames > 0)
			{
				yield return MMCoroutine.WaitForFrames(frames);
			}
			if (Mode == Modes.Enable)
			{
				VirtualCamera.enabled = state;
			}
			else if (Mode == Modes.Priority)
			{
				VirtualCamera.Priority = (state ? EnabledPriority : DisabledPriority);
			}
		}
	}

	protected virtual void Reset()
	{
		GizmosColor = MMColors.RandomColor();
		GizmosColor.a = 0.2f;
	}
}
