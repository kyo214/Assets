using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feel;

public class Strike : MonoBehaviour
{
	[Header("Input")]
	[Tooltip("a key to use to throw the ball")]
	public KeyCode ActionKey = KeyCode.Space;

	[Tooltip("a secondary key to use to throw the ball")]
	public KeyCode ActionKeyAlt = KeyCode.Joystick1Button0;

	[Header("Bindings")]
	[Tooltip("the rigidbody of the bowling ball")]
	public Rigidbody BowlingBallRb;

	[Tooltip("a collider used to count points (still standing pins will overlap with it)")]
	public Collider PointsCollider;

	[Tooltip("the rigidbody of the pins")]
	public List<Rigidbody> Pins;

	[Tooltip("the wiggler that makes the launcher rotate")]
	public MMWiggle BowlingBallLauncherWiggler;

	[Tooltip("the text component used to display the current last score")]
	public Text LastScoreText;

	[Tooltip("the text component used to display the total score")]
	public Text TotalScoreText;

	[Tooltip("the text component used to display the number of consecutive strikes")]
	public Text ConsecutiveStrikesText;

	[Tooltip("a list of elements to turn on/off in case of strike")]
	public List<GameObject> StrikeElements;

	[Header("Settings")]
	[Tooltip("the force to apply when throwing the ball")]
	public Vector3 ThrowingForce = new Vector3(0f, 0f, 10f);

	[Tooltip("the gravity to apply")]
	public Vector3 Gravity = new Vector3(0f, -9.81f, 0f);

	[Tooltip("the max duration before a reset")]
	public float MaxDurationBeforeReset = 4f;

	[Tooltip("the delay to wait for (in seconds) before resetting the scene")]
	public float DelayBeforeReset = 1f;

	[Tooltip("the delay to wait for (in seconds) while counting/displaying points")]
	public float DelayForPoints = 1f;

	[Header("Feedbacks")]
	[Tooltip("a feedback to call when throwing the ball")]
	public MMFeedbacks ThrowBallFeedback;

	[Tooltip("a feedback to call when resetting the scene")]
	public MMFeedbacks ResetFeedback;

	[Tooltip("a feedback played when hitting a strike")]
	public MMFeedbacks StrikeFeedback;

	[Tooltip("a feedback played when missing a strike")]
	public MMFeedbacks NoStrikeFeedback;

	[Header("Scores")]
	[Tooltip("the last score you hit")]
	[MMReadOnly]
	public int LastScore;

	[Tooltip("The total amount of points since the start")]
	[MMReadOnly]
	public int TotalPoints;

	[Tooltip("the amount of consecutive strikes")]
	[MMReadOnly]
	public int ConsecutiveStrikes;

	protected bool _ballThrown;

	protected Vector3 _initialBallPosition;

	protected Quaternion _initialBallRotation;

	protected List<StrikePin> _strikePins;

	protected List<Collider> _pinColliders;

	protected Coroutine _resetCoroutine;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		Physics.gravity = Gravity;
		Physics.bounceThreshold = 2f;
		Physics.sleepThreshold = 0.005f;
		Physics.defaultContactOffset = 0.01f;
		Physics.defaultSolverIterations = 6;
		Physics.defaultSolverVelocityIterations = 1;
		Physics.queriesHitTriggers = true;
		ConsecutiveStrikes = 0;
		LastScore = 0;
		TotalPoints = 0;
		ConsecutiveStrikesText.text = "0";
		LastScoreText.text = "0";
		TotalScoreText.text = "0";
		SetStrikeElements(status: false);
		_initialBallPosition = BowlingBallRb.transform.position;
		_initialBallRotation = BowlingBallRb.transform.localRotation;
		_strikePins = new List<StrikePin>();
		_pinColliders = new List<Collider>();
		foreach (Rigidbody pin in Pins)
		{
			StrikePin item = new StrikePin
			{
				Rb = pin,
				InitialPosition = pin.transform.position,
				InitialRotation = pin.transform.rotation
			};
			_strikePins.Add(item);
			_pinColliders.Add(item.Rb.gameObject.GetComponent<Collider>());
		}
	}

	protected virtual void SetStrikeElements(bool status)
	{
		foreach (GameObject strikeElement in StrikeElements)
		{
			strikeElement.SetActive(status);
		}
	}

	protected virtual void Update()
	{
		HandleInput();
	}

	protected virtual void HandleInput()
	{
		if (FeelDemosInputHelper.CheckMainActionInputPressedThisFrame())
		{
			StartBall();
		}
	}

	protected virtual void StartBall()
	{
		if (!_ballThrown)
		{
			ThrowBallFeedback?.PlayFeedbacks();
			BowlingBallLauncherWiggler.RotationActive = false;
			_ballThrown = true;
		}
	}

	public virtual void ThrowBall()
	{
		if (BowlingBallRb != null)
		{
			BowlingBallRb.AddRelativeForce(ThrowingForce, ForceMode.Impulse);
			BowlingBallRb.AddTorque(ThrowingForce, ForceMode.Impulse);
			_resetCoroutine = StartCoroutine(ResetCountdown());
		}
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.MMGetComponentNoAlloc<StrikeBall>() != null)
		{
			StartCoroutine(ResetSceneCo());
			if (_resetCoroutine != null)
			{
				StopCoroutine(_resetCoroutine);
			}
		}
	}

	protected virtual IEnumerator ResetCountdown()
	{
		yield return MMCoroutine.WaitFor(MaxDurationBeforeReset);
		StartCoroutine(ResetSceneCo());
	}

	protected virtual IEnumerator ResetSceneCo()
	{
		yield return MMCoroutine.WaitFor(DelayBeforeReset);
		CountPoints();
		yield return MMCoroutine.WaitFor(DelayForPoints);
		ResetFeedback?.PlayFeedbacks();
		yield return MMCoroutine.WaitFor(0.1f);
		BowlingBallRb.MovePosition(_initialBallPosition);
		BowlingBallRb.transform.localRotation = _initialBallRotation;
		BowlingBallRb.velocity = Vector3.zero;
		BowlingBallRb.angularVelocity = Vector3.zero;
		yield return MMCoroutine.WaitForFrames(1);
		BowlingBallRb.transform.position = _initialBallPosition;
		BowlingBallLauncherWiggler.RotationActive = true;
		foreach (StrikePin strikePin in _strikePins)
		{
			strikePin.ResetPin();
		}
		_ballThrown = false;
	}

	protected virtual void CountPoints()
	{
		int num = 10;
		foreach (Collider pinCollider in _pinColliders)
		{
			if (pinCollider.bounds.Intersects(PointsCollider.bounds))
			{
				num--;
			}
		}
		LastScore = num;
		ConsecutiveStrikes = ((num == 10) ? (ConsecutiveStrikes + 1) : 0);
		if (num == 10)
		{
			StrikeFeedback?.PlayFeedbacks();
			SetStrikeElements(status: true);
		}
		else
		{
			NoStrikeFeedback?.PlayFeedbacks();
			SetStrikeElements(status: false);
		}
		TotalPoints += num;
		ConsecutiveStrikesText.text = ConsecutiveStrikes.ToString();
		LastScoreText.text = LastScore.ToString();
		TotalScoreText.text = TotalPoints.ToString();
	}
}
