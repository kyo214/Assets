using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.UIMenu;

[RequireComponent(typeof(RectTransform))]
[AddComponentMenu("UIMenu/Editor Only/UIMenu Camera")]
public class UIMenuCamera : MonoBehaviour
{
	public enum State
	{
		Idle = 0,
		Running = 1,
		Processing = 2
	}

	public readonly struct SnapshotData
	{
		public string fileName { get; }

		public string path { get; }

		public byte[] bytes { get; }

		public SnapshotData(string fileName, string path, byte[] bytes)
		{
			this.bytes = bytes;
			this.fileName = fileName;
			this.path = PathUtils.CleanPath(path);
		}

		public void CreateSnapshot()
		{
			File.WriteAllBytes(path, bytes);
		}
	}

	private const float k_FPSFrequencyModifier = 1.001f;

	private const int k_MinFPS = 1;

	public RectTransform SnapshotTarget;

	public Camera SnapshotCamera;

	public string SnapshotsFolderName = "_Snapshots";

	public string TargetPath = string.Empty;

	public bool AutoDeleteFilesFromTargetPath;

	public int MultiShotFPS = 24;

	public bool GenerateSpriteSheet = true;

	public float CustomMultiShotDuration = 2f;

	public UIContainer TargetUIContainer;

	public float UIContainerShowDelay = 1f;

	public UIToggle TargetUIToggle;

	public float UIToggleAnimationDuration = 1f;

	public UISelectable TargetUISelectable;

	public float UISelectableStateDuration = 1.5f;

	public ReactorController TargetReactorController;

	public ReactorAnimator TargetAnimator;

	public Progressor TargetProgressor;

	[SerializeField]
	private State CurrentState;

	public UnityAction<State> OnStateChanged;

	public bool AutoGenerateSnapshots = true;

	public bool AutoOverrideSettings = true;

	public UnityAction<string, string, List<SnapshotData>> OnSnapshot;

	private bool m_MultiShotStarted;

	private int m_Width;

	private int m_Height;

	private double m_ElapsedTime;

	private double m_LastTickTime;

	private List<SnapshotData> m_SnapshotData;

	private InputAction m_Space;

	public State currentState
	{
		get
		{
			return CurrentState;
		}
		set
		{
			CurrentState = value;
			OnStateChanged?.Invoke(value);
		}
	}

	public string defaultTargetPath => PathUtils.CleanPath(Path.Combine("Assets", SnapshotsFolderName));

	private bool initialized { get; set; }

	private RenderTexture camRenderTexture { get; set; }

	private Rect target { get; set; }

	private Canvas rootCanvas { get; set; }

	private CanvasScaler canvasScaler { get; set; }

	private float tickInterval { get; set; }

	private float timeSinceStartup => Time.realtimeSinceStartup;

	public List<SnapshotData> snapshotData => m_SnapshotData ?? (m_SnapshotData = new List<SnapshotData>());

	private static bool canRun => Application.isPlaying;

	private CameraClearFlags snapshotCameraClearFlags { get; set; }

	private Color snapshotCameraBackgroundColor { get; set; }

	private RenderMode rootCanvasRenderMode { get; set; }

	private Camera rootCanvasWorldCamera { get; set; }

	private CanvasScaler.ScaleMode canvasScalerUiScaleMode { get; set; }

	private float canvasScalerScaleFactor { get; set; }

	private static float GetTickInterval(int fps)
	{
		return 1f / ((float)Mathf.Max(1, fps) * 1.001f);
	}

	private void Reset()
	{
		ResetTargetPath();
	}

	public void FindTarget()
	{
		SnapshotTarget = (SnapshotTarget ? SnapshotTarget : GetComponent<RectTransform>());
		if (SnapshotCamera != null)
		{
			return;
		}
		Canvas componentInParent = GetComponentInParent<Canvas>();
		if (componentInParent == null)
		{
			Debug.LogError("UIMenuCamera: No Canvas found in the hierarchy");
			return;
		}
		if (!componentInParent.isRootCanvas)
		{
			componentInParent = componentInParent.rootCanvas;
		}
		if (componentInParent == null)
		{
			Debug.LogError("UIMenuCamera: No ROOT Canvas found in the hierarchy");
			return;
		}
		Camera camera = componentInParent.worldCamera;
		if (camera == null)
		{
			camera = Camera.main;
			if (camera == null)
			{
				Debug.LogError("UIMenuCamera: No Camera found in the hierarchy");
				return;
			}
		}
		SnapshotCamera = camera;
	}

	private void Initialize()
	{
		if (!initialized)
		{
			target = SnapshotTarget.rect;
			rootCanvas = SnapshotTarget.GetComponentInParent<Canvas>().rootCanvas;
			canvasScaler = SnapshotTarget.GetComponentInParent<CanvasScaler>();
			initialized = true;
			currentState = State.Idle;
		}
	}

	private void Awake()
	{
		if (canRun)
		{
			FindTarget();
			Initialize();
			m_Space = new InputAction("Space", InputActionType.Button, "<Keyboard>/space");
			m_Space.performed += (InputAction.CallbackContext _) =>
			{
				StartStopMultiShot();
			};
		}
	}

	private void OnEnable()
	{
		if (canRun)
		{
			m_Space.Enable();
		}
	}

	private void OnDisable()
	{
		if (canRun)
		{
			m_Space.Dispose();
		}
	}

	private void Update()
	{
		if (Application.isPlaying && m_MultiShotStarted)
		{
			m_ElapsedTime += (double)timeSinceStartup - m_LastTickTime;
			m_LastTickTime = timeSinceStartup;
			if (!(m_ElapsedTime < (double)tickInterval))
			{
				m_ElapsedTime = 0.0;
				TakeSnapshot(singleShot: false);
			}
		}
	}

	private void StartStopMultiShot()
	{
		if (m_MultiShotStarted)
		{
			StopMultiShot();
		}
		else
		{
			StartMultiShot();
		}
	}

	public void StartMultiShot()
	{
		if (!m_MultiShotStarted)
		{
			if (AutoOverrideSettings)
			{
				OverrideSettings();
			}
			ResetTime();
			snapshotData.Clear();
			m_MultiShotStarted = true;
			currentState = State.Running;
		}
	}

	public void StopMultiShot()
	{
		if (m_MultiShotStarted)
		{
			if (AutoOverrideSettings)
			{
				RestoreSettings();
			}
			m_MultiShotStarted = false;
			OnSnapshot?.Invoke(SnapshotTarget.name, TargetPath, snapshotData);
			if (AutoGenerateSnapshots)
			{
				GenerateSnapshots(SnapshotTarget.name, TargetPath, snapshotData);
			}
		}
	}

	public void CancelMultiShot()
	{
		if (m_MultiShotStarted)
		{
			m_MultiShotStarted = false;
			snapshotData.Clear();
			currentState = State.Idle;
		}
	}

	public void TakeSnapshot(bool singleShot = true)
	{
		if (CanRun())
		{
			if (singleShot)
			{
				CancelMultiShot();
				snapshotData.Clear();
				currentState = State.Idle;
				currentState = State.Running;
			}
			Run(singleShot);
		}
	}

	public void CustomMultiShot()
	{
		StopAllCoroutines();
		StartCoroutine(MultiShotForCustomDuration());
	}

	private IEnumerator MultiShotForCustomDuration()
	{
		yield return new WaitForSecondsRealtime(tickInterval);
		StartMultiShot();
		yield return new WaitForSecondsRealtime(tickInterval);
		yield return new WaitForSeconds(CustomMultiShotDuration);
		yield return new WaitForSecondsRealtime(tickInterval);
		StopMultiShot();
	}

	public void UIContainerMultiShot()
	{
		if (!(TargetUIContainer == null))
		{
			StopAllCoroutines();
			StartCoroutine(MultiShotUIContainer());
		}
	}

	private IEnumerator MultiShotUIContainer()
	{
		TargetUIContainer.InstantShow();
		StartMultiShot();
		yield return new WaitForSecondsRealtime(tickInterval);
		TargetUIContainer.Hide();
		yield return new WaitForSecondsRealtime(TargetUIContainer.totalDurationForHide);
		yield return new WaitForSecondsRealtime(UIContainerShowDelay);
		yield return new WaitForSecondsRealtime(tickInterval);
		TargetUIContainer.Show();
		yield return new WaitForSecondsRealtime(TargetUIContainer.totalDurationForShow);
		yield return new WaitForSecondsRealtime(tickInterval);
		StopMultiShot();
	}

	public void UIToggleMultiShot()
	{
		if (!(TargetUIToggle == null))
		{
			StopAllCoroutines();
			StartCoroutine(MultiShotUIToggle());
		}
	}

	private IEnumerator MultiShotUIToggle()
	{
		TargetUIToggle.SetIsOn(newValue: false, animateChange: false);
		yield return new WaitForSecondsRealtime(0.5f);
		StartMultiShot();
		yield return new WaitForSecondsRealtime(tickInterval);
		TargetUIToggle.isOn = true;
		yield return new WaitForSecondsRealtime(UIToggleAnimationDuration);
		yield return new WaitForSecondsRealtime(tickInterval);
		TargetUIToggle.isOn = false;
		yield return new WaitForSecondsRealtime(UIToggleAnimationDuration);
		yield return new WaitForSecondsRealtime(tickInterval);
		StopMultiShot();
	}

	public void UISelectableMultiShot()
	{
		if (!(TargetUISelectable == null))
		{
			StopAllCoroutines();
			StartCoroutine(MultiShotUISelectable());
		}
	}

	private IEnumerator MultiShotUISelectable()
	{
		if (TargetUISelectable.isToggle)
		{
			TargetUISelectable.isOn = true;
			yield return new WaitForSecondsRealtime(UISelectableStateDuration);
		}
		yield return new WaitForSecondsRealtime(0.5f);
		StartMultiShot();
		yield return new WaitForSecondsRealtime(0.1f);
		TargetUISelectable.SetState(UISelectionState.Highlighted);
		yield return new WaitForSecondsRealtime(UISelectableStateDuration * 0.25f);
		TargetUISelectable.SetState(UISelectionState.Pressed);
		if (TargetUISelectable.isToggle)
		{
			TargetUISelectable.isOn = false;
		}
		yield return new WaitForSecondsRealtime(UISelectableStateDuration * 0.25f);
		TargetUISelectable.SetState(UISelectionState.Normal);
		yield return new WaitForSecondsRealtime(UISelectableStateDuration * 0.5f);
		if (TargetUISelectable.isToggle)
		{
			TargetUISelectable.SetState(UISelectionState.Pressed);
			TargetUISelectable.isOn = true;
			yield return new WaitForSecondsRealtime(UISelectableStateDuration * 0.5f);
		}
		TargetUISelectable.SetState(UISelectionState.Disabled);
		yield return new WaitForSecondsRealtime(UISelectableStateDuration * 0.25f);
		TargetUISelectable.SetState(UISelectionState.Normal);
		yield return new WaitForSecondsRealtime(0.1f);
		StopMultiShot();
	}

	public void ReactorControllerMultiShot()
	{
		if (!(TargetReactorController == null))
		{
			StopAllCoroutines();
			StartCoroutine(MultiShotReactorController());
		}
	}

	private IEnumerator MultiShotReactorController()
	{
		StartMultiShot();
		yield return new WaitForSecondsRealtime(tickInterval * 3f);
		TargetReactorController.Play();
		yield return null;
		yield return new WaitForSecondsRealtime(TargetReactorController.GetTotalDuration());
		yield return new WaitForSecondsRealtime(tickInterval * 3f);
		StopMultiShot();
	}

	public void AnimatorMultiShot()
	{
		if (!(TargetAnimator == null))
		{
			StopAllCoroutines();
			StartCoroutine(MultiShotAnimator());
		}
	}

	private IEnumerator MultiShotAnimator()
	{
		StartMultiShot();
		yield return new WaitForSecondsRealtime(tickInterval * 3f);
		TargetAnimator.Play();
		yield return null;
		yield return new WaitForSecondsRealtime(TargetAnimator.GetTotalDuration());
		yield return new WaitForSecondsRealtime(tickInterval * 3f);
		StopMultiShot();
	}

	public void ProgressorMultiShot()
	{
		if (!(TargetProgressor == null))
		{
			StopAllCoroutines();
			StartCoroutine(MultiShotTargetProgressor());
		}
	}

	private IEnumerator MultiShotTargetProgressor()
	{
		StartMultiShot();
		yield return new WaitForSecondsRealtime(0.1f);
		TargetProgressor.Play();
		yield return null;
		yield return new WaitForSecondsRealtime(TargetProgressor.GetTotalDuration());
		yield return new WaitForSecondsRealtime(0.1f);
		StopMultiShot();
	}

	private void ResetTime()
	{
		tickInterval = GetTickInterval(MultiShotFPS);
		m_ElapsedTime = 0.0;
		m_LastTickTime = timeSinceStartup;
	}

	public void ResetTargetPath()
	{
		TargetPath = defaultTargetPath;
	}

	private bool CanRun()
	{
		return false;
	}

	private UIMenuCamera OverrideSnapshotCameraSettings()
	{
		snapshotCameraClearFlags = SnapshotCamera.clearFlags;
		snapshotCameraBackgroundColor = SnapshotCamera.backgroundColor;
		SnapshotCamera.clearFlags = CameraClearFlags.Color;
		SnapshotCamera.backgroundColor = Color.clear;
		return this;
	}

	private UIMenuCamera RestoreSnapshotCameraSettings()
	{
		SnapshotCamera.clearFlags = snapshotCameraClearFlags;
		SnapshotCamera.backgroundColor = snapshotCameraBackgroundColor;
		return this;
	}

	private UIMenuCamera OverrideRootCanvasSettings()
	{
		rootCanvasRenderMode = rootCanvas.renderMode;
		rootCanvasWorldCamera = rootCanvas.worldCamera;
		rootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		rootCanvas.worldCamera = SnapshotCamera;
		return this;
	}

	private UIMenuCamera RestoreRootCanvasSettings()
	{
		rootCanvas.renderMode = rootCanvasRenderMode;
		rootCanvas.worldCamera = rootCanvasWorldCamera;
		return this;
	}

	private UIMenuCamera OverrideCanvasScalerSettings()
	{
		if (canvasScaler == null)
		{
			return this;
		}
		canvasScalerUiScaleMode = canvasScaler.uiScaleMode;
		canvasScalerScaleFactor = canvasScaler.scaleFactor;
		canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
		canvasScaler.scaleFactor = 1f;
		return this;
	}

	private UIMenuCamera RestoreCanvasScalerSettings()
	{
		if (canvasScaler == null)
		{
			return this;
		}
		canvasScaler.uiScaleMode = canvasScalerUiScaleMode;
		canvasScaler.scaleFactor = canvasScalerScaleFactor;
		return this;
	}

	public void OverrideSettings()
	{
		OverrideSnapshotCameraSettings();
		OverrideRootCanvasSettings();
		OverrideCanvasScalerSettings();
	}

	public void RestoreSettings()
	{
		RestoreSnapshotCameraSettings();
		RestoreRootCanvasSettings();
		RestoreCanvasScalerSettings();
	}

	private void Run(bool singleShot = true)
	{
		target = SnapshotTarget.rect;
		if (singleShot)
		{
			OverrideSettings();
		}
		m_Width = Convert.ToInt32(target.width);
		m_Height = Convert.ToInt32(target.height);
		Texture2D texture2D = new Texture2D(m_Width, m_Height, TextureFormat.ARGB32, mipChain: false);
		RenderTexture renderTexture = new RenderTexture(m_Width, m_Height, 24);
		camRenderTexture = SnapshotCamera.targetTexture;
		SnapshotCamera.targetTexture = renderTexture;
		SnapshotCamera.Render();
		SnapshotCamera.targetTexture = camRenderTexture;
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, m_Width, m_Height), 0, 0);
		texture2D.Apply();
		if (singleShot)
		{
			RestoreSettings();
		}
		string text = SnapshotTarget.name;
		text = (singleShot ? text : $"{text}/{text}_{snapshotData.Count:000}");
		string path = PathUtils.ToAbsolutePath(Path.Combine(TargetPath, text + ".png"));
		snapshotData.Add(new SnapshotData(SnapshotTarget.name, path, texture2D.EncodeToPNG()));
		RenderTexture.active = null;
		UnityEngine.Object.Destroy(renderTexture);
		UnityEngine.Object.Destroy(texture2D);
		if (singleShot)
		{
			OnSnapshot?.Invoke(SnapshotTarget.name, TargetPath, snapshotData);
			if (AutoGenerateSnapshots)
			{
				GenerateSnapshots(SnapshotTarget.name, TargetPath, snapshotData);
			}
		}
	}

	public void GenerateSnapshots(string fileName, string targetPath, List<SnapshotData> snapshot)
	{
	}

	public static void SetTextureSettingsToGUI(string filePath)
	{
	}
}
