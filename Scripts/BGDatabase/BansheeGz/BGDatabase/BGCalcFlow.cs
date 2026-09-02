using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGCalcFlow : BGCalcFlowI, BGCalcVarsOwnerI, BGCalcVarsOwnerBaseI
{
	private class FlowBranch
	{
		private readonly BGCalcFlowI flow;

		private readonly BGCalcControlInputI port;

		public FlowBranch(BGCalcFlowI flow, BGCalcControlInputI port)
		{
			this.flow = flow;
			this.port = port;
		}

		public void Run()
		{
			BGCalcControlInputI bGCalcControlInputI = port;
			while (bGCalcControlInputI != null)
			{
				BGCalcControlOutputI bGCalcControlOutputI;
				try
				{
					bGCalcControlOutputI = flow.Run(bGCalcControlInputI);
				}
				catch (Exception ex)
				{
					if (!ex.Data.Contains("u"))
					{
						ex.Data.Add("u", bGCalcControlInputI.Unit.Title);
					}
					throw;
				}
				bGCalcControlInputI = ((bGCalcControlOutputI == null || !bGCalcControlOutputI.IsConnected) ? null : bGCalcControlOutputI.ConnectedPort);
			}
		}
	}

	public const int MaximumIterations = 10000;

	public const string UnitExceptionKey = "u";

	private readonly Dictionary<BGCalcPortI, object> localVars = new Dictionary<BGCalcPortI, object>();

	private readonly BGCalcFlowContext context;

	private bool breakIsRequested;

	private readonly BGCalcVarContainer varsContainer;

	public object Result { get; set; }

	public bool BreakIsRequested
	{
		get
		{
			bool result = breakIsRequested;
			breakIsRequested = false;
			return result;
		}
		set
		{
			breakIsRequested = value;
		}
	}

	public int Level { get; set; }

	public BGCalcFlowI Parent { get; set; }

	public BGCalcFlowContext Context => context;

	public BGCalcFlow(BGCalcFlowContext context)
	{
		this.context = context;
		BGCalcGraph graph = context.Graph;
		BGCalcVarsProvider varsOverrides = context.VarsOverrides;
		varsContainer = new BGCalcVarContainer(this);
		BGCalcVarContainer vars = graph.GetVars();
		if (vars == null || vars.Variables.Count <= 0)
		{
			return;
		}
		foreach (BGCalcVar variable in graph.GetVars().Variables)
		{
			BGCalcVar bGCalcVar = variable.CloneTo(this, cloneId: true, cloneValue: true);
			if (varsOverrides != null && varsOverrides.TryGet(variable.Id, out var value))
			{
				bGCalcVar.Value = value;
			}
		}
	}

	public void Run()
	{
		localVars.Clear();
		BGCalcControlOutput startPort = context.Graph.StartUnit.StartPort;
		if (startPort.IsConnected)
		{
			new FlowBranch(this, startPort.ConnectedPort).Run();
		}
	}

	public BGCalcControlOutputI Run(BGCalcControlInputI port)
	{
		return port.Action(this);
	}

	public void RunNested(BGCalcControlInputI connectedPort)
	{
		new FlowBranch(this, connectedPort).Run();
	}

	public object GetLocalVar(BGCalcPortI port)
	{
		if (localVars.TryGetValue(port, out var value))
		{
			return value;
		}
		return null;
	}

	public T GetValue<T>(BGCalcValueInputI input)
	{
		return (T)GetValue(input);
	}

	public object GetValue(BGCalcValueInputI input)
	{
		if (localVars.TryGetValue(input, out var value))
		{
			return value;
		}
		BGCalcValueOutputI connectedPort = input.ConnectedPort;
		if (connectedPort != null)
		{
			object obj = GetValue(connectedPort);
			if (input.TypeCode != null)
			{
				if (connectedPort.TypeCode != null)
				{
					if (input.TypeCode.TypeCode != connectedPort.TypeCode.TypeCode)
					{
						obj = input.TypeCode.ConvertFrom(connectedPort.TypeCode, obj);
					}
				}
				else
				{
					obj = input.TypeCode.ConvertFrom(null, obj);
				}
			}
			return obj;
		}
		if (input.SupportDefaultValue && input.DefaultValue != null)
		{
			return input.DefaultValue;
		}
		throw new Exception("Can not get value from port " + input.Unit.Title + "." + input.Name + ": no connection and no default value!");
	}

	public object GetValue(BGCalcValueOutputI output)
	{
		if (localVars.TryGetValue(output, out var value))
		{
			return value;
		}
		if (output.GetValue != null)
		{
			try
			{
				return output.GetValue(this);
			}
			catch (Exception ex)
			{
				if (!ex.Data.Contains("u"))
				{
					ex.Data.Add("u", output.Unit.Title);
				}
				throw;
			}
		}
		throw new Exception("Can not get a value for output port [" + output.Unit.Title + "." + output.Name + "]: no local value and no function defined");
	}

	public void SetValue(BGCalcPortI port, object value)
	{
		localVars[port] = value;
	}

	public bool IsLocal(BGCalcPort port)
	{
		return localVars.ContainsKey(port);
	}

	public bool RemoveLocal(BGCalcPort port)
	{
		return localVars.Remove(port);
	}

	public BGCalcVarContainer GetVars(bool createIfMissing = false)
	{
		return varsContainer;
	}

	public void OnVarsChange()
	{
	}
}
