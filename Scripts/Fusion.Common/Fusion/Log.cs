using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Fusion;

public static class Log
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct DefaultContext : ILogBuilder
	{
		public void BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
		{
			builder.Append(message);
		}

		void ILogBuilder.BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
		{
			BuildLogMessage(builder, message, in options);
		}
	}

	private class LegacyLogger : ILogger
	{
		private StringBuilder _builder = new StringBuilder();

		public Action<string> Info;

		public Action<string> Warn;

		public Action<string> Error;

		public Action<Exception> Exception;

		public void Log<T>(LogType logType, string prefix, ref T loggable, string message) where T : ILogBuilder
		{
			switch (logType)
			{
			case LogType.Info:
			case LogType.Debug:
			case LogType.Trace:
				NewMethod(prefix, ref loggable, message, Info);
				break;
			case LogType.Warn:
				NewMethod(prefix, ref loggable, message, Warn);
				break;
			case LogType.Error:
				NewMethod(prefix, ref loggable, message, Error);
				break;
			}
		}

		private void NewMethod<T>(string prefix, ref T loggable, string message, Action<string> handler) where T : ILogBuilder
		{
			if (handler == null)
			{
				return;
			}
			try
			{
				if (!string.IsNullOrEmpty(prefix))
				{
					_builder.Append(prefix);
					_builder.Append(": ");
				}
				loggable.BuildLogMessage(_builder, message, default(LogOptions));
				string obj = _builder.ToString();
				handler(obj);
			}
			finally
			{
				_builder.Clear();
			}
		}

		public void LogException<T>(string prefix, ref T context, Exception ex) where T : ILogBuilder
		{
			Exception?.Invoke(ex);
		}
	}

	private static class Lock
	{
	}

	private const string DefaultPrefix = "";

	private static ILogger _logger;

	public static LogType LogLevel = LogType.Debug;

	public static bool Initialized => _logger != null;

	public static void InitForConsole()
	{
		Init(new ConsoleLogger());
	}

	public static void Init(ILogger logger, LogType LogLevel = LogType.Debug)
	{
		lock (typeof(Lock))
		{
			_logger = logger;
			Log.LogLevel = LogLevel;
		}
	}

	[Obsolete]
	public static void Init(Action<string> info, Action<string> warn, Action<string> error, Action<Exception> exn)
	{
		Init(new LegacyLogger
		{
			Info = info,
			Warn = warn,
			Error = error,
			Exception = exn
		});
	}

	public static void Reset()
	{
		LogLevel = LogType.Debug;
	}

	public static void Exception(Exception exn)
	{
		ILogger logger = _logger;
		if (logger != null && (int)LogLevel >= 0)
		{
			lock (typeof(Lock))
			{
				DefaultContext context = default;
				logger.LogException(null, ref context, exn);
			}
		}
	}

	internal static void Exception<T>(T loggable, Exception exn) where T : class, ILogBuilder
	{
		ILogger logger = _logger;
		if (logger == null || (int)LogLevel < 0)
		{
			return;
		}
		lock (typeof(Lock))
		{
			if (loggable != null)
			{
				DefaultContext context = default;
				logger.LogException(null, ref context, exn);
			}
			else
			{
				logger.LogException(null, ref loggable, exn);
			}
		}
	}

	private static void ExceptionInternal(Exception exn, string stream = null)
	{
		ILogger logger = _logger;
		if (logger != null && (int)LogLevel >= 0)
		{
			lock (typeof(Lock))
			{
				DefaultContext context = default;
				logger.LogException(stream, ref context, exn);
			}
		}
	}

	[Conditional("TRACE")]
	public static void Trace(object msg)
	{
		InfoInternal(msg);
	}

	[Conditional("TRACE")]
	public static void TraceWarn(object msg)
	{
		WarnInternal(msg);
	}

	[Conditional("TRACE")]
	public static void TraceError(object msg)
	{
		ErrorInternal(msg);
	}

	[Conditional("TRACE")]
	internal static void Trace<T>(T context, object msg) where T : class, ILogBuilder
	{
		InfoInternalValue(context, msg);
	}

	[Conditional("TRACE")]
	internal static void TraceWarn<T>(T context, object msg) where T : class, ILogBuilder
	{
		WarnInternalValue(context, msg);
	}

	[Conditional("TRACE")]
	internal static void TraceError<T>(T context, object msg) where T : class, ILogBuilder
	{
		ErrorInternalValue(context, msg);
	}

	[Conditional("TRACE")]
	internal unsafe static void Trace<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		InfoInternalPtr(context, msg);
	}

	[Conditional("TRACE")]
	internal unsafe static void TraceWarn<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		WarnInternalPtr(context, msg);
	}

	[Conditional("TRACE")]
	internal unsafe static void TraceError<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		ErrorInternalPtr(context, msg);
	}

	[Conditional("TRACE")]
	internal static void Trace<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		InfoInternalValue(context, msg);
	}

	[Conditional("TRACE")]
	internal static void TraceWarn<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		WarnInternalValue(context, msg);
	}

	[Conditional("TRACE")]
	internal static void TraceError<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		ErrorInternalValue(context, msg);
	}

	[Conditional("DEBUG")]
	public static void Debug(object msg)
	{
		InfoInternal(msg);
	}

	[Conditional("DEBUG")]
	public static void DebugWarn(object msg)
	{
		WarnInternal(msg);
	}

	[Conditional("DEBUG")]
	public static void DebugError(object msg)
	{
		ErrorInternal(msg);
	}

	[Conditional("DEBUG")]
	internal static void Debug<T>(T context, object msg) where T : class, ILogBuilder
	{
		InfoInternalValue(context, msg);
	}

	[Conditional("DEBUG")]
	internal static void DebugWarn<T>(T context, object msg) where T : class, ILogBuilder
	{
		WarnInternalValue(context, msg);
	}

	[Conditional("DEBUG")]
	internal static void DebugError<T>(T context, object msg) where T : class, ILogBuilder
	{
		ErrorInternalValue(context, msg);
	}

	[Conditional("DEBUG")]
	internal unsafe static void Debug<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		InfoInternalPtr(context, msg);
	}

	[Conditional("DEBUG")]
	internal unsafe static void DebugWarn<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		WarnInternalPtr(context, msg);
	}

	[Conditional("DEBUG")]
	internal unsafe static void DebugError<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		ErrorInternalPtr(context, msg);
	}

	[Conditional("DEBUG")]
	internal static void Debug<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		InfoInternalValue(context, msg);
	}

	[Conditional("DEBUG")]
	internal static void DebugWarn<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		WarnInternalValue(context, msg);
	}

	[Conditional("DEBUG")]
	internal static void DebugError<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		ErrorInternalValue(context, msg);
	}

	[Conditional("TRACE_STUN")]
	public static void TraceStun(object msg)
	{
		InfoInternal(msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	public static void TraceStunWarn(object msg)
	{
		WarnInternal(msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	public static void TraceStunError(object msg)
	{
		ErrorInternal(msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	internal static void TraceStun<T>(T context, object msg) where T : class, ILogBuilder
	{
		InfoInternalValue(context, msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	internal static void TraceStunWarn<T>(T context, object msg) where T : class, ILogBuilder
	{
		WarnInternalValue(context, msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	internal static void TraceStunError<T>(T context, object msg) where T : class, ILogBuilder
	{
		ErrorInternalValue(context, msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	internal unsafe static void TraceStun<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		InfoInternalPtr(context, msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	internal unsafe static void TraceStunWarn<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		WarnInternalPtr(context, msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	internal unsafe static void TraceStunError<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		ErrorInternalPtr(context, msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	internal static void TraceStun<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		InfoInternalValue(context, msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	internal static void TraceStunWarn<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		WarnInternalValue(context, msg, "STUN");
	}

	[Conditional("TRACE_STUN")]
	internal static void TraceStunError<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		ErrorInternalValue(context, msg, "STUN");
	}

	[Conditional("TRACE_OBJECT")]
	public static void TraceObject(object msg)
	{
		InfoInternal(msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	public static void TraceObjectWarn(object msg)
	{
		WarnInternal(msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	public static void TraceObjectError(object msg)
	{
		ErrorInternal(msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	internal static void TraceObject<T>(T context, object msg) where T : class, ILogBuilder
	{
		InfoInternalValue(context, msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	internal static void TraceObjectWarn<T>(T context, object msg) where T : class, ILogBuilder
	{
		WarnInternalValue(context, msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	internal static void TraceObjectError<T>(T context, object msg) where T : class, ILogBuilder
	{
		ErrorInternalValue(context, msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	internal unsafe static void TraceObject<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		InfoInternalPtr(context, msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	internal unsafe static void TraceObjectWarn<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		WarnInternalPtr(context, msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	internal unsafe static void TraceObjectError<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		ErrorInternalPtr(context, msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	internal static void TraceObject<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		InfoInternalValue(context, msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	internal static void TraceObjectWarn<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		WarnInternalValue(context, msg, "Object");
	}

	[Conditional("TRACE_OBJECT")]
	internal static void TraceObjectError<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		ErrorInternalValue(context, msg, "Object");
	}

	[Conditional("TRACE_NETWORK")]
	public static void TraceNetwork(object msg)
	{
		InfoInternal(msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	public static void TraceNetworkWarn(object msg)
	{
		WarnInternal(msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	public static void TraceNetworkError(object msg)
	{
		ErrorInternal(msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	internal static void TraceNetwork<T>(T context, object msg) where T : class, ILogBuilder
	{
		InfoInternalValue(context, msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	internal static void TraceNetworkWarn<T>(T context, object msg) where T : class, ILogBuilder
	{
		WarnInternalValue(context, msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	internal static void TraceNetworkError<T>(T context, object msg) where T : class, ILogBuilder
	{
		ErrorInternalValue(context, msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	internal unsafe static void TraceNetwork<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		InfoInternalPtr(context, msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	internal unsafe static void TraceNetworkWarn<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		WarnInternalPtr(context, msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	internal unsafe static void TraceNetworkError<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		ErrorInternalPtr(context, msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	internal static void TraceNetwork<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		InfoInternalValue(context, msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	internal static void TraceNetworkWarn<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		WarnInternalValue(context, msg, "Network");
	}

	[Conditional("TRACE_NETWORK")]
	internal static void TraceNetworkError<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		ErrorInternalValue(context, msg, "Network");
	}

	[Conditional("TRACE_PREFAB")]
	public static void TracePrefab(object msg)
	{
		InfoInternal(msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	public static void TracePrefabWarn(object msg)
	{
		WarnInternal(msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	public static void TracePrefabError(object msg)
	{
		ErrorInternal(msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	internal static void TracePrefab<T>(T context, object msg) where T : class, ILogBuilder
	{
		InfoInternalValue(context, msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	internal static void TracePrefabWarn<T>(T context, object msg) where T : class, ILogBuilder
	{
		WarnInternalValue(context, msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	internal static void TracePrefabError<T>(T context, object msg) where T : class, ILogBuilder
	{
		ErrorInternalValue(context, msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	internal unsafe static void TracePrefab<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		InfoInternalPtr(context, msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	internal unsafe static void TracePrefabWarn<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		WarnInternalPtr(context, msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	internal unsafe static void TracePrefabError<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		ErrorInternalPtr(context, msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	internal static void TracePrefab<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		InfoInternalValue(context, msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	internal static void TracePrefabWarn<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		WarnInternalValue(context, msg, "Prefab");
	}

	[Conditional("TRACE_PREFAB")]
	internal static void TracePrefabError<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		ErrorInternalValue(context, msg, "Prefab");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	public static void TraceSimulationMessage(object msg)
	{
		InfoInternal(msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	public static void TraceSimulationMessageWarn(object msg)
	{
		WarnInternal(msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	public static void TraceSimulationMessageError(object msg)
	{
		ErrorInternal(msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	internal static void TraceSimulationMessage<T>(T context, object msg) where T : class, ILogBuilder
	{
		InfoInternalValue(context, msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	internal static void TraceSimulationMessageWarn<T>(T context, object msg) where T : class, ILogBuilder
	{
		WarnInternalValue(context, msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	internal static void TraceSimulationMessageError<T>(T context, object msg) where T : class, ILogBuilder
	{
		ErrorInternalValue(context, msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	internal unsafe static void TraceSimulationMessage<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		InfoInternalPtr(context, msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	internal unsafe static void TraceSimulationMessageWarn<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		WarnInternalPtr(context, msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	internal unsafe static void TraceSimulationMessageError<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		ErrorInternalPtr(context, msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	internal static void TraceSimulationMessage<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		InfoInternalValue(context, msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	internal static void TraceSimulationMessageWarn<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		WarnInternalValue(context, msg, "SimMsg");
	}

	[Conditional("TRACE_SIMULATION_MESSAGE")]
	internal static void TraceSimulationMessageError<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		ErrorInternalValue(context, msg, "SimMsg");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	public static void TraceDummyTraffic(object msg)
	{
		InfoInternal(msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	public static void TraceDummyTrafficWarn(object msg)
	{
		WarnInternal(msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	public static void TraceDummyTrafficError(object msg)
	{
		ErrorInternal(msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	internal static void TraceDummyTraffic<T>(T context, object msg) where T : class, ILogBuilder
	{
		InfoInternalValue(context, msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	internal static void TraceDummyTrafficWarn<T>(T context, object msg) where T : class, ILogBuilder
	{
		WarnInternalValue(context, msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	internal static void TraceDummyTrafficError<T>(T context, object msg) where T : class, ILogBuilder
	{
		ErrorInternalValue(context, msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	internal unsafe static void TraceDummyTraffic<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		InfoInternalPtr(context, msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	internal unsafe static void TraceDummyTrafficWarn<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		WarnInternalPtr(context, msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	internal unsafe static void TraceDummyTrafficError<T>(T* context, object msg) where T : unmanaged, ILogBuilder
	{
		ErrorInternalPtr(context, msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	internal static void TraceDummyTraffic<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		InfoInternalValue(context, msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	internal static void TraceDummyTrafficWarn<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		WarnInternalValue(context, msg, "DummyTraffic");
	}

	[Conditional("TRACE_DUMMY_TRAFFIC")]
	internal static void TraceDummyTrafficError<T>(in T context, object msg) where T : struct, ILogBuilder, LogBuilderUtils.ICombinedLogBuilder
	{
		ErrorInternalValue(context, msg, "DummyTraffic");
	}

	[Conditional("DEBUG")]
	internal static void InfoRealtime(object msg)
	{
		InfoInternal(msg, "RealtimeSDK");
	}

	[Conditional("DEBUG")]
	internal static void WarnRealtime(object msg)
	{
		WarnInternal(msg, "RealtimeSDK");
	}

	[Conditional("DEBUG")]
	internal static void ErrorRealtime(object msg)
	{
		ErrorInternal(msg, "RealtimeSDK");
	}

	[Conditional("DEBUG")]
	internal static void ExceptionRealtime(Exception ex)
	{
		ExceptionInternal(ex, "RealtimeSDK");
	}

	public static void Info(object msg)
	{
		ILogger logger = _logger;
		if (logger != null && (int)LogLevel >= 2)
		{
			lock (typeof(Lock))
			{
				DefaultContext context = default;
				logger.Log(LogType.Info, null, ref context, msg?.ToString() ?? "NULL");
			}
		}
	}

	internal static void Info<T>(T loggable, object msg) where T : class, ILogBuilder
	{
		ILogger logger = _logger;
		if (logger == null || (int)LogLevel < 2)
		{
			return;
		}
		lock (typeof(Lock))
		{
			if (loggable != null)
			{
				logger.Log(LogType.Info, null, ref loggable, msg?.ToString() ?? "NULL");
				return;
			}
			DefaultContext context = default;
			logger.Log(LogType.Info, null, ref context, msg?.ToString() ?? "NULL");
		}
	}

	private static void InfoInternal(object msg, string stream = null)
	{
		ILogger logger = _logger;
		if (logger != null && (int)LogLevel >= 2)
		{
			lock (typeof(Lock))
			{
				DefaultContext context = default;
				logger.Log(LogType.Info, stream, ref context, msg?.ToString() ?? "NULL");
			}
		}
	}

	private static void InfoInternalValue<T>(T loggable, object msg, string stream = null) where T : ILogBuilder
	{
		ILogger logger = _logger;
		if (logger == null || (int)LogLevel < 2)
		{
			return;
		}
		lock (typeof(Lock))
		{
			if (loggable != null)
			{
				logger.Log(LogType.Info, stream, ref loggable, msg?.ToString() ?? "NULL");
				return;
			}
			DefaultContext context = default;
			logger.Log(LogType.Info, stream, ref context, msg?.ToString() ?? "NULL");
		}
	}

	private unsafe static void InfoInternalPtr<T>(T* loggable, object msg, string stream = null) where T : unmanaged, ILogBuilder
	{
		ILogger logger = _logger;
		if (logger == null || (int)LogLevel < 2)
		{
			return;
		}
		lock (typeof(Lock))
		{
			if (loggable != null)
			{
				logger.Log(LogType.Info, stream, ref *loggable, msg?.ToString() ?? "NULL");
				return;
			}
			DefaultContext context = default;
			logger.Log(LogType.Info, stream, ref context, msg?.ToString() ?? "NULL");
		}
	}

	public static void Warn(object msg)
	{
		ILogger logger = _logger;
		if (logger != null && (int)LogLevel >= 1)
		{
			lock (typeof(Lock))
			{
				DefaultContext context = default;
				logger.Log(LogType.Warn, null, ref context, msg?.ToString() ?? "NULL");
			}
		}
	}

	internal static void Warn<T>(T loggable, object msg) where T : class, ILogBuilder
	{
		ILogger logger = _logger;
		if (logger == null || (int)LogLevel < 1)
		{
			return;
		}
		lock (typeof(Lock))
		{
			if (loggable != null)
			{
				logger.Log(LogType.Warn, null, ref loggable, msg?.ToString() ?? "NULL");
				return;
			}
			DefaultContext context = default;
			logger.Log(LogType.Warn, null, ref context, msg?.ToString() ?? "NULL");
		}
	}

	private static void WarnInternal(object msg, string stream = null)
	{
		ILogger logger = _logger;
		if (logger != null && (int)LogLevel >= 1)
		{
			lock (typeof(Lock))
			{
				DefaultContext context = default;
				logger.Log(LogType.Warn, stream, ref context, msg?.ToString() ?? "NULL");
			}
		}
	}

	private static void WarnInternalValue<T>(T loggable, object msg, string stream = null) where T : ILogBuilder
	{
		ILogger logger = _logger;
		if (logger == null || (int)LogLevel < 1)
		{
			return;
		}
		lock (typeof(Lock))
		{
			if (loggable != null)
			{
				logger.Log(LogType.Warn, stream, ref loggable, msg?.ToString() ?? "NULL");
				return;
			}
			DefaultContext context = default;
			logger.Log(LogType.Warn, stream, ref context, msg?.ToString() ?? "NULL");
		}
	}

	private unsafe static void WarnInternalPtr<T>(T* loggable, object msg, string stream = null) where T : unmanaged, ILogBuilder
	{
		ILogger logger = _logger;
		if (logger == null || (int)LogLevel < 1)
		{
			return;
		}
		lock (typeof(Lock))
		{
			if (loggable != null)
			{
				logger.Log(LogType.Warn, stream, ref *loggable, msg?.ToString() ?? "NULL");
				return;
			}
			DefaultContext context = default;
			logger.Log(LogType.Warn, stream, ref context, msg?.ToString() ?? "NULL");
		}
	}

	public static void Error(object msg)
	{
		ILogger logger = _logger;
		if (logger != null && (int)LogLevel >= 0)
		{
			lock (typeof(Lock))
			{
				DefaultContext context = default;
				logger.Log(LogType.Error, null, ref context, msg?.ToString() ?? "NULL");
			}
		}
	}

	internal static void Error<T>(T loggable, object msg) where T : class, ILogBuilder
	{
		ILogger logger = _logger;
		if (logger == null || (int)LogLevel < 0)
		{
			return;
		}
		lock (typeof(Lock))
		{
			if (loggable != null)
			{
				logger.Log(LogType.Error, null, ref loggable, msg?.ToString() ?? "NULL");
				return;
			}
			DefaultContext context = default;
			logger.Log(LogType.Error, null, ref context, msg?.ToString() ?? "NULL");
		}
	}

	private static void ErrorInternal(object msg, string stream = null)
	{
		ILogger logger = _logger;
		if (logger != null && (int)LogLevel >= 0)
		{
			lock (typeof(Lock))
			{
				DefaultContext context = default;
				logger.Log(LogType.Error, stream, ref context, msg?.ToString() ?? "NULL");
			}
		}
	}

	private static void ErrorInternalValue<T>(T loggable, object msg, string stream = null) where T : ILogBuilder
	{
		ILogger logger = _logger;
		if (logger == null || (int)LogLevel < 0)
		{
			return;
		}
		lock (typeof(Lock))
		{
			if (loggable != null)
			{
				logger.Log(LogType.Error, stream, ref loggable, msg?.ToString() ?? "NULL");
				return;
			}
			DefaultContext context = default;
			logger.Log(LogType.Error, stream, ref context, msg?.ToString() ?? "NULL");
		}
	}

	private unsafe static void ErrorInternalPtr<T>(T* loggable, object msg, string stream = null) where T : unmanaged, ILogBuilder
	{
		ILogger logger = _logger;
		if (logger == null || (int)LogLevel < 0)
		{
			return;
		}
		lock (typeof(Lock))
		{
			if (loggable != null)
			{
				logger.Log(LogType.Error, stream, ref *loggable, msg?.ToString() ?? "NULL");
				return;
			}
			DefaultContext context = default;
			logger.Log(LogType.Error, stream, ref context, msg?.ToString() ?? "NULL");
		}
	}
}
