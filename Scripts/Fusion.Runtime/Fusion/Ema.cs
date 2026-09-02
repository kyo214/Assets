using System;

namespace Fusion;

public struct Ema
{
	private const int LENGTH = 64;

	private int _cnt;

	private double _val;

	private double _var;

	private double _lst;

	private unsafe fixed double _wnd[64];

	public bool Full => _cnt >= 64;

	public double Lst => _lst;

	public double Val => _val;

	public double Dev
	{
		get
		{
			double var = GetVar();
			if (var >= double.Epsilon)
			{
				double num = Math.Sqrt(var);
				return double.IsNaN(num) ? 0.0 : num;
			}
			return 0.0;
		}
	}

	private double GetVar()
	{
		return (_cnt > 1) ? (_var / (double)(Math.Min(_cnt, 64) - 1)) : 0.0;
	}

	public unsafe void Add(double val)
	{
		_lst = val;
		int num = _cnt % 64;
		double num2 = _wnd[num];
		_wnd[num] = val;
		_cnt++;
		double val2 = _val;
		if (_cnt <= 64)
		{
			double num3 = val - val2;
			_val += num3 / (double)_cnt;
			_var += num3 * (val - _val);
		}
		else
		{
			double num4 = val - num2;
			_val += num4 / 64.0;
			_var += num4 * (val - _val + (num2 - val2));
		}
	}
}
