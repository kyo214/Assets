using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Reflection.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Reactor.Reflection.Internal;

[Serializable]
public abstract class MetaReflectedValue<T> : ReflectedValue
{
	public T value
	{
		get
		{
			return GetValue();
		}
		set
		{
			SetValue(value);
		}
	}

	public T GetValue()
	{
		if (!base.initialized)
		{
			Initialize();
		}
		if (!base.initialized)
		{
			return default;
		}
		return ValueDetails switch
		{
			ValueDetails.IsProperty => (T)base.targetProperty.GetValue(Target), 
			ValueDetails.IsField => (T)base.targetField.GetValue(Target), 
			ValueDetails.None => default, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public void SetValue(T newValue)
	{
		if (!base.initialized)
		{
			Initialize();
		}
		if (base.initialized)
		{
			switch (ValueDetails)
			{
			case ValueDetails.IsProperty:
				base.targetProperty.SetValue(Target, newValue);
				break;
			case ValueDetails.IsField:
				base.targetField.SetValue(Target, newValue);
				break;
			case ValueDetails.None:
				break;
			}
		}
	}

	public override bool Initialize()
	{
		base.initialized = false;
		base.targetField = null;
		base.targetProperty = null;
		if (!IsValid())
		{
			return false;
		}
		base.initialized = true;
		return true;
	}

	public override bool IsValid()
	{
		if (Target == null)
		{
			return false;
		}
		switch (ValueDetails)
		{
		case ValueDetails.None:
			return false;
		case ValueDetails.IsProperty:
			if (PropertyName.IsNullOrEmpty())
			{
				return false;
			}
			if (base.targetProperty != null && base.targetProperty.Name.Equals(PropertyName))
			{
				return true;
			}
			base.targetProperty = GetPropertyInfos<T>(Target).FirstOrDefault((PropertyInfo p) => p.Name.Equals(PropertyName));
			return base.targetProperty != null;
		case ValueDetails.IsField:
			if (FieldName.IsNullOrEmpty())
			{
				return false;
			}
			if (base.targetField != null && base.targetField.Name.Equals(FieldName))
			{
				return true;
			}
			base.targetField = GetFieldInfos<T>(Target).FirstOrDefault((FieldInfo f) => f.Name.Equals(FieldName));
			return base.targetField != null;
		default:
			return false;
		}
	}

	public override List<KeyValuePair<string, UnityAction>> GetSearchMenuItems()
	{
		if (base.searchItems == null)
		{
			HashSet<SearchItem> hashSet = (base.searchItems = new HashSet<SearchItem>());
		}
		base.searchItems.Clear();
		List<KeyValuePair<string, UnityAction>> list = new List<KeyValuePair<string, UnityAction>>();
		if (Target == null)
		{
			return list;
		}
		GameObject gameObject = GetGameObject();
		base.searchItems.Add(new SearchItem(gameObject, GetFieldInfos<T>(gameObject), GetPropertyInfos<T>(gameObject), base.SetTarget, base.SetField, base.SetProperty));
		Component[] components = gameObject.GetComponents(typeof(Component));
		foreach (Component targetObject in components)
		{
			base.searchItems.Add(new SearchItem(targetObject, GetFieldInfos<T>(targetObject), GetPropertyInfos<T>(targetObject), base.SetTarget, base.SetField, base.SetProperty));
		}
		foreach (SearchItem searchItem in base.searchItems)
		{
			foreach (KeyValuePair<string, UnityAction> searchAction in searchItem.GetSearchActions())
			{
				list.Add(new KeyValuePair<string, UnityAction>(searchAction.Key, searchAction.Value));
			}
		}
		return list;
	}
}
