using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Events;

[Serializable]
public class BaseEventDataEvent : UnityEvent<BaseEventData>
{
}
