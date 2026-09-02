using System;

namespace Fusion;

internal struct TimerDelta
{
	private Timer _timer;

	private double _timerLast;

	public bool IsRunning => _timer.IsRunning;

	public double Consume()
	{
		double elapsedInSeconds = _timer.ElapsedInSeconds;
		double result = Math.Max(elapsedInSeconds - _timerLast, 0.0);
		_timerLast = elapsedInSeconds;
		return result;
	}

	public double Peek()
	{
		return Math.Max(_timer.ElapsedInSeconds - _timerLast, 0.0);
	}

	public static TimerDelta StartNew()
	{
		return new TimerDelta
		{
			_timer = Timer.StartNew()
		};
	}
}
