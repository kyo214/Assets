using System;

namespace Doozy.Runtime.Reactor.Targets;

[Serializable]
public abstract class ReactorMetaSpriteTarget<T> : ReactorSpriteTarget
{
	public T Target;

	public override bool hasTarget => Target != null;
}
