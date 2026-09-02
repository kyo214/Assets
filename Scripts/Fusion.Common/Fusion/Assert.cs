using System;
using System.Diagnostics;
using System.Text;

namespace Fusion;

public static class Assert
{
	[Conditional("DEBUG")]
	public static void Fail()
	{
		throw new AssertException();
	}

	[Conditional("DEBUG")]
	public static void Fail(string error)
	{
		throw new AssertException(error);
	}

	[Conditional("DEBUG")]
	public static void Fail(string format, params object[] args)
	{
		throw new AssertException(string.Format(format, args));
	}

	[Conditional("DEBUG")]
	public static void Check(object condition)
	{
		if (condition == null)
		{
			throw new AssertException();
		}
	}

	[Conditional("DEBUG")]
	public unsafe static void Check(void* condition)
	{
		if (condition == null)
		{
			throw new AssertException();
		}
	}

	[Conditional("DEBUG")]
	public static void Check(bool condition)
	{
		if (!condition)
		{
			throw new AssertException();
		}
	}

	[Conditional("DEBUG")]
	public static void Check(bool condition, string error)
	{
		if (!condition)
		{
			throw new AssertException(error);
		}
	}

	[Conditional("DEBUG")]
	public static void Check(bool condition, string format, params object[] args)
	{
		if (!condition)
		{
			throw new AssertException(string.Format(format, args));
		}
	}

	[Conditional("DEBUG")]
	public static void Check<T0>(bool condition, T0 arg0)
	{
		if (!condition)
		{
			throw new AssertException($"arg0:{arg0}");
		}
	}

	[Conditional("DEBUG")]
	public static void Check<T0, T1>(bool condition, T0 arg0, T1 arg1)
	{
		if (!condition)
		{
			throw new AssertException($"arg0:{arg0} arg1:{arg1}");
		}
	}

	[Conditional("DEBUG")]
	internal static void Check<TLogBuilder, T0, T1>(bool condition, T0 arg0, T1 arg1, TLogBuilder builder = null) where TLogBuilder : class, ILogBuilder
	{
		if (!condition)
		{
			throw new AssertException(MakeMessage(builder, $"arg0:{arg0} arg1:{arg1}"));
		}
	}

	[Conditional("DEBUG")]
	public static void Check<T0, T1, T2>(bool condition, T0 arg0, T1 arg1, T2 arg2)
	{
		if (!condition)
		{
			throw new AssertException($"arg0:{arg0} arg1:{arg1} arg2:{arg2}");
		}
	}

	[Conditional("DEBUG")]
	public static void Check<T0, T1, T2, T3>(bool condition, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!condition)
		{
			throw new AssertException($"arg0:{arg0} arg1:{arg1} arg2:{arg2} arg3:{arg3}");
		}
	}

	[Conditional("DEBUG")]
	public static void Check<T0, T1, T2, T3, T4>(bool condition, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		if (!condition)
		{
			throw new AssertException($"arg0:{arg0} arg1:{arg1} arg2:{arg2} arg3:{arg3} arg4:{arg4}");
		}
	}

	[Obsolete("Use overload with a message instead")]
	public static void AlwaysFail()
	{
		throw new AssertException();
	}

	public static void AlwaysFail(string error)
	{
		throw new AssertException(error);
	}

	public static void AlwaysFail(object error)
	{
		throw new AssertException(error?.ToString());
	}

	public static void AlwaysFail<T>(T error) where T : struct
	{
		throw new AssertException(error.ToString());
	}

	[Obsolete("Use overload with a message instead")]
	public static void Always(bool condition)
	{
		if (!condition)
		{
			throw new AssertException();
		}
	}

	public static void Always(bool condition, string error)
	{
		if (!condition)
		{
			throw new AssertException(error);
		}
	}

	public static void Always(bool condition, string format, params object[] args)
	{
		if (!condition)
		{
			throw new AssertException(string.Format(format, args));
		}
	}

	public static void Always<T0>(bool condition, T0 arg0)
	{
		if (!condition)
		{
			throw new AssertException($"arg0:{arg0}");
		}
	}

	public static void Always<T0, T1>(bool condition, T0 arg0, T1 arg1)
	{
		if (!condition)
		{
			throw new AssertException($"arg0:{arg0} arg1:{arg1}");
		}
	}

	public static void Always<T0, T1, T2>(bool condition, T0 arg0, T1 arg1, T2 arg2)
	{
		if (!condition)
		{
			throw new AssertException($"arg0:{arg0} arg1:{arg1} arg2:{arg2}");
		}
	}

	public static void Always<T0, T1, T2, T3>(bool condition, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!condition)
		{
			throw new AssertException($"arg0:{arg0} arg1:{arg1} arg2:{arg2} arg3:{arg3}");
		}
	}

	private static string MakeMessage<T>(T builder, string message) where T : class, ILogBuilder
	{
		if (builder != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			builder.BuildLogMessage(stringBuilder, message, default(LogOptions));
			return stringBuilder.ToString();
		}
		return message;
	}
}
