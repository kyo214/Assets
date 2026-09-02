namespace Doozy.Runtime.UIManager.Input;

public static class BackButtonExtras
{
	public static bool SendsBackButtonSignal<T>(this T target) where T : InputToSignal
	{
		if (target != null && target.isConnected)
		{
			return target.inputActionName.Equals(UIInputActionName.Cancel.ToString());
		}
		return false;
	}
}
