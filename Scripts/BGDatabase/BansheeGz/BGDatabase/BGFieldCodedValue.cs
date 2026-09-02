using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGFieldCodedValue : BGFieldDictionaryClonebleValueI
{
	[Serializable]
	private class JsonConfig
	{
		public string DelegateClass;
	}

	private readonly BGField field;

	private readonly BGEntity entity;

	private string delegateClass;

	private BGCodedFieldDelegateI @delegate;

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
				delegateClass = value;
				@delegate = null;
				FireChange();
			}
		}
	}

	private BGCodedFieldDelegateI Delegate
	{
		get
		{
			if (@delegate != null)
			{
				return @delegate;
			}
			Type type = BGUtil.GetType(delegateClass);
			if (type == null)
			{
				return null;
			}
			try
			{
				@delegate = (BGCodedFieldDelegateI)Activator.CreateInstance(type);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return @delegate;
		}
	}

	public BGCodedFieldDelegateI DelegateInstance => Delegate;

	public BGFieldCodedValue(BGField field, BGEntity entity)
	{
		this.field = field ?? throw new Exception("field can not be null");
		this.entity = entity ?? throw new Exception("entity can not be null");
	}

	public object CloneTo(BGEntity e)
	{
		return new BGFieldCodedValue(field, e)
		{
			delegateClass = delegateClass
		};
	}

	public T Call<T>(BGFieldCodedA<T> fieldCodedA, BGEntity entity)
	{
		BGCodedFieldDelegateI bGCodedFieldDelegateI = Delegate;
		if (bGCodedFieldDelegateI == null)
		{
			throw new Exception("Can not create delegate class for programmable field value, class name is [" + delegateClass + "]");
		}
		if (!(bGCodedFieldDelegateI is BGCodedFieldDelegateI<T> bGCodedFieldDelegateI2))
		{
			throw new Exception("Can not cast delegate instance to generic interface BGCodedFieldDelegateI<T>, generic type is is [" + typeof(T).FullName + "]");
		}
		using BGCodedFieldContext bGCodedFieldContext = BGCodedFieldContext.Get();
		bGCodedFieldContext.Field = fieldCodedA;
		bGCodedFieldContext.Entity = entity;
		return bGCodedFieldDelegateI2.Get(bGCodedFieldContext);
	}

	private void FireChange()
	{
		field.FireValueChanged(entity);
	}

	public byte[] ToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter();
		bGBinaryWriter.AddByte(1);
		bGBinaryWriter.AddString(delegateClass);
		return bGBinaryWriter.ToArray();
	}

	public void FromBytes(ArraySegment<byte> content)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(content);
		byte b = bGBinaryReader.ReadByte();
		if (b == 1)
		{
			delegateClass = bGBinaryReader.ReadString();
			@delegate = null;
			return;
		}
		throw new Exception("Unsupported version " + b);
	}

	public string ToJsonString()
	{
		JsonConfig obj = new JsonConfig
		{
			DelegateClass = delegateClass
		};
		return JsonUtility.ToJson(obj);
	}

	public void FromJsonString(string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(value);
			delegateClass = jsonConfig.DelegateClass;
			@delegate = null;
		}
	}

	protected bool Equals(BGFieldCodedValue other)
	{
		return delegateClass == other.delegateClass;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((BGFieldCodedValue)obj);
	}

	public override int GetHashCode()
	{
		if (delegateClass == null)
		{
			return 0;
		}
		return delegateClass.GetHashCode();
	}

	public static bool operator ==(BGFieldCodedValue left, BGFieldCodedValue right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(BGFieldCodedValue left, BGFieldCodedValue right)
	{
		return !object.Equals(left, right);
	}
}
