using UnityEngine;

public class LightShowHide : MonoBehaviour
{
	private Light mylight;

	private Renderer m_Renderer;

	private void Start()
	{
		mylight = GetComponent<Light>();
		m_Renderer = GetComponent<Renderer>();
	}

	private void FixedUpdate()
	{
		if (m_Renderer != null)
		{
			if (m_Renderer.isVisible)
			{
				mylight.enabled = true;
			}
			else
			{
				mylight.enabled = false;
			}
		}
	}
}
