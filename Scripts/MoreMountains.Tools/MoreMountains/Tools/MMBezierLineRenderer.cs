using UnityEngine;

namespace MoreMountains.Tools;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
[AddComponentMenu("More Mountains/Tools/Sprites/MMBezierLineRenderer")]
public class MMBezierLineRenderer : MonoBehaviour
{
	public Transform[] AdjustmentHandles;

	public int NumberOfSegments = 50;

	public string SortingLayerName = "Default";

	[MMReadOnly]
	public int NumberOfCurves;

	protected int _sortingLayerID;

	protected LineRenderer _lineRenderer;

	protected Vector3 _point;

	protected Vector3 _p;

	protected bool _initialized;

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		if (!_initialized)
		{
			_sortingLayerID = SortingLayer.NameToID(SortingLayerName);
			NumberOfCurves = AdjustmentHandles.Length / 3;
			_lineRenderer = GetComponent<LineRenderer>();
			if (_lineRenderer != null)
			{
				_lineRenderer.sortingLayerID = _sortingLayerID;
			}
			_initialized = true;
		}
	}

	protected virtual void LateUpdate()
	{
		DrawCurve();
	}

	protected virtual void DrawCurve()
	{
		for (int i = 0; i < NumberOfCurves; i++)
		{
			for (int j = 1; j <= NumberOfSegments; j++)
			{
				float t = (float)(j - 1) / (float)(NumberOfSegments - 1);
				int num = i * 3;
				_point = BezierPoint(t, AdjustmentHandles[num].position, AdjustmentHandles[num + 1].position, AdjustmentHandles[num + 2].position, AdjustmentHandles[num + 3].position);
				_lineRenderer.positionCount = i * NumberOfSegments + j;
				_lineRenderer.SetPosition(i * NumberOfSegments + (j - 1), _point);
			}
		}
	}

	protected virtual Vector3 BezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		float num = 1f - t;
		float num2 = t * t;
		float num3 = num * num;
		float num4 = num3 * num;
		float num5 = num2 * t;
		_p = num4 * p0;
		_p += 3f * num3 * t * p1;
		_p += 3f * num * num2 * p2;
		_p += num5 * p3;
		return _p;
	}
}
