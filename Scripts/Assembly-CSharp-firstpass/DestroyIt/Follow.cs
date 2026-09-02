using UnityEngine;

namespace DestroyIt;

public class Follow : MonoBehaviour
{
	public Transform objectToFollow;

	public FacingDirection facingDirection = FacingDirection.FollowedObject;

	[HideInInspector]
	public bool isPositionFixed;

	[HideInInspector]
	public Vector3 fixedFromPosition = Vector3.zero;

	[HideInInspector]
	public float fixedDistance;

	private void Start()
	{
		if (objectToFollow == null)
		{
			Debug.Log("[DestroyIt-Follow]: No transform was provided. Nothing to follow. Removing script...");
			Object.Destroy(this);
		}
	}

	private void LateUpdate()
	{
		if (objectToFollow != null)
		{
			if (isPositionFixed)
			{
				Vector3 position = objectToFollow.position.LerpByDistance(fixedFromPosition, fixedDistance);
				base.transform.position = position;
			}
			else
			{
				base.transform.position = objectToFollow.position;
			}
			switch (facingDirection)
			{
			case FacingDirection.FollowedObject:
				base.transform.LookAt(objectToFollow);
				break;
			case FacingDirection.FixedPosition:
				base.transform.LookAt(fixedFromPosition);
				break;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (isPositionFixed && objectToFollow != null)
		{
			Gizmos.DrawLine(fixedFromPosition, objectToFollow.position);
		}
		Gizmos.DrawWireSphere(base.transform.position, 0.5f);
	}
}
