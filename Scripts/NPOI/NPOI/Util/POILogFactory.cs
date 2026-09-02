using System;
using System.Collections;
using System.Configuration;

namespace NPOI.Util;

public class POILogFactory
{
	private static Hashtable _loggers = new Hashtable();

	private static POILogger _nullLogger = new NullLogger();

	private static string _loggerClassName = null;

	private POILogFactory()
	{
	}

	public static POILogger GetLogger(Type type)
	{
		return GetLogger(type.Name);
	}

	public static POILogger GetLogger(string cat)
	{
		POILogger pOILogger = null;
		if (_loggerClassName == null)
		{
			try
			{
				_loggerClassName = ConfigurationManager.AppSettings["loggername"];
			}
			catch (Exception)
			{
			}
			if (_loggerClassName == null)
			{
				_loggerClassName = _nullLogger.GetType().Name;
			}
		}
		if (_loggerClassName.Equals(_nullLogger.GetType().Name))
		{
			return _nullLogger;
		}
		if (_loggers.ContainsKey(cat))
		{
			pOILogger = (POILogger)_loggers[cat];
		}
		else
		{
			try
			{
				pOILogger = Activator.CreateInstance(Type.GetType(_loggerClassName)) as POILogger;
				pOILogger.Initialize(cat);
			}
			catch (Exception)
			{
				pOILogger = _nullLogger;
			}
			_loggers[cat] = pOILogger;
		}
		return pOILogger;
	}
}
