using System;

namespace Doozy.Runtime.Reactor.Targets;

[Serializable]
public abstract class ReactorMetaColorTarget<T> : ReactorColorTarget
{
	public T Target;

	public override bool hasTarget => Target != null;
}
