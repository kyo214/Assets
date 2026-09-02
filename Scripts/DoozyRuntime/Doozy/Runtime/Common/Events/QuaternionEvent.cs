using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Common.Events;

[Serializable]
public class QuaternionEvent : UnityEvent<Quaternion>
{
}
