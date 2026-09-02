using System.Collections.Generic;
using Toked;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
	public float hp = 130f;

	public float initHp;

	public bool isCannotBroken;

	[SerializeField]
	private List<GameObject> doorList = new List<GameObject>();

	private Collider objCollider;

	private Animator objAnimator;

	public ItemInteractable interactObj;

	[SerializeField]
	private List<Collider> listDoorCollider = new List<Collider>();

	[SerializeField]
	private List<Rigidbody> listDoorRigidbody = new List<Rigidbody>();

	private void Start()
	{
		initHp = hp;
		objCollider = GetComponent<Collider>();
		objAnimator = GetComponent<Animator>();
		for (int i = 0; i < base.gameObject.transform.childCount; i++)
		{
			if (base.gameObject.transform.GetChild(i).gameObject.name != "DoorMap" && base.gameObject.transform.GetChild(i).gameObject.name != "DoorInteraction")
			{
				doorList.Add(base.gameObject.transform.GetChild(i).gameObject);
				listDoorCollider.Add(doorList[doorList.Count - 1].GetComponent<Collider>());
				listDoorRigidbody.Add(doorList[doorList.Count - 1].GetComponent<Rigidbody>());
			}
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (!collision.gameObject.CompareTag("Enemy") || !objCollider.enabled || !(hp > 0f) || !(interactObj != null) || isCannotBroken)
		{
			return;
		}
		EnemyController component = collision.gameObject.GetComponent<EnemyController>();
		if (!component.network.networkPhoton.isChasing || interactObj.isLocked || component.timerAttackDoor.isRunning)
		{
			return;
		}
		component.timerAttackDoor.StartDuration(Random.Range(1, 2));
		if (NetworkGameManager.Instance.isServer)
		{
			hp -= 10f;
			if (!component.isAlwaysChasing)
			{
				component.attack.StopChasing();
			}
			if (hp <= 0f)
			{
				hp = initHp;
				component.network.ExecDoorBroken((byte)interactObj.UniqueID, collision.gameObject.transform.position, 1);
			}
		}
		if (hp > 0f && hp != initHp && !interactObj.triggerOnReverse && (interactObj.animatorTrigger1.GetCurrentAnimatorStateInfo(0).normalizedTime < 0f || (interactObj.animatorTrigger1.GetCurrentAnimatorStateInfo(0).IsName("Attacked") && interactObj.animatorTrigger1.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)))
		{
			component.network.ExecDoorAttacked((byte)interactObj.UniqueID);
		}
	}

	public void ExecuteDoorAttacked()
	{
		objAnimator.Play("Attacked", -1, 0f);
		AudioManager.PlaySFXTransform("door-knockedHard", base.transform, isLocalPlayerTrigger: false);
	}

	public void ExecuteDoorBroken(Vector3 sourcePos, int type)
	{
		if (type == 1)
		{
			AudioManager.PlaySFXTransform("door-slammed", base.transform, isLocalPlayerTrigger: false);
			interactObj.TriggerAnimation(isUsedByLocalPlayer: false, null, playSFX: false, 1.7f, noTriggerReverse: true);
			return;
		}
		objAnimator.enabled = false;
		objCollider.enabled = false;
		for (int i = 0; i < doorList.Count; i++)
		{
			interactObj.DisableCollider();
			if (interactObj.syncObject != null)
			{
				interactObj.syncObject.DisableCollider();
			}
			doorList[i].layer = 10;
			listDoorCollider[i].enabled = true;
			listDoorRigidbody[i].isKinematic = false;
			listDoorRigidbody[i].AddForce((base.transform.position - sourcePos).normalized * 8f, ForceMode.Impulse);
		}
	}
}
