using Toked;
using UnityEngine;

public class EventStartBGM : MonoBehaviour
{
	[SerializeField]
	private string playlistName;

	[SerializeField]
	private string BGMName;

	[SerializeField]
	private float fadingTime;

	private void Start()
	{
		AudioManager.ChangeVolumeMaster((float)GlobalSaveData.instance.optionData.volMaster / 100f);
		AudioManager.ChangeVolumeSFX((float)GlobalSaveData.instance.optionData.volSFX / 100f);
		AudioManager.ChangeVolumeBGM((float)GlobalSaveData.instance.optionData.volMusic / 100f);
		AudioManager.SetBGMFixed(value: false);
		AudioManager.PlayBGM(playlistName, BGMName, fadingTime);
	}
}
