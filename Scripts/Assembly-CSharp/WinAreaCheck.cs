using UnityEngine;

public class WinAreaCheck : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerController component = other.GetComponent<PlayerController>();
		GameManager.Instance.ListPlayerInAreaWin[component.network.GetIDX()] = true;
		if (!MissionManager.Instance.IsCountAgentInCircle)
		{
			return;
		}
		int num = 0;
		foreach (bool item in GameManager.Instance.ListPlayerInAreaWin)
		{
			if (item)
			{
				num++;
			}
		}
		ChatSystem.Instance.TextCountEscape.text = num + " / " + NetworkGameManager.Instance.arrPlayerController.Count;
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		GameManager.Instance.ListPlayerInAreaWin[other.GetComponent<PlayerController>().network.GetIDX()] = false;
		if (!MissionManager.Instance.IsCountAgentInCircle)
		{
			return;
		}
		int num = 0;
		foreach (bool item in GameManager.Instance.ListPlayerInAreaWin)
		{
			if (item)
			{
				num++;
			}
		}
		ChatSystem.Instance.TextCountEscape.text = num + " / " + NetworkGameManager.Instance.arrPlayerController.Count;
	}
}
