namespace Doozy.Runtime.Reactor;

public enum ReactionState
{
	Pooled = 0,
	Idle = 1,
	StartDelay = 2,
	Playing = 3,
	Paused = 4,
	LoopDelay = 5
}
