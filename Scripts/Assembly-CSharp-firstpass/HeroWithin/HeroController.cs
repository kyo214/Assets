using UnityEngine;
using UnityEngine.InputSystem;

namespace HeroWithin;

public class HeroController : MonoBehaviour
{
	private Rigidbody rb;

	[SerializeField]
	private Transform vCam;

	private Vector3 direction;

	private float speedRotation = 0.1f;

	private float shaderDirectionX;

	private float shaderDirectionZ;

	[SerializeField]
	private Transform body25D;

	[SerializeField]
	private Transform body;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Transform gauntlet;

	[SerializeField]
	private Transform shadow;

	private Vector2 move;

	public float moveSpeed = 5f;

	[SerializeField]
	private LayerMask layerMask;

	private string currentState;

	private const string HERO_IDLE_NORMAL = "hero_idleNormal";

	private const string HERO_IDLE_CRITICAL = "hero_idleCritical";

	private const string HERO_RESPAWN = "hero_respawn";

	private const string HERO_WALK = "hero_walk";

	private const string HERO_RUN = "hero_run";

	private const string HERO_DASH = "hero_dash";

	private const string HERO_GRAPPLE_START = "hero_grappleStart";

	private const string HERO_GRAPPLE_LOOP = "hero_grappleLoop";

	private const string HERO_GRAPPLE_END = "hero_grappleEnd";

	private const string HERO_ATTACK = "hero_attack";

	private const string HERO_HIT = "hero_hit";

	private const string HERO_DEAD = "hero_dead";

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Start()
	{
		direction = vCam.transform.eulerAngles;
		Renderer component = GameObject.FindGameObjectsWithTag("Props")[0].GetComponent<Renderer>();
		component.material.shader = Shader.Find("Shader Graphs/Advance 3D Skew");
		Vector3 vector = component.material.GetVector("_Direction");
		shaderDirectionX = vector.x;
		shaderDirectionZ = vector.z;
	}

	private void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Space))
		{
			return;
		}
		direction = new Vector3(vCam.transform.localEulerAngles.x, (direction.y + 90f) % 360f, vCam.transform.localEulerAngles.z);
		float y = direction.y;
		if (y != 45f)
		{
			if (y != 135f)
			{
				if (y == 315f)
				{
					shaderDirectionX = 1f;
					shaderDirectionZ = 1f;
				}
				else
				{
					shaderDirectionX = 1f;
					shaderDirectionZ = -1f;
				}
			}
			else
			{
				shaderDirectionX = -1f;
				shaderDirectionZ = -1f;
			}
		}
		else
		{
			shaderDirectionX = -1f;
			shaderDirectionZ = 1f;
		}
	}

	private void FixedUpdate()
	{
		HandleRotation();
		HandleMovement();
		HandleAiming();
	}

	private void HandleRotation()
	{
		if (vCam.rotation.y != Quaternion.Euler(direction).y)
		{
			if (Mathf.Ceil(vCam.transform.localEulerAngles.y * 1000f) != Mathf.Ceil(direction.y * 1000f))
			{
				vCam.rotation = Quaternion.Lerp(vCam.rotation, Quaternion.Euler(direction), speedRotation);
			}
			else
			{
				vCam.rotation = Quaternion.Euler(direction);
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag("Props");
			for (int i = 0; i < array.Length; i++)
			{
				Renderer component = array[i].GetComponent<Renderer>();
				Vector3 vector = component.material.GetVector("_Direction");
				component.material.SetVector("_Direction", new Vector4(Mathf.Lerp(vector.x, shaderDirectionX, speedRotation), 0f, Mathf.Lerp(vector.z, shaderDirectionZ, speedRotation)));
			}
			body25D.transform.localRotation = Quaternion.Euler(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, 0f);
			shadow.localRotation = Quaternion.Euler(shadow.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, 0f);
		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		move = context.ReadValue<Vector2>();
		move = Vector2.ClampMagnitude(move, 1f);
	}

	public void HandleMovement()
	{
		if (move != Vector2.zero)
		{
			Vector3 vector = new Vector3(move.x, 0f, move.y);
			base.transform.Translate(Quaternion.Euler(0f, Camera.main.transform.localEulerAngles.y, 0f) * vector * moveSpeed * Time.deltaTime, Space.World);
			ChangeAnimationState("hero_walk");
		}
		else
		{
			ChangeAnimationState("hero_idleNormal");
		}
	}

	private void HandleAiming()
	{
		Vector3 pos = Mouse.current.position.ReadValue();
		Vector3 vector = default;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(pos), out var hitInfo, float.PositiveInfinity, layerMask))
		{
			vector = hitInfo.point;
		}
		Vector3 vector2 = Quaternion.Euler(0f, 0f - Camera.main.transform.localEulerAngles.y, 0f) * (vector - gauntlet.position).normalized;
		float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
		gauntlet.eulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, num);
		Vector3 localScale = gauntlet.localScale;
		Vector3 localScale2 = body.localScale;
		if (num > 90f || num < -90f)
		{
			localScale.y = 0f - Mathf.Abs(localScale.y);
			localScale2.x = 0f - Mathf.Abs(localScale2.x);
		}
		else
		{
			localScale.y = Mathf.Abs(localScale.y);
			localScale2.x = Mathf.Abs(localScale2.x);
		}
		gauntlet.localScale = localScale;
		body.localScale = localScale2;
	}

	private void ChangeAnimationState(string newState)
	{
		if (!(currentState == newState))
		{
			animator.Play(newState);
			currentState = newState;
		}
	}
}
