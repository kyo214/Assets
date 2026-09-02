using UnityEngine;

public class CharControllerPhoton : MonoBehaviour
{
	public PlayerController player;

	public Collider Collider;

	public CharacterController charControl;

	public float gravity = -9.81f;

	private bool isGrounded;

	public Transform groundCheck;

	public float groundDistance = 0.4f;

	public LayerMask groundMask;

	private Vector3 velocity;

	public bool DisableMoveTemporary;

	public int CtrDelayMove;

	public void SetKinematicVelocity(Vector3 newVelocity, float deltaTime)
	{
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
		if (isGrounded && newVelocity.y < 0f)
		{
			newVelocity.y = -2f;
		}
		if (player.network.isLocalPlayer)
		{
			newVelocity.y += gravity;
		}
		if (charControl.enabled)
		{
			charControl.Move(newVelocity * deltaTime);
		}
	}

	public void SetLayerMask(LayerMask newLayerMask)
	{
		Collider.includeLayers = newLayerMask;
	}

	public void ExludeLayerCharCollider(LayerMask newLayerMask)
	{
		charControl.excludeLayers = newLayerMask;
	}

	public void SetPosition(Vector3 transformPosition)
	{
		base.transform.position = transformPosition;
		base.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
		DisableMoveTemporary = true;
		CtrDelayMove = 10;
	}
}
