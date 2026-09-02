using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Fusion;

public class OrderSorter
{
	private List<Type> types = new List<Type>();

	private List<OrderNode> unorderedNodes = new List<OrderNode>();

	private Stack<OrderNode> defaultOrderNodes = new Stack<OrderNode>();

	public Dictionary<Type, OrderNode> NodeLookup = new Dictionary<Type, OrderNode>();

	public OrderNode[] SortedNodes;

	public OrderNode FirstNode;

	public OrderNode LastNode;

	public OrderNode Physics2DNode;

	public OrderNode Physics3DNode;

	public OrderNode HitboxManagerNode;

	private static void AddIfMissing(List<Type> group, Type type)
	{
		if (!group.Contains(type))
		{
			group.Add(type);
		}
	}

	public OrderNode[] RunConversion(List<Type> group)
	{
		try
		{
			types.Clear();
			unorderedNodes.Clear();
			defaultOrderNodes.Clear();
			NodeLookup.Clear();
			GenerateInitialNodeList(group);
			foreach (OrderNode unorderedNode in unorderedNodes)
			{
				unorderedNode.InitializeNode(NodeLookup);
			}
			if (MergeChains(unorderedNodes))
			{
				return null;
			}
			Sort(unorderedNodes, ref FirstNode, ref LastNode);
			ConvertNodesToSortedArray();
			return SortedNodes;
		}
		catch (ReflectionTypeLoadException ex)
		{
			Exception[] loaderExceptions = ex.LoaderExceptions;
			foreach (Exception ex2 in loaderExceptions)
			{
				Debug.LogException(ex2);
				Debug.LogError(ex2.Message);
				Debug.LogError(((object)ex2.InnerException) ?? ((object)"NULL"));
				Debug.LogError(ex2.Data.Keys.Count);
			}
			Type[] array = ex.Types;
			foreach (Type message in array)
			{
				Debug.LogError(message);
			}
			Debug.LogError(ex.StackTrace);
			Debug.LogException(ex);
			return new OrderNode[0];
		}
	}

	public OrderNode[] Run()
	{
		try
		{
			types.Clear();
			unorderedNodes.Clear();
			defaultOrderNodes.Clear();
			NodeLookup.Clear();
			Scanlibrary();
			AlphabetizeTypes(types);
			GenerateInitialNodeList();
			foreach (OrderNode unorderedNode in unorderedNodes)
			{
				unorderedNode.InitializeNode(NodeLookup);
			}
			if (MergeChains(unorderedNodes))
			{
				return null;
			}
			Sort(unorderedNodes, ref FirstNode, ref LastNode);
			InsertDefaultOrderNodes();
			ConvertNodesToSortedArray();
			return SortedNodes;
		}
		catch (ReflectionTypeLoadException ex)
		{
			Exception[] loaderExceptions = ex.LoaderExceptions;
			foreach (Exception ex2 in loaderExceptions)
			{
				Debug.LogException(ex2);
				Debug.LogError(ex2.Message);
				Debug.LogError(((object)ex2.InnerException) ?? ((object)"NULL"));
				Debug.LogError(ex2.Data.Keys.Count);
			}
			Type[] array = ex.Types;
			foreach (Type message in array)
			{
				Debug.LogError(message);
			}
			Debug.LogError(ex.StackTrace);
			Debug.LogException(ex);
			return new OrderNode[0];
		}
	}

	private void Scanlibrary()
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			if (assembly.GetCustomAttribute<NetworkAssemblyIgnoreAttribute>() != null)
			{
				continue;
			}
			try
			{
				Type[] typesIgnoreErrors = assembly.GetTypesIgnoreErrors();
				Type[] array = typesIgnoreErrors;
				foreach (Type type in array)
				{
					if (typeof(SimulationBehaviour).IsAssignableFrom(type) && !types.Contains(type))
					{
						types.Add(type);
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}

	private static void AlphabetizeTypes(List<Type> types)
	{
		types.Sort((Type a, Type b) => StringComparer.Ordinal.Compare(a.AssemblyQualifiedName, b.AssemblyQualifiedName));
	}

	public void GenerateInitialNodeList(List<Type> types)
	{
		foreach (Type type in types)
		{
			RegisterNode(type);
		}
	}

	private void GenerateInitialNodeList()
	{
		foreach (Type type in types)
		{
			RegisterNode(type);
		}
		if (Physics2DNode != null)
		{
			Remove(Physics2DNode, ref FirstNode, ref LastNode);
			InsertAfter(LastNode, Physics2DNode, ref LastNode);
		}
		if (Physics3DNode != null)
		{
			Remove(Physics3DNode, ref FirstNode, ref LastNode);
			InsertAfter(LastNode, Physics3DNode, ref LastNode);
		}
		if (HitboxManagerNode != null)
		{
			Remove(HitboxManagerNode, ref FirstNode, ref LastNode);
			InsertAfter(LastNode, HitboxManagerNode, ref LastNode);
		}
	}

	public OrderNode RegisterNode(Type type)
	{
		OrderNode orderNode = new OrderNode(type);
		unorderedNodes.Add(orderNode);
		NodeLookup.Add(type, orderNode);
		if (orderNode.Type == typeof(NetworkPhysicsSimulation2D))
		{
			Physics2DNode = orderNode;
		}
		else if (orderNode.Type == typeof(NetworkPhysicsSimulation3D))
		{
			Physics3DNode = orderNode;
		}
		else if (orderNode.Type == typeof(HitboxManager))
		{
			HitboxManagerNode = orderNode;
		}
		if (orderNode.IsDefaultOrder)
		{
			defaultOrderNodes.Push(orderNode);
			return orderNode;
		}
		if (LastNode == null)
		{
			FirstNode = orderNode;
			LastNode = orderNode;
		}
		else
		{
			InsertAfter(LastNode, orderNode, ref LastNode);
		}
		return orderNode;
	}

	public static bool MergeChains(List<OrderNode> unorderedNodes)
	{
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		do
		{
			num++;
			if (num > 5000)
			{
				WriteError("Unsolvable Before/After, led to endless loop in OrderSorter.");
				return false;
			}
			flag = false;
			foreach (OrderNode unorderedNode in unorderedNodes)
			{
				OrderNode[] array = new OrderNode[unorderedNode.After.Count];
				unorderedNode.After.CopyTo(array);
				OrderNode[] array2 = array;
				foreach (OrderNode orderNode in array2)
				{
					if (unorderedNode.Before.Overlaps(orderNode.After))
					{
						WriteError($"Before / After Conflict between {unorderedNode}.SortBefore and {orderNode}.SortAfter");
						flag2 = true;
					}
					if (orderNode.After.Contains(unorderedNode))
					{
						WriteError($"Before / After Conflict {unorderedNode} cyclical before with {orderNode}.SortAfter");
						flag2 = true;
					}
					int count = unorderedNode.After.Count;
					unorderedNode.After.UnionWith(orderNode.After);
					flag |= count != unorderedNode.After.Count;
					int count2 = orderNode.Before.Count;
					orderNode.Before.UnionWith(unorderedNode.Before);
					if (!orderNode.Before.Contains(unorderedNode))
					{
						orderNode.Before.Add(unorderedNode);
					}
					flag |= count2 != orderNode.Before.Count;
				}
				OrderNode[] array3 = new OrderNode[unorderedNode.Before.Count];
				unorderedNode.Before.CopyTo(array3);
				OrderNode[] array4 = array3;
				foreach (OrderNode orderNode2 in array4)
				{
					if (unorderedNode.After.Overlaps(orderNode2.Before))
					{
						WriteError($"Before / After Conflict between {unorderedNode}.SortAfter and {orderNode2}.SortBefore");
						flag2 = true;
					}
					if (orderNode2.Before.Contains(unorderedNode))
					{
						WriteError($"Before / After Conflict {unorderedNode} cyclical after with {orderNode2}.SortAfter");
						flag2 = true;
					}
					int count3 = unorderedNode.Before.Count;
					unorderedNode.Before.UnionWith(orderNode2.Before);
					flag |= count3 != unorderedNode.Before.Count;
					int count4 = orderNode2.After.Count;
					orderNode2.After.UnionWith(unorderedNode.After);
					if (!orderNode2.After.Contains(unorderedNode))
					{
						orderNode2.After.Add(unorderedNode);
					}
					flag |= count4 != orderNode2.After.Count;
				}
			}
		}
		while (flag && !flag2);
		return flag2;
	}

	public static void Sort(List<OrderNode> unorderedNodes, ref OrderNode firstNode, ref OrderNode lastNode)
	{
		int num = 0;
		int count = unorderedNodes.Count;
		bool flag;
		do
		{
			num++;
			if (num > count)
			{
				throw new Exception("Taking too many cycles of resorting to solve the Before/AFter chain. May be unresolvable.");
			}
			OrderNode orderNode = firstNode;
			flag = false;
			int num2 = unorderedNodes.Count * 4;
			while (orderNode != null)
			{
				num2--;
				if (num2 == 0)
				{
					WriteError("Before/After sort stuck in a loop. Indicates a flaw in the sorting algorithm.");
					break;
				}
				flag |= ResortNode(orderNode, ref firstNode, ref lastNode);
				if (flag)
				{
					break;
				}
				orderNode = orderNode.Next;
			}
		}
		while (flag);
	}

	public static bool ResortNode(OrderNode node, ref OrderNode firstNode, ref OrderNode lastNode)
	{
		OrderNode orderNode = firstNode;
		OrderNode orderNode2 = null;
		OrderNode orderNode3 = null;
		bool flag = true;
		bool flag2 = false;
		bool flag3 = false;
		int num = 0;
		while (orderNode != null)
		{
			num++;
			if (num > 10000)
			{
				Debug.LogError("Iterating Fusion SortBefore and SortAFter tags exceeded allowed number of re-sorts.");
				break;
			}
			if (node == orderNode)
			{
				flag2 = true;
				flag = false;
			}
			else if (node.After.Contains(orderNode))
			{
				if (flag2)
				{
					flag3 = true;
				}
				orderNode3 = orderNode;
			}
			else if (node.Before.Contains(orderNode))
			{
				if (!flag2)
				{
					flag3 = true;
				}
				if (orderNode2 == null)
				{
					orderNode2 = orderNode;
				}
			}
			orderNode = orderNode.Next;
		}
		if (flag3)
		{
			if ((flag && orderNode2 != null) || orderNode3 == null)
			{
				Remove(node, ref firstNode, ref lastNode);
				InsertBefore(node, orderNode2, ref firstNode);
			}
			else
			{
				Remove(node, ref firstNode, ref lastNode);
				InsertAfter(orderNode3, node, ref lastNode);
			}
			return true;
		}
		return false;
	}

	public void InsertDefaultOrderNodes()
	{
		OrderNode existing = NodeLookup[typeof(SimulationBehaviour)];
		while (defaultOrderNodes.Count > 0)
		{
			InsertAfter(existing, defaultOrderNodes.Pop(), ref LastNode);
		}
	}

	private void ConvertNodesToSortedArray()
	{
		SortedNodes = new OrderNode[unorderedNodes.Count];
		OrderNode orderNode = FirstNode;
		int i = 0;
		for (int count = unorderedNodes.Count; i < count; i++)
		{
			SortedNodes[i] = orderNode;
			orderNode = orderNode.Next;
		}
	}

	public static void InsertBefore(OrderNode insert, OrderNode existing, ref OrderNode firstNode)
	{
		if (insert == existing)
		{
			WriteError("ERROR - " + insert?.ToString() + " attempting to insert itself before itself, which would create an infinite loop in the linked list.");
			return;
		}
		OrderNode prev = existing.Prev;
		if (prev != null)
		{
			prev.Next = insert;
		}
		insert.Next = existing;
		insert.Prev = existing.Prev;
		existing.Prev = insert;
		if (existing == firstNode)
		{
			firstNode = insert;
		}
	}

	public static void InsertAfter(OrderNode existing, OrderNode insert, ref OrderNode lastNode)
	{
		if (insert == existing)
		{
			WriteError("ERROR - " + insert?.ToString() + " attempting to insert itself after itself, which would create an infinite loop in the linked list.");
			return;
		}
		insert.Next = existing.Next;
		insert.Prev = existing;
		if (existing.Next != null)
		{
			existing.Next.Prev = insert;
		}
		existing.Next = insert;
		if (existing == lastNode)
		{
			lastNode = insert;
		}
	}

	public static void Remove(OrderNode node, ref OrderNode firstNode, ref OrderNode lastNode)
	{
		if (node == firstNode)
		{
			firstNode = node.Next;
		}
		if (node == lastNode)
		{
			lastNode = node.Prev;
		}
		if (node.Prev != null)
		{
			node.Prev.Next = node.Next;
		}
		if (node.Next != null)
		{
			node.Next.Prev = node.Prev;
		}
		node.Prev = null;
		node.Next = null;
	}

	private static void WriteError(object obj)
	{
		Debug.LogError("OrderSorter ERROR: " + obj);
		Log.Error(obj);
	}
}
