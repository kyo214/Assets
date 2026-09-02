using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddComponentMenu("BansheeGz/BGDataBinderComponentsGo")]
public class BGDataBinderComponentsGo : BGDataBinderGoA
{
	public enum DataBinderComponentsSourceType : byte
	{
		Field = 0,
		Template = 1,
		Graph = 2
	}

	[SerializeField]
	[HideInInspector]
	private DataBinderComponentsSourceType sourceType;

	[SerializeField]
	[HideInInspector]
	private string metaIdString;

	[SerializeField]
	[HideInInspector]
	private string entityIdString;

	[SerializeField]
	[HideInInspector]
	private string fieldIdString;

	[SerializeField]
	[HideInInspector]
	private string template;

	[SerializeField]
	[HideInInspector]
	private byte typeCode = 3;

	[SerializeField]
	private byte[] graphContent;

	[SerializeField]
	[HideInInspector]
	private bool liveUpdate;

	[SerializeField]
	[HideInInspector]
	private string targetComponentClassName;

	[SerializeField]
	[HideInInspector]
	private string targetFieldName;

	[SerializeField]
	[HideInInspector]
	private string includeTag;

	[SerializeField]
	[HideInInspector]
	private string excludeTag;

	private bool listenersWasAdded;

	[NonSerialized]
	private BGDBA BindDelegate;

	private Type targetComponentType;

	public byte[] GraphContent
	{
		get
		{
			return graphContent;
		}
		set
		{
			graphContent = value;
		}
	}

	public string Template
	{
		get
		{
			return template;
		}
		set
		{
			template = value;
		}
	}

	public string IncludeTag
	{
		get
		{
			return includeTag;
		}
		set
		{
			includeTag = value;
		}
	}

	public string ExcludeTag
	{
		get
		{
			return excludeTag;
		}
		set
		{
			excludeTag = value;
		}
	}

	public override bool SupportReverseBinding => false;

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

	public DataBinderComponentsSourceType SourceType
	{
		get
		{
			return sourceType;
		}
		set
		{
			if (sourceType != value)
			{
				sourceType = value;
				BindDelegate = null;
			}
		}
	}

	public string TargetComponentClassName
	{
		get
		{
			return targetComponentClassName;
		}
		set
		{
			if (!(targetComponentClassName == value))
			{
				targetComponentClassName = value;
				targetComponentType = null;
			}
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
			if (!(targetFieldName == value))
			{
				targetFieldName = value;
			}
		}
	}

	public MemberInfo TargetField
	{
		get
		{
			Type type = TargetComponentType;
			if (type == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(targetFieldName))
			{
				return null;
			}
			MemberInfo memberInfo = type.GetProperty(targetFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (memberInfo == null)
			{
				memberInfo = type.GetField(targetFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			return memberInfo;
		}
	}

	public Type TargetComponentType
	{
		get
		{
			if (string.IsNullOrEmpty(targetComponentClassName))
			{
				return null;
			}
			if (targetComponentType != null && string.Equals(targetComponentType.FullName, targetComponentClassName))
			{
				return targetComponentType;
			}
			targetComponentType = BGUtil.GetType(targetComponentClassName);
			return targetComponentType;
		}
	}

	protected bool IsLiveUpdateOn
	{
		get
		{
			if (liveUpdate && (Application.isPlaying || BGUtil.TestIsRunning))
			{
				return Error == null;
			}
			return false;
		}
	}

	public override string Error
	{
		get
		{
			if (string.IsNullOrEmpty(targetComponentClassName))
			{
				return "Target component is not set";
			}
			Type type = TargetComponentType;
			if (type == null)
			{
				return "Can not load target type  " + targetComponentClassName;
			}
			if (!typeof(Component).IsAssignableFrom(type))
			{
				return "Target type  " + targetComponentClassName + " is not Unity component!";
			}
			MemberInfo targetField = TargetField;
			if (targetField == null)
			{
				return "Can not load target field/property " + targetFieldName + " at  " + targetComponentClassName + " class";
			}
			return null;
		}
	}

	public Type GraphTargetType
	{
		get
		{
			return typeCode switch
			{
				2 => typeof(bool), 
				3 => typeof(string), 
				4 => typeof(int), 
				5 => typeof(float), 
				_ => typeof(object), 
			};
		}
		set
		{
			if (value == typeof(bool))
			{
				typeCode = 2;
			}
			else if (value == typeof(string))
			{
				typeCode = 3;
			}
			else if (value == typeof(int))
			{
				typeCode = 4;
			}
			else if (value == typeof(float))
			{
				typeCode = 5;
			}
			else
			{
				typeCode = 10;
			}
		}
	}

	public byte GraphTypeCode
	{
		get
		{
			return typeCode;
		}
		set
		{
			typeCode = value;
		}
	}

	public BGCalcGraph Graph
	{
		get
		{
			InjectToDelegate();
			if (!(BindDelegate is BGDBGraph bGDBGraph))
			{
				return null;
			}
			return bGDBGraph.Graph;
		}
	}

	public string MetaIdString => metaIdString;

	public string EntityIdString => entityIdString;

	public string FieldIdString
	{
		get
		{
			return fieldIdString;
		}
		set
		{
			fieldIdString = value;
		}
	}

	public BGDBField.FieldPath FieldPath
	{
		get
		{
			InjectToDelegate();
			if (!(BindDelegate is BGDBField bGDBField))
			{
				return null;
			}
			return bGDBField.BindSourceProvider.FieldPath;
		}
	}

	public BGId MetaId => Meta?.Id ?? BGId.Empty;

	public BGMetaEntity Meta
	{
		get
		{
			InjectToDelegate();
			if (!(BindDelegate is BGDBField bGDBField))
			{
				return null;
			}
			return bGDBField.Meta;
		}
		set
		{
			if (value == null)
			{
				metaIdString = null;
				entityIdString = null;
				fieldIdString = null;
			}
			else
			{
				metaIdString = value.Id.ToString();
			}
			InjectToDelegate();
		}
	}

	public BGId EntityId
	{
		get
		{
			return Entity?.Id ?? BGId.Empty;
		}
		set
		{
			Entity = BGRepo.I.GetEntity(value);
		}
	}

	public BGEntity Entity
	{
		get
		{
			InjectToDelegate();
			if (!(BindDelegate is BGDBField bGDBField))
			{
				return null;
			}
			return bGDBField.Entity;
		}
		set
		{
			BGEntity entity = Entity;
			if ((value != null || entity != null) && (value == null || entity == null || !(entity.Id == value.Id)))
			{
				if (value == null)
				{
					metaIdString = null;
					entityIdString = null;
				}
				else
				{
					metaIdString = value.Meta.Id.ToString();
					entityIdString = value.Id.ToString();
				}
				InjectToDelegate();
			}
		}
	}

	private BGId FieldId => Field?.Id ?? BGId.Empty;

	public BGField Field
	{
		get
		{
			InjectToDelegate();
			if (!(BindDelegate is BGDBField bGDBField))
			{
				return null;
			}
			return bGDBField.Field;
		}
		set
		{
			if (value == null)
			{
				fieldIdString = null;
			}
			else
			{
				fieldIdString = value.Id.ToString();
				metaIdString = value.MetaId.ToString();
			}
			InjectToDelegate();
		}
	}

	protected override void OnDestroy()
	{
		RemoveListeners();
	}

	protected override void FirstBind()
	{
		Bind();
		AddListeners();
	}

	public override void Bind()
	{
		if (!bindedOnce)
		{
			bindedOnce = true;
			FirstBind();
		}
		else
		{
			BindInternal();
		}
		FireOnBind();
	}

	public override void ReverseBind()
	{
	}

	private void BindInternal()
	{
		try
		{
			InjectToDelegate();
			object value = BindDelegate.GetValue();
			Type type = TargetComponentType;
			if (type == null)
			{
				throw new Exception("Can not load target type component class " + targetComponentClassName);
			}
			if (!typeof(Component).IsAssignableFrom(type))
			{
				throw new Exception("Target type  " + targetComponentClassName + " is not Unity component!");
			}
			MemberInfo targetField = TargetField;
			if (targetField == null)
			{
				throw new Exception("Can not find target field/property with name " + targetFieldName + " at class " + targetComponentClassName);
			}
			FieldInfo fieldInfo = null;
			PropertyInfo propertyInfo = null;
			if (targetField is PropertyInfo)
			{
				propertyInfo = (PropertyInfo)targetField;
			}
			else
			{
				fieldInfo = (FieldInfo)targetField;
			}
			bool flag = !string.IsNullOrEmpty(includeTag);
			bool flag2 = !string.IsNullOrEmpty(excludeTag);
			UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(type);
			UnityEngine.Object[] array2 = array;
			foreach (UnityEngine.Object obj in array2)
			{
				Component component = (Component)obj;
				GameObject gameObject = component.gameObject;
				if (gameObject.scene.name != null && (!flag || gameObject.CompareTag(includeTag)) && (!flag2 || !gameObject.CompareTag(excludeTag)))
				{
					if (propertyInfo != null)
					{
						propertyInfo.SetValue(obj, value);
					}
					else
					{
						fieldInfo.SetValue(obj, value);
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.Log("BGDatabase.BGDataBinderComponentsGo: Exception while binding! See the exception log below for more details");
			Debug.LogException(exception);
		}
	}

	private HashSet<string> StringToHashSet(string tag)
	{
		return null;
	}

	internal void InjectToDelegate()
	{
		switch (sourceType)
		{
		case DataBinderComponentsSourceType.Field:
		{
			BGDBField bGDBField = (BGDBField)((BindDelegate is BGDBField bGDBField2) ? bGDBField2 : (BindDelegate = new BGDBField()));
			bGDBField.MetaIdString = metaIdString;
			bGDBField.FieldIdString = fieldIdString;
			bGDBField.EntityIdString = entityIdString;
			break;
		}
		case DataBinderComponentsSourceType.Template:
		{
			BGDBTemplate bGDBTemplate = (BGDBTemplate)((BindDelegate is BGDBTemplate bGDBTemplate2) ? bGDBTemplate2 : (BindDelegate = new BGDBTemplate()));
			bGDBTemplate.Template = template;
			break;
		}
		case DataBinderComponentsSourceType.Graph:
		{
			BGDBGraph bGDBGraph = (BGDBGraph)((BindDelegate is BGDBGraph bGDBGraph2) ? bGDBGraph2 : (BindDelegate = new BGDBGraph()));
			BGCalcGraph bGCalcGraph = bGDBGraph.Graph;
			if (bGCalcGraph == null)
			{
				if (graphContent != null && graphContent.Length != 0)
				{
					bGCalcGraph = BGCalcGraph.ExistingGraph();
					bGCalcGraph.FromBytes(new ArraySegment<byte>(graphContent));
				}
				else
				{
					bGCalcGraph = BGCalcGraph.NewGraph((typeCode == 0) ? BGCalcTypeCodeRegistry.String : BGCalcTypeCodeRegistry.Get(typeCode));
				}
			}
			bGDBGraph.Graph = bGCalcGraph;
			break;
		}
		default:
			throw new ArgumentOutOfRangeException("sourceType");
		}
		BindDelegate.Error = null;
	}

	protected void AddListeners()
	{
		if (IsLiveUpdateOn && !listenersWasAdded)
		{
			listenersWasAdded = true;
			BGRepo.OnLoad += OnLoad;
			BGRepo.I.Events.OnBatchUpdate += OnBatch;
			BindDelegate?.AddFieldsListeners(Bind);
		}
	}

	private void RemoveListeners()
	{
		if (listenersWasAdded)
		{
			BGRepo.OnLoad -= OnLoad;
			BGRepo.I.Events.OnBatchUpdate -= OnBatch;
			BindDelegate?.RemoveFieldsListeners();
		}
	}

	private void OnLoad(bool loaded)
	{
		if (loaded)
		{
			Bind();
		}
	}

	private void OnBatch(object sender, BGEventArgsBatch e)
	{
		Bind();
	}
}
