using System;

namespace NPOI.Util;

public class NullLogger : POILogger
{
	public override void Initialize(string cat)
	{
	}

	public override void Log(int level, object obj1)
	{
	}

	public override bool Check(int level)
	{
		return false;
	}

	public override void Log(int level, object obj1, object obj2)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, object obj4)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8)
	{
	}

	public override void Log(int level, object obj1, Exception exception)
	{
	}

	public override void Log(int level, object obj1, object obj2, Exception exception)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, Exception exception)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, object obj4, Exception exception)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, Exception exception)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, Exception exception)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, Exception exception)
	{
	}

	public override void Log(int level, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8, Exception exception)
	{
	}
}
