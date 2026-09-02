using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
	public Color requiredColor = Color.magenta;

	public GameObject doorToOpen;

	public float colorTolerance = 0.1f;

	private bool isHitThisFrame;

	public void CheckColor(Color incomingColor)
	{
		isHitThisFrame = true;
		if (Mathf.Abs(incomingColor.r - requiredColor.r) < colorTolerance && Mathf.Abs(incomingColor.g - requiredColor.g) < colorTolerance && Mathf.Abs(incomingColor.b - requiredColor.b) < colorTolerance)
		{
			OpenDoor();
		}
		else
		{
			CloseDoor();
		}
	}

	private void OpenDoor()
	{
		if (doorToOpen != null && doorToOpen.activeSelf)
		{
			doorToOpen.SetActive(value: false);
			Debug.Log("Puzzle Solved! Door Opened.");
		}
	}

	private void CloseDoor()
	{
		if (doorToOpen != null && !doorToOpen.activeSelf)
		{
			doorToOpen.SetActive(value: true);
		}
	}

	private void LateUpdate()
	{
		if (!isHitThisFrame)
		{
			CloseDoor();
		}
		isHitThisFrame = false;
	}
}
