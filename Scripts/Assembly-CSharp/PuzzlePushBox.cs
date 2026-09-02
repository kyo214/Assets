using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PuzzlePushBox : MonoBehaviour
{
	public enum EnumAxis
	{
		X = 0,
		Z = 1
	}

	[Header("Intertnal Reference")]
	[SerializeField]
	private TriggerPushBox[] _triggers;

	[Header("Data Reference")]
	[SerializeField]
	private Vector3 _beginPos;

	[SerializeField]
	private Vector3 _endPos;

	[SerializeField]
	private float _pushForce;

	[SerializeField]
	private EnumAxis _pushAlongAxis;

	private Rigidbody _rb;

	public bool _isMoving;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		for (int i = 0; i < _triggers.Length; i++)
		{
			if (_triggers[i] != null)
			{
				_triggers[i].TriggerEnter += ExecutePushBox;
			}
		}
	}

	private void OnDestroy()
	{
		for (int i = 0; i < _triggers.Length; i++)
		{
			if (_triggers[i] != null)
			{
				_triggers[i].TriggerEnter -= ExecutePushBox;
			}
		}
	}

	private void ExecutePushBox(Collider collider, Vector3 dir)
	{
		if (collider.gameObject.name == "PlayerPhoton(Clone)")
		{
			_rb.AddForce(dir * _pushForce);
		}
	}

	private void Start()
	{
		_beginPos = base.transform.position;
	}

	public void OnEndPush()
	{
	}

	private void NetworkSync()
	{
	}
}
