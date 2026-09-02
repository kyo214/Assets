using System;
using System.IO;
using System.Text;

namespace Fusion;

public class TextWriterLogger : ILogger, IDisposable
{
	private StringBuilder _builder = new StringBuilder();

	private TextWriter _writer;

	private bool _disposeWriter;

	public TextWriterLogger(TextWriter writer, bool disposeWriter)
	{
		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}
		_writer = writer;
		_disposeWriter = disposeWriter;
	}

	public virtual void Dispose()
	{
		if (_disposeWriter && _writer != null)
		{
			TextWriter writer = _writer;
			_writer = null;
			writer.Dispose();
		}
	}

	public virtual void Log<T>(LogType logType, string prefix, ref T loggable, string message) where T : ILogBuilder
	{
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
			if (!string.IsNullOrEmpty(prefix))
			{
				_builder.Append(prefix);
				_builder.Append(": ");
			}
			loggable.BuildLogMessage(_builder, message, default(LogOptions));
			_writer.WriteLine(_builder.ToString());
		}
		finally
		{
			_builder.Clear();
		}
	}

	public virtual void LogException<T>(string prefix, ref T context, Exception ex) where T : ILogBuilder
	{
		try
		{
			_builder.Append(prefix);
			context.BuildLogMessage(_builder, ex.Message, default(LogOptions));
			_writer.WriteLine(_builder.ToString());
			_writer.WriteLine(ex.StackTrace);
		}
		finally
		{
			_builder.Clear();
		}
	}
}
