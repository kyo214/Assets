using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGCalcVarContainerBaseA<T> where T : BGCalcVarA
{
	protected readonly List<T> vars = new List<T>();

	private readonly BGCalcVarsOwnerBaseI owner;

	public List<T> Variables => vars;

	public int Count => vars.Count;

	protected BGCalcVarContainerBaseA(BGCalcVarsOwnerBaseI owner)
	{
		this.owner = owner;
	}

	public void AddVar(T variable)
	{
		if (vars.Count >= 255)
		{
			throw new Exception($"Can not add a variable: maximum number of variables={byte.MaxValue} is reached");
		}
		vars.Add(variable);
		FireOnAnyChange();
	}

	public T GetVar(int index)
	{
		return vars[index];
	}

	public void ClearVars()
	{
		ClearVarsNoEvent();
		FireOnAnyChange();
	}

	public void ClearVarsNoEvent()
	{
		vars.Clear();
	}

	public void FireOnAnyChange()
	{
		owner.OnVarsChange();
	}

	protected bool Equals(BGCalcVarContainerBaseA<T> other)
	{
		if (vars.Count != other.vars.Count)
		{
			return false;
		}
		for (int i = 0; i < vars.Count; i++)
		{
			T objA = vars[i];
			T objB = other.vars[i];
			if (!object.Equals(objA, objB))
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((BGCalcVarContainerBaseA<T>)obj);
	}

	public override int GetHashCode()
	{
		if (vars == null)
		{
			return 0;
		}
		return vars.GetHashCode();
	}

	public static bool IsEqual(BGCalcVarContainerBaseA<T> left, BGCalcVarContainerBaseA<T> right)
	{
		bool flag = left == null || left.vars.Count == 0;
		bool flag2 = right == null || right.vars.Count == 0;
		if (flag & flag2)
		{
			return true;
		}
		if (flag | flag2)
		{
			return false;
		}
		return left.Equals(right);
	}
}
