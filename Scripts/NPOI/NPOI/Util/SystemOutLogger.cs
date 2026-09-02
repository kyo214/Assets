using System;
using System.Configuration;
using System.Globalization;

namespace NPOI.Util;

public class SystemOutLogger : POILogger
{
	private string _cat;

	public override void Initialize(string cat)
	{
		_cat = cat;
	}

	public override void Log(int level, object obj1)
	{
		Log(level, obj1, null);
	}

	public override void Log(int level, object obj1, Exception exception)
	{
		if (Check(level))
		{
			Console.WriteLine("[" + _cat + "] " + obj1);
			if (exception != null)
			{
				Console.Write(exception.StackTrace);
			}
		}
	}

	public override bool Check(int level)
	{
		int num;
		try
		{
			string text = ConfigurationManager.AppSettings["poi.log.level"];
			if (string.IsNullOrEmpty(text))
			{
				text = 5.ToString(CultureInfo.InvariantCulture);
			}
			num = int.Parse(text, CultureInfo.InvariantCulture);
		}
		catch (Exception)
		{
			num = 1;
		}
		if (level >= num)
		{
			return true;
		}
		return false;
	}
}
