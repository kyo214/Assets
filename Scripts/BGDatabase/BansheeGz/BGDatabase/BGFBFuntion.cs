using System;

namespace BansheeGz.BGDatabase;

public abstract class BGFBFuntion
{
	public const short CustomCode = 1;

	public abstract string Name { get; }

	public abstract Type ReturnType { get; }

	public abstract bool Supports(BGField field);

	public abstract object Convert(BGField field, BGEntity e, object value);
}
