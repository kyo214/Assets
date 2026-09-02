using System;

namespace BansheeGz.BGDatabase;

public class BGCodedFieldContext : IDisposable
{
	private static readonly BGObjectPool<BGCodedFieldContext> cellsPool = new BGObjectPool<BGCodedFieldContext>(() => new BGCodedFieldContext());

	private BGField field;

	private BGEntity entity;

	public BGField Field
	{
		get
		{
			return this.field;
		}
		set
		{
			this.field = value;
		}
	}

	public BGEntity Entity
	{
		get
		{
			return entity;
		}
		set
		{
			entity = value;
		}
	}

	private BGCodedFieldContext()
	{
	}

	public static BGCodedFieldContext Get()
	{
		return cellsPool.Get();
	}

	public void Dispose()
	{
		field = null;
		entity = null;
		cellsPool.Return(this);
	}
}
