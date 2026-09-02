using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGFieldCalcValue : BGFieldDictionaryClonebleValueI, BGCalcVarsProvider
{
	[Serializable]
	private class JsonConfig
	{
		public List<CalcVarModel> Vars;

		public BGCalcGraphModel Graph;
	}

	[Serializable]
	private class CalcVarModel
	{
		public byte CodeRef;

		public string Id;

		public string Value;
	}

	private readonly BGField field;

	private readonly BGEntity entity;

	private BGCalcGraph graph;

	private BGFieldCalcVarRef.VarRefContainer vars;

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
				if (graph != null)
				{
					graph.OnAnyChange -= OnGraphChange;
				}
				graph = value;
				if (graph != null)
				{
					graph.OnAnyChange += OnGraphChange;
				}
				FireChange();
			}
		}
	}

	public bool IsEmpty
	{
		get
		{
			if (vars == null || vars.Count == 0)
			{
				return Graph == null;
			}
			return false;
		}
	}

	public BGField Field => this.field;

	public BGEntity Entity => entity;

	private void OnGraphChange()
	{
		FireChange();
	}

	public BGFieldCalcValue(BGField field, BGEntity entity)
	{
		this.field = field ?? throw new Exception("field can not be null");
		this.entity = entity ?? throw new Exception("entity can not be null");
	}

	public bool TryGet(BGId variableId, out object value)
	{
		value = null;
		if (vars == null)
		{
			return false;
		}
		for (int i = 0; i < vars.Count; i++)
		{
			BGFieldCalcVarRef bGFieldCalcVarRef = vars[i];
			if (!(bGFieldCalcVarRef.Id != variableId))
			{
				value = bGFieldCalcVarRef.Value;
				return true;
			}
		}
		return false;
	}

	public int RemoveVar(BGId id)
	{
		if (vars == null)
		{
			return 0;
		}
		int num = 0;
		for (int num2 = vars.Count - 1; num2 >= 0; num2--)
		{
			BGFieldCalcVarRef bGFieldCalcVarRef = vars[num2];
			if (!(bGFieldCalcVarRef.Id != id))
			{
				vars.RemoveAt(num2);
				num++;
			}
		}
		return num;
	}

	public BGFieldCalcVarRef AddVar(BGId id)
	{
		if (vars == null)
		{
			vars = BGFieldCalcVarRef.NewContainer();
		}
		if (GetVar(id) != null)
		{
			BGId bGId = id;
			throw new Exception("Var with such id=" + bGId.ToString() + " already added");
		}
		return vars.NewVar(id);
	}

	public BGFieldCalcVarRef FindVar(Predicate<BGFieldCalcVarRef> filter)
	{
		if (vars == null)
		{
			return null;
		}
		for (int i = 0; i < vars.Count; i++)
		{
			BGFieldCalcVarRef bGFieldCalcVarRef = vars[i];
			if (filter == null || filter(bGFieldCalcVarRef))
			{
				return bGFieldCalcVarRef;
			}
		}
		return null;
	}

	public BGFieldCalcVarRef GetVar(BGId id)
	{
		if (vars == null)
		{
			return null;
		}
		for (int i = 0; i < vars.Count; i++)
		{
			BGFieldCalcVarRef bGFieldCalcVarRef = vars[i];
			if (bGFieldCalcVarRef.Id == id)
			{
				return bGFieldCalcVarRef;
			}
		}
		return null;
	}

	private List<Tuple<BGFieldCalcVarRef, BGCalcTypeCode>> GetExistingVarsOverrides(BGCalcGraph parentGraph)
	{
		if (parentGraph?.GetVars() == null || vars == null)
		{
			return null;
		}
		List<Tuple<BGFieldCalcVarRef, BGCalcTypeCode>> existingVarsOverrides = null;
		BGCalcVarContainer parentGraphVars = parentGraph.GetVars();
		vars.ForEach((BGFieldCalcVarRef varRef) =>
		{
			BGCalcVar var = parentGraphVars.GetVar(varRef.Id);
			if (var != null)
			{
				existingVarsOverrides = existingVarsOverrides ?? new List<Tuple<BGFieldCalcVarRef, BGCalcTypeCode>>();
				existingVarsOverrides.Add(Tuple.Create(varRef, var.TypeCode));
			}
		});
		return existingVarsOverrides;
	}

	private bool CheckVar(BGId id, BGCalcTypeCode typeCode, BGCalcVarContainer parentVars)
	{
		BGCalcVar var = parentVars.GetVar(id);
		if (var == null)
		{
			BGId bGId = id;
			Debug.Log("Can not find variable with id " + bGId.ToString() + ", the overriden var value will be lost");
			return false;
		}
		if (!object.Equals(var.TypeCode, typeCode))
		{
			Debug.Log("Variable " + var.Name + " changed type from " + typeCode.Name + " to " + var.TypeCode.Name + ". Variable override value will be lost");
			return false;
		}
		return true;
	}

	public object CloneTo(BGEntity e)
	{
		BGFieldCalcValue clone = new BGFieldCalcValue(field, e);
		if (vars != null && vars.Count > 0)
		{
			clone.vars = new BGFieldCalcVarRef.VarRefContainer();
			vars.ForEach((BGFieldCalcVarRef varRef) =>
			{
				varRef.CloneTo(clone.vars);
			});
		}
		if (graph != null)
		{
			clone.graph = (BGCalcGraph)graph.Clone();
		}
		return clone;
	}

	public byte[] ToBytes(BGCalcGraph parentGraph)
	{
		List<Tuple<BGFieldCalcVarRef, BGCalcTypeCode>> existingVarsOverrides = GetExistingVarsOverrides(parentGraph);
		if (existingVarsOverrides == null && graph == null)
		{
			return null;
		}
		BGBinaryWriter writer = new BGBinaryWriter();
		writer.AddByte(1);
		if (graph != null)
		{
			writer.AddByte(1);
			writer.AddByteArray(graph.ToBytes());
		}
		else
		{
			writer.AddByte(2);
			writer.AddArray(() =>
			{
				foreach (Tuple<BGFieldCalcVarRef, BGCalcTypeCode> item in existingVarsOverrides)
				{
					writer.AddId(item.Item1.Id);
					writer.AddByte(item.Item2.TypeCode);
					item.Item2.ValueToBytes(writer, item.Item1.Value);
				}
			}, existingVarsOverrides.Count);
		}
		return writer.ToArray();
	}

	public void FromBytes(ArraySegment<byte> content, BGCalcGraph parentGraph)
	{
		BGBinaryReader reader = new BGBinaryReader(content);
		byte b = reader.ReadByte();
		if (b == 1)
		{
			byte b2 = reader.ReadByte();
			switch (b2)
			{
			case 1:
				graph = BGCalcGraph.ExistingGraph();
				graph.FromBytes(reader.ReadByteArray());
				graph.OnAnyChange += OnGraphChange;
				break;
			case 2:
				reader.ReadArray(() =>
				{
					BGId id = reader.ReadId();
					byte code = reader.ReadByte();
					BGCalcTypeCode bGCalcTypeCode = BGCalcTypeCodeRegistry.Get(code);
					object value = bGCalcTypeCode.ValueFromBytes(reader);
					if (CheckVar(id, bGCalcTypeCode, parentGraph.GetVars(createIfMissing: true)))
					{
						vars = EnsureContainer();
						vars.NewVar(id, value);
					}
				});
				break;
			default:
				throw new Exception("Unknown graph value state " + b2);
			}
			return;
		}
		throw new Exception("Unsupported version " + b);
	}

	private BGFieldCalcVarRef.VarRefContainer EnsureContainer()
	{
		if (vars != null)
		{
			return vars;
		}
		vars = BGFieldCalcVarRef.NewContainer();
		vars.OnAnyChange += FireChange;
		return vars;
	}

	public string ToJsonString(BGCalcGraph parentGraph)
	{
		JsonConfig jsonConfig = new JsonConfig();
		if (graph != null)
		{
			jsonConfig.Graph = new BGCalcGraphModel(graph);
		}
		if (vars != null && vars.Count > 0 && parentGraph != null && parentGraph.GetVars() != null)
		{
			List<Tuple<BGFieldCalcVarRef, BGCalcTypeCode>> existingVarsOverrides = GetExistingVarsOverrides(parentGraph);
			if (existingVarsOverrides != null)
			{
				jsonConfig.Vars = new List<CalcVarModel>();
				foreach (Tuple<BGFieldCalcVarRef, BGCalcTypeCode> item in existingVarsOverrides)
				{
					jsonConfig.Vars.Add(new CalcVarModel
					{
						Id = item.Item1.Id.ToString(),
						CodeRef = item.Item2.TypeCode,
						Value = item.Item2.ValueToString(item.Item1.Value)
					});
				}
			}
		}
		return JsonUtility.ToJson(jsonConfig);
	}

	public void FromJsonString(string value, BGCalcGraph parentGraph)
	{
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(value);
		if (jsonConfig.Vars != null && jsonConfig.Vars.Count > 0 && parentGraph != null && parentGraph.GetVars() != null)
		{
			vars = BGFieldCalcVarRef.NewContainer();
			BGCalcVarContainer parentVars = parentGraph.GetVars();
			foreach (CalcVarModel var in jsonConfig.Vars)
			{
				BGId id = BGId.Parse(var.Id);
				BGCalcTypeCode bGCalcTypeCode = BGCalcTypeCodeRegistry.Get(var.CodeRef);
				if (!CheckVar(id, bGCalcTypeCode, parentVars))
				{
					return;
				}
				vars.NewVar(id, bGCalcTypeCode.ValueFromString(var.Value));
			}
		}
		if (jsonConfig.Graph?.units != null && jsonConfig.Graph.units.Count > 0)
		{
			graph = BGCalcGraph.ExistingGraph();
			jsonConfig.Graph.ToGraph(graph);
			graph.OnAnyChange += OnGraphChange;
		}
	}

	private void FireChange()
	{
		field.FireValueChanged(entity);
	}

	protected bool Equals(BGFieldCalcValue other)
	{
		if (object.Equals(field, other.field) && object.Equals(entity, other.entity) && object.Equals(graph, other.graph))
		{
			return object.Equals(vars, other.vars);
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((BGFieldCalcValue)obj);
	}

	public override int GetHashCode()
	{
		int num = ((field != null) ? field.GetHashCode() : 0);
		num = (num * 397) ^ ((entity != null) ? entity.GetHashCode() : 0);
		num = (num * 397) ^ ((graph != null) ? graph.GetHashCode() : 0);
		return (num * 397) ^ ((vars != null) ? vars.GetHashCode() : 0);
	}
}
