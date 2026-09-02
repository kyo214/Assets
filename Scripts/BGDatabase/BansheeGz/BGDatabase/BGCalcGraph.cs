using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGCalcGraph : BGCalcVarsOwnerI, BGCalcVarsOwnerBaseI, ICloneable
{
	public const int MaxUnitsNumber = 255;

	private readonly List<BGCalcUnitI> units = new List<BGCalcUnitI>();

	private BGCalcUnitGraphStart startUnit;

	private byte[] byteContent;

	private string stringContent;

	private bool disableEvents;

	private readonly BGCalcVarContainer varsContainer;

	public BGCalcUnitGraphStart StartUnit
	{
		get
		{
			EnsureGraphIsLoaded();
			return startUnit;
		}
	}

	public int UnitsCount
	{
		get
		{
			EnsureGraphIsLoaded();
			return units.Count;
		}
	}

	public event Action OnAnyChange;

	private BGCalcGraph()
	{
		varsContainer = new BGCalcVarContainer(this);
		varsContainer.OnDelete += OnVarDelete;
	}

	public BGCalcUnitI GetUnit(int index)
	{
		EnsureGraphIsLoaded();
		return units[index];
	}

	public BGCalcUnitI FindUnit(Predicate<BGCalcUnitI> filter = null)
	{
		EnsureGraphIsLoaded();
		for (int i = 0; i < units.Count; i++)
		{
			BGCalcUnitI bGCalcUnitI = units[i];
			if (filter == null || filter(bGCalcUnitI))
			{
				return bGCalcUnitI;
			}
		}
		return null;
	}

	public List<BGCalcUnitI> FindUnits(Predicate<BGCalcUnitI> filter = null, List<BGCalcUnitI> result = null)
	{
		EnsureGraphIsLoaded();
		List<BGCalcUnitI> list = result ?? new List<BGCalcUnitI>();
		for (int i = 0; i < units.Count; i++)
		{
			BGCalcUnitI bGCalcUnitI = units[i];
			if (filter == null || filter(bGCalcUnitI))
			{
				list.Add(bGCalcUnitI);
			}
		}
		return list;
	}

	public void AddUnitNoInit(BGCalcUnitI unit)
	{
		EnsureGraphIsLoaded();
		if (units.Count >= 255)
		{
			throw new Exception($"Maximum number of units [{255}] was already added!");
		}
		if (unit is BGCalcUnitGraphStart bGCalcUnitGraphStart)
		{
			if (startUnit != null)
			{
				throw new Exception("Can not add a start node cause start node already added!");
			}
			startUnit = bGCalcUnitGraphStart;
		}
		units.Add(unit);
		unit.Graph = this;
		FireOnAnyChange();
	}

	public void ForEachUnit(Action<BGCalcUnitI> action, Func<BGCalcUnitI, bool> filter = null)
	{
		EnsureGraphIsLoaded();
		for (int i = 0; i < units.Count; i++)
		{
			BGCalcUnitI bGCalcUnitI = units[i];
			if (filter == null || filter(bGCalcUnitI))
			{
				action(bGCalcUnitI);
			}
		}
	}

	public void RemoveUnit(BGCalcUnitI unit)
	{
		EnsureGraphIsLoaded();
		Batch(() =>
		{
			RemoveUnitInternal(unit);
		});
	}

	public void RemoveUnits(List<BGCalcUnitI> unitsToRemove)
	{
		if (unitsToRemove == null || unitsToRemove.Count == 0)
		{
			return;
		}
		EnsureGraphIsLoaded();
		Batch(() =>
		{
			foreach (BGCalcUnitI item in unitsToRemove)
			{
				RemoveUnitInternal(item);
			}
		});
	}

	private void RemoveUnitInternal(BGCalcUnitI unit)
	{
		if (unit is BGCalcUnitGraphStart)
		{
			throw new Exception("Can not delete a start unit");
		}
		foreach (BGCalcPortI port in unit.Ports)
		{
			port.DisconnectAll();
		}
		units.Remove(unit);
	}

	public BGCalcVarContainer GetVars(bool createIfMissing = false)
	{
		EnsureGraphIsLoaded();
		return varsContainer;
	}

	private void OnVarDelete(List<BGCalcVar> list)
	{
		List<BGCalcUnitI> list2 = null;
		foreach (BGCalcUnitI unit in units)
		{
			if (!(unit is BGCalcUnitVarA bGCalcUnitVarA))
			{
				continue;
			}
			foreach (BGCalcVar item in list)
			{
				BGCalcVar graphVar = bGCalcUnitVarA.GraphVar;
				if (object.Equals(graphVar, item))
				{
					list2 = list2 ?? new List<BGCalcUnitI>();
					list2.Add(bGCalcUnitVarA);
				}
			}
		}
		RemoveUnits(list2);
	}

	private void OnBeforeAdd(BGCalcVar variable)
	{
		if (variable == null)
		{
			throw new Exception("Variable is null");
		}
		string text = BGMetaObject.CheckName(variable.Name);
		if (text != null)
		{
			throw new Exception(text);
		}
		if (varsContainer.HasVar(variable.Id))
		{
			throw new Exception("var with such id already added!");
		}
	}

	public void OnVarsChange()
	{
		FireOnAnyChange();
	}

	public BGCalcFlowI Launch(BGCalcFlowContext context)
	{
		EnsureGraphIsLoaded();
		BGCalcControlOutput startPort = StartUnit.StartPort;
		context.Graph = this;
		BGCalcFlow bGCalcFlow = new BGCalcFlow(context);
		if (!startPort.IsConnected)
		{
			return bGCalcFlow;
		}
		bGCalcFlow.Run();
		return bGCalcFlow;
	}

	public object Execute(BGCalcFlowContext context)
	{
		BGCalcFlowI bGCalcFlowI = Launch(context);
		return bGCalcFlowI.Result;
	}

	public T Execute<T>(BGCalcFlowContext context)
	{
		object obj = Execute(context);
		if (obj == null)
		{
			return default;
		}
		return (T)obj;
	}

	public byte[] ToBytes()
	{
		EnsureGraphIsLoaded();
		BGCalcSaver bGCalcSaver = new BGCalcSaver(this);
		return bGCalcSaver.Save();
	}

	public void FromBytes(ArraySegment<byte> arraySegment)
	{
		byteContent = BGUtil.ToArray(arraySegment);
		stringContent = null;
	}

	public string ToJsonString()
	{
		EnsureGraphIsLoaded();
		BGCalcSaverString bGCalcSaverString = new BGCalcSaverString(this);
		return bGCalcSaverString.Save();
	}

	public void FromJsonString(string json)
	{
		stringContent = json;
		byteContent = null;
	}

	public void EnsureGraphIsLoaded()
	{
		if (byteContent != null)
		{
			Batch(() =>
			{
				byte[] array = byteContent;
				Clear();
				BGCalcLoader bGCalcLoader = new BGCalcLoader(this, new ArraySegment<byte>(array));
				bGCalcLoader.Load();
			}, fireEventInTheEnd: false);
		}
		else if (stringContent != null)
		{
			Batch(() =>
			{
				string json = stringContent;
				Clear();
				BGCalcLoaderString bGCalcLoaderString = new BGCalcLoaderString();
				bGCalcLoaderString.Load(this, json);
			}, fireEventInTheEnd: false);
		}
	}

	public object Clone()
	{
		byte[] array = new BGCalcSaver(this).Save();
		BGCalcGraph bGCalcGraph = ExistingGraph();
		BGCalcLoader bGCalcLoader = new BGCalcLoader(bGCalcGraph, new ArraySegment<byte>(array));
		bGCalcLoader.Load();
		return bGCalcGraph;
	}

	public void FireOnAnyChange()
	{
		if (!disableEvents)
		{
			OnAnyChange?.Invoke();
		}
	}

	public void Clear()
	{
		units.Clear();
		varsContainer.ClearVarsNoEvent();
		startUnit = null;
		byteContent = null;
		stringContent = null;
	}

	public void Batch(Action action, bool fireEventInTheEnd = true)
	{
		disableEvents = true;
		try
		{
			action();
		}
		finally
		{
			disableEvents = false;
		}
		if (fireEventInTheEnd)
		{
			FireOnAnyChange();
		}
	}

	public void ClearOnAnyChange()
	{
		OnAnyChange = null;
	}

	public bool IsEqual(BGCalcGraph other)
	{
		if (!BGCalcVarContainerBaseA<BGCalcVar>.IsEqual(GetVars(), other.GetVars()))
		{
			return false;
		}
		if (units.Count != other.units.Count)
		{
			return false;
		}
		for (int i = 0; i < units.Count; i++)
		{
			BGCalcUnitI bGCalcUnitI = units[i];
			BGCalcUnitI other2 = other.units[i];
			if (!bGCalcUnitI.IsEqual(other2))
			{
				return false;
			}
		}
		return true;
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
		if (!(obj is BGCalcGraph other))
		{
			return false;
		}
		return IsEqual(other);
	}

	public override int GetHashCode()
	{
		if (units == null)
		{
			return 0;
		}
		return units.GetHashCode();
	}

	public static BGCalcGraph NewGraph(BGCalcTypeCode resultCode)
	{
		BGCalcGraph bGCalcGraph = new BGCalcGraph();
		bGCalcGraph.AddUnitNoInit(new BGCalcUnitGraphStart());
		bGCalcGraph.Init(bGCalcGraph.StartUnit);
		if (resultCode != null && !object.Equals(resultCode, BGCalcTypeCodeRegistry.CalcAction))
		{
			BGCalcUnitSetResult bGCalcUnitSetResult = new BGCalcUnitSetResult();
			bGCalcUnitSetResult.Init(resultCode);
			bGCalcUnitSetResult.Position = new Vector2(250f, 0f);
			bGCalcGraph.AddUnit(bGCalcUnitSetResult);
			BGCalcControlOutputI bGCalcControlOutputI = (BGCalcControlOutputI)bGCalcGraph.StartUnit.FindPort();
			BGCalcPortI port = bGCalcUnitSetResult.FindPort((BGCalcPortI p) => p is BGCalcControlInput);
			bGCalcControlOutputI.Connect(port);
		}
		return bGCalcGraph;
	}

	public static BGCalcGraph ExistingGraph()
	{
		return new BGCalcGraph();
	}

	public void Init(BGCalcUnitI unitI)
	{
		unitI.Graph = this;
		unitI.Definition();
	}

	public void AddUnit(BGCalcUnitI unitI)
	{
		Init(unitI);
		AddUnitNoInit(unitI);
	}
}
