using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
	public struct ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
	{
		public bool hit = _hit;

		public Vector3 point = _point;

		public float distance = _distance;

		public float angle = _angle;
	}

	public bool isDisable;

	public bool isPlayer;

	public float viewRadius;

	[Range(0f, 360f)]
	public float viewAngle;

	public LayerMask targetMask;

	public LayerMask obstacleMask;

	public LayerMask obstacleMaskDeaf;

	public List<Transform> visibleTargets = new List<Transform>();

	public List<Transform> prevVisibleTargets = new List<Transform>();

	[SerializeField]
	private float distToTarget;

	[SerializeField]
	private EnemyController enemyController;

	private Mesh viewMesh;

	private float interval;

	public float maxInterval = 0.5f;

	private void Start()
	{
		viewMesh = new Mesh();
		viewMesh.name = "View Mesh";
		enemyController = base.transform.parent.GetComponent<EnemyController>();
	}

	public void SetDisable(bool value)
	{
		isDisable = value;
		visibleTargets.Clear();
	}

	private void FixedUpdate()
	{
		if (!isDisable)
		{
			interval -= Time.deltaTime;
			if (interval < 0f)
			{
				FindVisibleTargets();
				interval = maxInterval;
			}
		}
	}

	public void FindVisibleTargets()
	{
		visibleTargets.Clear();
		Collider[] array = Physics.OverlapSphere(base.transform.position, viewRadius, targetMask);
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] != null))
			{
				continue;
			}
			Transform transform = array[i].transform;
			Vector3 vector = new Vector3(base.transform.position.x, transform.position.y, base.transform.position.z);
			Vector3 normalized = (transform.position - vector).normalized;
			if (!(Vector3.Angle(base.transform.forward, normalized) < viewAngle / 2f))
			{
				continue;
			}
			distToTarget = Vector3.Distance(vector, transform.position);
			LayerMask layerMask = obstacleMask;
			if (!isPlayer && enemyController != null && enemyController.network.networkPhoton.isDeaf)
			{
				layerMask = obstacleMaskDeaf;
			}
			if (Physics.Raycast(vector, normalized, distToTarget, layerMask))
			{
				continue;
			}
			EnemyController component = null;
			if (transform.parent != null)
			{
				transform.parent.TryGetComponent<EnemyController>(out component);
			}
			if (1 == 0)
			{
				continue;
			}
			visibleTargets.Add(transform);
			int num = -1;
			for (int j = 0; j < prevVisibleTargets.Count; j++)
			{
				if (transform == prevVisibleTargets[j])
				{
					num = j;
				}
			}
			if (GameModes.Instance.modeGame == "PVP")
			{
				PlayerController component2 = null;
				transform.parent.TryGetComponent<PlayerController>(out component2);
				if (component2 != null)
				{
					component2.characterRenderController.ShowCharacter();
				}
			}
			if ((transform.gameObject.layer == 12 && num == -1) || (component != null && component.allSpriteParts[0].color.a == 0f))
			{
				if (component != null)
				{
					component.VisibleSprite();
				}
			}
			else if (transform.gameObject.layer == 30 && num == -1)
			{
				SpriteOutlineCollider component4;
				Outline component5;
				if (transform.TryGetComponent<ItemInteractable>(out var component3))
				{
					Outline outline = component3.outline;
					if (outline != null && outline.highlight != null)
					{
						outline.highlight.highlighted = true;
					}
					if (component3.IconMap != null && component3.IconMap.sprite != null && !component3.IconMap.gameObject.activeSelf)
					{
						component3.IconMap.gameObject.SetActive(value: true);
					}
				}
				else if (transform.TryGetComponent<SpriteOutlineCollider>(out component4))
				{
					component4.SetOutline(value: true);
				}
				else if (transform.TryGetComponent<Outline>(out component5) && component5.highlight != null)
				{
					component5.highlight.highlighted = true;
				}
			}
			else if (num != -1)
			{
				prevVisibleTargets.Remove(transform);
			}
		}
		if (!isPlayer)
		{
			return;
		}
		for (int k = 0; k < prevVisibleTargets.Count; k++)
		{
			if (!(prevVisibleTargets[k] != null))
			{
				continue;
			}
			if (GameModes.Instance.modeGame == "PVP")
			{
				prevVisibleTargets[k].parent.TryGetComponent<PlayerController>(out var component6);
				if (component6 != null)
				{
					bool flag = false;
					foreach (string roomCollider in component6.roomColliders)
					{
						if (roomCollider == NetworkGameManager.Instance.ownPlayer.RoomName)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						component6.characterRenderController.HideCharacter();
					}
				}
			}
			if (prevVisibleTargets[k].gameObject.layer == 12)
			{
				prevVisibleTargets[k].parent.TryGetComponent<EnemyController>(out var component7);
				if (!(component7 != null))
				{
					continue;
				}
				bool flag2 = false;
				foreach (string roomCollider2 in component7.roomColliders)
				{
					if (roomCollider2 == NetworkGameManager.Instance.ownPlayer.RoomName)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2 && component7.network.GetHealth() > 0f && !component7.network.IsNonActive() && !component7.isDeadAnimationPlaying)
				{
					component7.HideSprite();
				}
			}
			else
			{
				if (prevVisibleTargets[k].gameObject.layer != 30)
				{
					continue;
				}
				SpriteOutlineCollider component9;
				Outline component10;
				if (prevVisibleTargets[k].TryGetComponent<ItemInteractable>(out var component8))
				{
					Outline outline2 = component8.outline;
					if (outline2 != null && outline2.highlight != null)
					{
						outline2.highlight.highlighted = false;
					}
				}
				else if (prevVisibleTargets[k].TryGetComponent<SpriteOutlineCollider>(out component9))
				{
					component9.SetOutline(value: false);
				}
				else if (prevVisibleTargets[k].TryGetComponent<Outline>(out component10) && component10.highlight != null)
				{
					component10.highlight.highlighted = false;
				}
			}
		}
		prevVisibleTargets.Clear();
		foreach (Transform visibleTarget in visibleTargets)
		{
			prevVisibleTargets.Add(visibleTarget);
		}
	}

	public Transform NearestTarget()
	{
		return GetNearestTarget(base.transform, visibleTargets);
	}

	public static Transform GetNearestTarget(Transform origin, List<Transform> targets)
	{
		Transform result = null;
		float num = float.MaxValue;
		Vector3 position = origin.position;
		for (int i = 0; i < targets.Count; i++)
		{
			Transform transform = targets[i];
			if (!(transform == null))
			{
				float sqrMagnitude = (transform.position - position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = transform;
				}
			}
		}
		return result;
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += base.transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * (MathF.PI / 180f)), 0f, Mathf.Cos(angleInDegrees * (MathF.PI / 180f)));
	}
}
