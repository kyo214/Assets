using System;
using UnityEngine;

namespace MoreMountains.Tools;

[RequireComponent(typeof(LineRenderer))]
public class MMLineRendererCircle : MonoBehaviour
{
	public enum DrawAxis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	[Header("Draw Axis")]
	[Tooltip("the axis on which to draw the circle")]
	public DrawAxis Axis = DrawAxis.Z;

	[Tooltip("the distance by which to push the circle on the draw axis")]
	public float NormalOffset;

	[Header("Geometry")]
	[Tooltip("the amount of segments on the line renderer. More segments, more smoothness, more performance cost")]
	[Range(0f, 2000f)]
	public int PositionsCount = 60;

	[Header("Shape")]
	[Tooltip("the length of the circle's horizontal radius")]
	public float HorizontalRadius = 10f;

	[Tooltip("the length of the circle's vertical radius")]
	public float VerticalRadius = 10f;

	[Header("Debug")]
	[Tooltip("if this is true, the circle will be redrawn every time you change a value in the inspector, otherwise you'll have to call the DrawCircle method (or press the debug button below)")]
	public bool AutoRedrawOnValuesChange;

	[MMInspectorButton("DrawCircle")]
	public bool DrawCircleButton;

	protected LineRenderer _line;

	protected Vector3 _newPosition;

	protected float _angle;

	protected float _x;

	protected float _y;

	protected float _z;

	protected virtual void Awake()
	{
		Initialization();
		DrawCircle();
	}

	protected virtual void Initialization()
	{
		_line = base.gameObject.GetComponent<LineRenderer>();
		_line.positionCount = PositionsCount + 1;
		_line.useWorldSpace = false;
	}

	public virtual void DrawCircle()
	{
		_angle = 0f;
		_z = NormalOffset;
		switch (Axis)
		{
		case DrawAxis.X:
			DrawCircleX();
			break;
		case DrawAxis.Y:
			DrawCircleY();
			break;
		case DrawAxis.Z:
			DrawCircleZ();
			break;
		}
	}

	protected virtual float ComputeX()
	{
		return Mathf.Cos(MathF.PI / 180f * _angle) * HorizontalRadius;
	}

	protected virtual float ComputeY()
	{
		return Mathf.Sin(MathF.PI / 180f * _angle) * VerticalRadius;
	}

	protected virtual void DrawCircleX()
	{
		for (int i = 0; i < PositionsCount + 1; i++)
		{
			_x = ComputeX();
			_y = ComputeY();
			_newPosition.x = _z;
			_newPosition.y = _y;
			_newPosition.z = _x;
			_line.SetPosition(i, _newPosition);
			_angle += 360f / (float)PositionsCount;
		}
	}

	protected virtual void DrawCircleY()
	{
		for (int i = 0; i < PositionsCount + 1; i++)
		{
			_x = ComputeX();
			_y = ComputeY();
			_newPosition.x = _y;
			_newPosition.y = _z;
			_newPosition.z = _x;
			_line.SetPosition(i, _newPosition);
			_angle += 360f / (float)PositionsCount;
		}
	}

	protected virtual void DrawCircleZ()
	{
		for (int i = 0; i < PositionsCount + 1; i++)
		{
			_x = ComputeX();
			_y = ComputeY();
			_newPosition.x = _x;
			_newPosition.y = _y;
			_newPosition.z = _z;
			_line.SetPosition(i, _newPosition);
			_angle += 360f / (float)PositionsCount;
		}
	}

	protected virtual void OnValidate()
	{
		if (AutoRedrawOnValuesChange)
		{
			DrawCircle();
		}
	}
}
