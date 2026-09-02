using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "animationCurve2017", Folder = "Unity Primitive", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerAnimationCurve2017")]
public class BGFieldAnimationCurve2017 : BGFieldUnityClassA<AnimationCurve>
{
	[Serializable]
	private class JsonModel
	{
		public WrapMode preWrapMode;

		public WrapMode postWrapMode;

		public KeyframeModel[] keys;
	}

	[Serializable]
	private struct KeyframeModel(Keyframe key)
	{
		public float time = key.time;

		public float value = key.value;

		public float inTangent = key.inTangent;

		public float outTangent = key.outTangent;

		public int tangentMode = key.tangentMode;

		public Keyframe ToKeyframe()
		{
			Keyframe result = new Keyframe(time, value, inTangent, outTangent);
			result.tangentMode = tangentMode;
			return result;
		}
	}

	public const ushort CodeType = 59;

	private static readonly BGBinaryReader reader = new BGBinaryReader(null);

	private static readonly List<Keyframe> reusableList = new List<Keyframe>();

	public override ushort TypeCode => 59;

	public override bool SupportMultiThreadedLoading => false;

	public override int MinValueSize => 12;

	public BGFieldAnimationCurve2017(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldAnimationCurve2017(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldAnimationCurve2017(meta, id, name);
	}

	protected override AnimationCurve FromBytes(ArraySegment<byte> segment)
	{
		reusableList.Clear();
		reader.Reset(segment);
		WrapMode preWrapMode = (WrapMode)reader.ReadInt();
		WrapMode postWrapMode = (WrapMode)reader.ReadInt();
		reader.ReadArray(() =>
		{
			reusableList.Add(new Keyframe(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat())
			{
				tangentMode = reader.ReadInt()
			});
		});
		AnimationCurve result = new AnimationCurve(reusableList.ToArray())
		{
			preWrapMode = preWrapMode,
			postWrapMode = postWrapMode
		};
		reusableList.Clear();
		reader.Dispose();
		return result;
	}

	protected override void ToBytes(BGBinaryWriter writer, AnimationCurve value)
	{
		Keyframe[] keys = value.keys;
		int count = ((keys != null) ? keys.Length : 0);
		writer.AddInt((int)value.preWrapMode);
		writer.AddInt((int)value.postWrapMode);
		writer.AddArray(() =>
		{
			Keyframe[] keys2 = value.keys;
			for (int i = 0; i < keys2.Length; i++)
			{
				Keyframe keyframe = keys2[i];
				writer.AddFloat(keyframe.time);
				writer.AddFloat(keyframe.value);
				writer.AddFloat(keyframe.inTangent);
				writer.AddFloat(keyframe.outTangent);
				writer.AddInt(keyframe.tangentMode);
			}
		}, count);
	}

	protected override AnimationCurve FromString(string value)
	{
		JsonModel jsonModel = JsonUtility.FromJson<JsonModel>(value);
		AnimationCurve animationCurve = new AnimationCurve
		{
			preWrapMode = jsonModel.preWrapMode,
			postWrapMode = jsonModel.postWrapMode
		};
		if (jsonModel.keys != null && jsonModel.keys.Length != 0)
		{
			KeyframeModel[] keys = jsonModel.keys;
			foreach (KeyframeModel keyframeModel in keys)
			{
				animationCurve.AddKey(keyframeModel.ToKeyframe());
			}
		}
		return animationCurve;
	}

	protected override string ToString(AnimationCurve value)
	{
		JsonModel jsonModel = new JsonModel
		{
			preWrapMode = value.preWrapMode,
			postWrapMode = value.postWrapMode
		};
		Keyframe[] keys = value.keys;
		if (keys != null && keys.Length != 0)
		{
			jsonModel.keys = new KeyframeModel[keys.Length];
			for (int i = 0; i < keys.Length; i++)
			{
				Keyframe key = keys[i];
				jsonModel.keys[i] = new KeyframeModel(key);
			}
		}
		return JsonUtility.ToJson(jsonModel);
	}

	public override AnimationCurve CloneValue(AnimationCurve value)
	{
		return Clone(value);
	}

	public override bool AreEqual(AnimationCurve myValue, AnimationCurve otherValue)
	{
		return Equals(myValue, otherValue);
	}

	public static bool Equals(AnimationCurve myValue, AnimationCurve otherValue)
	{
		if (myValue == null && otherValue == null)
		{
			return true;
		}
		if (myValue == null || otherValue == null)
		{
			return false;
		}
		if (myValue.preWrapMode != otherValue.preWrapMode)
		{
			return false;
		}
		if (myValue.postWrapMode != otherValue.postWrapMode)
		{
			return false;
		}
		Keyframe[] keys = myValue.keys;
		Keyframe[] keys2 = otherValue.keys;
		if (keys == null && keys2 == null)
		{
			return true;
		}
		if (keys == null || keys2 == null)
		{
			return false;
		}
		if (keys.Length != keys2.Length)
		{
			return false;
		}
		for (int i = 0; i < keys.Length; i++)
		{
			Keyframe keyframe = keys[i];
			Keyframe keyframe2 = keys2[i];
			if (!Mathf.Approximately(keyframe.time, keyframe2.time))
			{
				return false;
			}
			if (!Mathf.Approximately(keyframe.value, keyframe2.value))
			{
				return false;
			}
			if (!Mathf.Approximately(keyframe.inTangent, keyframe2.inTangent))
			{
				return false;
			}
			if (!Mathf.Approximately(keyframe.outTangent, keyframe2.outTangent))
			{
				return false;
			}
			if (keyframe.tangentMode != keyframe2.tangentMode)
			{
				return false;
			}
		}
		return true;
	}

	public static AnimationCurve Clone(AnimationCurve curve)
	{
		if (curve == null)
		{
			return null;
		}
		Keyframe[] keys = curve.keys;
		Keyframe[] array;
		if (keys == null)
		{
			array = null;
		}
		else
		{
			array = new Keyframe[keys.Length];
			for (int i = 0; i < keys.Length; i++)
			{
				Keyframe keyframe = keys[i];
				array[i] = new Keyframe(keyframe.time, keyframe.value, keyframe.inTangent, keyframe.outTangent)
				{
					tangentMode = keyframe.tangentMode
				};
			}
		}
		return new AnimationCurve(array)
		{
			preWrapMode = curve.preWrapMode,
			postWrapMode = curve.postWrapMode
		};
	}
}
