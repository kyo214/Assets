using UnityEngine;

public class UIPuzzle : MonoBehaviour
{
	public static UIPuzzle Instance { get; private set; }

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
}
