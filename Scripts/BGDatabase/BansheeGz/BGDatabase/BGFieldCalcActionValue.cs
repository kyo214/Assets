using System;

namespace BansheeGz.BGDatabase;

public struct BGFieldCalcActionValue
{
	private readonly Action action;

	public Action Action => action;

	public bool IsEmpty => action == null;

	public BGFieldCalcActionValue(Action action)
	{
		this.action = action;
	}

	public static implicit operator Action(BGFieldCalcActionValue value)
	{
		return value.action;
	}

	public override string ToString()
	{
		if (action != null)
		{
			return "Action";
		}
		return "No action";
	}

	public void Invoke()
	{
		action();
	}
}
