using System;

namespace MoreMountains.Tools;

[Serializable]
public class AITransition
{
	public AIDecision Decision;

	public string TrueState;

	public string FalseState;
}
