using System;
using UnityEngine;

namespace MoreMountains.Tools;

public struct MMStateChangeEvent<T>(MMStateMachine<T> stateMachine) where T : struct, IComparable, IConvertible, IFormattable
{
	public GameObject Target = stateMachine.Target;

	public MMStateMachine<T> TargetStateMachine = stateMachine;

	public T NewState = stateMachine.CurrentState;

	public T PreviousState = stateMachine.PreviousState;
}
