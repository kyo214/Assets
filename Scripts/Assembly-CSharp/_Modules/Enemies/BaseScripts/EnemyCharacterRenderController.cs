using UnityEngine;
using _Modules.Player.BaseScripts;

namespace _Modules.Enemies.BaseScripts;

public class EnemyCharacterRenderController : CharacterRenderController
{
	[SerializeField]
	private EnemyController _enemyController;

	public override void ShowCharacter()
	{
		base.ShowCharacter();
		_enemyController.enemyCollider.gameObject.SetActive(value: true);
	}

	public override void HideCharacter()
	{
		base.HideCharacter();
		_enemyController.enemyCollider.gameObject.SetActive(value: false);
	}
}
