using System;

namespace Fusion;

public interface ILogger
{
	void Log<T>(LogType logType, string prefix, ref T context, string message) where T : ILogBuilder;

	void LogException<T>(string prefix, ref T context, Exception ex) where T : ILogBuilder;
}
