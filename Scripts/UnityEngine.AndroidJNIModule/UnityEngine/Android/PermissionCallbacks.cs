using System;

namespace UnityEngine.Android;

public class PermissionCallbacks : AndroidJavaProxy
{
	public event Action<string> PermissionGranted;

	public event Action<string> PermissionDenied;

	public event Action<string> PermissionDeniedAndDontAskAgain;

	public PermissionCallbacks()
		: base("com.unity3d.player.IPermissionRequestCallbacks")
	{
	}

	private void onPermissionGranted(string permissionName)
	{
		PermissionGranted?.Invoke(permissionName);
	}

	private void onPermissionDenied(string permissionName)
	{
		PermissionDenied?.Invoke(permissionName);
	}

	private void onPermissionDeniedAndDontAskAgain(string permissionName)
	{
		if (PermissionDeniedAndDontAskAgain != null)
		{
			PermissionDeniedAndDontAskAgain(permissionName);
		}
		else
		{
			PermissionDenied?.Invoke(permissionName);
		}
	}
}
