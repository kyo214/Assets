using System;
using System.Text;
using UnityEngine;

namespace Fusion;

[Serializable]
public class FusionUnityLogger : ILogger
{
	private StringBuilder _builder = new StringBuilder();

	public bool UseGlobalPrefix;

	public bool UseColorTags;

	public string GlobalPrefixColor;

	public Color32 MinRandomColor;

	public Color32 MaxRandomColor;

	public Color ServerColor;

	public Func<object, int> GetColor { get; set; }

	public FusionUnityLogger()
	{
		bool flag = false;
		MinRandomColor = (flag ? new Color32(158, 158, 158, byte.MaxValue) : new Color32(30, 30, 30, byte.MaxValue));
		MaxRandomColor = (flag ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(90, 90, 90, byte.MaxValue));
		ServerColor = (flag ? new Color32(byte.MaxValue, byte.MaxValue, 158, byte.MaxValue) : new Color32(30, 90, 200, byte.MaxValue));
		UseColorTags = true;
		UseGlobalPrefix = true;
		GlobalPrefixColor = Color32ToRGBString(flag ? new Color32(115, 172, 229, byte.MaxValue) : new Color32(20, 64, 120, byte.MaxValue));
		GetColor = (object obj) =>
		{
			if (obj is NetworkRunner networkRunner)
			{
				int hashCodeForLogger = networkRunner.GetHashCodeForLogger();
				return GetRandomColor(hashCodeForLogger);
			}
			return 0;
		};
	}

	public void Log<T>(LogType logType, string prefix, ref T context, string message) where T : ILogBuilder
	{
		string message2;
		try
		{
			switch (logType)
			{
			case LogType.Debug:
				_builder.Append("[DEBUG] ");
				break;
			case LogType.Trace:
				_builder.Append("[TRACE] ");
				break;
			}
			if (UseGlobalPrefix)
			{
				if (UseColorTags)
				{
					_builder.Append("<color=");
					_builder.Append(GlobalPrefixColor);
					_builder.Append(">");
				}
				_builder.Append("[Fusion");
				if (!string.IsNullOrEmpty(prefix))
				{
					_builder.Append("/");
					_builder.Append(prefix);
				}
				_builder.Append("]");
				if (UseColorTags)
				{
					_builder.Append("</color>");
				}
				_builder.Append(" ");
			}
			else if (!string.IsNullOrEmpty(prefix))
			{
				_builder.Append(prefix);
				_builder.Append(": ");
			}
			LogOptions options = new LogOptions(UseColorTags, GetColor);
			StringBuilder builder = _builder;
			context.BuildLogMessage(builder, message, in options);
			message2 = _builder.ToString();
		}
		finally
		{
			_builder.Clear();
		}
		UnityEngine.Object context2 = context as UnityEngine.Object;
		switch (logType)
		{
		case LogType.Error:
			Debug.LogError(message2, context2);
			break;
		case LogType.Warn:
			Debug.LogWarning(message2, context2);
			break;
		default:
			Debug.Log(message2, context2);
			break;
		}
	}

	public void LogException<T>(string prefix, ref T context, Exception ex) where T : ILogBuilder
	{
		Log(LogType.Error, string.Empty, ref context, $"{ex.GetType()}\n<i>See next error log entry for details.</i>");
		if (context is UnityEngine.Object context2)
		{
			Debug.LogException(ex, context2);
		}
		else
		{
			Debug.LogException(ex);
		}
	}

	private int GetRandomColor(int seed)
	{
		return GetRandomColor(seed, MinRandomColor, MaxRandomColor, ServerColor);
	}

	private static int GetRandomColor(int seed, Color32 min, Color32 max, Color32 svr)
	{
		NetworkRNG networkRNG = new NetworkRNG(seed);
		int value;
		int value2;
		int value3;
		if (seed == -1)
		{
			value = svr.r;
			value2 = svr.g;
			value3 = svr.b;
		}
		else
		{
			value = networkRNG.RangeInclusive(min.r, max.r);
			value2 = networkRNG.RangeInclusive(min.g, max.g);
			value3 = networkRNG.RangeInclusive(min.b, max.b);
		}
		value = Mathf.Clamp(value, 0, 255);
		value2 = Mathf.Clamp(value2, 0, 255);
		value3 = Mathf.Clamp(value3, 0, 255);
		return (value << 16) | (value2 << 8) | value3;
	}

	private static int Color32ToRGB24(Color32 c)
	{
		return (c.r << 16) | (c.g << 8) | c.b;
	}

	private static string Color32ToRGBString(Color32 c)
	{
		return $"#{Color32ToRGB24(c):X6}";
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void Initialize()
	{
		if (!Fusion.Log.Initialized)
		{
			FusionUnityLogger fusionUnityLogger = new FusionUnityLogger();
			if (fusionUnityLogger != null)
			{
				Fusion.Log.Init(fusionUnityLogger);
			}
		}
	}
}
