using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Signals;

namespace Doozy.Runtime.Mody;

[Serializable]
public abstract class ModyEventBase
{
	public const string k_DefaultEventName = "Unnamed";

	public string EventName;

	public List<ModyActionRunner> Runners;

	public bool hasRunners => Runners.Count > 0;

	public virtual bool hasCallbacks => hasRunners;

	protected ModyEventBase()
		: this("Unnamed")
	{
	}

	protected ModyEventBase(string eventName)
	{
		EventName = eventName;
		Runners = new List<ModyActionRunner>();
	}

	public virtual void Execute(Signal signal = null)
	{
		Runners.RemoveNulls();
		Runners.ForEach((ModyActionRunner r) =>
		{
			r.Execute();
		});
	}

	public bool RunsAction(ModyModule module, string actionName)
	{
		Runners.RemoveNulls();
		return Runners.Where((ModyActionRunner runner) => runner.Module == module).Any((ModyActionRunner runner) => runner.ActionName.Equals(actionName));
	}

	public bool RunsModule(ModyModule module)
	{
		Runners.RemoveNulls();
		return Runners.Any((ModyActionRunner runner) => runner.Module == module);
	}
}
