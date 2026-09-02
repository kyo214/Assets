#define DEBUG
using System;

namespace Fusion;

public class SmoothRandom
{
	private Random _random;

	private double _periodMin;

	private double _periodMax;

	private double _p;

	private double _a;

	private double _b;

	private Timer _t;

	public SmoothRandom(double periodMin = 0.5, double periodMax = 1.0)
	{
		Assert.Check(periodMin > 0.0);
		Assert.Check(periodMax >= periodMin);
		_random = new Random();
		_periodMin = periodMin;
		_periodMax = periodMax;
		_a = Rng();
		_b = Rng();
		StartNewPeriod();
	}

	public double Next()
	{
		double elapsedInSeconds = _t.ElapsedInSeconds;
		if (elapsedInSeconds >= _p)
		{
			_a = _b;
			_b = Rng();
			StartNewPeriod();
			return _a;
		}
		return Maths.CosineInterpolate(_a, _b, elapsedInSeconds / _p);
	}

	private void StartNewPeriod()
	{
		_p = _periodMin + _random.NextDouble() * (_periodMax - _periodMin);
		_t = Timer.StartNew();
	}

	private double Rng()
	{
		return _random.NextDouble();
	}
}
