using System;
using System.Text;

namespace NPOI.Util;

public abstract class POILogger
{
	public const int DEBUG = 1;

	public const int INFO = 3;

	public const int WARN = 5;

	public const int ERROR = 7;

	public const int FATAL = 9;

	public POILogger()
	{
	}

	public abstract void Initialize(string cat);

	public abstract void Log(int level, object obj1);

	public abstract void Log(int level, object obj1, Exception exception);

	public abstract bool Check(int level);

	public virtual void Log(int level, object obj1, object obj2)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(32).Append(obj1).Append(obj2));
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(48).Append(obj1).Append(obj2).Append(obj3));
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, object obj4)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(64).Append(obj1).Append(obj2).Append(obj3)
				.Append(obj4));
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(80).Append(obj1).Append(obj2).Append(obj3)
				.Append(obj4)
				.Append(obj5));
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(96).Append(obj1).Append(obj2).Append(obj3)
				.Append(obj4)
				.Append(obj5)
				.Append(obj6));
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(112).Append(obj1).Append(obj2).Append(obj3)
				.Append(obj4)
				.Append(obj5)
				.Append(obj6)
				.Append(obj7));
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(128).Append(obj1).Append(obj2).Append(obj3)
				.Append(obj4)
				.Append(obj5)
				.Append(obj6)
				.Append(obj7)
				.Append(obj8));
		}
	}

	public virtual void Log(int level, Exception exception)
	{
		Log(level, null, exception);
	}

	public virtual void Log(int level, object obj1, object obj2, Exception exception)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(32).Append(obj1).Append(obj2), exception);
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, Exception exception)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(48).Append(obj1).Append(obj2).Append(obj3), exception);
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, object obj4, Exception exception)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(64).Append(obj1).Append(obj2).Append(obj3)
				.Append(obj4), exception);
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, Exception exception)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(80).Append(obj1).Append(obj2).Append(obj3)
				.Append(obj4)
				.Append(obj5), exception);
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, Exception exception)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(96).Append(obj1).Append(obj2).Append(obj3)
				.Append(obj4)
				.Append(obj5)
				.Append(obj6), exception);
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, Exception exception)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(112).Append(obj1).Append(obj2).Append(obj3)
				.Append(obj4)
				.Append(obj5)
				.Append(obj6)
				.Append(obj7), exception);
		}
	}

	public virtual void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8, Exception exception)
	{
		if (Check(level))
		{
			Log(level, new StringBuilder(128).Append(obj1).Append(obj2).Append(obj3)
				.Append(obj4)
				.Append(obj5)
				.Append(obj6)
				.Append(obj7)
				.Append(obj8), exception);
		}
	}
}
