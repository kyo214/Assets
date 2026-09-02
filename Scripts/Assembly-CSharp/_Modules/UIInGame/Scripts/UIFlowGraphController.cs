using Doozy.Runtime.Nody;
using UnityEngine;

namespace _Modules.UIInGame.Scripts;

public class UIFlowGraphController : MonoBehaviour
{
	[SerializeField]
	private FlowController _flowController;

	[SerializeField]
	private FlowGraph _mainMenuSaveFlowGraph;

	[SerializeField]
	private FlowGraph _mainMenuDisableSaveFlowGraph;

	private void Awake()
	{
		if (GameModes.Instance.CheckDisableSaveData())
		{
			_flowController.SetFlowGraph(_mainMenuDisableSaveFlowGraph);
		}
		else
		{
			_flowController.SetFlowGraph(_mainMenuSaveFlowGraph);
		}
	}
}
