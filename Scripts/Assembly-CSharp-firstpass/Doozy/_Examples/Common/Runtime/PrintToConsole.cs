using UnityEngine;

namespace Doozy._Examples.Common.Runtime;

public class PrintToConsole : MonoBehaviour
{
	public void DebugLog(string message)
	{
		Debug.Log(message);
	}
}
