using UnityEngine;

public class GlobalDebugManager : MonoBehaviour
{
	public static GlobalDebugManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public void ShowLog(string logText)
	{
		Debug.Log(logText);
	}
}
