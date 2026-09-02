using System;
using System.Collections.Generic;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Nody.Nodes.PortData;
using UnityEngine;

namespace Doozy.Runtime.Nody.Nodes;

[Serializable]
[NodyMenuPath("Utils", "Random")]
public sealed class RandomNode : SimpleNode
{
	private readonly List<int> m_SelectChances = new List<int>();

	public int maxChance { get; private set; }

	public override bool showClearGraphHistoryInEditor => true;

	public RandomNode()
	{
		AddInputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		AddOutputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		AddOutputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		SelectRandomOutput();
	}

	public void UpdateMaxChance()
	{
		maxChance = 0;
		foreach (FlowPort outputPort in base.outputPorts)
		{
			if (outputPort.isConnected)
			{
				int weight = outputPort.GetValue<RandomNodeOutputPortData>().Weight;
				if (weight > 0)
				{
					maxChance += weight;
				}
			}
		}
	}

	private void SelectRandomOutput()
	{
		m_SelectChances.Clear();
		maxChance = 0;
		foreach (FlowPort outputPort in base.outputPorts)
		{
			if (outputPort.isConnected)
			{
				int weight = outputPort.GetValue<RandomNodeOutputPortData>().Weight;
				if (weight <= 0)
				{
					m_SelectChances.Add(-1);
					continue;
				}
				maxChance += weight;
				m_SelectChances.Add(maxChance);
			}
		}
		int index = 0;
		int num = UnityEngine.Random.Range(0, maxChance);
		for (int i = 0; i < m_SelectChances.Count; i++)
		{
			if (m_SelectChances[i] != -1 && m_SelectChances[i] >= num)
			{
				index = i;
				break;
			}
		}
		GoToNextNode(base.outputPorts[index]);
	}

	public override FlowPort AddOutputPort(PortCapacity capacity = PortCapacity.Single)
	{
		return base.AddOutputPort(capacity).SetValue(new RandomNodeOutputPortData());
	}
}
