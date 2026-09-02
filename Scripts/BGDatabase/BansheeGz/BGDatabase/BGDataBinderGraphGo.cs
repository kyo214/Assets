using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddComponentMenu("BansheeGz/BGDataBinderGraphGo")]
public class BGDataBinderGraphGo : BGDataBinderSingleGoA<BGDBGraph>
{
	[SerializeField]
	[HideInInspector]
	private byte typeCode = 3;

	[SerializeField]
	private byte[] graphContent;

	[SerializeField]
	[HideInInspector]
	private bool liveUpdate;

	[NonSerialized]
	private bool listenersWasAdded;

	[NonSerialized]
	private bool graphIsInjected;

	public byte[] GraphContent
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

	public bool LiveUpdate
	{
		get
		{
			return liveUpdate;
		}
		set
		{
			liveUpdate = value;
		}
	}

	public override string Error => null;

	public override Type TargetType
	{
		get
		{
			return typeCode switch
			{
				2 => typeof(bool), 
				3 => typeof(string), 
				4 => typeof(int), 
				5 => typeof(float), 
				_ => typeof(object), 
			};
		}
		set
		{
			if (value == typeof(bool))
			{
				typeCode = 2;
			}
			else if (value == typeof(string))
			{
				typeCode = 3;
			}
			else if (value == typeof(int))
			{
				typeCode = 4;
			}
			else if (value == typeof(float))
			{
				typeCode = 5;
			}
			else
			{
				typeCode = 10;
			}
		}
	}

	public byte TypeCode
	{
		get
		{
			return typeCode;
		}
		set
		{
			typeCode = value;
		}
	}

	public BGCalcGraph Graph
	{
		get
		{
			InjectGraph();
			return BindDelegate.Graph;
		}
		set
		{
			BindDelegate.Graph = value;
		}
	}

	protected bool IsLiveUpdateOn
	{
		get
		{
			if (liveUpdate && (Application.isPlaying || BGUtil.TestIsRunning))
			{
				return BindDelegate.Error == null;
			}
			return false;
		}
	}

	protected override void InjectToDelegate()
	{
		base.InjectToDelegate();
		BindDelegate.SetContext(base.gameObject);
		InjectGraph();
	}

	private void OnLoad(bool loaded)
	{
		if (loaded)
		{
			Bind();
		}
	}

	private void OnBatch(object sender, BGEventArgsBatch e)
	{
		Bind();
	}

	private void InjectGraph()
	{
		BindDelegate.LiveUpdate = LiveUpdate;
		BGCalcGraph bGCalcGraph = BindDelegate.Graph;
		if (graphIsInjected && bGCalcGraph != null)
		{
			return;
		}
		graphIsInjected = true;
		if (bGCalcGraph == null)
		{
			if (graphContent != null && graphContent.Length != 0)
			{
				bGCalcGraph = BGCalcGraph.ExistingGraph();
				bGCalcGraph.FromBytes(new ArraySegment<byte>(graphContent));
			}
			else
			{
				bGCalcGraph = BGCalcGraph.NewGraph((typeCode == 0) ? BGCalcTypeCodeRegistry.String : BGCalcTypeCodeRegistry.Get(typeCode));
			}
		}
		BindDelegate.Graph = bGCalcGraph;
	}

	protected override void OnDestroy()
	{
		if (listenersWasAdded)
		{
			BGRepo.OnLoad -= OnLoad;
			BGRepo.I.Events.OnBatchUpdate -= OnBatch;
			BindDelegate.RemoveFieldsListeners();
		}
	}

	protected override void AddListeners()
	{
		if (IsLiveUpdateOn && !listenersWasAdded)
		{
			listenersWasAdded = true;
			BGRepo.OnLoad += OnLoad;
			BGRepo.I.Events.OnBatchUpdate += OnBatch;
			BindDelegate.AddFieldsListeners(Bind);
		}
	}
}
