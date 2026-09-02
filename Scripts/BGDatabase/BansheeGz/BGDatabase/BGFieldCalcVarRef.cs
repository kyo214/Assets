using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGFieldCalcVarRef : BGObjectI
{
	public class VarRefContainer
	{
		private readonly List<BGFieldCalcVarRef> vars = new List<BGFieldCalcVarRef>();

		public int Count => vars.Count;

		public BGFieldCalcVarRef this[int i] => vars[i];

		public event Action OnAnyChange;

		internal VarRefContainer()
		{
		}

		public void RemoveAt(int i)
		{
			vars.RemoveAt(i);
			FireAnyChange();
		}

		public void Add(BGFieldCalcVarRef varRef)
		{
			vars.Add(varRef);
			FireAnyChange();
		}

		public void ForEach(Action<BGFieldCalcVarRef> action)
		{
			for (int i = 0; i < vars.Count; i++)
			{
				action(vars[i]);
			}
		}

		public BGFieldCalcVarRef NewVar(BGId id, object value)
		{
			return new BGFieldCalcVarRef(this, id)
			{
				Value = value
			};
		}

		public BGFieldCalcVarRef NewVar(BGId id)
		{
			return new BGFieldCalcVarRef(this, id);
		}

		public void FireAnyChange()
		{
			OnAnyChange?.Invoke();
		}

		protected bool Equals(VarRefContainer other)
		{
			if (vars.Count != other.vars.Count)
			{
				return false;
			}
			for (int i = 0; i < vars.Count; i++)
			{
				BGFieldCalcVarRef objA = vars[i];
				BGFieldCalcVarRef objB = other.vars[i];
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
			return Equals((VarRefContainer)obj);
		}

		public override int GetHashCode()
		{
			if (vars == null)
			{
				return 0;
			}
			return vars.GetHashCode();
		}
	}

	private readonly VarRefContainer container;

	private BGId id;

	private object value;

	public BGId Id => id;

	public object Value
	{
		get
		{
			return value;
		}
		set
		{
			if (!object.Equals(this.value, value))
			{
				this.value = value;
				Container.FireAnyChange();
			}
		}
	}

	public VarRefContainer Container => container;

	private BGFieldCalcVarRef(VarRefContainer container)
	{
		this.container = container;
		container.Add(this);
	}

	private BGFieldCalcVarRef(VarRefContainer container, BGId id)
	{
		this.container = container;
		this.id = id;
		container.Add(this);
	}

	public BGFieldCalcVarRef CloneTo(VarRefContainer newContainer)
	{
		return new BGFieldCalcVarRef(newContainer, Id)
		{
			Value = Value
		};
	}

	public static VarRefContainer NewContainer()
	{
		return new VarRefContainer();
	}

	protected bool Equals(BGFieldCalcVarRef other)
	{
		if (id.Equals(other.id))
		{
			return object.Equals(value, other.value);
		}
		return false;
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
		return Equals((BGFieldCalcVarRef)obj);
	}

	public override int GetHashCode()
	{
		return (id.GetHashCode() * 397) ^ ((value != null) ? value.GetHashCode() : 0);
	}

	public static bool operator ==(BGFieldCalcVarRef left, BGFieldCalcVarRef right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(BGFieldCalcVarRef left, BGFieldCalcVarRef right)
	{
		return !object.Equals(left, right);
	}
}
