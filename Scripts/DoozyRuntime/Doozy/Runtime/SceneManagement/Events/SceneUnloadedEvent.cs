using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Doozy.Runtime.SceneManagement.Events;

[Serializable]
public class SceneUnloadedEvent : UnityEvent<Scene>
{
}
