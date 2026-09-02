using UnityEngine;

namespace MoreMountains.Tools;

public class MMConsole : MonoBehaviour
{
	protected string _messageStack;

	protected int _numberOfMessages;

	protected bool _messageStackHasBeenDisplayed;

	protected int _largestMessageLength;

	protected int _marginTop = 10;

	protected int _marginLeft = 10;

	protected int _padding = 10;

	protected int _fontSize = 10;

	protected int _characterHeight = 16;

	protected int _characterWidth = 6;

	protected virtual void OnGUI()
	{
		GUI.skin.GetStyle("label").fontSize = _fontSize;
		int num = _numberOfMessages * _characterHeight;
		int num2 = _largestMessageLength * _characterWidth;
		GUI.Box(new Rect(_marginLeft, _marginTop, num2 + _padding * 2, num + _padding * 2), "");
		GUI.Label(new Rect(_marginLeft + _padding, _marginTop + _padding, num2, num), _messageStack);
		_messageStackHasBeenDisplayed = true;
	}

	public virtual void SetFontSize(int fontSize)
	{
		_fontSize = fontSize;
		_characterHeight = (int)Mathf.Round(1.6f * (float)fontSize + 0.49f);
		_characterWidth = (int)Mathf.Round(0.6f * (float)fontSize + 0.49f);
	}

	public virtual void SetScreenOffset(int top = 10, int left = 10)
	{
		_marginTop = top;
		_marginLeft = left;
	}

	public virtual void SetMessage(string newMessage)
	{
		_messageStack = newMessage;
		_numberOfMessages = 1;
	}

	public virtual void AddMessage(string newMessage)
	{
		if (_messageStackHasBeenDisplayed)
		{
			_messageStack = "";
			_messageStackHasBeenDisplayed = false;
			_numberOfMessages = 0;
			_largestMessageLength = 0;
		}
		_messageStack = _messageStack + newMessage + "\n";
		if (newMessage.Length > _largestMessageLength)
		{
			_largestMessageLength = newMessage.Length;
		}
		_numberOfMessages++;
	}
}
