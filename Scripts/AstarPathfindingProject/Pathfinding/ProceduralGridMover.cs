using System;
using System.Collections;
using UnityEngine;

namespace Pathfinding;

[HelpURL("http://arongranberg.com/astar/documentation/stable/class_pathfinding_1_1_procedural_grid_mover.php")]
public class ProceduralGridMover : VersionedMonoBehaviour
{
	public float updateDistance = 10f;

	public Transform target;

	private GridNodeBase[] buffer;

	public GridGraph graph;

	[HideInInspector]
	public int graphIndex;

	public bool updatingGraph { get; private set; }

	private void Start()
	{
		if (AstarPath.active == null)
		{
			throw new Exception("There is no AstarPath object in the scene");
		}
		if (graph == null)
		{
			if (graphIndex < 0)
			{
				throw new Exception("Graph index should not be negative");
			}
			if (graphIndex >= AstarPath.active.data.graphs.Length)
			{
				throw new Exception("The ProceduralGridMover was configured to use graph index " + graphIndex + ", but only " + AstarPath.active.data.graphs.Length + " graphs exist");
			}
			graph = AstarPath.active.data.graphs[graphIndex] as GridGraph;
			if (graph == null)
			{
				throw new Exception("The ProceduralGridMover was configured to use graph index " + graphIndex + " but that graph either does not exist or is not a GridGraph or LayerGridGraph");
			}
		}
		UpdateGraph();
	}

	private void Update()
	{
		if (graph != null)
		{
			Vector3 a = PointToGraphSpace(graph.center);
			Vector3 b = PointToGraphSpace(target.position);
			if (VectorMath.SqrDistanceXZ(a, b) > updateDistance * updateDistance)
			{
				UpdateGraph();
			}
		}
	}

	private Vector3 PointToGraphSpace(Vector3 p)
	{
		return graph.transform.InverseTransform(p);
	}

	public void UpdateGraph()
	{
		if (updatingGraph)
		{
			return;
		}
		updatingGraph = true;
		IEnumerator ie = UpdateGraphCoroutine();
		AstarPath.active.AddWorkItem(new AstarWorkItem((IWorkItemContext context, bool force) =>
		{
			if (force)
			{
				while (ie.MoveNext())
				{
				}
			}
			bool flag;
			try
			{
				flag = !ie.MoveNext();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, this);
				flag = true;
			}
			if (flag)
			{
				updatingGraph = false;
			}
			return flag;
		}));
	}

	private IEnumerator UpdateGraphCoroutine()
	{
		Vector3 vector = PointToGraphSpace(target.position) - PointToGraphSpace(graph.center);
		vector.x = Mathf.Round(vector.x);
		vector.z = Mathf.Round(vector.z);
		vector.y = 0f;
		if (vector == Vector3.zero)
		{
			yield break;
		}
		Int2 offset = new Int2(-Mathf.RoundToInt(vector.x), -Mathf.RoundToInt(vector.z));
		graph.center += graph.transform.TransformVector(vector);
		graph.UpdateTransform();
		int width = graph.width;
		int depth = graph.depth;
		int layers = graph.LayerCount;
		GridNodeBase[] nodes = ((!(graph is LayerGridGraph layerGridGraph)) ? graph.nodes : layerGridGraph.nodes);
		if (buffer == null || buffer.Length != width * depth)
		{
			buffer = new GridNodeBase[width * depth];
		}
		int counter;
		int yieldEvery;
		if (Mathf.Abs(offset.x) <= width && Mathf.Abs(offset.y) <= depth)
		{
			IntRect recalculateRect = new IntRect(0, 0, offset.x, offset.y);
			if (recalculateRect.xmin > recalculateRect.xmax)
			{
				int xmax = recalculateRect.xmax;
				recalculateRect.xmax = width + recalculateRect.xmin;
				recalculateRect.xmin = width + xmax;
			}
			if (recalculateRect.ymin > recalculateRect.ymax)
			{
				int ymax = recalculateRect.ymax;
				recalculateRect.ymax = depth + recalculateRect.ymin;
				recalculateRect.ymin = depth + ymax;
			}
			IntRect connectionRect = recalculateRect.Expand(1);
			connectionRect = IntRect.Intersection(connectionRect, new IntRect(0, 0, width, depth));
			for (int l = 0; l < layers; l++)
			{
				int layerOffset = l * width * depth;
				for (int i = 0; i < depth; i++)
				{
					int num = i * width;
					int num2 = (i + offset.y + depth) % depth * width;
					for (int j = 0; j < width; j++)
					{
						buffer[num2 + (j + offset.x + width) % width] = nodes[layerOffset + num + j];
					}
				}
				yield return null;
				for (int k = 0; k < depth; k++)
				{
					int num3 = k * width;
					for (int m = 0; m < width; m++)
					{
						int num4 = num3 + m;
						GridNodeBase gridNodeBase = buffer[num4];
						if (gridNodeBase != null)
						{
							gridNodeBase.NodeInGridIndex = num4;
						}
						nodes[layerOffset + num4] = gridNodeBase;
					}
					int num5;
					int num6;
					if (k >= recalculateRect.ymin && k < recalculateRect.ymax)
					{
						num5 = 0;
						num6 = depth;
					}
					else
					{
						num5 = recalculateRect.xmin;
						num6 = recalculateRect.xmax;
					}
					for (int n = num5; n < num6; n++)
					{
						buffer[num3 + n]?.ClearConnections(alsoReverse: false);
					}
				}
				yield return null;
			}
			yieldEvery = 1000;
			int num7 = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y)) * Mathf.Max(width, depth);
			yieldEvery = Mathf.Max(yieldEvery, num7 / 10);
			counter = 0;
			for (int l = 0; l < depth; l++)
			{
				int num8;
				int num9;
				if (l >= recalculateRect.ymin && l < recalculateRect.ymax)
				{
					num8 = 0;
					num9 = width;
				}
				else
				{
					num8 = recalculateRect.xmin;
					num9 = recalculateRect.xmax;
				}
				for (int num10 = num8; num10 < num9; num10++)
				{
					graph.RecalculateCell(num10, l, resetPenalties: false, resetTags: false);
				}
				counter += num9 - num8;
				if (counter > yieldEvery)
				{
					counter = 0;
					yield return null;
				}
			}
			for (int l = 0; l < depth; l++)
			{
				int num11;
				int num12;
				if (l >= connectionRect.ymin && l < connectionRect.ymax)
				{
					num11 = 0;
					num12 = width;
				}
				else
				{
					num11 = connectionRect.xmin;
					num12 = connectionRect.xmax;
				}
				for (int num13 = num11; num13 < num12; num13++)
				{
					graph.CalculateConnections(num13, l);
				}
				counter += num12 - num11;
				if (counter > yieldEvery)
				{
					counter = 0;
					yield return null;
				}
			}
			yield return null;
			for (int num14 = 0; num14 < depth; num14++)
			{
				for (int num15 = 0; num15 < width; num15++)
				{
					if (num15 == 0 || num14 == 0 || num15 == width - 1 || num14 == depth - 1)
					{
						graph.CalculateConnections(num15, num14);
					}
				}
			}
			yield break;
		}
		counter = Mathf.Max(depth * width / 20, 1000);
		yieldEvery = 0;
		for (int l = 0; l < depth; l++)
		{
			for (int num16 = 0; num16 < width; num16++)
			{
				graph.RecalculateCell(num16, l);
			}
			yieldEvery += width;
			if (yieldEvery > counter)
			{
				yieldEvery = 0;
				yield return null;
			}
		}
		for (int l = 0; l < depth; l++)
		{
			for (int num17 = 0; num17 < width; num17++)
			{
				graph.CalculateConnections(num17, l);
			}
			yieldEvery += width;
			if (yieldEvery > counter)
			{
				yieldEvery = 0;
				yield return null;
			}
		}
	}
}
