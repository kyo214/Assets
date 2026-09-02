using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

public class MMDebugMenuDebugTab : MonoBehaviour
{
	public ScrollRect DebugScrollRect;

	public Text DebugText;

	public InputField CommandPrompt;

	public Text CommandPromptCharacter;

	public bool TouchScreenVisible;

	protected TouchScreenKeyboard _touchScreenKeyboard;

	protected RectTransform _rectTransform;

	protected float _mobileMenuOffset = -1000f;

	protected bool _touchScreenVisibleLastFrame;

	protected virtual void Awake()
	{
		MMDebug.MMDebugLogEvent.Register(OnMMDebugLogEvent);
		DebugText.text = "";
		_rectTransform = base.gameObject.GetComponent<RectTransform>();
		CommandPrompt.onEndEdit.AddListener((string val) =>
		{
			CommandPrompt.text = "";
			if (val != "")
			{
				MMDebug.DebugLogCommand(val);
			}
		});
	}

	protected virtual void Update()
	{
		TouchScreenVisible = TouchScreenKeyboard.visible;
		if (TouchScreenVisible)
		{
			_rectTransform.MMSetBottom(650f);
		}
		else
		{
			_rectTransform.MMSetBottom(0f);
		}
	}

	protected virtual void LateUpdate()
	{
		if (_touchScreenVisibleLastFrame != TouchScreenVisible)
		{
			StartCoroutine(ScrollToLogBottomCo());
		}
		_touchScreenVisibleLastFrame = TouchScreenVisible;
	}

	protected virtual void OnEnable()
	{
		StartCoroutine(ScrollToLogBottomCo());
	}

	protected virtual void OnMMDebugLogEvent(MMDebug.DebugLogItem item)
	{
		DebugText.text = MMDebug.LogHistoryText;
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(ScrollToLogBottomCo());
		}
	}

	protected virtual IEnumerator ScrollToLogBottomCo()
	{
		yield return new WaitForEndOfFrame();
		DebugScrollRect.normalizedPosition = Vector2.zero;
		CommandPrompt.ActivateInputField();
		CommandPrompt.Select();
	}

	public virtual void OnDestroy()
	{
		MMDebug.MMDebugLogEvent.Unregister(OnMMDebugLogEvent);
	}
}
