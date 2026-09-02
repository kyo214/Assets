using UnityEngine;

namespace DestroyIt;

public class ChipAwayDebris : MonoBehaviour
{
	public float debrisMass = 1f;

	public float debrisDrag;

	public float debrisAngularDrag = 0.05f;

	private Renderer _rend;

	public void BreakOff(Vector3 force, Vector3 point)
	{
		if (CheckCanBreakOff())
		{
			Rigidbody component = _rend.gameObject.GetComponent<Rigidbody>();
			Rigidbody obj = ((component == null) ? _rend.gameObject.AddComponent<Rigidbody>() : component);
			obj.mass = debrisMass;
			obj.drag = debrisDrag;
			obj.angularDrag = debrisAngularDrag;
			obj.AddForceAtPosition(force, point, ForceMode.Impulse);
			obj.gameObject.transform.SetParent(null);
			Object.Destroy(this);
		}
	}

	public void BreakOff(float blastForce, float explosionRadius, float upwardsModifier)
	{
		if (CheckCanBreakOff())
		{
			Rigidbody rigidbody = _rend.gameObject.AddComponent<Rigidbody>();
			rigidbody.mass = debrisMass;
			rigidbody.drag = debrisDrag;
			rigidbody.angularDrag = debrisAngularDrag;
			rigidbody.AddExplosionForce(blastForce, base.transform.position, explosionRadius, upwardsModifier);
			Object.Destroy(this);
		}
	}

	private bool CheckCanBreakOff()
	{
		if (GetComponent<Collider>() == null)
		{
			Object.Destroy(this);
			return false;
		}
		_rend = base.gameObject.GetComponentInParent<Renderer>();
		if (_rend == null)
		{
			Object.Destroy(this);
			return false;
		}
		if (GetComponent<Collider>().attachedRigidbody != null)
		{
			Object.Destroy(this);
			return false;
		}
		return true;
	}
}
