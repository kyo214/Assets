using UnityEngine;

namespace DestroyIt;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FirstPersonController : MonoBehaviour
{
	[SerializeField]
	private bool m_IsWalking;

	[SerializeField]
	private float m_WalkSpeed;

	[SerializeField]
	private float m_RunSpeed;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_RunstepLenghten;

	[SerializeField]
	private float m_JumpSpeed;

	[SerializeField]
	private float m_StickToGroundForce;

	[SerializeField]
	private float m_GravityMultiplier;

	[SerializeField]
	private MouseLook m_MouseLook = new MouseLook();

	[SerializeField]
	private float m_StepInterval;

	private Camera m_Camera;

	private bool m_Jump;

	private float m_YRotation;

	private Vector2 m_Input;

	private Vector3 m_MoveDir = Vector3.zero;

	private CharacterController m_CharacterController;

	private CollisionFlags m_CollisionFlags;

	private bool m_PreviouslyGrounded;

	private float m_StepCycle;

	private float m_NextStep;

	private bool m_Jumping;

	private void Start()
	{
		m_CharacterController = GetComponent<CharacterController>();
		m_Camera = Camera.main;
		m_StepCycle = 0f;
		m_NextStep = m_StepCycle / 2f;
		m_Jumping = false;
		m_MouseLook.Init(base.transform, m_Camera.transform);
	}

	private void Update()
	{
		RotateView();
		if (!m_Jump)
		{
			m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
		}
		if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
		{
			m_MoveDir.y = 0f;
			m_Jumping = false;
		}
		if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
		{
			m_MoveDir.y = 0f;
		}
		m_PreviouslyGrounded = m_CharacterController.isGrounded;
	}

	private void FixedUpdate()
	{
		GetInput(out var speed);
		Vector3 vector = base.transform.forward * m_Input.y + base.transform.right * m_Input.x;
		Physics.SphereCast(base.transform.position, m_CharacterController.radius, Vector3.down, out var hitInfo, m_CharacterController.height / 2f, -1, QueryTriggerInteraction.Ignore);
		vector = Vector3.ProjectOnPlane(vector, hitInfo.normal).normalized;
		m_MoveDir.x = vector.x * speed;
		m_MoveDir.z = vector.z * speed;
		if (m_CharacterController.isGrounded)
		{
			m_MoveDir.y = 0f - m_StickToGroundForce;
			if (m_Jump)
			{
				m_MoveDir.y = m_JumpSpeed;
				m_Jump = false;
				m_Jumping = true;
			}
		}
		else
		{
			m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
		}
		m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
		ProgressStepCycle(speed);
		m_MouseLook.UpdateCursorLock();
	}

	private void ProgressStepCycle(float speed)
	{
		if (m_CharacterController.velocity.sqrMagnitude > 0f && (m_Input.x != 0f || m_Input.y != 0f))
		{
			m_StepCycle += (m_CharacterController.velocity.magnitude + speed * (m_IsWalking ? 1f : m_RunstepLenghten)) * Time.fixedDeltaTime;
		}
		if (m_StepCycle > m_NextStep)
		{
			m_NextStep = m_StepCycle + m_StepInterval;
		}
	}

	private void GetInput(out float speed)
	{
		float axis = CrossPlatformInputManager.GetAxis("Horizontal");
		float axis2 = CrossPlatformInputManager.GetAxis("Vertical");
		m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
		speed = (m_IsWalking ? m_WalkSpeed : m_RunSpeed);
		m_Input = new Vector2(axis, axis2);
		if (m_Input.sqrMagnitude > 1f)
		{
			m_Input.Normalize();
		}
	}

	private void RotateView()
	{
		m_MouseLook.LookRotation(base.transform, m_Camera.transform);
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
		if (m_CollisionFlags != CollisionFlags.Below && !(attachedRigidbody == null) && !attachedRigidbody.isKinematic)
		{
			attachedRigidbody.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
		}
	}
}
