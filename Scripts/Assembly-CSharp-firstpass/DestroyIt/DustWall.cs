using UnityEngine;

namespace DestroyIt;

public class DustWall : MonoBehaviour
{
	public GameObject playerDustPrefab;

	public float dustDurationSeconds = 10f;

	public float dustStartDistance = 50f;

	public Vector3 fixedFromPosition;

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Player" && playerDustPrefab != null)
		{
			Transform transform = collider.gameObject.transform;
			GameObject obj = Object.Instantiate(playerDustPrefab, transform.position, Quaternion.identity);
			Follow follow = obj.AddComponent<Follow>();
			follow.isPositionFixed = true;
			follow.objectToFollow = transform;
			follow.facingDirection = FacingDirection.FollowedObject;
			follow.fixedFromPosition = fixedFromPosition;
			follow.fixedDistance = dustStartDistance;
			obj.AddComponent<FadeParticleEffect>().delaySeconds = dustDurationSeconds - 2f;
		}
	}
}
