using System;
using System.Collections.Generic;
using System.Globalization;

namespace NPOI.SS.Format;

public abstract class CellFormatCondition
{
	private class LT_CellFormatCondition : CellFormatCondition
	{
		private double _c;

		public LT_CellFormatCondition(double c)
		{
			_c = c;
		}

		public override bool Pass(double value)
		{
			return value < _c;
		}
	}

	private class LE_CellFormatCondition : CellFormatCondition
	{
		private double _c;

		public LE_CellFormatCondition(double c)
		{
			_c = c;
		}

		public override bool Pass(double value)
		{
			return value <= _c;
		}
	}

	private class GT_CellFormatCondition : CellFormatCondition
	{
		private double _c;

		public GT_CellFormatCondition(double c)
		{
			_c = c;
		}

		public override bool Pass(double value)
		{
			return value > _c;
		}
	}

	private class GE_CellFormatCondition : CellFormatCondition
	{
		private double _c;

		public GE_CellFormatCondition(double c)
		{
			_c = c;
		}

		public override bool Pass(double value)
		{
			return value >= _c;
		}
	}

	private class EQ_CellFormatCondition : CellFormatCondition
	{
		private double _c;

		public EQ_CellFormatCondition(double c)
		{
			_c = c;
		}

		public override bool Pass(double value)
		{
			return value == _c;
		}
	}

	private class NE_CellFormatCondition : CellFormatCondition
	{
		private double _c;

		public NE_CellFormatCondition(double c)
		{
			_c = c;
		}

		public override bool Pass(double value)
		{
			return value != _c;
		}
	}

	private const int LT = 0;

	private const int LE = 1;

	private const int GT = 2;

	private const int GE = 3;

	private const int EQ = 4;

	private const int NE = 5;

	private static Dictionary<string, int> TESTS;

	static CellFormatCondition()
	{
		TESTS = new Dictionary<string, int>();
		TESTS.Add("<", 0);
		TESTS.Add("<=", 1);
		TESTS.Add(">", 2);
		TESTS.Add(">=", 3);
		TESTS.Add("=", 4);
		TESTS.Add("==", 4);
		TESTS.Add("!=", 5);
		TESTS.Add("<>", 5);
	}

	public static CellFormatCondition GetInstance(string opString, string constStr)
	{
		if (!TESTS.ContainsKey(opString))
		{
			throw new ArgumentException("Unknown test: " + opString);
		}
		int num = TESTS[opString];
		double c = double.Parse(constStr, CultureInfo.InvariantCulture);
		return num switch
		{
			0 => new LT_CellFormatCondition(c), 
			1 => new LE_CellFormatCondition(c), 
			2 => new GT_CellFormatCondition(c), 
			3 => new GE_CellFormatCondition(c), 
			4 => new EQ_CellFormatCondition(c), 
			5 => new NE_CellFormatCondition(c), 
			_ => throw new ArgumentException("Cannot create for test number " + num + "(\"" + opString + "\")"), 
		};
	}

	public abstract bool Pass(double value);
}
