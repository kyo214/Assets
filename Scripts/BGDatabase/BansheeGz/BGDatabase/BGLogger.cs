using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGLogger
{
	private class SubSectionInfo
	{
		private readonly BGLogger logger;

		private readonly StringBuilder oldBuilder;

		private readonly Stopwatch stopwatch;

		private readonly string message;

		private readonly object[] parameters;

		private readonly StringBuilder myBuilder = new StringBuilder();

		public SubSectionInfo(BGLogger logger, string message, object[] parameters)
		{
			this.logger = logger;
			oldBuilder = logger.builder;
			logger.builder = myBuilder;
			logger.tab++;
			stopwatch = Stopwatch.StartNew();
			this.message = message;
			this.parameters = parameters;
		}

		public void End()
		{
			stopwatch.Stop();
			logger.builder = oldBuilder;
			object[] array;
			if (parameters == null)
			{
				array = new object[1] { stopwatch.ElapsedMilliseconds };
			}
			else
			{
				array = new object[parameters.Length + 1];
				parameters.CopyTo(array, 0);
				array[^1] = stopwatch.ElapsedMilliseconds;
			}
			logger.AppendLine("-----[" + message + ", executed in $ mls]---------------------------", array);
			logger.builder.Append((object)myBuilder);
			logger.tab--;
		}
	}

	private readonly Stack<SubSectionInfo> subsections = new Stack<SubSectionInfo>();

	private StringBuilder builder = new StringBuilder();

	private readonly bool useRichText;

	private int tab;

	private int warnings;

	private SubSectionInfo subSectionInfo;

	public string Log => builder.ToString();

	public int Warnings => warnings;

	public int Indent
	{
		get
		{
			return tab;
		}
		set
		{
			tab = value;
		}
	}

	public string Tab
	{
		get
		{
			if (tab != 0)
			{
				return new string('\t', tab);
			}
			return "";
		}
	}

	public BGLogger(bool useRichText = true)
	{
		this.useRichText = useRichText;
	}

	public void Clear()
	{
		builder.Length = 0;
		subsections.Clear();
		tab = 0;
		warnings = 0;
		subSectionInfo = null;
	}

	public void AppendLine(string message, params object[] parameters)
	{
		if (message != null)
		{
			message = Highlight(message, null, parameters);
			builder.AppendLine(Tab + BGUtil.Format(message, parameters));
		}
	}

	public void Append(BGLogger logger)
	{
		builder.AppendLine(logger.Log);
	}

	public bool AppendLine(bool condition, string message, params object[] parameters)
	{
		if (!condition)
		{
			return false;
		}
		AppendLine(message, parameters);
		return true;
	}

	public void Section(string message, Action action)
	{
		tab = 0;
		StringBuilder stringBuilder = builder;
		builder = new StringBuilder();
		tab++;
		Stopwatch stopwatch = Stopwatch.StartNew();
		try
		{
			action();
		}
		finally
		{
			StringBuilder value = builder;
			builder = stringBuilder;
			stopwatch.Stop();
			AppendLine("");
			AppendLine("============ [[$, executed in $ mls]] ===========================", message, stopwatch.ElapsedMilliseconds);
			builder.Append((object)value);
			tab = 0;
		}
	}

	public void SubSection(Action action, string message, params object[] parameters)
	{
		SubSectionStart(message, parameters);
		try
		{
			action();
		}
		finally
		{
			SubSectionEnd();
		}
	}

	public void SubSectionStart(string message, params object[] parameters)
	{
		subsections.Push(subSectionInfo);
		subSectionInfo = new SubSectionInfo(this, message, parameters);
	}

	public void SubSectionEnd()
	{
		subSectionInfo?.End();
		subSectionInfo = ((subsections.Count > 0) ? subsections.Pop() : null);
	}

	public void AppendWarning(string message, params object[] parameters)
	{
		if (message != null)
		{
			warnings++;
			message = Highlight(message, "red", parameters);
			builder.AppendLine(Tab + BGUtil.Format("WARNING: " + message, parameters));
		}
	}

	public bool AppendWarning(bool condition, string message, params object[] parameters)
	{
		if (!condition)
		{
			return false;
		}
		AppendWarning(message, parameters);
		return true;
	}

	private string Highlight(string message, string color, object[] parameters)
	{
		if (!useRichText || parameters == null || parameters.Length == 0)
		{
			return message;
		}
		return message.Replace("$", BGRichText.Highlight("$", color));
	}

	public void Exception(bool condition, string reason, params object[] parameters)
	{
		if (!condition)
		{
			return;
		}
		AppendWarning(reason, parameters);
		throw new BGException(reason, parameters);
	}
}
