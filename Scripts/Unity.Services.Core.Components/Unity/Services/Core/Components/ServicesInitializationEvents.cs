using System;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Services.Core.Components;

[Serializable]
public class ServicesInitializationEvents
{
	[SerializeField]
	public UnityEvent Initialized = new UnityEvent();

	[SerializeField]
	public UnityEvent<Exception> InitializeFailed = new UnityEvent<Exception>();
}
