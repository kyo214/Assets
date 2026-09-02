using System;
using Doozy.Runtime.Nody.Nodes.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Nody.Nodes;

[Serializable]
[NodyMenuPath("Utils", "Pivot")]
public sealed class PivotNode : SimpleNode
{
	public enum Orientation
	{
		Horizontal = 0,
		HorizontalReversed = 1,
		Vertical = 2,
		VerticalReversed = 3
	}

	[SerializeField]
	private Orientation PivotOrientation;

	public Orientation pivotOrientation
	{
		get
		{
			return PivotOrientation;
		}
		set
		{
			PivotOrientation = value;
			onOrientationChanged?.Invoke(value);
		}
	}

	public UnityAction<Orientation> onOrientationChanged { get; set; }

	public PivotNode()
	{
		AddInputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		AddOutputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		GoToNextNode(base.firstOutputPort);
	}
}
