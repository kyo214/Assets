using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public abstract class BGDBA
{
	public class FieldEventHandler
	{
		public readonly BGId metaId;

		public readonly BGId fieldId;

		public readonly BGId entityId;

		public readonly Action action;

		public FieldEventHandler(BGId metaId, BGId fieldId, BGId entityId, Action action)
		{
			this.metaId = metaId;
			this.fieldId = fieldId;
			this.entityId = entityId;
			this.action = action;
			BGField bGField = BGRepo.I.GetMeta(metaId)?.GetField(fieldId, errorIfNotFound: false);
			if (bGField != null)
			{
				bGField.ValueChanged += FieldListener;
			}
		}

		public void Release()
		{
			BGField bGField = BGRepo.I.GetMeta(metaId)?.GetField(fieldId, errorIfNotFound: false);
			if (bGField != null)
			{
				bGField.ValueChanged -= FieldListener;
			}
		}

		private void FieldListener(object sender, BGEventArgsField e)
		{
			if (e.Entity == null || !(e.Entity.Id != entityId))
			{
				action();
			}
		}
	}

	[SerializeField]
	private GameObject targetGameObject;

	[SerializeField]
	private Component targetComponent;

	[SerializeField]
	private string targetFieldName;

	[SerializeField]
	private bool isTargetProperty;

	[SerializeField]
	private List<BGDataBinderGoA.PathItem> path = new List<BGDataBinderGoA.PathItem>();

	[SerializeField]
	private bool includePrivate;

	[SerializeField]
	private bool liveUpdate;

	[NonSerialized]
	protected string error;

	[NonSerialized]
	private Type targetType;

	[NonSerialized]
	protected object target;

	[NonSerialized]
	protected PropertyInfo targetProperty;

	[NonSerialized]
	protected FieldInfo targetField;

	[NonSerialized]
	private int pathHashCode;

	[NonSerialized]
	protected readonly List<FieldEventHandler> eventHandlers = new List<FieldEventHandler>();

	public GameObject TargetGameObject
	{
		get
		{
			return targetGameObject;
		}
		set
		{
			targetGameObject = value;
		}
	}

	public Component TargetComponent
	{
		get
		{
			return targetComponent;
		}
		set
		{
			targetComponent = value;
		}
	}

	public string TargetFieldName
	{
		get
		{
			return targetFieldName;
		}
		set
		{
			targetFieldName = value;
		}
	}

	public bool IsTargetProperty
	{
		get
		{
			return isTargetProperty;
		}
		set
		{
			isTargetProperty = value;
		}
	}

	public string TargetAsString
	{
		get
		{
			if (targetComponent == null || string.IsNullOrEmpty(targetFieldName))
			{
				return null;
			}
			string text = targetComponent.GetType().Name;
			if (path != null && path.Count > 0)
			{
				for (int i = 0; i < path.Count; i++)
				{
					text = text + "." + path[i].Field;
				}
			}
			return text + "." + targetFieldName;
		}
	}

	public MemberInfo TargetAsMember
	{
		get
		{
			if (targetComponent == null)
			{
				return null;
			}
			object value = targetComponent;
			if (path != null && path.Count > 0)
			{
				for (int i = 0; i < path.Count; i++)
				{
					BGDataBinderGoA.PathItem pathItem = path[i];
					BindingFlags bindingAttr = (includePrivate ? (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Instance | BindingFlags.Public));
					if (pathItem.IsProperty)
					{
						PropertyInfo property = value.GetType().GetProperty(pathItem.Field, bindingAttr);
						if (property == null)
						{
							return null;
						}
						value = property.GetValue(value, null);
						if (value == null)
						{
							return null;
						}
					}
					else
					{
						FieldInfo field2 = value.GetType().GetField(pathItem.Field, bindingAttr);
						if (field2 == null)
						{
							return null;
						}
						value = field2.GetValue(value);
						if (value == null)
						{
							return null;
						}
					}
				}
			}
			if (isTargetProperty)
			{
				return value.GetType().GetProperty(targetFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			return value.GetType().GetField(targetFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}
	}

	public List<BGDataBinderGoA.PathItem> Path
	{
		get
		{
			return path;
		}
		set
		{
			path = value;
		}
	}

	public bool IncludePrivate
	{
		get
		{
			return includePrivate;
		}
		set
		{
			includePrivate = value;
		}
	}

	public bool LiveUpdate
	{
		get
		{
			return liveUpdate;
		}
		set
		{
			liveUpdate = value;
		}
	}

	public string Error
	{
		get
		{
			object valueToBind = ValueToBind;
			return error;
		}
		set
		{
			error = value;
		}
	}

	public virtual bool SupportReverseBinding => true;

	public virtual Type TargetType
	{
		get
		{
			EnsureTarget();
			return targetType;
		}
	}

	public abstract object ValueToBind { get; }

	protected void EnsureTarget()
	{
		if (targetType != null)
		{
			bool flag = false;
			if (isTargetProperty)
			{
				if (targetProperty != null && string.Equals(targetProperty.Name, targetFieldName))
				{
					flag = true;
				}
			}
			else if (targetField != null && string.Equals(targetField.Name, targetFieldName))
			{
				flag = true;
			}
			if (flag && pathHashCode == PathHashCode())
			{
				return;
			}
		}
		pathHashCode = -1;
		if (targetComponent == null)
		{
			error = "No target component";
			return;
		}
		target = targetComponent;
		if (!InitPath() || IsError(string.IsNullOrEmpty(targetFieldName), "Target field/property name is not defined"))
		{
			return;
		}
		if (isTargetProperty)
		{
			targetProperty = target.GetType().GetProperty(targetFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (IsError(targetProperty == null, "Can not find property: " + targetFieldName))
			{
				return;
			}
			targetType = targetProperty.PropertyType;
		}
		else
		{
			targetField = target.GetType().GetField(targetFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (IsError(targetField == null, "Can not find field: " + targetFieldName))
			{
				return;
			}
			targetType = targetField.FieldType;
		}
		pathHashCode = PathHashCode();
	}

	private int PathHashCode()
	{
		if (path == null || path.Count == 0)
		{
			return 0;
		}
		int num = 487;
		for (int i = 0; i < path.Count; i++)
		{
			num = num * 31 + path[i].GetHashCode();
		}
		return num;
	}

	public abstract object GetValue();

	public string Bind()
	{
		try
		{
			object valueToBind = ValueToBind;
			if (error != null)
			{
				return error;
			}
			if (isTargetProperty)
			{
				targetProperty.SetValue(target, valueToBind, null);
			}
			else
			{
				targetField.SetValue(target, valueToBind);
			}
		}
		catch (Exception ex)
		{
			error = ex.Message;
		}
		return error;
	}

	public virtual string ReverseBind()
	{
		return null;
	}

	private bool InitPath()
	{
		if (path == null || path.Count <= 0)
		{
			return true;
		}
		for (int i = 0; i < path.Count; i++)
		{
			BGDataBinderGoA.PathItem pathItem = path[i];
			BindingFlags bindingAttr = (includePrivate ? (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Instance | BindingFlags.Public));
			if (pathItem.IsProperty)
			{
				PropertyInfo property = target.GetType().GetProperty(pathItem.Field, bindingAttr);
				if (property == null)
				{
					error = "Can not find property: " + pathItem.Field;
					return false;
				}
				target = property.GetValue(target, null);
				if (target == null)
				{
					error = "Target object is null: " + pathItem.Field;
					return false;
				}
			}
			else
			{
				FieldInfo field = target.GetType().GetField(pathItem.Field, bindingAttr);
				if (field == null)
				{
					error = "Can not find field: " + pathItem.Field;
					return false;
				}
				target = field.GetValue(target);
				if (target == null)
				{
					error = "Target object is null: " + pathItem.Field;
					return false;
				}
			}
		}
		return true;
	}

	private bool IsError(bool condition, string error)
	{
		if (!condition)
		{
			return false;
		}
		this.error = error;
		return true;
	}

	public abstract int AddFieldsListeners(Action action);

	public virtual void RemoveFieldsListeners()
	{
		if (eventHandlers.Count <= 0)
		{
			return;
		}
		foreach (FieldEventHandler eventHandler in eventHandlers)
		{
			eventHandler.Release();
		}
		eventHandlers.Clear();
	}
}
