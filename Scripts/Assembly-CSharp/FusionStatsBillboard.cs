using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Olive)]
[ExecuteAlways]
public class FusionStatsBillboard : Fusion.Behaviour
{
	[InlineHelp]
	public Camera Camera;

	private static float _lastCameraFindTime;

	private static Camera _currentCam;

	private FusionStats _fusionStats;

	private Camera MainCamera
	{
		get
		{
			float time = Time.time;
			if (time == _lastCameraFindTime)
			{
				return _currentCam;
			}
			_lastCameraFindTime = time;
			return _currentCam = Camera.main;
		}
		set
		{
			_currentCam = value;
		}
	}

	private void Awake()
	{
		_fusionStats = GetComponent<FusionStats>();
	}

	private void OnEnable()
	{
		UpdateLookAt();
	}

	private void OnDisable()
	{
		base.transform.localRotation = default;
	}

	private void LateUpdate()
	{
		UpdateLookAt();
	}

	public void UpdateLookAt()
	{
		if (!_fusionStats || _fusionStats.CanvasType != FusionStats.StatCanvasTypes.Overlay)
		{
			Camera camera = (Camera ? Camera : MainCamera);
			if ((bool)camera && base.enabled)
			{
				base.transform.rotation = camera.transform.rotation;
			}
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ResetStatics()
	{
		_currentCam = null;
		_lastCameraFindTime = 0f;
	}
}
