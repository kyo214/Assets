using UnityEngine;

namespace _Modules.UITitle.Scripts;

public class StartContinueSpriteSwapper : MonoBehaviour
{
	[SerializeField]
	private ButtonSpriteSwapper _startSpriteSwapper;

	[SerializeField]
	private ButtonSpriteSwapper _continueSpriteSwapper;

	public void Init(int index, GameData gameData)
	{
		int index2 = index + 1;
		_startSpriteSwapper.Init(index2, gameData);
		_continueSpriteSwapper.Init(index2, gameData);
	}

	public void ActiveStartSpriteImage()
	{
		DisableStartSpriteImage();
		_startSpriteSwapper.SetActiveImage(value: true);
	}

	public void ActiveContinueSpriteImage()
	{
		DisableStartSpriteImage();
		_continueSpriteSwapper.SetActiveImage(value: true);
	}

	private void DisableStartSpriteImage()
	{
		_startSpriteSwapper.SetActiveImage(value: false);
		_continueSpriteSwapper.SetActiveImage(value: false);
	}
}
