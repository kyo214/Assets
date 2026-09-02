using System;
using UnityEngine;

namespace Doozy.Runtime.Signals;

[Serializable]
public class SignalPayload
{
	public enum ValueType
	{
		None = 0,
		Integer = 1,
		Boolean = 2,
		Float = 3,
		String = 4,
		Color = 5,
		Vector2 = 6,
		Vector3 = 7,
		Vector4 = 8
	}

	[SerializeField]
	private StreamId StreamId;

	[SerializeField]
	private ValueType SignalValueType;

	[SerializeField]
	private int IntegerValue;

	[SerializeField]
	private bool BooleanValue;

	[SerializeField]
	private float FloatValue;

	[SerializeField]
	private string StringValue;

	[SerializeField]
	private Color ColorValue;

	[SerializeField]
	private Vector2 Vector2Value;

	[SerializeField]
	private Vector3 Vector3Value;

	[SerializeField]
	private Vector4 Vector4Value;

	public StreamId streamId
	{
		get
		{
			return StreamId;
		}
		set
		{
			StreamId = value;
		}
	}

	public ValueType signalValueType
	{
		get
		{
			return SignalValueType;
		}
		set
		{
			SignalValueType = value;
		}
	}

	public int integerValue
	{
		get
		{
			return IntegerValue;
		}
		set
		{
			Reset();
			SignalValueType = ValueType.Integer;
			IntegerValue = value;
		}
	}

	public bool booleanValue
	{
		get
		{
			return BooleanValue;
		}
		set
		{
			Reset();
			SignalValueType = ValueType.Boolean;
			BooleanValue = value;
		}
	}

	public float floatValue
	{
		get
		{
			return FloatValue;
		}
		set
		{
			Reset();
			SignalValueType = ValueType.Float;
			FloatValue = value;
		}
	}

	public string stringValue
	{
		get
		{
			return StringValue;
		}
		set
		{
			Reset();
			SignalValueType = ValueType.String;
			StringValue = value;
		}
	}

	public Color colorValue
	{
		get
		{
			return ColorValue;
		}
		set
		{
			Reset();
			SignalValueType = ValueType.Color;
			ColorValue = value;
		}
	}

	public Vector2 vector2Value
	{
		get
		{
			return Vector2Value;
		}
		set
		{
			Reset();
			SignalValueType = ValueType.Vector2;
			Vector2Value = value;
		}
	}

	public Vector3 vector3Value
	{
		get
		{
			return Vector3Value;
		}
		set
		{
			Reset();
			SignalValueType = ValueType.Vector3;
			Vector3Value = value;
		}
	}

	public Vector4 vector4Value
	{
		get
		{
			return Vector4Value;
		}
		set
		{
			Reset();
			SignalValueType = ValueType.Vector4;
			Vector4Value = value;
		}
	}

	public SignalPayload()
	{
		Reset();
	}

	public void Reset()
	{
		SignalValueType = ValueType.None;
		IntegerValue = 0;
		BooleanValue = false;
		FloatValue = 0f;
		StringValue = null;
		ColorValue = default;
		Vector2Value = default;
		Vector3Value = default;
		Vector4Value = default;
	}

	public SignalPayload SetValue(int value)
	{
		integerValue = value;
		return this;
	}

	public SignalPayload SetValue(bool value)
	{
		booleanValue = value;
		return this;
	}

	public SignalPayload SetValue(float value)
	{
		floatValue = value;
		return this;
	}

	public SignalPayload SetValue(string value)
	{
		stringValue = value;
		return this;
	}

	public SignalPayload SetValue(Color value)
	{
		colorValue = value;
		return this;
	}

	public SignalPayload SetValue(Vector2 value)
	{
		vector2Value = value;
		return this;
	}

	public SignalPayload SetValue(Vector3 value)
	{
		vector3Value = value;
		return this;
	}

	public SignalPayload SetValue(Vector4 value)
	{
		vector4Value = value;
		return this;
	}

	public SignalPayload SendSignal()
	{
		if (StreamId.Category.Equals("None"))
		{
			return this;
		}
		if (StreamId.Name.Equals("None"))
		{
			return this;
		}
		SignalStream stream = SignalsService.GetStream(StreamId.Category, StreamId.Name);
		switch (SignalValueType)
		{
		case ValueType.None:
			stream.SendSignal();
			break;
		case ValueType.Integer:
			stream.SendSignal(integerValue);
			break;
		case ValueType.Boolean:
			stream.SendSignal(booleanValue);
			break;
		case ValueType.Float:
			stream.SendSignal(floatValue);
			break;
		case ValueType.String:
			stream.SendSignal(stringValue, "");
			break;
		case ValueType.Color:
			stream.SendSignal(colorValue);
			break;
		case ValueType.Vector2:
			stream.SendSignal(vector2Value);
			break;
		case ValueType.Vector3:
			stream.SendSignal(vector3Value);
			break;
		case ValueType.Vector4:
			stream.SendSignal(vector4Value);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return this;
	}

	public override string ToString()
	{
		string text = "";
		return string.Concat(text + SignalValueType switch
		{
			ValueType.None => string.Empty, 
			ValueType.Integer => "(int)", 
			ValueType.Boolean => "(bool)", 
			ValueType.Float => "(float)", 
			ValueType.String => "(string)", 
			ValueType.Color => "(color)", 
			ValueType.Vector2 => "(Vector2)", 
			ValueType.Vector3 => "(Vector3)", 
			ValueType.Vector4 => "(Vector4)", 
			_ => throw new ArgumentOutOfRangeException(), 
		}, $" {streamId}");
	}
}
