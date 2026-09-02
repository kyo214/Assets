using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Mody.Actions;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Modules;

[AddComponentMenu("Mody/UnityEvent Module")]
public class UnityEventModule : ModyModule
{
	public const string k_DefaultModuleName = "UnityEvent";

	public UnityEvent Event = new UnityEvent();

	public SimpleModyAction InvokeEvent;

	public UnityEventModule()
		: this("UnityEvent")
	{
	}

	public UnityEventModule(string moduleName)
		: base(moduleName.IsNullOrEmpty() ? "UnityEvent" : moduleName)
	{
	}

	protected override void SetupActions()
	{
		this.AddAction(InvokeEvent ?? (InvokeEvent = new SimpleModyAction(this, "InvokeEvent", ExecuteInvokeEvent)));
	}

	public void ExecuteInvokeEvent()
	{
		Event?.Invoke();
	}
}
