using UnityEngine;

namespace MoreMountains.Tools;

public class MMRigidbodyCenterOfMass : MonoBehaviour
{
	public enum AutomaticSetModes
	{
		Awake = 0,
		Start = 1,
		ScriptOnly = 2
	}

	[Header("CenterOfMass")]
	public Vector3 CenterOfMassOffset;

	[Header("Automation")]
	public AutomaticSetModes AutomaticSetMode;

	public bool AutoDestroyComponentAfterSet = true;

	[Header("Test")]
	public float GizmoPointSize = 0.05f;

	[MMInspectorButton("SetCenterOfMass")]
	public bool SetCenterOfMassButton;

	protected Vector3 _gizmoCenter;

	protected Rigidbody _rigidbody;

	protected Rigidbody2D _rigidbody2D;

	protected virtual void Awake()
	{
		Initialization();
		if (AutomaticSetMode == AutomaticSetModes.Awake)
		{
			SetCenterOfMass();
		}
	}

	protected virtual void Start()
	{
		if (AutomaticSetMode == AutomaticSetModes.Start)
		{
			SetCenterOfMass();
		}
	}

	protected virtual void Initialization()
	{
		_rigidbody = base.gameObject.MMGetComponentNoAlloc<Rigidbody>();
		_rigidbody2D = base.gameObject.MMGetComponentNoAlloc<Rigidbody2D>();
	}

	public virtual void SetCenterOfMass()
	{
		if (_rigidbody != null)
		{
			_rigidbody.centerOfMass = CenterOfMassOffset;
		}
		if (_rigidbody2D != null)
		{
			_rigidbody2D.centerOfMass = CenterOfMassOffset;
		}
		if (AutoDestroyComponentAfterSet)
		{
			Object.Destroy(this);
		}
	}

	protected virtual void OnDrawGizmosSelected()
	{
		_gizmoCenter = base.transform.TransformPoint(CenterOfMassOffset);
		MMDebug.DrawGizmoPoint(_gizmoCenter, GizmoPointSize, Color.yellow);
	}
}
