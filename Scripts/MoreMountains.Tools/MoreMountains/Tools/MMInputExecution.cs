using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMInputExecution : MonoBehaviour
{
	[Header("Bindings")]
	public List<MMInputExecutionBinding> Bindings;

	protected virtual void Update()
	{
		HandleInput();
	}

	protected virtual void HandleInput()
	{
		foreach (MMInputExecutionBinding binding in Bindings)
		{
			binding.ProcessInput();
		}
	}
}
