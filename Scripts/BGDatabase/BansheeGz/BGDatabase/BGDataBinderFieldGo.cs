using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddComponentMenu("BansheeGz/BGDataBinderFieldGo")]
public class BGDataBinderFieldGo : BGDataBinderSingleGoA<BGDBField>
{
	[SerializeField]
	[HideInInspector]
	private string targetTypeString = typeof(string).FullName;

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
	private short functionCode;

	[SerializeField]
	[HideInInspector]
	private string functionClass;

	[SerializeField]
	[HideInInspector]
	private bool liveUpdate;

	[NonSerialized]
	private Type targetType;

	[NonSerialized]
	private bool listenersWasAdded;

	[NonSerialized]
	private bool dirty;

	public string TargetTypeString => targetTypeString;

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
			dirty = true;
		}
	}

	public BGId MetaId => Meta?.Id ?? BGId.Empty;

	public BGMetaEntity Meta
	{
		get
		{
			InjectToDelegate();
			return BindDelegate.Meta;
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
			dirty = true;
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
			return BindDelegate.Entity;
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
				dirty = true;
			}
		}
	}

	private BGId FieldId => Field?.Id ?? BGId.Empty;

	public BGField Field
	{
		get
		{
			InjectToDelegate();
			return BindDelegate.Field;
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
			functionCode = 0;
			functionClass = null;
			InjectToDelegate();
			dirty = true;
		}
	}

	public BGDBField.FieldPath FieldPath
	{
		get
		{
			InjectToDelegate();
			return BindDelegate.BindSourceProvider.FieldPath;
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

	public bool IsUsingSpecialField => BindDelegate.IsUsingSpecialField;

	public override Type TargetType
	{
		get
		{
			if (string.IsNullOrEmpty(targetTypeString))
			{
				return null;
			}
			if (targetType == null || !targetType.Name.Equals(targetTypeString))
			{
				targetType = BGUtil.GetType(targetTypeString);
			}
			return targetType;
		}
		set
		{
			if (value == null)
			{
				targetTypeString = null;
				targetType = null;
			}
			else
			{
				targetType = value;
				targetTypeString = targetType.FullName;
			}
		}
	}

	protected bool IsLiveUpdateOn
	{
		get
		{
			if (liveUpdate && (Application.isPlaying || BGUtil.TestIsRunning))
			{
				return BindDelegate.Error == null;
			}
			return false;
		}
	}

	public short FunctionCode
	{
		get
		{
			return functionCode;
		}
		set
		{
			functionCode = value;
		}
	}

	public string FunctionClass
	{
		get
		{
			return functionClass;
		}
		set
		{
			functionClass = value;
		}
	}

	protected override void InjectToDelegate()
	{
		base.InjectToDelegate();
		BindDelegate.MetaIdString = metaIdString;
		BindDelegate.FieldIdString = fieldIdString;
		BindDelegate.EntityIdString = entityIdString;
		BindDelegate.FunctionCode = functionCode;
		BindDelegate.FunctionClass = functionClass;
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
		if (e.WasEntitiesUpdated(MetaId))
		{
			Bind();
		}
	}

	public override void Bind()
	{
		bool flag = bindedOnce;
		base.Bind();
		if (flag && IsLiveUpdateOn && dirty)
		{
			dirty = false;
			BindDelegate.AddFieldsListeners(Bind);
		}
	}

	protected override void OnDestroy()
	{
		if (listenersWasAdded)
		{
			BGRepo.OnLoad -= OnLoad;
			BGRepo.I.Events.OnBatchUpdate -= OnBatch;
			BindDelegate.RemoveFieldsListeners();
		}
	}

	protected override void AddListeners()
	{
		if (IsLiveUpdateOn && !listenersWasAdded)
		{
			listenersWasAdded = true;
			BGRepo.OnLoad += OnLoad;
			BGRepo.I.Events.OnBatchUpdate += OnBatch;
			BindDelegate.AddFieldsListeners(Bind);
		}
	}

	public void AssignFunction(BGFBFuntion function)
	{
		if (function == null)
		{
			FunctionCode = 0;
			FunctionClass = null;
		}
		else if (function is BGFBFuntionToString)
		{
			FunctionCode = 2;
			FunctionClass = null;
		}
		else
		{
			FunctionCode = 1;
			FunctionClass = function.GetType().FullName;
		}
	}
}
