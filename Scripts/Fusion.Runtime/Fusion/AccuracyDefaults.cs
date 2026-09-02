using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion;

[Serializable]
public class AccuracyDefaults
{
	internal const int CORE_COUNT = 5;

	public const string UNCOMPRESSED = "Uncompressed";

	public const string DEFAULT = "Default";

	public const string POSITION = "Position";

	public const string ROTATION = "Rotation";

	public const string NORMALIZED_TIME = "NormalizedTime";

	public const float DEFAULT_ACCURACY = 0.001f;

	public const float DEFAULT_UNCOMPRESSED_VALUE = 0f;

	internal const float DEFAULT_POSITION_VALUE = 0.001f;

	internal const float DEFAULT_ROTATION_VALUE = 0.001f;

	internal const float DEFAULT_NRM_TIME_VALUE = 0.0001f;

	internal static readonly Accuracy Uncompressed = new Accuracy("Uncompressed", 0f);

	internal static readonly Accuracy Default = new Accuracy("Default", 0.001f);

	internal static readonly Accuracy DefaultPosition = new Accuracy("Position", 0.001f);

	internal static readonly Accuracy DefaultRotation = new Accuracy("Rotation", 0.001f);

	internal static readonly Accuracy DefaultNormalizedTime = new Accuracy("NormalizedTime", 0.0001f);

	[SerializeField]
	internal string[] coreKeys;

	[SerializeField]
	internal Accuracy[] coreDefs;

	[SerializeField]
	internal Accuracy[] coreVals;

	internal readonly Dictionary<int, string> coreTagLookup = new Dictionary<int, string>(5);

	[SerializeField]
	internal List<string> tags = new List<string>();

	[SerializeField]
	internal List<Accuracy> values = new List<Accuracy>();

	[NonSerialized]
	internal Dictionary<int, Accuracy> Lookup;

	internal static int ZeroHashRemap => "Default".GetHashDeterministic();

	internal static string ZeroHashNameRemap => "Default";

	internal static Accuracy ZeroHashAccuracyRemap => Default;

	internal AccuracyDefaults()
	{
		InitializeCoreValues();
	}

	internal void ValidateAndFix()
	{
		if (coreKeys.Length != 5)
		{
			InitializeCoreValues();
		}
	}

	internal void InitializeCoreValues()
	{
		coreKeys = new string[5] { "Uncompressed", "Default", "Position", "Rotation", "NormalizedTime" };
		coreDefs = new Accuracy[5] { Uncompressed, Default, DefaultPosition, DefaultRotation, DefaultNormalizedTime };
		coreVals = new Accuracy[5] { Uncompressed, Default, DefaultPosition, DefaultRotation, DefaultNormalizedTime };
		coreTagLookup.Clear();
		coreTagLookup.Add("Uncompressed".GetHashDeterministic(), "Uncompressed");
		coreTagLookup.Add("Default".GetHashDeterministic(), "Default");
		coreTagLookup.Add("Position".GetHashDeterministic(), "Position");
		coreTagLookup.Add("Rotation".GetHashDeterministic(), "Rotation");
		coreTagLookup.Add("NormalizedTime".GetHashDeterministic(), "NormalizedTime");
	}

	internal void RebuildLookup()
	{
		ValidateAndFix();
		Lookup = new Dictionary<int, Accuracy>(5 + tags.Count);
		for (int i = 0; i < 5; i++)
		{
			int hashDeterministic = coreKeys[i].GetHashDeterministic();
			if (!Lookup.ContainsKey(hashDeterministic))
			{
				Lookup.Add(hashDeterministic, coreVals[i]);
			}
		}
		int j = 0;
		for (int count = tags.Count; j < count; j++)
		{
			int hashDeterministic2 = tags[j].GetHashDeterministic();
			if (!Lookup.ContainsKey(hashDeterministic2))
			{
				Lookup.Add(hashDeterministic2, values[j]);
			}
		}
	}

	internal bool TryGetAccuracy(string tag, out Accuracy accuracy)
	{
		int hashDeterministic = tag.GetHashDeterministic();
		bool flag = TryGetAccuracy(hashDeterministic, out accuracy);
		if (!flag)
		{
			Debug.LogError("Accuracy for tag '" + tag + "' was not found in " + GetType().Name + ".Lookup. Make sure that an entry in AccuracyDefaults for that name is defined in the NetworkProjectConfig.");
		}
		return flag;
	}

	public bool TryGetAccuracy(int hash, out Accuracy accuracy)
	{
		if (Lookup == null)
		{
			RebuildLookup();
		}
		if (Lookup.TryGetValue(hash, out accuracy))
		{
			return true;
		}
		if (hash == 0)
		{
			accuracy = ZeroHashAccuracyRemap;
			return true;
		}
		accuracy = ZeroHashAccuracyRemap;
		return false;
	}

	public Accuracy GetAccuracyOrThrow(string tag)
	{
		int hashDeterministic = tag.GetHashDeterministic();
		if (!TryGetAccuracy(hashDeterministic, out var accuracy))
		{
			Debug.LogError("Accuracy for tag '" + tag + "' was not found in " + GetType().Name + ".Lookup. Make sure that an entry in AccuracyDefaults for that name is defined in the NetworkProjectConfig.");
			throw new KeyNotFoundException(tag);
		}
		return accuracy;
	}

	internal string GetNameFromHash(int hash)
	{
		if (hash == 0)
		{
			return ZeroHashNameRemap;
		}
		if (coreTagLookup.TryGetValue(hash, out var value))
		{
			return value;
		}
		foreach (string tag in tags)
		{
			if (tag.GetHashDeterministic() == hash)
			{
				return tag;
			}
		}
		return null;
	}

	internal void Rename(string newtag, int index)
	{
		int num = 0;
		while (num++ < 20 && tags.Contains(newtag))
		{
			newtag += "X";
		}
		tags[index] = newtag;
	}

	internal void Add(string tag, float value)
	{
		int num = 0;
		while (num++ < 20 && tags.Contains(tag))
		{
			tag += "X";
		}
		int count = tags.Count;
		tags.Add(tag);
		values.Add(value);
	}

	internal void Remove(string tag)
	{
		int num = tags.IndexOf(tag);
		if (num >= 0)
		{
			tags.RemoveAt(num);
			values.RemoveAt(num);
		}
	}

	internal void RemoveAt(int index)
	{
		tags.RemoveAt(index);
		values.RemoveAt(index);
	}
}
