using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks;

[ExecuteAlways]
public class DemoButton : MonoBehaviour
{
	[Header("Behaviour")]
	public bool NotSupportedInWebGL;

	[Header("Bindings")]
	public Button TargetButton;

	public Text ButtonText;

	public Text WebGL;

	public MMF_Player TargetMMF_Player;

	public MMFeedbacks TargetMMFeedbacks;

	protected Color _disabledColor = new Color(255f, 255f, 255f, 0.5f);

	protected virtual void OnEnable()
	{
		HandleWebGL();
	}

	protected virtual void ConvertButtonToMMFPlayerDemo()
	{
	}

	public void OnClickEvent()
	{
		TargetMMF_Player.PlayFeedbacks();
	}

	protected virtual void HandleWebGL()
	{
		if (WebGL != null)
		{
			WebGL.gameObject.SetActive(value: false);
			TargetButton.interactable = true;
			ButtonText.color = Color.white;
		}
	}
}
