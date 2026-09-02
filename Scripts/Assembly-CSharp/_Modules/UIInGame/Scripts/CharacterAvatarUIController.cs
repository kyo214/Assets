using UnityEngine;
using UnityEngine.UI;

namespace _Modules.UIInGame.Scripts;

public class CharacterAvatarUIController : MonoBehaviour
{
	[SerializeField]
	private Image _headAvatarImage;

	[SerializeField]
	private Image _lowerAvatarImage;

	public void ChangeHeadAvatarSprite(Sprite sprite)
	{
		_headAvatarImage.sprite = sprite;
	}

	public void ChangeBodyAvatarSprite(Sprite sprite)
	{
		_lowerAvatarImage.sprite = sprite;
	}

	public void ChangeColorHeadUI(Color color)
	{
		_headAvatarImage.color = color;
	}

	public void ChangeColorLowerUI(Color color)
	{
		_lowerAvatarImage.color = color;
	}
}
