using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion;

[AddComponentMenu("Fusion/Runner Visibility Nodes")]
public class RunnerVisibilityNodes : Behaviour
{
	[InlineHelp]
	[SerializeField]
	public RunnerVisibilityNode.PreferredRunners PreferredRunner;

	[InlineHelp]
	public Component[] Components = new Component[0];

	[HideInInspector]
	[SerializeField]
	private string _guid = Guid.NewGuid().ToString().Substring(0, 19);

	private static List<Component> reusableComponentsList = new List<Component>();

	private static List<Component> reusableComponentsList2 = new List<Component>();

	internal void AddNodes()
	{
		int i = 0;
		for (int num = Components.Length; i < num; i++)
		{
			RunnerVisibilityNode runnerVisibilityNode = base.gameObject.AddComponent<RunnerVisibilityNode>();
			runnerVisibilityNode.Guid = _guid + i;
			runnerVisibilityNode.Component = Components[i];
			runnerVisibilityNode.PreferredRunner = PreferredRunner;
		}
	}

	[BehaviourButtonAction("Find on GameObject", null, null, ConditionFlags = (BehaviourActionAttribute.ActionFlags.ShowAtNotRuntime | BehaviourActionAttribute.ActionFlags.DirtyAfterButton))]
	public void FindRecognizedTypes()
	{
		Components = FindRecognizedComponentsOnGameObject(base.gameObject);
	}

	[BehaviourButtonAction("Find in Nested Children", null, null, ConditionFlags = (BehaviourActionAttribute.ActionFlags.ShowAtNotRuntime | BehaviourActionAttribute.ActionFlags.DirtyAfterButton))]
	public void FindNestedRecognizedTypes()
	{
		Components = FindRecognizedNestedComponents(base.gameObject);
	}

	internal static Component[] FindRecognizedComponentsOnGameObject(GameObject go)
	{
		try
		{
			go.GetComponents(reusableComponentsList);
			reusableComponentsList2.Clear();
			foreach (Component reusableComponents in reusableComponentsList)
			{
				Type type = reusableComponents.GetType();
				if (RunnerVisibilityNode.IsRecognized(type))
				{
					reusableComponentsList2.Add(reusableComponents);
				}
			}
			return reusableComponentsList2.ToArray();
		}
		finally
		{
			reusableComponentsList.Clear();
			reusableComponentsList2.Clear();
		}
	}

	internal static Component[] FindRecognizedNestedComponents(GameObject go)
	{
		try
		{
			go.transform.GetNestedComponentsInChildren<Component, NetworkObject>(reusableComponentsList);
			reusableComponentsList2.Clear();
			foreach (Component reusableComponents in reusableComponentsList)
			{
				Type type = reusableComponents.GetType();
				if (RunnerVisibilityNode.IsRecognized(type))
				{
					reusableComponentsList2.Add(reusableComponents);
				}
			}
			return reusableComponentsList2.ToArray();
		}
		finally
		{
			reusableComponentsList.Clear();
			reusableComponentsList2.Clear();
		}
	}
}
