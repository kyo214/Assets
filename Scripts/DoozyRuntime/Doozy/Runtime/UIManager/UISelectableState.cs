using System;
using Doozy.Runtime.Mody;
using UnityEngine;

namespace Doozy.Runtime.UIManager;

[Serializable]
public struct UISelectableState
{
	[SerializeField]
	private UISelectionState StateType;

	[SerializeField]
	private ModyEvent StateEvent;

	public UISelectionState stateType => StateType;

	public ModyEvent stateEvent => StateEvent;

	public UISelectableState(UISelectionState type)
	{
		StateType = type;
		StateEvent = new ModyEvent(type.ToString());
	}
}
