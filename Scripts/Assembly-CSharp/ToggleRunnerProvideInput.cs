using System.Collections.Generic;
using Fusion;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Fusion/Prototyping/Toggle Runner Provide Input")]
[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class ToggleRunnerProvideInput : Fusion.Behaviour
{
	private static ToggleRunnerProvideInput _instance;

	public void Awake()
	{
		if (NetworkProjectConfig.Global.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
		{
			Debug.LogWarning("ToggleRunnerProvideInput only works in Multi-Peer mode. Destroying.");
			Object.Destroy(this);
			return;
		}
		if ((bool)_instance)
		{
			Object.Destroy(this);
		}
		_instance = this;
	}

	public void Update()
	{
		if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftMeta)) && Input.GetKey(KeyCode.LeftShift))
		{
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				ToggleAll(-1);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				ToggleAll(0);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				ToggleAll(1);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				ToggleAll(2);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				ToggleAll(3);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				ToggleAll(4);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				ToggleAll(5);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				ToggleAll(6);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha8))
			{
				ToggleAll(7);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha9))
			{
				ToggleAll(8);
			}
		}
	}

	private void ToggleAll(int runnerIndex)
	{
		List<NetworkRunner>.Enumerator instancesEnumerator = NetworkRunner.GetInstancesEnumerator();
		int num = 0;
		while (instancesEnumerator.MoveNext())
		{
			NetworkRunner current = instancesEnumerator.Current;
			if (!(current == null) && current.IsRunning)
			{
				bool provideInput = runnerIndex == -1 || num == runnerIndex;
				current.ProvideInput = provideInput;
				num++;
			}
		}
	}
}
