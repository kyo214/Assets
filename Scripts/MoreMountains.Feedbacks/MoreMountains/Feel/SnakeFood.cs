using System.Collections;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feel;

public class SnakeFood : MonoBehaviour
{
	public float OffDelay = 1f;

	public GameObject Model;

	public MMFeedbacks EatFeedback;

	public MMFeedbacks AppearFeedback;

	protected Snake _snake;

	public SnakeFoodSpawner Spawner { get; set; }

	protected void OnTriggerEnter2D(Collider2D other)
	{
		_snake = other.GetComponent<Snake>();
		if (_snake != null)
		{
			_snake.Eat();
			EatFeedback?.PlayFeedbacks();
			StartCoroutine(MoveFood());
		}
	}

	protected virtual IEnumerator MoveFood()
	{
		Model.SetActive(value: false);
		yield return MMCoroutine.WaitFor(OffDelay);
		Model.SetActive(value: true);
		base.transform.position = Spawner.DetermineSpawnPosition();
		AppearFeedback?.PlayFeedbacks();
	}
}
