using UnityEngine;

public class FPSDemoGUI : MonoBehaviour
{
	public GameObject[] Prefabs;

	public Transform muzzleFlashPoint;

	public GameObject Gun;

	public float reactivateTime = 4f;

	public Light Sun;

	public bool IsMobile;

	private int currentNomber;

	private GameObject currentInstance;

	private GUIStyle guiStyleHeader = new GUIStyle();

	private float sunIntensity;

	private float dpiScale;

	private void Start()
	{
		if (Screen.dpi < 1f)
		{
			dpiScale = 1f;
		}
		if (Screen.dpi < 200f)
		{
			dpiScale = 1f;
		}
		else
		{
			dpiScale = Screen.dpi / 200f;
		}
		guiStyleHeader.fontSize = (int)(15f * dpiScale);
		guiStyleHeader.normal.textColor = new Color(0.15f, 0.15f, 0.15f);
		currentInstance = Object.Instantiate(Prefabs[currentNomber], base.transform.position, base.transform.rotation);
		currentInstance.AddComponent<FPSDemoReactivator>().TimeDelayToReactivate = reactivateTime;
		sunIntensity = Sun.intensity;
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f * dpiScale, 15f * dpiScale, 135f * dpiScale, 37f * dpiScale), "PREVIOUS EFFECT"))
		{
			ChangeCurrent(-1);
		}
		if (GUI.Button(new Rect(160f * dpiScale, 15f * dpiScale, 135f * dpiScale, 37f * dpiScale), "NEXT EFFECT"))
		{
			ChangeCurrent(1);
		}
		Sun.intensity = sunIntensity;
		GUI.Label(new Rect(400f * dpiScale, 15f * dpiScale, 100f * dpiScale, 20f * dpiScale), "Prefab name is \"" + Prefabs[currentNomber].name + "\"  \r\nHold any mouse button that would move the camera", guiStyleHeader);
	}

	private void ChangeCurrent(int delta)
	{
		currentNomber += delta;
		if (currentNomber > Prefabs.Length - 1)
		{
			currentNomber = 0;
		}
		else if (currentNomber < 0)
		{
			currentNomber = Prefabs.Length - 1;
		}
		if (currentInstance != null)
		{
			Object.Destroy(currentInstance);
		}
		if (currentNomber < 10)
		{
			currentInstance = Object.Instantiate(Prefabs[currentNomber], base.transform.position, base.transform.rotation);
			Gun.SetActive(value: false);
		}
		else
		{
			currentInstance = Object.Instantiate(Prefabs[currentNomber], muzzleFlashPoint.position, muzzleFlashPoint.rotation);
			Gun.SetActive(value: true);
		}
		currentInstance.AddComponent<FPSDemoReactivator>().TimeDelayToReactivate = reactivateTime;
		if (IsMobile)
		{
			CancelInvoke("Reactivator");
			InvokeRepeating("Reactivator", 0f, 2f);
		}
	}

	private void Reactivator()
	{
		currentInstance.SetActive(value: false);
		currentInstance.SetActive(value: true);
	}
}
