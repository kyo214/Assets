using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AssetLoaderDescriptor(Name = "Addressables", ManagerType = "BansheeGz.BGDatabase.Editor.BGAssetLoaderManagerAddressables")]
public class BGAssetLoaderAddressables : BGAssetLoaderA
{
	public interface AddressablesLoader
	{
		T Load<T>(string path) where T : UnityEngine.Object;

		IList<T> LoadAll<T>(string path) where T : UnityEngine.Object;
	}

	public const string NoPluginWarning = "Important!! Addressables plug-in for BGDatabase is not installed or outdated. Please, download latest version here: www.bansheegz.com/BGDatabase/Downloads/";

	private static AddressablesLoader loaderDelegate;

	private static bool loaderDelegateLoadAttempted;

	public override string Name => "Addressables";

	public static AddressablesLoader LoaderDelegate
	{
		get
		{
			InitDelegate();
			return loaderDelegate;
		}
	}

	public override T Load<T>(string path)
	{
		InitDelegate();
		if (loaderDelegate == null)
		{
			return null;
		}
		try
		{
			return loaderDelegate.Load<T>(path);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	public override T[] LoadAll<T>(string path)
	{
		InitDelegate();
		if (loaderDelegate == null)
		{
			return null;
		}
		IList<T> list;
		try
		{
			list = loaderDelegate.LoadAll<T>(path);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
		if (list == null || list.Count == 0)
		{
			return null;
		}
		T[] array = new T[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			array[i] = list[i];
		}
		return array;
	}

	private static void InitDelegate()
	{
		if (loaderDelegate == null && !loaderDelegateLoadAttempted)
		{
			loaderDelegateLoadAttempted = true;
			try
			{
				loaderDelegate = BGUtil.Create<AddressablesLoader>("BansheeGz.BGDatabase.BGAddressablesLoader", includePrivateConstructors: false, Array.Empty<object>());
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			if (loaderDelegate == null)
			{
				Debug.Log("Important!! Addressables plug-in for BGDatabase is not installed or outdated. Please, download latest version here: www.bansheegz.com/BGDatabase/Downloads/");
			}
		}
	}
}
