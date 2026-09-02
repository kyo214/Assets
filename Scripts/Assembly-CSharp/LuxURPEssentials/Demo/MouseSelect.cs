using UnityEngine;

namespace LuxURPEssentials.Demo;

public class MouseSelect : MonoBehaviour
{
	private Transform selectedTransform;

	private void Update()
	{
		if (!Input.GetMouseButtonDown(0))
		{
			return;
		}
		RaycastHit hitInfo = default;
		if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
		{
			return;
		}
		if (selectedTransform != null)
		{
			ToggleOutlineSelection component = selectedTransform.GetComponent<ToggleOutlineSelection>();
			if (component != null)
			{
				component.Select();
			}
		}
		if (selectedTransform != hitInfo.transform)
		{
			ToggleOutlineSelection component2 = hitInfo.transform.GetComponent<ToggleOutlineSelection>();
			if (component2 != null)
			{
				selectedTransform = hitInfo.transform;
				component2.Select();
			}
			else
			{
				selectedTransform = null;
			}
		}
		else
		{
			selectedTransform = null;
		}
	}
}
