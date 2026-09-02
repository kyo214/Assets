using UnityEngine;

namespace DestroyIt;

public class StructuralSupport : MonoBehaviour
{
	private class StructuralPiece
	{
		public GameObject GameObject { get; set; }

		public Rigidbody Rigidbody { get; set; }

		public Vector3 CenterPoint { get; set; }
	}

	[Tooltip("This is the maximum distance allowed to make a structural support connection. Reduce it if you're getting pieces that float in the air and defy physics. Increase it if too many pieces aren't connecting when they should be.")]
	public float maxConnectionDistance = 1.25f;

	[Tooltip("The force required to break a joint on the structure. Set to -1 for Infinity.")]
	public float breakForce = 1250f;

	[Tooltip("The torque required to break a joint on the structure. Set to -1 for Infinity.")]
	public float breakTorque = 3000f;

	public void FixedUpdate()
	{
		FixedJoint[] componentsInChildren = base.gameObject.GetComponentsInChildren<FixedJoint>();
		bool flag = false;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].connectedBody == null)
			{
				Object.Destroy(componentsInChildren[i]);
				flag = true;
			}
		}
		if (flag)
		{
			Rigidbody[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<Rigidbody>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				componentsInChildren2[j].WakeUp();
			}
		}
	}

	[ExecuteInEditMode]
	public void AddStructuralSupport()
	{
	}

	[ExecuteInEditMode]
	public void RemoveStructuralSupport()
	{
	}
}
