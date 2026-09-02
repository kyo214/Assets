using UnityEngine;

namespace DestroyIt;

public class OpenCloseChest : MonoBehaviour
{
	private void Start()
	{
		InvokeRepeating("SwapOpenClose", 3.5f, 3.5f);
	}

	public void SwapOpenClose()
	{
		HingeJoint component = GetComponent<HingeJoint>();
		if (component != null)
		{
			component.motor = new JointMotor
			{
				targetVelocity = -1f * component.motor.targetVelocity,
				force = 10f
			};
			component.useMotor = true;
			GetComponent<Rigidbody>().WakeUp();
		}
		else
		{
			Object.Destroy(this);
		}
	}
}
