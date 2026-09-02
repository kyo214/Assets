using System;
using System.Collections;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Orientation;

[RequireComponent(typeof(RectTransform), typeof(Canvas))]
[DisallowMultipleComponent]
public class OrientationDetector : SingletonBehaviour<OrientationDetector>
{
	[ClearOnReload]
	private static SignalStream s_stream;

	private RectTransform m_RectTransform;

	private Canvas m_Canvas;

	public DetectedOrientationEvent OnOrientationChanged = new DetectedOrientationEvent();

	public ModyEvent OnAnyOrientation = new ModyEvent();

	public ModyEvent OnPortraitOrientation = new ModyEvent();

	public ModyEvent OnLandscapeOrientation = new ModyEvent();

	[SerializeField]
	private DetectedOrientation CurrentOrientation;

	private static string streamCategory => "Orientation";

	private static string streamName => "OrientationDetector";

	public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream(streamCategory, streamName));

	public RectTransform rectTransform
	{
		get
		{
			if (!m_RectTransform)
			{
				return m_RectTransform = GetComponent<RectTransform>();
			}
			return m_RectTransform;
		}
	}

	public Canvas canvas
	{
		get
		{
			if (!m_Canvas)
			{
				return m_Canvas = GetComponent<Canvas>();
			}
			return m_Canvas;
		}
	}

	public DetectedOrientation currentOrientation
	{
		get
		{
			return CurrentOrientation;
		}
		private set
		{
			CurrentOrientation = value;
		}
	}

	public ScreenOrientation previousScreenOrientation { get; private set; }

	private int orientationCheckCount { get; set; }

	private Coroutine orientationCheckCoroutine { get; set; }

	private float checkInterval { get; set; } = 0.1f;

	private bool firstOrientationCheck { get; set; } = true;

	public void Initialize()
	{
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		if (!canvas.isRootCanvas)
		{
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
		}
	}

	private void Reset()
	{
		Initialize();
	}

	protected override void Awake()
	{
		firstOrientationCheck = true;
		base.Awake();
		currentOrientation = DetectedOrientation.Unknown;
		Initialize();
	}

	private IEnumerator Start()
	{
		yield return null;
		yield return new WaitForEndOfFrame();
		CheckDeviceOrientation();
	}

	private void OnEnable()
	{
		StartCheckOrientation();
	}

	private void OnDisable()
	{
		StopCheckOrientation();
	}

	private void StartCheckOrientation()
	{
		if (orientationCheckCoroutine == null)
		{
			orientationCheckCoroutine = StartCoroutine(CheckOrientation());
		}
	}

	private void StopCheckOrientation()
	{
		if (orientationCheckCoroutine != null)
		{
			StopCoroutine(orientationCheckCoroutine);
			orientationCheckCoroutine = null;
		}
	}

	private IEnumerator CheckOrientation()
	{
		WaitForSecondsRealtime wait = new WaitForSecondsRealtime(checkInterval);
		yield return new WaitForEndOfFrame();
		while (true)
		{
			yield return wait;
			CheckDeviceOrientation();
		}
	}

	private void OnRectTransformDimensionsChange()
	{
		orientationCheckCount++;
		if (orientationCheckCount >= 2)
		{
			orientationCheckCount = 0;
			CheckDeviceOrientation();
		}
	}

	public void CheckDeviceOrientation()
	{
		CheckDeviceOrientation(forceUpdate: false);
	}

	public void CheckDeviceOrientation(bool forceUpdate)
	{
		if (!firstOrientationCheck && previousScreenOrientation == Screen.orientation && !forceUpdate)
		{
			return;
		}
		firstOrientationCheck = false;
		switch (Screen.orientation)
		{
		case ScreenOrientation.Portrait:
			if (currentOrientation != DetectedOrientation.Portrait || forceUpdate)
			{
				UpdateOrientation(DetectedOrientation.Portrait);
			}
			break;
		case ScreenOrientation.PortraitUpsideDown:
			if (currentOrientation != DetectedOrientation.Portrait || forceUpdate)
			{
				UpdateOrientation(DetectedOrientation.Portrait);
			}
			break;
		case ScreenOrientation.LandscapeLeft:
			if (currentOrientation != DetectedOrientation.Landscape || forceUpdate)
			{
				UpdateOrientation(DetectedOrientation.Landscape);
			}
			break;
		case ScreenOrientation.LandscapeRight:
			if (currentOrientation != DetectedOrientation.Landscape || forceUpdate)
			{
				UpdateOrientation(DetectedOrientation.Landscape);
			}
			break;
		case ScreenOrientation.AutoRotation:
			if (currentOrientation != DetectedOrientation.Landscape || forceUpdate)
			{
				UpdateOrientation(DetectedOrientation.Landscape);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void UpdateOrientation(DetectedOrientation orientation)
	{
		stream.SendSignal(orientation);
		OnOrientationChanged.Invoke(orientation);
		OnAnyOrientation?.Execute();
		switch (orientation)
		{
		case DetectedOrientation.Portrait:
			OnPortraitOrientation?.Execute();
			break;
		case DetectedOrientation.Landscape:
			OnLandscapeOrientation?.Execute();
			break;
		default:
			throw new ArgumentOutOfRangeException("orientation", orientation, null);
		case DetectedOrientation.Unknown:
			break;
		}
		currentOrientation = orientation;
		previousScreenOrientation = Screen.orientation;
	}
}
