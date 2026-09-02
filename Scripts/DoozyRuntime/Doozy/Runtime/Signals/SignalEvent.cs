using System;
using UnityEngine.Events;

namespace Doozy.Runtime.Signals;

[Serializable]
public class SignalEvent : UnityEvent<Signal>
{
}
