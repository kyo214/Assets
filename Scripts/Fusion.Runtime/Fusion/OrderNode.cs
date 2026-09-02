using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion;

public class OrderNode
{
	public const SimulationModes ALL_MODES = SimulationModes.Server | SimulationModes.Host | SimulationModes.Client;

	public const SimulationStages ALL_STAGES = SimulationStages.Forward | SimulationStages.Resimulate;

	public Type Type;

	public OrderNode Prev;

	public OrderNode Next;

	public HashSet<OrderNode> Before = new HashSet<OrderNode>();

	public HashSet<OrderNode> After = new HashSet<OrderNode>();

	public HashSet<OrderNode> OrigBefore;

	public HashSet<OrderNode> OrigAfter;

	public (SimulationModes modes, SimulationStages stages) SimFlags;

	public bool IsDefaultOrder;

	public bool FoundBefores;

	public bool FoundAfters;

	public int UnityScriptOrder = 0;

	public OrderNode(Type type)
	{
		Type = type;
		SimFlags = (modes: SimulationModes.Server | SimulationModes.Host | SimulationModes.Client, stages: SimulationStages.Forward | SimulationStages.Resimulate);
		while (typeof(SimulationBehaviour).IsAssignableFrom(type))
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(SimulationBehaviourAttribute), inherit: true);
			if (customAttributes.Length != 0)
			{
				SimulationBehaviourAttribute simulationBehaviourAttribute = (SimulationBehaviourAttribute)customAttributes[0];
				SimFlags = (modes: (simulationBehaviourAttribute.Modes == (SimulationModes)0) ? (SimulationModes.Server | SimulationModes.Host | SimulationModes.Client) : simulationBehaviourAttribute.Modes, stages: (simulationBehaviourAttribute.Stages == (SimulationStages)0) ? (SimulationStages.Forward | SimulationStages.Resimulate) : simulationBehaviourAttribute.Stages);
				break;
			}
			type = type.BaseType;
		}
	}

	public void InitializeNode(Dictionary<Type, OrderNode> nodeLookupDict)
	{
		Type type = Type;
		FoundAfters = false;
		FoundBefores = false;
		Type typeFromHandle = typeof(SimulationBehaviour);
		do
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(OrderAttribute), inherit: false);
			object[] array = customAttributes;
			foreach (object obj in array)
			{
				if (obj is OrderAfterAttribute { After: not null, After: var after })
				{
					foreach (Type type2 in after)
					{
						if (!typeFromHandle.IsAssignableFrom(type2))
						{
							Debug.LogWarning(string.Format("{0} has an {1} with the type <b>{2}</b>, which is neither {3} nor {4}. Ignoring.", Type, "OrderBeforeAttribute", type2, "SimulationBehaviour", "NetworkBehaviour"));
						}
						else if (nodeLookupDict.ContainsKey(type2))
						{
							OrderNode item = nodeLookupDict[type2];
							if (!After.Contains(item))
							{
								After.Add(item);
							}
							FoundAfters = true;
						}
					}
				}
				else
				{
					if (!(obj is OrderBeforeAttribute { Before: not null, Before: var before }))
					{
						continue;
					}
					foreach (Type type3 in before)
					{
						if (!typeFromHandle.IsAssignableFrom(type3))
						{
							Debug.LogWarning(string.Format("{0} has an {1} with the type <b>{2}</b>, which is neither {3} nor {4}. Ignoring.", Type, "OrderBeforeAttribute", type3, "SimulationBehaviour", "NetworkBehaviour"));
						}
						else
						{
							OrderNode item2 = nodeLookupDict[type3];
							if (!Before.Contains(item2))
							{
								Before.Add(item2);
							}
							FoundBefores = true;
						}
					}
				}
			}
			if (FoundBefores || FoundAfters)
			{
				StoreOriginals();
				return;
			}
			if (type == typeof(SimulationBehaviour))
			{
				StoreOriginals();
				return;
			}
			type = type.BaseType;
		}
		while (type != typeof(SimulationBehaviour));
		if (!FoundBefores && !FoundAfters)
		{
			IsDefaultOrder = true;
			if (nodeLookupDict.ContainsKey(typeof(SimulationBehaviour)))
			{
				OrderNode orderNode = nodeLookupDict[typeof(SimulationBehaviour)];
				After = new HashSet<OrderNode>(orderNode.After);
				Before = new HashSet<OrderNode>(orderNode.Before);
				After.Add(orderNode);
			}
		}
		StoreOriginals();
	}

	private void StoreOriginals()
	{
		if (Application.isEditor)
		{
			OrigAfter = new HashSet<OrderNode>(After);
			OrigBefore = new HashSet<OrderNode>(Before);
		}
	}

	public override string ToString()
	{
		return Type.Name;
	}
}
