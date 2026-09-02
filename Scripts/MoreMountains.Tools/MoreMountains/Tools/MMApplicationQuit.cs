using UnityEngine;

namespace MoreMountains.Tools;

public class MMApplicationQuit : MonoBehaviour
{
	[Header("Debug")]
	[MMInspectorButton("Quit")]
	public bool QuitButton;

	public virtual void Quit()
	{
		Application.Quit();
	}
}
