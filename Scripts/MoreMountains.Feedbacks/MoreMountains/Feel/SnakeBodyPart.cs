using System.Collections;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feel;

public class SnakeBodyPart : MonoBehaviour
{
	public MMPositionRecorder TargetRecorder;

	public MMFeedbacks EatFeedback;

	public MMFeedbacks NewFeedback;

	public int Offset = 20;

	public int Index;

	protected Snake _snake;

	protected BoxCollider2D _collider2D;

	protected virtual void Awake()
	{
		_collider2D = base.gameObject.MMGetComponentNoAlloc<BoxCollider2D>();
		StartCoroutine(ActivateCollider());
	}

	protected virtual IEnumerator ActivateCollider()
	{
		yield return MMCoroutine.WaitFor(1f);
		_collider2D.enabled = true;
	}

	protected void Update()
	{
		base.transform.position = TargetRecorder.Positions[Offset];
	}

	public virtual void Eat(float intensity)
	{
		EatFeedback?.PlayFeedbacks(base.transform.position, intensity);
	}

	public virtual void New()
	{
		NewFeedback?.Initialization();
		NewFeedback?.PlayFeedbacks();
	}

	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (Index != 0)
		{
			_snake = other.GetComponent<Snake>();
			if (_snake != null)
			{
				_snake.Lose(this);
			}
		}
	}
}
