using Toked;
using UnityEngine;

public class EventStartAmbient : MonoBehaviour
{
	[SerializeField]
	private string AmbientName;

	[SerializeField]
	private float fadingTime;

	private void Start()
	{
		AudioManager.PlayAmbient(AmbientName, fadingTime);
	}
}
