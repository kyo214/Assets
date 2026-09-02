using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCUpdater : MonoBehaviour
{
	private Action _fixedUpdate;

	private Action _update;

	public void Initialize(Action fixedUpdate, Action update)
	{
		_fixedUpdate = fixedUpdate;
		_update = update;
		base.enabled = true;
	}

	public void Deinitialize()
	{
		base.enabled = false;
		_fixedUpdate = null;
		_update = null;
	}

	private void FixedUpdate()
	{
		if (_fixedUpdate != null)
		{
			_fixedUpdate();
		}
	}

	private void Update()
	{
		if (_update != null)
		{
			_update();
		}
	}
}
