using Pathfinding;
using UnityEngine;

public class AIChase : MonoBehaviour
{
	public Transform _targetPosition;

	private Seeker _seeker;

	private CharacterController _controller;

	private Path _path;

	private int _cWaypoint;

	public float _speed = 2f;

	public float _nextWpDistance = 3f;

	public bool _isReachingEnd;

	private void Start()
	{
		_controller = GetComponent<CharacterController>();
		_seeker = GetComponent<Seeker>();
		_seeker.StartPath(base.transform.position, _targetPosition.position, OnPathComplete);
	}

	private void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			_path = p;
			_cWaypoint = 0;
		}
	}

	private void Update()
	{
		if (_path == null)
		{
			return;
		}
		_isReachingEnd = false;
		float num;
		while (true)
		{
			num = Vector3.Distance(base.transform.position, _path.vectorPath[_cWaypoint]);
			if (!(num < _nextWpDistance))
			{
				break;
			}
			if (_cWaypoint + 1 < _path.vectorPath.Count)
			{
				_cWaypoint++;
				continue;
			}
			_isReachingEnd = true;
			break;
		}
		float num2 = 1f;
		if (_isReachingEnd)
		{
			num2 = Mathf.Sqrt(num / _nextWpDistance);
		}
		Vector3 vector = (_path.vectorPath[_cWaypoint] - base.transform.position).normalized * _speed * num2;
		_controller.Move(vector);
		Debug.Log(vector);
	}
}
