using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "gradient", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerGradient")]
public class BGFieldGradient : BGFieldUnityClassA<Gradient>
{
	[Serializable]
	private class JsonModel
	{
		public GradientMode mode;

		public GradientColorKeyModel[] colorKeys;

		public GradientAlphaKeyModel[] alphaKeys;
	}

	[Serializable]
	private struct GradientColorKeyModel
	{
		public Color color;

		public float time;

		private GradientColorKeyModel(GradientColorKey valueColorKey)
		{
			color = valueColorKey.color;
			time = valueColorKey.time;
		}

		internal static GradientColorKeyModel[] From(GradientColorKey[] valueColorKeys)
		{
			if (valueColorKeys == null)
			{
				return null;
			}
			GradientColorKeyModel[] array = new GradientColorKeyModel[valueColorKeys.Length];
			for (int i = 0; i < valueColorKeys.Length; i++)
			{
				GradientColorKey valueColorKey = valueColorKeys[i];
				array[i] = new GradientColorKeyModel(valueColorKey);
			}
			return array;
		}

		public static GradientColorKey[] To(GradientColorKeyModel[] modelColorKeys)
		{
			if (modelColorKeys == null)
			{
				return null;
			}
			GradientColorKey[] array = new GradientColorKey[modelColorKeys.Length];
			for (int i = 0; i < modelColorKeys.Length; i++)
			{
				GradientColorKeyModel gradientColorKeyModel = modelColorKeys[i];
				array[i] = new GradientColorKey(gradientColorKeyModel.color, gradientColorKeyModel.time);
			}
			return array;
		}
	}

	[Serializable]
	private struct GradientAlphaKeyModel
	{
		public float alpha;

		public float time;

		private GradientAlphaKeyModel(GradientAlphaKey valueAlphaKey)
		{
			alpha = valueAlphaKey.alpha;
			time = valueAlphaKey.time;
		}

		internal static GradientAlphaKeyModel[] From(GradientAlphaKey[] valueAlphaKeys)
		{
			if (valueAlphaKeys == null)
			{
				return null;
			}
			GradientAlphaKeyModel[] array = new GradientAlphaKeyModel[valueAlphaKeys.Length];
			for (int i = 0; i < valueAlphaKeys.Length; i++)
			{
				GradientAlphaKey valueAlphaKey = valueAlphaKeys[i];
				array[i] = new GradientAlphaKeyModel(valueAlphaKey);
			}
			return array;
		}

		public static GradientAlphaKey[] To(GradientAlphaKeyModel[] modelColorKeys)
		{
			if (modelColorKeys == null)
			{
				return null;
			}
			GradientAlphaKey[] array = new GradientAlphaKey[modelColorKeys.Length];
			for (int i = 0; i < modelColorKeys.Length; i++)
			{
				GradientAlphaKeyModel gradientAlphaKeyModel = modelColorKeys[i];
				array[i] = new GradientAlphaKey(gradientAlphaKeyModel.alpha, gradientAlphaKeyModel.time);
			}
			return array;
		}
	}

	public const ushort CodeType = 62;

	private static readonly BGBinaryReader reader = new BGBinaryReader(null);

	private static readonly List<GradientColorKey> reusableList = new List<GradientColorKey>();

	private static readonly List<GradientAlphaKey> reusableList2 = new List<GradientAlphaKey>();

	public override ushort TypeCode => 62;

	public override bool SupportMultiThreadedLoading => false;

	public override int MinValueSize => 12;

	public BGFieldGradient(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldGradient(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldGradient(meta, id, name);
	}

	protected override Gradient FromBytes(ArraySegment<byte> segment)
	{
		reusableList.Clear();
		reusableList2.Clear();
		reader.Reset(segment);
		GradientMode mode = (GradientMode)reader.ReadInt();
		reader.ReadArray(() =>
		{
			reusableList.Add(new GradientColorKey(new Color(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat()), reader.ReadFloat()));
		});
		reader.ReadArray(() =>
		{
			reusableList2.Add(new GradientAlphaKey(reader.ReadFloat(), reader.ReadFloat()));
		});
		Gradient result = new Gradient
		{
			mode = mode,
			colorKeys = reusableList.ToArray(),
			alphaKeys = reusableList2.ToArray()
		};
		reusableList.Clear();
		reusableList2.Clear();
		reader.Dispose();
		return result;
	}

	protected override void ToBytes(BGBinaryWriter writer, Gradient value)
	{
		GradientColorKey[] colorKeys = value.colorKeys;
		GradientAlphaKey[] alphaKeys = value.alphaKeys;
		GradientColorKey[] array = colorKeys;
		int count = ((array != null) ? array.Length : 0);
		GradientAlphaKey[] array2 = alphaKeys;
		int count2 = ((array2 != null) ? array2.Length : 0);
		writer.AddInt((int)value.mode);
		writer.AddArray(() =>
		{
			GradientColorKey[] array3 = colorKeys;
			for (int i = 0; i < array3.Length; i++)
			{
				GradientColorKey gradientColorKey = array3[i];
				Color color = gradientColorKey.color;
				writer.AddFloat(color.r);
				writer.AddFloat(color.g);
				writer.AddFloat(color.b);
				writer.AddFloat(color.a);
				writer.AddFloat(gradientColorKey.time);
			}
		}, count);
		writer.AddArray(() =>
		{
			GradientAlphaKey[] array3 = alphaKeys;
			for (int i = 0; i < array3.Length; i++)
			{
				GradientAlphaKey gradientAlphaKey = array3[i];
				writer.AddFloat(gradientAlphaKey.alpha);
				writer.AddFloat(gradientAlphaKey.time);
			}
		}, count2);
	}

	protected override Gradient FromString(string value)
	{
		JsonModel jsonModel = JsonUtility.FromJson<JsonModel>(value);
		Gradient gradient = new Gradient
		{
			mode = jsonModel.mode
		};
		if (jsonModel.colorKeys != null)
		{
			gradient.colorKeys = GradientColorKeyModel.To(jsonModel.colorKeys);
		}
		if (jsonModel.alphaKeys != null)
		{
			gradient.alphaKeys = GradientAlphaKeyModel.To(jsonModel.alphaKeys);
		}
		return gradient;
	}

	protected override string ToString(Gradient value)
	{
		JsonModel obj = new JsonModel
		{
			mode = value.mode,
			colorKeys = GradientColorKeyModel.From(value.colorKeys),
			alphaKeys = GradientAlphaKeyModel.From(value.alphaKeys)
		};
		return JsonUtility.ToJson(obj);
	}

	public override Gradient CloneValue(Gradient value)
	{
		return Clone(value);
	}

	public override bool AreEqual(Gradient myValue, Gradient otherValue)
	{
		return Equals(myValue, otherValue);
	}

	public static bool Equals(Gradient myValue, Gradient otherValue)
	{
		if (myValue == null && otherValue == null)
		{
			return true;
		}
		if (myValue == null || otherValue == null)
		{
			return false;
		}
		if (myValue.mode != otherValue.mode)
		{
			return false;
		}
		GradientColorKey[] colorKeys = myValue.colorKeys;
		GradientColorKey[] colorKeys2 = otherValue.colorKeys;
		if (colorKeys == null && colorKeys2 == null)
		{
			return true;
		}
		if (colorKeys == null || colorKeys2 == null)
		{
			return false;
		}
		if (colorKeys.Length != colorKeys2.Length)
		{
			return false;
		}
		for (int i = 0; i < colorKeys.Length; i++)
		{
			GradientColorKey gradientColorKey = colorKeys[i];
			GradientColorKey gradientColorKey2 = colorKeys2[i];
			if (!Mathf.Approximately(gradientColorKey.time, gradientColorKey2.time))
			{
				return false;
			}
			Color color = gradientColorKey.color;
			Color color2 = gradientColorKey2.color;
			if (!Mathf.Approximately(color.r, color2.r))
			{
				return false;
			}
			if (!Mathf.Approximately(color.g, color2.g))
			{
				return false;
			}
			if (!Mathf.Approximately(color.b, color2.b))
			{
				return false;
			}
			if (!Mathf.Approximately(color.a, color2.a))
			{
				return false;
			}
		}
		GradientAlphaKey[] alphaKeys = myValue.alphaKeys;
		GradientAlphaKey[] alphaKeys2 = otherValue.alphaKeys;
		if (alphaKeys == null && alphaKeys2 == null)
		{
			return true;
		}
		if (alphaKeys == null || alphaKeys2 == null)
		{
			return false;
		}
		if (alphaKeys.Length != alphaKeys2.Length)
		{
			return false;
		}
		for (int j = 0; j < alphaKeys.Length; j++)
		{
			GradientAlphaKey gradientAlphaKey = alphaKeys[j];
			GradientAlphaKey gradientAlphaKey2 = alphaKeys2[j];
			if (!Mathf.Approximately(gradientAlphaKey.time, gradientAlphaKey2.time))
			{
				return false;
			}
			if (!Mathf.Approximately(gradientAlphaKey.alpha, gradientAlphaKey2.alpha))
			{
				return false;
			}
		}
		return true;
	}

	public static Gradient Clone(Gradient gradient)
	{
		if (gradient == null)
		{
			return null;
		}
		GradientColorKey[] colorKeys = gradient.colorKeys;
		GradientColorKey[] array;
		if (colorKeys == null)
		{
			array = null;
		}
		else
		{
			array = new GradientColorKey[colorKeys.Length];
			for (int i = 0; i < colorKeys.Length; i++)
			{
				GradientColorKey gradientColorKey = colorKeys[i];
				array[i] = new GradientColorKey(gradientColorKey.color, gradientColorKey.time);
			}
		}
		GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
		GradientAlphaKey[] array2;
		if (alphaKeys == null)
		{
			array2 = Array.Empty<GradientAlphaKey>();
		}
		else
		{
			array2 = new GradientAlphaKey[alphaKeys.Length];
			for (int j = 0; j < alphaKeys.Length; j++)
			{
				GradientAlphaKey gradientAlphaKey = alphaKeys[j];
				array2[j] = new GradientAlphaKey(gradientAlphaKey.alpha, gradientAlphaKey.time);
			}
		}
		return new Gradient
		{
			mode = gradient.mode,
			colorKeys = array,
			alphaKeys = array2
		};
	}
}
