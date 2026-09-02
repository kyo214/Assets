using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

public class MMDebugMenuSpriteReplace : MonoBehaviour
{
	public Sprite OnSprite;

	public Sprite OffSprite;

	public bool StartsOn = true;

	protected Image _image;

	protected MMTouchButton _mmTouchButton;

	public bool CurrentValue => _image.sprite == OnSprite;

	protected virtual void Awake()
	{
	}

	public virtual void Initialization()
	{
		_image = base.gameObject.GetComponent<Image>();
		_mmTouchButton = base.gameObject.GetComponent<MMTouchButton>();
		if (_mmTouchButton != null)
		{
			_mmTouchButton.ReturnToInitialSpriteAutomatically = false;
		}
		if (!(_image == null) && !(OnSprite == null) && !(OffSprite == null))
		{
			if (StartsOn)
			{
				_image.sprite = OnSprite;
			}
			else
			{
				_image.sprite = OffSprite;
			}
		}
	}

	public virtual void Swap()
	{
		if (_image.sprite != OnSprite)
		{
			SwitchToOnSprite();
		}
		else
		{
			SwitchToOffSprite();
		}
	}

	public virtual void SwitchToOffSprite()
	{
		if (!(_image == null) && !(OffSprite == null))
		{
			SpriteOff();
		}
	}

	protected virtual void SpriteOff()
	{
		_image.sprite = OffSprite;
	}

	public virtual void SwitchToOnSprite()
	{
		if (!(_image == null) && !(OnSprite == null))
		{
			SpriteOn();
		}
	}

	protected virtual void SpriteOn()
	{
		_image.sprite = OnSprite;
	}
}
