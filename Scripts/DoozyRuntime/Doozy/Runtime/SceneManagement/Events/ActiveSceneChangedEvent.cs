using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Doozy.Runtime.SceneManagement.Events;

[Serializable]
public class ActiveSceneChangedEvent : UnityEvent<Scene, Scene>
{
}
