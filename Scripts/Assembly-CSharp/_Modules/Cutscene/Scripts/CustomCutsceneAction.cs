using UnityEngine;

namespace _Modules.Cutscene.Scripts;

public abstract class CustomCutsceneAction : ScriptableObject
{
	public abstract void Invoke();
}
