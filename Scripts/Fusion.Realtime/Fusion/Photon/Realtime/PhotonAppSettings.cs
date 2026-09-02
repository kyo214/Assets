using System;
using UnityEngine;

namespace Fusion.Photon.Realtime;

[Serializable]
[HelpURL("https://doc.photonengine.com/en-us/pun/v2/getting-started/initial-setup")]
[CreateAssetMenu(menuName = "Fusion/Photon Application Settings", fileName = "PhotonAppSettings")]
public class PhotonAppSettings : ScriptableObject
{
	public AppSettings AppSettings;

	private static PhotonAppSettings _instance;

	public const string ExpectedResourceName = "PhotonAppSettings";

	public const string ExpectedAssetName = "PhotonAppSettings.asset";

	public static PhotonAppSettings Instance
	{
		get
		{
			if ((bool)_instance)
			{
				return _instance;
			}
			_instance = Resources.Load<PhotonAppSettings>("PhotonAppSettings");
			return _instance;
		}
		set
		{
			_instance = value;
		}
	}
}
