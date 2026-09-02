using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGMonitorPageLiveUpdateStatus : BGMonitorPage
{
	public BGRTScrollView scrollView;

	public override string Name => "LiveUpdate addon status";

	public override void Gui()
	{
		BGAddonLiveUpdate addon = BGRepo.I.Addons.Get<BGAddonLiveUpdate>();
		if (addon == null)
		{
			BGRTUtilities.Label("LiveUpdate plugin is not installed!");
			return;
		}
		BGLiveUpdateLog log = addon.Log;
		BGRTUtilities.Label($"LiveUpdate plugin status: {log.Status}");
		scrollView = scrollView ?? new BGRTScrollView(() =>
		{
			ShowLog(addon);
		});
		scrollView.Gui();
	}

	private void ShowLog(BGAddonLiveUpdate addon)
	{
		string log = addon.Log.GetLog();
		GUILayout.TextArea(log, new GUIStyle("textArea")
		{
			fontSize = BGDatabaseMonitorGo.I.fontSize
		}, GUILayout.ExpandWidth(expand: true), GUILayout.ExpandHeight(expand: true));
	}
}
