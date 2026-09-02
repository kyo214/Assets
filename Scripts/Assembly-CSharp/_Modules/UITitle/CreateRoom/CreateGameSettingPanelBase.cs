using System;
using UnityEngine;

namespace _Modules.UITitle.CreateRoom;

public abstract class CreateGameSettingPanelBase : MonoBehaviour
{
	public Action<bool> OnChangeValueEvent;

	public bool IsCurrentValueValid { get; set; } = true;

	public abstract void OnValueChangedAction(int index);

	public abstract void SetDataWhenCreateGame(bool isLoad);
}
