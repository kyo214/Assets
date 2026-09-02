using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[ExecuteAlways]
public static class MMEventManager
{
	private static Dictionary<Type, List<MMEventListenerBase>> _subscribersList;

	static MMEventManager()
	{
		_subscribersList = new Dictionary<Type, List<MMEventListenerBase>>();
	}

	public static void AddListener<MMEvent>(MMEventListener<MMEvent> listener) where MMEvent : struct
	{
		Type typeFromHandle = typeof(MMEvent);
		if (!_subscribersList.ContainsKey(typeFromHandle))
		{
			_subscribersList[typeFromHandle] = new List<MMEventListenerBase>();
		}
		if (!SubscriptionExists(typeFromHandle, listener))
		{
			_subscribersList[typeFromHandle].Add(listener);
		}
	}

	public static void RemoveListener<MMEvent>(MMEventListener<MMEvent> listener) where MMEvent : struct
	{
		Type typeFromHandle = typeof(MMEvent);
		if (!_subscribersList.ContainsKey(typeFromHandle))
		{
			return;
		}
		List<MMEventListenerBase> list = _subscribersList[typeFromHandle];
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num] == listener)
			{
				list.Remove(list[num]);
				if (list.Count == 0)
				{
					_subscribersList.Remove(typeFromHandle);
				}
				break;
			}
		}
	}

	public static void TriggerEvent<MMEvent>(MMEvent newEvent) where MMEvent : struct
	{
		if (_subscribersList.TryGetValue(typeof(MMEvent), out var value))
		{
			for (int num = value.Count - 1; num >= 0; num--)
			{
				(value[num] as MMEventListener<MMEvent>).OnMMEvent(newEvent);
			}
		}
	}

	private static bool SubscriptionExists(Type type, MMEventListenerBase receiver)
	{
		if (!_subscribersList.TryGetValue(type, out var value))
		{
			return false;
		}
		bool result = false;
		for (int num = value.Count - 1; num >= 0; num--)
		{
			if (value[num] == receiver)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
