using System.Collections.Generic;
using UnityEngine;

namespace Lofelt.NiceVibrations;

public class HapticCurve : MonoBehaviour
{
	[Range(0f, 1f)]
	public float Amplitude = 1f;

	[Range(0f, 1f)]
	public float Frequency;

	public int PointsCount = 50;

	public float AmplitudeFactor = 3f;

	[Range(1f, 4f)]
	private float Period = 1f;

	public RectTransform StartPoint;

	public RectTransform EndPoint;

	[Header("Movement")]
	public bool Move;

	public float MovementSpeed = 1f;

	protected LineRenderer _targetLineRenderer;

	protected List<Vector3> Points;

	protected Canvas _canvas;

	protected Camera _camera;

	protected Vector3 _startPosition;

	protected Vector3 _endPosition;

	protected Vector3 _workPoint;

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		Points = new List<Vector3>();
		_canvas = base.gameObject.GetComponentInParent<Canvas>();
		_targetLineRenderer = base.gameObject.GetComponent<LineRenderer>();
		_camera = _canvas.worldCamera;
		DrawCurve();
	}

	protected virtual void DrawCurve()
	{
		_startPosition = StartPoint.transform.position;
		_startPosition.z -= 0.1f;
		_endPosition = EndPoint.transform.position;
		_endPosition.z -= 0.1f;
		Points.Clear();
		for (int i = 0; i < PointsCount; i++)
		{
			float num = NiceVibrationsDemoHelpers.Remap(i, 0f, PointsCount, 0f, 1f);
			float value = MMSignal.GetValue(num, MMSignal.SignalType.Sine, 1f, AmplitudeFactor, Period, 0f);
			if (Move)
			{
				value = MMSignal.GetValue(num + Time.time * MovementSpeed, MMSignal.SignalType.Sine, 1f, AmplitudeFactor, Period, 0f);
			}
			_workPoint.x = Mathf.Lerp(_startPosition.x, _endPosition.x, num);
			_workPoint.y = value * Amplitude + _startPosition.y;
			_workPoint.z = _startPosition.z;
			Points.Add(_workPoint);
		}
		_targetLineRenderer.positionCount = PointsCount;
		_targetLineRenderer.SetPositions(Points.ToArray());
	}

	protected virtual void Update()
	{
		UpdateCurve(Amplitude, Frequency);
	}

	public virtual void UpdateCurve(float amplitude, float frequency)
	{
		Amplitude = amplitude;
		Frequency = frequency;
		Period = NiceVibrationsDemoHelpers.Remap(frequency, 0f, 1f, 1f, 4f);
		DrawCurve();
	}
}
