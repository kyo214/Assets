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
public abstract class ReflectedValue
{
	[Serializable]
	public struct SearchItem
	{
		public UnityAction<UnityEngine.Object> TargetSetter;

		public UnityAction<string> FieldSetter;

		public UnityAction<string> PropertySetter;

		public UnityEngine.Object target { get; }

		public List<string> fields { get; }

		public List<string> properties { get; }

		private string typeName => target.GetType().Name;

		private string GetPath(string s)
		{
			return typeName + "/" + s;
		}

		public List<KeyValuePair<string, UnityAction>> GetSearchActions()
		{
			List<KeyValuePair<string, UnityAction>> list = new List<KeyValuePair<string, UnityAction>>();
			for (int i = 0; i < fields.Count; i++)
			{
				string f = fields[i];
				SearchItem tmpThis = this;
				list.Add(new KeyValuePair<string, UnityAction>(tmpThis.GetPath(f), () =>
				{
					tmpThis.TargetSetter(tmpThis.target);
					tmpThis.FieldSetter(f);
				}));
			}
			for (int num = 0; num < properties.Count; num++)
			{
				string p = properties[num];
				SearchItem tmpThis2 = this;
				list.Add(new KeyValuePair<string, UnityAction>(tmpThis2.GetPath(p), () =>
				{
					tmpThis2.TargetSetter(tmpThis2.target);
					tmpThis2.PropertySetter(p);
				}));
			}
			return list;
		}

		public SearchItem(UnityEngine.Object target, IEnumerable<FieldInfo> fields, IEnumerable<PropertyInfo> properties, UnityAction<UnityEngine.Object> targetSetter, UnityAction<string> fieldSetter, UnityAction<string> propertySetter)
		{
			this.target = target;
			this.fields = fields.Select((FieldInfo f) => f.Name).ToList();
			this.properties = properties.Select((PropertyInfo p) => p.Name).ToList();
			TargetSetter = targetSetter;
			FieldSetter = fieldSetter;
			PropertySetter = propertySetter;
		}
	}

	[SerializeField]
	protected UnityEngine.Object Target;

	[SerializeField]
	protected string FieldName = "";

	[SerializeField]
	protected string PropertyName = "";

	[SerializeField]
	protected ValueDetails ValueDetails;

	public UnityEngine.Object target => Target;

	public string fieldName => FieldName;

	public string propertyName => PropertyName;

	public ValueDetails valueDetails => ValueDetails;

	protected FieldInfo targetField { get; set; }

	protected PropertyInfo targetProperty { get; set; }

	protected bool initialized { get; set; }

	protected HashSet<SearchItem> searchItems { get; set; }

	public abstract bool Initialize();

	public abstract bool IsValid();

	public abstract List<KeyValuePair<string, UnityAction>> GetSearchMenuItems();

	protected void SetTarget(UnityEngine.Object targetObject)
	{
		ClearValueDetails();
		Target = targetObject;
	}

	protected void SetProperty(string nameOfProperty)
	{
		FieldName = string.Empty;
		PropertyName = nameOfProperty;
		ValueDetails = ((!nameOfProperty.IsNullOrEmpty()) ? ValueDetails.IsProperty : ValueDetails.None);
	}

	protected void SetField(string nameOfField)
	{
		FieldName = nameOfField;
		PropertyName = string.Empty;
		ValueDetails = ((!nameOfField.IsNullOrEmpty()) ? ValueDetails.IsField : ValueDetails.None);
	}

	protected void ClearValueDetails()
	{
		FieldName = string.Empty;
		PropertyName = string.Empty;
		ValueDetails = ValueDetails.None;
		targetField = null;
		targetProperty = null;
	}

	protected GameObject GetGameObject()
	{
		UnityEngine.Object obj = Target;
		if (!(obj is GameObject result))
		{
			if (!(obj is Component { gameObject: var gameObject }))
			{
				return null;
			}
			return gameObject;
		}
		return result;
	}

	protected IEnumerable<FieldInfo> GetFieldInfos<T>(IReflect targetType)
	{
		return FieldInfos(targetType, typeof(T));
	}

	protected IEnumerable<PropertyInfo> GetPropertyInfos<T>(IReflect targetType)
	{
		return PropertyInfos(targetType, typeof(T));
	}

	protected IEnumerable<FieldInfo> GetFieldInfos<T>(UnityEngine.Object targetObject)
	{
		return GetFieldInfos<T>(targetObject.GetType());
	}

	protected IEnumerable<PropertyInfo> GetPropertyInfos<T>(UnityEngine.Object targetObject)
	{
		return GetPropertyInfos<T>(targetObject.GetType());
	}

	protected static IEnumerable<FieldInfo> FieldInfos(IReflect targetType, Type ofType)
	{
		return from f in targetType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
			where f.FieldType == ofType
			select f;
	}

	protected static IEnumerable<PropertyInfo> PropertyInfos(IReflect targetType, Type ofType)
	{
		return from p in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
			where p.PropertyType == ofType && (p.CanRead & p.CanWrite)
			select p;
	}
}
