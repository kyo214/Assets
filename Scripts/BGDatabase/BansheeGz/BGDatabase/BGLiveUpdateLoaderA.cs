using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGLiveUpdateLoaderA
{
	public abstract class LoadResult<T>
	{
		public readonly string Error;

		public readonly T Result;

		public bool IsError => Error != null;

		public LoadResult(string error, T result)
		{
			Error = error;
			Result = result;
		}
	}

	public class LoadResultText : LoadResult<string>
	{
		public LoadResultText(string error, string result)
			: base(error, result)
		{
		}
	}

	public class LoadResultBinary : LoadResult<byte[]>
	{
		public LoadResultBinary(string error, byte[] result)
			: base(error, result)
		{
		}
	}

	public class LoadContext
	{
		public readonly string Url;

		public readonly BGLiveUpdateHttpMethodEnum Method;

		public readonly List<Tuple<string, string>> httpParameters = new List<Tuple<string, string>>();

		public readonly List<Tuple<string, string>> httpHeaders = new List<Tuple<string, string>>();

		public string LocalFileName;

		public readonly BGLiveUpdateLog Log;

		public LoadContext(string url, BGLiveUpdateLog log, BGLiveUpdateHttpMethodEnum method = BGLiveUpdateHttpMethodEnum.Default)
		{
			Url = url;
			Method = method;
			Log = log ?? new BGLiveUpdateLog(BGLiveUpdateLog.LogLevelEnum.Summary);
		}

		public LoadContext(string url, BGLiveUpdateLog log, BGLiveUpdateHttpMethodEnum method, List<Tuple<string, string>> httpParameters, List<Tuple<string, string>> httpHeaders)
		{
			Url = url;
			Method = method;
			Log = log ?? new BGLiveUpdateLog(BGLiveUpdateLog.LogLevelEnum.Summary);
			if (httpParameters != null)
			{
				this.httpParameters.AddRange(httpParameters);
			}
			if (httpHeaders != null)
			{
				this.httpHeaders.AddRange(httpHeaders);
			}
		}
	}

	public abstract void Complete();

	public abstract void Load(LoadContext context, Action<LoadResultText> callback);

	public abstract void Load(LoadContext context, Action<LoadResultBinary> callback);

	protected static void WriteToLocalFile(LoadContext context, Action<string> action)
	{
		string text = "";
		try
		{
			if (!string.IsNullOrEmpty(context.LocalFileName))
			{
				text = Path.Combine(Application.persistentDataPath, context.LocalFileName);
				action(text);
				context.Log?.AddDetail("Loaded data is written to local file $", text);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			context.Log?.AddWarning("Can not write local file with remote data at path " + text);
		}
	}

	protected void LoadFromLocalFile(LoadContext context, ref Exception e, Action<string> action)
	{
		if (string.IsNullOrEmpty(context.LocalFileName))
		{
			return;
		}
		string text = Path.Combine(Application.persistentDataPath, context.LocalFileName);
		try
		{
			if (File.Exists(text))
			{
				action(text);
				e = null;
				context.Log?.AddDetail("Loading failed, but local fallback file found at path $", text);
			}
			else
			{
				context.Log?.AddDetail("Loading failed, local fallback file can not be found at path $", text);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			context.Log?.AddWarning("Loading failed and reading data from local fallback file at path $ also failed!", text);
		}
	}
}
