using UnityEngine;

namespace BansheeGz.BGDatabase;

public static class BGRTDialog
{
	public static void Info(string message)
	{
		BGDatabaseMonitorGo.Popup(300, 140, "Info", () =>
		{
			GUIStyle textArea = GUI.skin.textArea;
			GUIStyle button = GUI.skin.button;
			GUILayout.TextArea(message, textArea, GUILayout.ExpandWidth(expand: true), GUILayout.ExpandHeight(expand: true));
			return GUILayout.Button("Ok", button);
		});
	}
}
