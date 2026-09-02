using System.Collections.Generic;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Reactor.Ticker;

public class TickService
{
	private readonly List<IUseTickService> m_Targets = new List<IUseTickService>();

	public UnityAction OnTick;

	public static int minFPS => 1;

	public static int maxFPS => 1000;

	public float tickInterval { get; private set; }

	public int fps { get; private set; }

	private List<IUseTickService> safeTargets { get; }

	public int registeredTargetsCount => m_Targets.Count;

	public bool hasRegisteredTargets => registeredTargetsCount > 0;

	public TickService(int fps)
	{
		SetFPS(fps);
		safeTargets = new List<IUseTickService>();
	}

	public void SetFPS(int value)
	{
		fps = Mathf.Max(minFPS, value);
		tickInterval = ReactorSettings.GetTickInterval(value);
	}

	public void SetFPS(FPS value)
	{
		SetFPS((int)value);
	}

	public void Register(IUseTickService target)
	{
		if (target != null && !m_Targets.Contains(target))
		{
			m_Targets.RemoveNulls();
			m_Targets.Add(target);
		}
	}

	public void Unregister(IUseTickService target)
	{
		if (target != null && m_Targets.Contains(target))
		{
			m_Targets.RemoveNulls();
			m_Targets.Remove(target);
		}
	}

	public void Tick()
	{
		m_Targets.RemoveNulls();
		safeTargets.Clear();
		safeTargets.AddRange(m_Targets);
		for (int i = 0; i < safeTargets.Count; i++)
		{
			safeTargets[i].Tick();
		}
		OnTick?.Invoke();
	}
}
