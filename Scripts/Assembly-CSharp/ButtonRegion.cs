using Fusion.Photon.Realtime;
using UnityEngine;

public class ButtonRegion : MonoBehaviour
{
	public string regionCode;

	public void ClickButton()
	{
		GlobalSaveData.instance.optionData.region = regionCode;
		GlobalSaveData.instance.optionData.lastRegion = regionCode;
		PhotonAppSettings.Instance.AppSettings.FixedRegion = regionCode;
		if (!UITitleMenuManager.Instance.isPopup && GlobalSaveData.instance.UserSaveData.UserName == "")
		{
			UITitleMenuManager.Instance.ShowTitle();
			UITitleMenuManager.Instance.UIRegion.Hide();
			UITitleMenuManager.Instance.UIUsername.Show();
		}
		else
		{
			if (UITitleMenuManager.Instance.UIMainMenu.isHidden)
			{
				UITitleMenuManager.Instance.flowControlGraph.SetActiveNodeByName("Main Menu");
			}
			UITitleMenuManager.Instance.UIRegion.Hide();
			UITitleMenuManager.Instance.UIMainMenu.Show();
			UITitleMenuManager.Instance.isPopup = false;
			UITitleMenuManager.Instance.ShowInfoBot();
			UITitleMenuManager.Instance.textUsername.text = GlobalSaveData.instance.UserSaveData.UserName;
		}
		GlobalSaveData.instance.SaveOptionData();
		UITitleMenuManager.Instance.textRegion.text = UITitleMenuManager.Instance.GetRegionName(regionCode);
		UITitleMenuManager.Instance.ShowInfoBot();
	}
}
