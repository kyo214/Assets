using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/GUI/MMCursorVisible")]
public class MMCursorVisible : MonoBehaviour
{
	public enum CursorVisibilities
	{
		Visible = 0,
		Invisible = 1
	}

	public CursorVisibilities CursorVisibility;

	protected virtual void Update()
	{
		if (CursorVisibility == CursorVisibilities.Visible)
		{
			Cursor.visible = true;
		}
		else
		{
			Cursor.visible = false;
		}
	}
}
