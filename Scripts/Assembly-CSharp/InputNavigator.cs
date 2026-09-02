using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputNavigator : MonoBehaviour
{
	private EventSystem system;

	private void Start()
	{
		system = EventSystem.current;
	}

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Tab))
		{
			return;
		}
		Selectable selectable = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
		if (selectable != null)
		{
			InputField component = selectable.GetComponent<InputField>();
			if (component != null)
			{
				component.OnPointerClick(new PointerEventData(system));
			}
			system.SetSelectedGameObject(selectable.gameObject, new BaseEventData(system));
		}
	}
}
