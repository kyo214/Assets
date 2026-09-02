using System;
using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Tools;

[Serializable]
public class JoystickEvent : UnityEvent<Vector2>
{
}
