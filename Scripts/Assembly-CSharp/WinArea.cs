using Cysharp.Threading.Tasks;
using UnityEngine;
using _Modules.Cutscene.Scripts;

public class WinArea : MonoBehaviour
{
	[SerializeField]
	private bool _cutscene;

	[SerializeField]
	private CutsceneTrigger _cutsceneTrigger;

	private bool _isWinChecked;

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerController component = other.GetComponent<PlayerController>();
		GameManager.Instance.ListPlayerWin[component.network.GetIDX()] = true;
		if (_isWinChecked || !NetworkGameManager.Instance.isServer || GameManagerPhoton.Instance.IsWin)
		{
			return;
		}
		GameManagerPhoton.Instance.IsWin = true;
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			if (item.network.GetHealth() > 0f && GameManager.Instance.ListPlayerInAreaWin[item.network.GetIDX()])
			{
				item.network.playerPhoton.IsSurvive = true;
			}
		}
		_isWinChecked = true;
		UniTaskUtil.DelayedCall(this, 0.2f, () =>
		{
			if (GameManager.Instance.CheckWin(isEnterWinArea: true))
			{
				GameManager.Instance.TriggerWin();
			}
		}).Forget();
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && !_isWinChecked)
		{
			GameManager.Instance.ListPlayerWin[other.GetComponent<PlayerController>().network.GetIDX()] = false;
		}
	}

	public void OnCompleteLevel()
	{
		if (_cutscene)
		{
			_cutsceneTrigger.PlayCutscene();
		}
		else
		{
			NetworkGameManager.Instance.StartCoroutine(NetworkGameManager.Instance.WinLevel());
		}
	}
}
