using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGCalcUnit : BGCalcUnitI, BGCalcVarsLiteOwnerI, BGCalcVarsOwnerBaseI
{
	private readonly List<BGCalcControlInputI> inControls = new List<BGCalcControlInputI>();

	private readonly List<BGCalcControlOutputI> outControls = new List<BGCalcControlOutputI>();

	private readonly List<BGCalcValueInputI> inValues = new List<BGCalcValueInputI>();

	private readonly List<BGCalcValueOutputI> outValues = new List<BGCalcValueOutputI>();

	private BGCalcVarLiteContainer varsContainer;

	private Vector2 position;

	public BGCalcGraph Graph { get; set; }

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			if (!position.Equals(value))
			{
				position = value;
				FireOnAnyChange();
			}
		}
	}

	public virtual ushort TypeCode => 0;

	public List<BGCalcControlInputI> InControls => inControls;

	public List<BGCalcControlOutputI> OutControls => outControls;

	public List<BGCalcValueInputI> InValues => inValues;

	public List<BGCalcValueOutputI> OutValues => outValues;

	public virtual string Title
	{
		get
		{
			Type type = GetType();
			BGCalcUnitDefinitionAttribute attribute = BGUtil.GetAttribute<BGCalcUnitDefinitionAttribute>(type);
			if (string.IsNullOrEmpty(attribute.name))
			{
				return "[Unknown]";
			}
			int num = attribute.name.LastIndexOf('/');
			if (num == -1)
			{
				return attribute.name;
			}
			return attribute.name.Substring(num + 1);
		}
	}

	public List<BGCalcPortI> Ports
	{
		get
		{
			List<BGCalcPortI> list = new List<BGCalcPortI>(PortsCount);
			list.AddRange(inControls);
			list.AddRange(inValues);
			list.AddRange(outControls);
			list.AddRange(outValues);
			return list;
		}
	}

	public int PortsCount => inControls.Count + inValues.Count + outControls.Count + outValues.Count;

	public abstract void Definition();

	public BGCalcVarLiteContainer GetVars(bool createIfMissing = false)
	{
		if ((varsContainer == null) & createIfMissing)
		{
			varsContainer = new BGCalcVarLiteContainer(this);
		}
		return varsContainer;
	}

	public BGCalcVarLite GetVar(byte id)
	{
		return GetVars(createIfMissing: true).GetVar(id);
	}

	protected BGCalcVarLite GetOrAddVar(byte id, BGCalcTypeCode codeType)
	{
		BGCalcVarLiteContainer vars = GetVars(createIfMissing: true);
		BGCalcVarLite bGCalcVarLite = vars.GetVar(id);
		if (bGCalcVarLite == null)
		{
			bGCalcVarLite = BGCalcVarLite.Create(this, id, codeType);
		}
		return bGCalcVarLite;
	}

	public virtual string GetPublicVarLabel(byte varId)
	{
		return null;
	}

	protected BGCalcControlInput ControlInput(string name, string id, Func<BGCalcFlowI, BGCalcControlOutputI> action)
	{
		BGCalcControlInput bGCalcControlInput = new BGCalcControlInput(this, name, id, action);
		inControls.Add(bGCalcControlInput);
		return bGCalcControlInput;
	}

	protected BGCalcControlOutput ControlOutput(string name, string id)
	{
		BGCalcControlOutput bGCalcControlOutput = new BGCalcControlOutput(this, name, id);
		outControls.Add(bGCalcControlOutput);
		return bGCalcControlOutput;
	}

	protected BGCalcValueInput ValueInput<T>(string name, string id)
	{
		return ValueInput(typeof(T), name, id);
	}

	protected BGCalcValueInput ValueInput(Type type, string name, string id)
	{
		BGCalcValueInput bGCalcValueInput = new BGCalcValueInput(this, name, id, type);
		inValues.Add(bGCalcValueInput);
		return bGCalcValueInput;
	}

	protected BGCalcValueInput ValueInput(BGCalcTypeCode typeCode, string name, string id)
	{
		BGCalcValueInput bGCalcValueInput = new BGCalcValueInput(this, name, id, typeCode);
		inValues.Add(bGCalcValueInput);
		return bGCalcValueInput;
	}

	protected BGCalcValueOutput ValueOutput<T>(string name, string id, Func<BGCalcFlowI, T> getValue)
	{
		return ValueOutput(typeof(T), name, id, (BGCalcFlowI flow) => getValue(flow));
	}

	protected BGCalcValueOutput ValueOutput(Type type, string name, string id, Func<BGCalcFlowI, object> getValue)
	{
		BGCalcValueOutput bGCalcValueOutput = new BGCalcValueOutput(this, name, id, type, getValue);
		outValues.Add(bGCalcValueOutput);
		return bGCalcValueOutput;
	}

	protected BGCalcValueOutput ValueOutput(BGCalcTypeCode typeCode, string name, string id, Func<BGCalcFlowI, object> getValue)
	{
		BGCalcValueOutput bGCalcValueOutput = new BGCalcValueOutput(this, name, id, typeCode, getValue);
		outValues.Add(bGCalcValueOutput);
		return bGCalcValueOutput;
	}

	protected BGCalcValueOutput ValueOutput<T>(BGCalcTypeCode<T> typeCode, string name, string id, Func<BGCalcFlowI, T> getValue)
	{
		BGCalcValueOutput bGCalcValueOutput = new BGCalcValueOutput(this, name, id, typeCode, (getValue == null) ? null : ((Func<BGCalcFlowI, object>)((BGCalcFlowI flow) => getValue(flow))));
		outValues.Add(bGCalcValueOutput);
		return bGCalcValueOutput;
	}

	internal void CheckPortName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new Exception("Port name can not be null or empty string");
		}
		if (!char.IsLetter(name[0]))
		{
			throw new Exception($"Invalid port name {name[0]}. Port name should start with a letter");
		}
		foreach (char c in name)
		{
			if (!char.IsLetter(c) && c != '_')
			{
				throw new Exception($"Invalid character {c} in port name. Port name should contain letters and underscore only");
			}
		}
		CheckUnique(inControls, name);
		CheckUnique(outControls, name);
		CheckUnique(inValues, name);
		CheckUnique(outValues, name);
	}

	private void CheckUnique<T>(List<T> collection, string name) where T : BGCalcPortI
	{
		for (int i = 0; i < collection.Count; i++)
		{
			if (collection[i].Name == name)
			{
				throw new Exception("Unit " + GetType().Name + " already has a port with name " + name);
			}
		}
	}

	public BGCalcPortI FindPort(Predicate<BGCalcPortI> filter = null)
	{
		BGCalcPortI result = null;
		if (FindPort(inControls, filter, ref result))
		{
			return result;
		}
		if (FindPort(outControls, filter, ref result))
		{
			return result;
		}
		if (FindPort(inValues, filter, ref result))
		{
			return result;
		}
		if (FindPort(outValues, filter, ref result))
		{
			return result;
		}
		return null;
	}

	public List<BGCalcPortI> FindPorts(Predicate<BGCalcPortI> filter)
	{
		List<BGCalcPortI> result = new List<BGCalcPortI>();
		FindPorts(inControls, filter, result);
		FindPorts(outControls, filter, result);
		FindPorts(inValues, filter, result);
		FindPorts(outValues, filter, result);
		return result;
	}

	private void FindPorts<T>(List<T> ports, Predicate<BGCalcPortI> predicate, List<BGCalcPortI> result) where T : BGCalcPortI
	{
		for (int i = 0; i < ports.Count; i++)
		{
			T val = ports[i];
			if (predicate == null || predicate(val))
			{
				result.Add(val);
			}
		}
	}

	public BGCalcPortI FindPort(string id)
	{
		if (FindPort(id, inControls, out var findPort))
		{
			return findPort;
		}
		if (FindPort(id, outControls, out var findPort2))
		{
			return findPort2;
		}
		if (FindPort(id, inValues, out var findPort3))
		{
			return findPort3;
		}
		if (FindPort(id, outValues, out var findPort4))
		{
			return findPort4;
		}
		return null;
	}

	private static bool FindPort<T>(string id, List<T> ports, out BGCalcPortI findPort) where T : BGCalcPortI
	{
		findPort = null;
		for (int i = 0; i < ports.Count; i++)
		{
			T val = ports[i];
			if (string.Equals(val.Id, id, StringComparison.Ordinal))
			{
				findPort = val;
				return true;
			}
		}
		return false;
	}

	private bool FindPort<T>(List<T> list, Predicate<T> filter, ref BGCalcPortI result) where T : BGCalcPortI
	{
		if (filter == null)
		{
			if (list.Count == 0)
			{
				result = null;
				return false;
			}
			result = list[0];
			return true;
		}
		result = list.Find(filter);
		return result != null;
	}

	public void RemovePort(BGCalcPortI port)
	{
		BGCalcPortI result = null;
		switch (port.PortType)
		{
		case BGCalcPortTypeEnum.ControlIn:
			if (FindPort(inControls, new Predicate<BGCalcPortI>(Filter), ref result))
			{
				inControls.Remove((BGCalcControlInputI)port);
			}
			break;
		case BGCalcPortTypeEnum.ControlOut:
			if (FindPort(outControls, new Predicate<BGCalcPortI>(Filter), ref result))
			{
				outControls.Remove((BGCalcControlOutputI)port);
			}
			break;
		case BGCalcPortTypeEnum.ValueIn:
			if (FindPort(inValues, new Predicate<BGCalcPortI>(Filter), ref result))
			{
				inValues.Remove((BGCalcValueInputI)port);
			}
			break;
		case BGCalcPortTypeEnum.ValueOut:
			if (FindPort(outValues, new Predicate<BGCalcPortI>(Filter), ref result))
			{
				outValues.Remove((BGCalcValueOutputI)port);
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("PortType");
		}
		bool Filter(BGCalcPortI p)
		{
			return p.Id == port.Id;
		}
	}

	public override string ToString()
	{
		return Title;
	}

	public void FireOnAnyChange()
	{
		Graph?.FireOnAnyChange();
	}

	public void OnVarsChange()
	{
		FireOnAnyChange();
	}

	public bool IsEqual(BGCalcUnitI other)
	{
		if (!BGCalcVarContainerBaseA<BGCalcVarLite>.IsEqual(GetVars(), other.GetVars()))
		{
			return false;
		}
		if (!Position.Equals(other.Position))
		{
			return false;
		}
		if (!ListAreEqual(inControls, other.InControls))
		{
			return false;
		}
		if (!ListAreEqual(outControls, other.OutControls))
		{
			return false;
		}
		if (!ListAreEqual(inValues, other.InValues))
		{
			return false;
		}
		if (!ListAreEqual(outValues, other.OutValues))
		{
			return false;
		}
		return true;
	}

	private bool ListAreEqual<T>(List<T> list, List<T> list2) where T : BGCalcPortI
	{
		if (list.Count != list2.Count)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			T val = list[i];
			T val2 = list2[i];
			if (!val.IsEqual(val2))
			{
				return false;
			}
		}
		return true;
	}
}
