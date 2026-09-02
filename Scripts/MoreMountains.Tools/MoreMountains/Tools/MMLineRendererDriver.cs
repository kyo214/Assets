using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[RequireComponent(typeof(LineRenderer))]
public class MMLineRendererDriver : MonoBehaviour
{
	[Header("Position Drivers")]
	public List<Transform> Targets;

	public bool BindPositionsToTargetsAtUpdate = true;

	[Header("Binding")]
	[MMInspectorButton("Bind")]
	public bool BindButton;

	protected LineRenderer _lineRenderer;

	protected bool _countsMatch;

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_lineRenderer = base.gameObject.GetComponent<LineRenderer>();
		_countsMatch = CheckPositionCounts();
		if (!_countsMatch)
		{
			Debug.LogWarning(base.name + ", MMLineRendererDriver's Targets list doesn't have the same amount of entries as the LineRender's Positions array. It won't work.");
		}
	}

	protected virtual void Update()
	{
		if (BindPositionsToTargetsAtUpdate)
		{
			BindPositionsToTargets();
		}
	}

	protected virtual void Bind()
	{
		Initialization();
		BindPositionsToTargets();
	}

	public virtual void BindPositionsToTargets()
	{
		if (_countsMatch)
		{
			for (int i = 0; i < Targets.Count; i++)
			{
				_lineRenderer.SetPosition(i, Targets[i].position);
			}
		}
	}

	protected virtual bool CheckPositionCounts()
	{
		return Targets.Count == _lineRenderer.positionCount;
	}
}
