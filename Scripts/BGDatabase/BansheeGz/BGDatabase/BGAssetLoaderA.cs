using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGAssetLoaderA : BGConfigurableI, BGConfigurableBinaryI
{
	public class AssetLoaderDescriptor : BGAttributeWithManager
	{
		public static AssetLoaderDescriptor Get(Type type)
		{
			return BGUtil.GetAttribute<AssetLoaderDescriptor>(type);
		}
	}

	public interface WithLoaderI
	{
		BGAssetLoaderA AssetLoader { get; set; }
	}

	private static readonly List<Type> AllLoadersTypes = new List<Type>();

	public static List<Type> LoaderTypes
	{
		get
		{
			if (AllLoadersTypes.Count != 0)
			{
				return AllLoadersTypes;
			}
			List<Type> allSubTypes = BGUtil.GetAllSubTypes(typeof(BGAssetLoaderA));
			foreach (Type item in allSubTypes)
			{
				AllLoadersTypes.Add(item);
			}
			return AllLoadersTypes;
		}
	}

	public abstract string Name { get; }

	public abstract T Load<T>(string path) where T : UnityEngine.Object;

	public abstract T[] LoadAll<T>(string path) where T : UnityEngine.Object;

	public virtual string ConfigToString()
	{
		return null;
	}

	public virtual void ConfigFromString(string config)
	{
	}

	public virtual byte[] ConfigToBytes()
	{
		return null;
	}

	public virtual void ConfigFromBytes(ArraySegment<byte> config)
	{
	}
}
