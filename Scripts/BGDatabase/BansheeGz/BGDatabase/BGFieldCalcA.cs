using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCalcA<T> : BGFieldDictionaryBasedA<T, BGFieldCalcValue>, BGFieldCalcI
{
	[Serializable]
	private class JsonConfig
	{
		public string Graph;
	}

	private BGCalcGraph graph;

	public BGCalcGraph Graph
	{
		get
		{
			return graph;
		}
		set
		{
			if (graph != value)
			{
				SetGraphNoEvent(value);
				FireMetaChanged();
			}
		}
	}

	public override bool ReadOnly => true;

	public override bool CustomStringFormatSupported => false;

	public override bool StoredValueIsTheSameAsValueType => false;

	public override T this[BGId entityId]
	{
		set
		{
		}
	}

	public override T this[int index]
	{
		set
		{
		}
	}

	public abstract BGCalcTypeCode ResultCode { get; }

	private void SetGraphNoEvent(BGCalcGraph value)
	{
		if (graph != null)
		{
			graph.OnAnyChange -= FireMetaChanged;
		}
		graph = value;
		if (graph != null)
		{
			graph.OnAnyChange += FireMetaChanged;
		}
	}

	private void FireMetaChanged()
	{
		base.Meta.Repo.Events.MetaWasChanged(base.Meta);
	}

	public BGFieldCalcA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCalcA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override string ConfigToString()
	{
		if (Graph == null)
		{
			return null;
		}
		JsonConfig obj = new JsonConfig
		{
			Graph = new BGCalcSaverString(graph).Save()
		};
		return JsonUtility.ToJson(obj);
	}

	public override void ConfigFromString(string config)
	{
		if (!string.IsNullOrEmpty(config))
		{
			JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
			if (jsonConfig != null && !string.IsNullOrEmpty(jsonConfig.Graph))
			{
				SetGraphNoEvent(BGCalcGraph.ExistingGraph());
				Graph.FromJsonString(jsonConfig.Graph);
			}
		}
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter((Graph == null) ? 8 : 1024);
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddBool(Graph != null);
		if (Graph != null)
		{
			bGBinaryWriter.AddByteArray(Graph.ToBytes());
		}
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			if (bGBinaryReader.ReadBool())
			{
				SetGraphNoEvent(BGCalcGraph.ExistingGraph());
				Graph.FromBytes(bGBinaryReader.ReadByteArray());
			}
			return;
		}
		throw new BGException("Unsupported version: $", num);
	}

	protected override BGFieldCalcValue Convert(BGEntity entity, T value)
	{
		throw new NotImplementedException();
	}

	protected override T Convert(BGEntity entity, BGFieldCalcValue value)
	{
		if (value?.Graph != null)
		{
			return Run(value.Graph, this, entity);
		}
		if (Graph != null)
		{
			return Run(Graph, this, entity, value);
		}
		return default;
	}

	private static T Run(BGCalcGraph graph, BGField field, BGEntity entity, BGCalcVarsProvider varsOverrides = null)
	{
		BGCalcFlowContext context = BGCalcFlowContext.Get();
		try
		{
			object obj = Run(context, graph, field, entity, varsOverrides);
			if (obj == null)
			{
				return default;
			}
			return (T)obj;
		}
		finally
		{
			BGCalcFlowContext.Return(context);
		}
	}

	public static object Run(BGCalcFlowContext context, BGCalcGraph graph, BGField field, BGEntity entity, BGCalcVarsProvider varsOverrides = null)
	{
		if (context.ContainsCell(field, entity))
		{
			throw new Exception("Recursive call is detected- field " + field.FullName + " for row " + entity.FullName + " is called more than once!");
		}
		context.Graph = graph;
		context.CurrentEntity = entity;
		context.VarsOverrides = varsOverrides;
		context.GraphType = BGCalcGraphTypeEnum.CalculatedField;
		context.AddCell(field, entity);
		return graph.Execute(context);
	}

	protected override byte[] ValueToBytes(BGFieldCalcValue value)
	{
		return value?.ToBytes(Graph);
	}

	protected override BGFieldCalcValue ValueFromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == 0)
		{
			return null;
		}
		BGFieldCalcValue bGFieldCalcValue = new BGFieldCalcValue(this, base.Meta.GetEntity(entityIndex));
		bGFieldCalcValue.FromBytes(segment, Graph);
		return bGFieldCalcValue;
	}

	protected override string ValueToString(BGFieldCalcValue value)
	{
		return value?.ToJsonString(Graph);
	}

	protected override BGFieldCalcValue ValueFromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		BGFieldCalcValue bGFieldCalcValue = new BGFieldCalcValue(this, base.Meta.GetEntity(entityIndex));
		bGFieldCalcValue.FromJsonString(value, Graph);
		return bGFieldCalcValue;
	}
}
