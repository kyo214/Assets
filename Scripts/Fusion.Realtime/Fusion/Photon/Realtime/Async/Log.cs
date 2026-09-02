#define TRACE
using System;
using System.Diagnostics;

namespace Fusion.Photon.Realtime.Async;

internal static class Log
{
	private static object sync;

	private static Action<string> infoCallback;

	private static Action<string> warnCallback;

	private static Action<string> errorCallback;

	private static Action<Exception> exnCallback;

	public static void Init(Action<string> info, Action<string> warn, Action<string> error, Action<Exception> exn)
	{
		sync = new object();
		infoCallback = info;
		warnCallback = warn;
		errorCallback = error;
		exnCallback = exn;
	}

	public static void InitForConsole()
	{
		Init((string info) =>
		{
			Console.WriteLine(info);
		}, (string warn) =>
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(warn);
			Console.ResetColor();
		}, (string error) =>
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(error);
			Console.ResetColor();
		}, (Exception exn) =>
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(exn.Message);
			Console.WriteLine(exn.StackTrace);
			Console.ResetColor();
		});
	}

	public static void InitForSystemDiagnostics()
	{
		Init((string info) =>
		{
			System.Diagnostics.Trace.WriteLine(info);
		}, (string warn) =>
		{
			System.Diagnostics.Trace.WriteLine(warn, "Warning");
		}, (string error) =>
		{
			System.Diagnostics.Trace.WriteLine(error, "Error");
		}, (Exception exn) =>
		{
			System.Diagnostics.Trace.WriteLine(exn.Message, "Error");
			System.Diagnostics.Trace.WriteLine(exn.StackTrace, "Error");
		});
	}

	[Conditional("DEBUG")]
	public static void Debug(object value)
	{
		Info(value);
	}

	[Conditional("TRACE")]
	public static void Trace(object value)
	{
		Info(value);
	}

	public static void Info(object value)
	{
		if (infoCallback != null)
		{
			lock (sync)
			{
				infoCallback((value == null) ? "NULL" : value.ToString());
			}
		}
	}

	public static void Warn(object value)
	{
		if (warnCallback != null)
		{
			lock (sync)
			{
				warnCallback((value == null) ? "NULL" : value.ToString());
			}
		}
	}

	public static void Error(object value)
	{
		if (errorCallback != null)
		{
			lock (sync)
			{
				errorCallback((value == null) ? "NULL" : value.ToString());
			}
		}
	}

	public static void Exception(Exception exn)
	{
		if (exnCallback != null)
		{
			lock (sync)
			{
				exnCallback(exn);
			}
		}
	}
}
