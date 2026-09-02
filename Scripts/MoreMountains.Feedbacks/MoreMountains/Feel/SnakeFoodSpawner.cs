using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Feel;

public class SnakeFoodSpawner : MonoBehaviour
{
	public SnakeFood SnakeFoodPrefab;

	public int AmountOfFood = 3;

	public Vector2 MinRandom = new Vector2(0.1f, 0.1f);

	public Vector2 MaxRandom = new Vector2(0.9f, 0.9f);

	protected List<SnakeFood> Foods;

	protected Camera _mainCamera;

	protected virtual void Start()
	{
		_mainCamera = Camera.main;
		Foods = new List<SnakeFood>();
		for (int i = 0; i < AmountOfFood; i++)
		{
			SnakeFood snakeFood = Object.Instantiate(SnakeFoodPrefab);
			SceneManager.MoveGameObjectToScene(snakeFood.gameObject, base.gameObject.scene);
			snakeFood.transform.position = DetermineSpawnPosition();
			snakeFood.Spawner = this;
			Foods.Add(snakeFood);
		}
	}

	public virtual Vector3 DetermineSpawnPosition()
	{
		Vector3 position = MMMaths.RandomVector2(MinRandom, MaxRandom);
		position.z = 10f;
		return _mainCamera.ViewportToWorldPoint(position);
	}
}
