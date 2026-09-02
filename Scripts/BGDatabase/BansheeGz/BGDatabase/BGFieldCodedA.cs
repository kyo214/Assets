using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCodedA<T> : BGFieldDictionaryBasedA<T, BGFieldCodedValue>, BGFieldCodedI, BGFieldWithCustomConfigI
{
	[Serializable]
	protected class JsonConfig
	{
		public string DelegateClass;
	}

	protected string delegateClass;

	private BGCodedFieldDelegateI<T> @delegate;

	private bool delegateLoadTried;

	public override bool ReadOnly => true;

	public override bool CustomStringFormatSupported => false;

	public override bool StoredValueIsTheSameAsValueType => false;

	public BGCodedFieldDelegateI DelegateInstance => Delegate;

	private BGCodedFieldDelegateI<T> Delegate
	{
		get
		{
			if (@delegate != null || delegateLoadTried)
			{
				return @delegate;
			}
			delegateLoadTried = true;
			if (string.IsNullOrEmpty(delegateClass))
			{
				return null;
			}
			Type type = BGUtil.GetType(delegateClass);
			if (type == null)
			{
				return null;
			}
			try
			{
				SetDelegate((BGCodedFieldDelegateI<T>)Activator.CreateInstance(type));
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return @delegate;
		}
		set
		{
			if (value != @delegate)
			{
				if (value == null)
				{
					delegateClass = null;
					SetDelegate(null);
				}
				else
				{
					delegateClass = value.GetType().FullName;
					SetDelegate(value);
				}
				base.events.MetaWasChanged(base.Meta);
			}
		}
	}

	public string DelegateClass
	{
		get
		{
			return delegateClass;
		}
		set
		{
			if (!(value == delegateClass))
			{
				SetDelegate(null);
				delegateLoadTried = false;
				delegateClass = value;
				base.events.MetaWasChanged(base.Meta);
			}
		}
	}

	public override T this[BGId entityId]
	{
		set
		{
		}
	}

	public override T this[int index]
	{
		set
		{
		}
	}

	public BGFieldCodedA(BGMetaEntity meta, string name, Type delegateType)
		: base(meta, name)
	{
		if (delegateType != null)
		{
			string errorForDelegateType = GetErrorForDelegateType(delegateType);
			if (!string.IsNullOrEmpty(errorForDelegateType))
			{
				base.Meta.Unregister(this);
				throw new BGException(errorForDelegateType);
			}
			delegateClass = delegateType.FullName;
		}
	}

	protected internal BGFieldCodedA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected internal override void Unload()
	{
		base.Unload();
		SetDelegate(null);
	}

	protected override BGFieldCodedValue Convert(BGEntity entity, T value)
	{
		throw new NotImplementedException();
	}

	protected override T Convert(BGEntity entity, BGFieldCodedValue value)
	{
		if (value != null)
		{
			return value.Call(this, entity);
		}
		if (!string.IsNullOrEmpty(DelegateClass))
		{
			return GetCodedValue(entity);
		}
		return default;
	}

	private T GetCodedValue(BGEntity entity)
	{
		BGCodedFieldDelegateI<T> bGCodedFieldDelegateI = Delegate;
		if (bGCodedFieldDelegateI == null)
		{
			throw new Exception("Delegate can not be created, class=" + delegateClass);
		}
		using BGCodedFieldContext bGCodedFieldContext = BGCodedFieldContext.Get();
		bGCodedFieldContext.Field = this;
		bGCodedFieldContext.Entity = entity;
		return bGCodedFieldDelegateI.Get(bGCodedFieldContext);
	}

	private void SetDelegate(BGCodedFieldDelegateI<T> newDelegate)
	{
		if (@delegate is BGCodedFieldDelegateLifeCycleI bGCodedFieldDelegateLifeCycleI)
		{
			try
			{
				bGCodedFieldDelegateLifeCycleI.OnUnload(new BGCodedFieldDelegateLifeCycleContext
				{
					Field = this
				});
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		@delegate = newDelegate;
		if (@delegate is BGCodedFieldDelegateLifeCycleI bGCodedFieldDelegateLifeCycleI2)
		{
			try
			{
				bGCodedFieldDelegateLifeCycleI2.OnLoad(new BGCodedFieldDelegateLifeCycleContext
				{
					Field = this
				});
			}
			catch (Exception exception2)
			{
				Debug.LogException(exception2);
			}
		}
	}

	public static string GetErrorForDelegateType(Type delegateType)
	{
		if (delegateType == null)
		{
			return null;
		}
		if (!typeof(BGCodedFieldDelegateI<T>).IsAssignableFrom(delegateType))
		{
			return BGUtil.Format("delegateType $ can not be casted to BGCodedFieldDelegateI interface with $ generic parameter", delegateType.FullName, typeof(T).FullName);
		}
		return null;
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			DelegateClass = delegateClass
		});
	}

	public override void ConfigFromString(string config)
	{
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		delegateClass = jsonConfig.DelegateClass;
		NullifyDelegate();
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(64);
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddString(delegateClass);
		ConfigToBytes(bGBinaryWriter);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			delegateClass = bGBinaryReader.ReadString();
			ConfigFromBytes(num, bGBinaryReader);
			NullifyDelegate();
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	protected virtual void ConfigToBytes(BGBinaryWriter writer)
	{
	}

	protected virtual void ConfigFromBytes(int version, BGBinaryReader reader)
	{
	}

	protected void NullifyDelegate()
	{
		SetDelegate(null);
		delegateLoadTried = false;
	}

	protected override byte[] ValueToBytes(BGFieldCodedValue value)
	{
		return value?.ToBytes();
	}

	protected override BGFieldCodedValue ValueFromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == 0)
		{
			return null;
		}
		BGFieldCodedValue bGFieldCodedValue = new BGFieldCodedValue(this, base.Meta.GetEntity(entityIndex));
		bGFieldCodedValue.FromBytes(segment);
		return bGFieldCodedValue;
	}

	protected override string ValueToString(BGFieldCodedValue value)
	{
		return value?.ToJsonString();
	}

	protected override BGFieldCodedValue ValueFromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		BGFieldCodedValue bGFieldCodedValue = new BGFieldCodedValue(this, base.Meta.GetEntity(entityIndex));
		bGFieldCodedValue.FromJsonString(value);
		return bGFieldCodedValue;
	}
}
