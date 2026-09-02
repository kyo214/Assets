using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MoreMountains.Feel;

public class Snake : MonoBehaviour
{
	[Header("Movement")]
	public float Speed = 5f;

	public float NormalSpeedMultiplier = 1f;

	public float SpeedChangeRate = 0.2f;

	public Vector3 Direction = Vector2.right;

	[Header("Boost")]
	public float BoostMultiplier = 2f;

	public float BoostDuration = 2f;

	[Header("BodyParts")]
	public SnakeBodyPart BodyPartPrefab;

	public int BodyPartsOffset = 7;

	public int MaxAmountOfBodyParts = 10;

	public float MinTimeBetweenLostParts = 2f;

	[Header("Bindings")]
	public Text PointsCounter;

	[Header("Feedbacks")]
	public MMFeedbacks TurnFeedback;

	public MMFeedbacks TeleportFeedback;

	public MMFeedbacks TeleportOnceFeedback;

	public MMFeedbacks EatFeedback;

	public MMFeedbacks LoseFeedback;

	[Header("Debug")]
	[MMReadOnly]
	public int SnakePoints;

	[MMReadOnly]
	public float _speed;

	[MMReadOnly]
	public float _speedMultiplier;

	[MMReadOnly]
	public float _lastFoodEatenAt = -100f;

	protected Vector3 _newPosition;

	protected MMPositionRecorder _recorder;

	public List<SnakeBodyPart> _snakeBodyParts;

	protected float _lastLostPart;

	protected void Awake()
	{
		_speed = Speed;
		SnakePoints = 0;
		_recorder = base.gameObject.GetComponent<MMPositionRecorder>();
		PointsCounter.text = "0";
		_snakeBodyParts = new List<SnakeBodyPart>();
	}

	protected virtual void Update()
	{
		HandleInput();
		HandleMovement();
	}

	protected virtual void HandleInput()
	{
		if (FeelDemosInputHelper.CheckMainActionInputPressedThisFrame())
		{
			Turn();
		}
	}

	protected virtual void HandleMovement()
	{
		_speedMultiplier = ((Time.time - _lastFoodEatenAt < BoostDuration) ? BoostMultiplier : NormalSpeedMultiplier);
		_speed = MMMaths.Lerp(_speed, Speed * _speedMultiplier, SpeedChangeRate, Time.deltaTime);
		_newPosition = _speed * Time.deltaTime * Direction;
		base.transform.position += _newPosition;
	}

	public virtual void Turn()
	{
		TurnFeedback?.PlayFeedbacks();
		Direction = MMMaths.RotateVector2(Direction, 90f);
		base.transform.Rotate(new Vector3(0f, 0f, 90f));
	}

	public virtual void Teleport()
	{
		StartCoroutine(TeleportCo());
	}

	protected virtual IEnumerator TeleportCo()
	{
		TeleportFeedback?.PlayFeedbacks();
		TeleportOnceFeedback?.PlayFeedbacks();
		yield return MMCoroutine.WaitForFrames(BodyPartsOffset);
		int total = _snakeBodyParts.Count;
		float part = 1f / (float)total;
		for (int i = 0; i < total; i++)
		{
			yield return MMCoroutine.WaitForFrames(BodyPartsOffset / 2);
			float feedbacksIntensity = 1f - (float)i * part;
			TeleportFeedback?.PlayFeedbacks(base.transform.position, feedbacksIntensity);
		}
	}

	public virtual void Eat()
	{
		EatEffect();
		EatFeedback?.PlayFeedbacks();
		SnakePoints++;
		PointsCounter.text = SnakePoints.ToString();
		StartCoroutine(EatCo());
	}

	protected virtual IEnumerator EatCo()
	{
		int total = _snakeBodyParts.Count;
		float part = 1f / (float)total;
		for (int i = 0; i < total && i < _snakeBodyParts.Count; i++)
		{
			yield return MMCoroutine.WaitForFrames(BodyPartsOffset / 2);
			float intensity = 1f - (float)i * part;
			if (i == total - 1)
			{
				if (i < MaxAmountOfBodyParts - 1 && _snakeBodyParts.Count > i && _snakeBodyParts[i] != null)
				{
					_snakeBodyParts[i].New();
				}
			}
			else
			{
				_snakeBodyParts[i].Eat(intensity);
			}
		}
	}

	public virtual void EatEffect()
	{
		_lastFoodEatenAt = Time.time;
		if (SnakePoints < MaxAmountOfBodyParts - 1)
		{
			SnakeBodyPart snakeBodyPart = Object.Instantiate(BodyPartPrefab);
			SceneManager.MoveGameObjectToScene(snakeBodyPart.gameObject, base.gameObject.scene);
			snakeBodyPart.transform.position = base.transform.position;
			snakeBodyPart.TargetRecorder = _recorder;
			snakeBodyPart.Offset = SnakePoints * BodyPartsOffset + BodyPartsOffset + 1;
			snakeBodyPart.Index = _snakeBodyParts.Count;
			snakeBodyPart.name = "SnakeBodyPart_" + snakeBodyPart.Index;
			_snakeBodyParts.Add(snakeBodyPart);
		}
	}

	public virtual void Lose(SnakeBodyPart part)
	{
		if (!(Time.time - _lastLostPart < MinTimeBetweenLostParts))
		{
			_lastLostPart = Time.time;
			LoseFeedback?.PlayFeedbacks(part.transform.position);
			Object.Destroy(_snakeBodyParts[_snakeBodyParts.Count - 1].gameObject);
			_snakeBodyParts.RemoveAt(_snakeBodyParts.Count - 1);
			SnakePoints--;
			PointsCounter.text = SnakePoints.ToString();
		}
	}
}
