using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGObject : BGObjectI
{
	private BGId id;

	protected bool deleted;

	public BGId Id => id;

	public bool IsDeleted => deleted;

	public event Action<BGObject> OnUnload;

	protected BGObject(BGId id)
	{
		this.id = id;
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
		return Id == ((BGObject)obj).Id;
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}

	public override string ToString()
	{
		BGId bGId = id;
		return "[id:" + bGId.ToString() + "]";
	}

	public virtual void Delete()
	{
		deleted = true;
	}

	protected internal virtual void Unload()
	{
		deleted = true;
		try
		{
			OnUnload?.Invoke(this);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}
}
