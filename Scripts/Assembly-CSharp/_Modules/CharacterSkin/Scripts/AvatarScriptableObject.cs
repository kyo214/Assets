using UnityEngine;

namespace _Modules.CharacterSkin.Scripts;

[CreateAssetMenu(fileName = "AvatarScriptableObject", menuName = "WMO/ScriptableObjects/Skin/AvatarScriptableObject", order = 0)]
public class AvatarScriptableObject : ScriptableObject
{
	[SerializeField]
	private Sprite _avatarSprite;

	[SerializeField]
	private Sprite _miniAvatarSprite;

	public Sprite AvatarSprite
	{
		get
		{
			return _avatarSprite;
		}
		set
		{
			_avatarSprite = value;
		}
	}

	public Sprite MiniAvatarSprite
	{
		get
		{
			return _miniAvatarSprite;
		}
		set
		{
			_miniAvatarSprite = value;
		}
	}
}
