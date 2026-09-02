using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Movement/MMPath")]
public class MMPath : MonoBehaviour
{
	public enum CycleOptions
	{
		BackAndForth = 0,
		Loop = 1,
		OnlyOnce = 2
	}

	public enum MovementDirection
	{
		Ascending = 0,
		Descending = 1
	}

	[Header("Path")]
	[MMInformation("Here you can select the '<b>Cycle Option</b>'. Back and Forth will have your object follow the path until its end, and go back to the original point. If you select Loop, the path will be closed and the object will move along it until told otherwise. If you select Only Once, the object will move along the path from the first to the last point, and remain there forever.", MMInformationAttribute.InformationType.Info, false)]
	public CycleOptions CycleOption;

	[MMInformation("Add points to the <b>Path</b> (set the size of the path first), then position the points using either the inspector or by moving the handles directly in scene view. For each path element you can specify a delay (in seconds). The order of the points will be the order the object follows.\nFor looping paths, you can then decide if the object will go through the points in the Path in Ascending (1, 2, 3...) or Descending (Last, Last-1, Last-2...) order.", MMInformationAttribute.InformationType.Info, false)]
	public MovementDirection LoopInitialMovementDirection;

	public List<MMPathMovementElement> PathElements;

	public MMPath ReferenceMMPath;

	public bool AbsoluteReferencePath;

	public float MinDistanceToGoal = 0.1f;

	[Header("Gizmos")]
	public bool LockHandlesOnXAxis;

	public bool LockHandlesOnYAxis;

	public bool LockHandlesOnZAxis;

	protected Vector3 _originalTransformPosition;

	protected bool _originalTransformPositionStatus;

	protected bool _active;

	protected IEnumerator<Vector3> _currentPoint;

	protected int _direction = 1;

	protected Vector3 _initialPosition;

	protected Vector3 _initialPositionThisFrame;

	protected Vector3 _finalPosition;

	protected Vector3 _previousPoint = Vector3.zero;

	protected int _currentIndex;

	protected float _distanceToNextPoint;

	protected bool _endReached;

	public virtual bool CanMove { get; set; }

	public virtual bool Initialized { get; set; }

	protected virtual void Start()
	{
		if (!Initialized)
		{
			Initialization();
		}
	}

	public virtual void Initialization()
	{
		_active = true;
		_endReached = false;
		CanMove = true;
		if (ReferenceMMPath != null && (ReferenceMMPath.PathElements != null || ReferenceMMPath.PathElements.Count > 0))
		{
			if (AbsoluteReferencePath)
			{
				base.transform.position = ReferenceMMPath.transform.position;
			}
			PathElements = ReferenceMMPath.PathElements;
		}
		if (PathElements != null && PathElements.Count >= 1)
		{
			if (LoopInitialMovementDirection == MovementDirection.Ascending)
			{
				_direction = 1;
			}
			else
			{
				_direction = -1;
			}
			_initialPosition = base.transform.position;
			_currentPoint = GetPathEnumerator();
			_previousPoint = _currentPoint.Current;
			_currentPoint.MoveNext();
			if (!_originalTransformPositionStatus)
			{
				_originalTransformPositionStatus = true;
				_originalTransformPosition = base.transform.position;
			}
			base.transform.position = _originalTransformPosition + _currentPoint.Current;
		}
	}

	public int CurrentIndex()
	{
		return _currentIndex;
	}

	public Vector3 CurrentPoint()
	{
		return _initialPosition + _currentPoint.Current;
	}

	public Vector3 CurrentPositionRelative()
	{
		return _currentPoint.Current;
	}

	protected virtual void Update()
	{
		if (PathElements != null && PathElements.Count >= 1 && !_endReached && CanMove)
		{
			ComputePath();
		}
	}

	protected virtual void ComputePath()
	{
		_initialPositionThisFrame = base.transform.position;
		_distanceToNextPoint = (base.transform.position - (_originalTransformPosition + _currentPoint.Current)).magnitude;
		if (_distanceToNextPoint < MinDistanceToGoal)
		{
			_previousPoint = _currentPoint.Current;
			_currentPoint.MoveNext();
		}
		_finalPosition = base.transform.position;
	}

	public virtual IEnumerator<Vector3> GetPathEnumerator()
	{
		if (PathElements == null || PathElements.Count < 1)
		{
			yield break;
		}
		int index = (_currentIndex = 0);
		while (true)
		{
			_currentIndex = index;
			yield return PathElements[index].PathElementPosition;
			if (PathElements.Count <= 1)
			{
				continue;
			}
			if (CycleOption == CycleOptions.Loop)
			{
				index += _direction;
				if (index < 0)
				{
					index = PathElements.Count - 1;
				}
				else if (index > PathElements.Count - 1)
				{
					index = 0;
				}
			}
			if (CycleOption == CycleOptions.BackAndForth)
			{
				if (index <= 0)
				{
					_direction = 1;
				}
				else if (index >= PathElements.Count - 1)
				{
					_direction = -1;
				}
				index += _direction;
			}
			if (CycleOption == CycleOptions.OnlyOnce)
			{
				if (index <= 0)
				{
					_direction = 1;
				}
				else if (index >= PathElements.Count - 1)
				{
					_direction = 0;
					_endReached = true;
				}
				index += _direction;
			}
		}
	}

	public virtual void ChangeDirection()
	{
		_direction = -_direction;
		_currentPoint.MoveNext();
	}

	protected virtual void OnDrawGizmos()
	{
	}

	public virtual void UpdateOriginalTransformPosition(Vector3 newOriginalTransformPosition)
	{
		_originalTransformPosition = newOriginalTransformPosition;
	}

	public virtual Vector3 GetOriginalTransformPosition()
	{
		return _originalTransformPosition;
	}

	public virtual void SetOriginalTransformPositionStatus(bool status)
	{
		_originalTransformPositionStatus = status;
	}

	public virtual bool GetOriginalTransformPositionStatus()
	{
		return _originalTransformPositionStatus;
	}
}
