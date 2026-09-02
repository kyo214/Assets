using System;

namespace Fusion;

public class ConsoleLogger : TextWriterLogger
{
	public ConsoleLogger()
		: base(Console.Out, disposeWriter: false)
	{
	}

	public override void Log<T>(LogType logType, string prefix, ref T loggable, string message)
	{
		switch (logType)
		{
		case LogType.Info:
		case LogType.Debug:
		case LogType.Trace:
			Console.ForegroundColor = ConsoleColor.Gray;
			break;
		case LogType.Warn:
			Console.ForegroundColor = ConsoleColor.Yellow;
			break;
		case LogType.Error:
			Console.ForegroundColor = ConsoleColor.Red;
			break;
		}
		try
		{
			base.Log(logType, prefix, ref loggable, message);
		}
		finally
		{
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}

	public override void LogException<T>(string prefix, ref T context, Exception ex)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		try
		{
			base.LogException(prefix, ref context, ex);
		}
		finally
		{
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}
}
