using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGDBGraph : BGDBA
{
	[SerializeField]
	private string graphContent;

	[SerializeField]
	private byte typeCode = 3;

	private BGCalcGraph graph;

	private BGCalcFlowEvents events;

	private bool addBatchListeners;

	public override bool SupportReverseBinding => false;

	public byte TypeCode
	{
		get
		{
			if (typeCode == 0)
			{
				typeCode = BGCalcTypeCodeRegistry.String.TypeCode;
			}
			return typeCode;
		}
		set
		{
			typeCode = ((value == 0) ? BGCalcTypeCodeRegistry.String.TypeCode : value);
		}
	}

	public BGCalcGraph Graph
	{
		get
		{
			return graph;
		}
		set
		{
			graph = value;
		}
	}

	public string GraphContent
	{
		get
		{
			return graphContent;
		}
		set
		{
			graphContent = value;
		}
	}

	public GameObject Owner { get; private set; }

	public override Type TargetType
	{
		get
		{
			if (typeCode != 0)
			{
				return BGCalcTypeCodeRegistry.Get(typeCode).Type;
			}
			return typeof(string);
		}
	}

	public override object ValueToBind
	{
		get
		{
			error = null;
			EnsureTarget();
			if (error != null)
			{
				return null;
			}
			return GetValue();
		}
	}

	public override object GetValue()
	{
		RemoveFieldsListeners();
		BGCalcGraph bGCalcGraph = Graph ?? EnsureGraph();
		try
		{
			if (base.LiveUpdate && (Application.isPlaying || BGUtil.TestIsRunning))
			{
				if (events == null)
				{
					events = new BGCalcFlowEvents(base.Bind);
				}
				events.AddBatchListeners = addBatchListeners;
			}
			else
			{
				events = null;
			}
			BGCalcFlowContext bGCalcFlowContext = BGCalcFlowContext.Get();
			try
			{
				bGCalcFlowContext.CurrentGameObject = Owner;
				bGCalcFlowContext.Events = events;
				bGCalcFlowContext.GraphType = BGCalcGraphTypeEnum.GraphBinder;
				BGCalcFlowI bGCalcFlowI = bGCalcGraph.Launch(bGCalcFlowContext);
				events?.AddListeners();
				return bGCalcFlowI.Result;
			}
			finally
			{
				BGCalcFlowContext.Return(bGCalcFlowContext);
			}
		}
		catch (Exception ex)
		{
			string text = ex.Data["u"] as string;
			error = "Graph throws exception" + ((text == null) ? ":" : (" at [" + text + "] unit: ")) + ex.Message;
			return null;
		}
	}

	public BGCalcGraph EnsureGraph()
	{
		if (graph != null)
		{
			return graph;
		}
		if (string.IsNullOrEmpty(graphContent))
		{
			graph = BGCalcGraph.NewGraph(BGCalcTypeCodeRegistry.Get(TypeCode));
			return graph;
		}
		graph = BGCalcGraph.ExistingGraph();
		graph.FromBytes(new ArraySegment<byte>(Convert.FromBase64String(graphContent)));
		return graph;
	}

	public override string ReverseBind()
	{
		return null;
	}

	public override int AddFieldsListeners(Action action)
	{
		return 0;
	}

	public override void RemoveFieldsListeners()
	{
		events?.Clear();
	}

	public void SetContext(GameObject owner)
	{
		Owner = owner;
	}

	public void SetContext(GameObject owner, bool addBatchListeners)
	{
		SetContext(owner);
		this.addBatchListeners = addBatchListeners;
	}
}
